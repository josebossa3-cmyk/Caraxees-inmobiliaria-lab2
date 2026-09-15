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
                Reservas = reservas,
                ReservaId = reservaId,
                Pagos = await _pagoRepo.ObtenerPorReservaAsync(null)
            };

            if (reservaId.HasValue)
            {
                model.ReservaSeleccionada = reservas.Find(reserva => reserva.Id == reservaId.Value);
                if (model.ReservaSeleccionada == null) return NotFound();

                model.TotalReserva = CalcularTotalReserva(model.ReservaSeleccionada);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(int id)
        {
            var reserva = await _reservaRepo.ObtenerPorIdAsync(id);
            if (reserva == null) return NotFound();

            var pagos = await _pagoRepo.ObtenerPorReservaAsync(id);
            var totalReserva = CalcularTotalReserva(reserva);
            var totalPagado = pagos
                .Where(pago => pago.Estado == "Activo")
                .Sum(pago => pago.Importe);
            var saldoPendiente = totalReserva - totalPagado;

            if (saldoPendiente <= 0)
            {
                TempData["Error"] = "La reserva ya está completamente pagada.";
                return RedirectToAction(nameof(Index), new { reservaId = id });
            }

            await _pagoRepo.CrearAsync(new Pago
            {
                ReservaId = id,
                Concepto = $"Pago de reserva #{id}",
                FechaPago = DateTime.Now,
                Importe = saldoPendiente,
                Estado = "Activo"
            });
            TempData["Mensaje"] = "Pago realizado correctamente.";

            return RedirectToAction(nameof(Index), new { reservaId = id });
        }

        private decimal CalcularTotalReserva(Reserva reserva)
        {
            var dias = (reserva.FechaFin - reserva.FechaInicio).Days;
            if (dias < 1) dias = 1;

            return dias * reserva.MontoPorDia;
        }
    }
}