using System;
using System.Linq;
using System.Threading.Tasks;
using inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria.Controllers
{
    [Authorize]
    public class PagosController : Controller
    {
        private readonly ReservaRepository _reservaRepo;
        private readonly PagoRepository _pagoRepo;
        private int UsuarioActualId
        {
            get
            {
                var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                return int.TryParse(idClaim, out var id) ? id : 0;
            }
        }

        public PagosController(ReservaRepository reservaRepo, PagoRepository pagoRepo)
        {
            _reservaRepo = reservaRepo;
            _pagoRepo = pagoRepo;
        }

        public async Task<IActionResult> Index(int? reservaId)
        {
            var reservas = await _reservaRepo.ObtenerTodosAsync();

            var model = new PagosIndexViewModel
            {
                Reservas = reservas.Where(r => r.Estado == "Vigente").ToList(),
                ReservaId = reservaId
            };

            if (reservaId.HasValue)
            {
                model.ReservaSeleccionada = reservas.FirstOrDefault(r => r.Id == reservaId.Value);
                if (model.ReservaSeleccionada == null) return NotFound();

                model.TotalReserva = CalcularTotalReserva(model.ReservaSeleccionada);

                var pagos = await _pagoRepo.ObtenerPorReservaAsync(reservaId.Value);
                model.Pagos = pagos;
                model.TotalPagado = pagos.Where(p => p.Estado == "Activo").Sum(p => p.Importe);
                model.MontoSeña = model.TotalReserva * (model.ReservaSeleccionada.PorcentajeReserva / 100m);
            }
            else
            {
                model.Pagos = await _pagoRepo.ObtenerPorReservaAsync(null);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(int id, string concepto, decimal importe)
        {
            var reserva = await _reservaRepo.ObtenerPorIdAsync(id);
            if (reserva == null) return NotFound();

            if (string.IsNullOrWhiteSpace(concepto) || importe <= 0)
            {
                TempData["Error"] = "Debe indicar un concepto y un importe mayor a cero.";
                return RedirectToAction(nameof(Index), new { reservaId = id });
            }

            var pagos = await _pagoRepo.ObtenerPorReservaAsync(id);
            var totalReserva = CalcularTotalReserva(reserva);
            var totalPagado = pagos
                .Where(p => p.Estado == "Activo").Sum(p => p.Importe);
            var saldoPendiente = totalReserva - totalPagado;

            if (saldoPendiente <= 0)
            {
                TempData["Error"] = "La reserva ya está completamente pagada.";
                return RedirectToAction(nameof(Index), new { reservaId = id });
            }

            if (importe > saldoPendiente)
            {
                TempData["Error"] = $"El importe no puede superar el saldo pendiente ({saldoPendiente:C}).";
                return RedirectToAction(nameof(Index), new { reservaId = id });
            }

            await _pagoRepo.CrearAsync(new Pago
            {
                ReservaId = id,
                Concepto = concepto,
                FechaPago = DateTime.Now,
                Importe = importe,
                Estado = "Activo",
                UsuarioCreadorId = UsuarioActualId
            });
            TempData["Mensaje"] = $"Pago de {importe:C} registrado correctamente.";
            return RedirectToAction(nameof(Index), new { reservaId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Anular(int id, int? reservaId)
        {
            await _pagoRepo.AnularAsync(id, UsuarioActualId);
            TempData["Mensaje"] = "Pago anulado correctamente.";
            return RedirectToAction(nameof(Index), new { reservaId });
        }

        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var pago = await _pagoRepo.ObtenerPorIdAsync(id);
            if (pago == null) return NotFound();
            return PartialView("_EditPartialPago", pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarConcepto(int id, string concepto, int? reservaId)
        {
            if (string.IsNullOrWhiteSpace(concepto))
            {
                TempData["Error"] = "El concepto no puede estar vacio.";
                return RedirectToAction(nameof(Index), new { reservaId });
            }

            await _pagoRepo.ActualizarConceptoAsync(id, concepto);
            TempData["Mensaje"] = "Concepto actualizado correctamente.";
            return RedirectToAction(nameof(Index), new { reservaId });
        }

        private decimal CalcularTotalReserva(Reserva reserva)
        {
            var dias = (reserva.FechaFin - reserva.FechaInicio).Days;
            if (dias < 1) dias = 1;

            return dias * reserva.MontoPorDia;
        }
    }
}