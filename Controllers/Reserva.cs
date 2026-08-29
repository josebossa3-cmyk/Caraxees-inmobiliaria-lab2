using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria.Controllers
{
    public class ReservaController : Controller
    {
        private readonly ReservaRepository _repo;
        private readonly InquilinoRepository _inquilinoRepo;
        private readonly InmuebleRepository _inmuebleRepo;

        private const int UsuarioActualId = 1;

        public ReservaController(ReservaRepository repo, InquilinoRepository inquilinoRepo, InmuebleRepository inmuebleRepo)
        {
            _repo = repo;
            _inquilinoRepo = inquilinoRepo;
            _inmuebleRepo = inmuebleRepo;
        }

        public async Task<IActionResult> Index()
        {
            var reservas = await _repo.ObtenerTodosAsync();
            return View(reservas);
        }

        public async Task<IActionResult> Details(int id)
        {
            var reserva = await _repo.ObtenerPorIdAsync(id);
            if (reserva == null) return NotFound();
            return View(reserva);
        }

        public async Task<IActionResult> Create()
        {
            await CargarCombos();
            var reserva = new Reserva()
            {
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddDays(30),
            };
            return View(reserva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reserva reserva)
        {
            ModelState.Remove(nameof(reserva.UsuarioCreadorId));
            ModelState.Remove(nameof(reserva.Estado));
            ValidarFechas(reserva);

            if (ModelState.IsValid && await _repo.FechaReservadaAsync(reserva.InmuebleId, reserva.FechaInicio, reserva.FechaFin))
            {
                ModelState.AddModelError(string.Empty, "Ya existe una reserva vigente para ese inmueble en ese rango de fechas.");
            }

            if (!ModelState.IsValid)
            {
                await CargarCombos();
                return View(reserva);
            }

            reserva.Estado = "Vigente";
            reserva.FechaCreacion = DateTime.Now;
            reserva.UsuarioCreadorId = UsuarioActualId;

            await _repo.CrearAsync(reserva);

            TempData["Mensaje"] = "Reserva creada correctamente.";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var reserva = await _repo.ObtenerPorIdAsync(id);
            if (reserva == null) return NotFound();

            await CargarCombos();
            return View(reserva);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Reserva reserva)
        {
            if (id != reserva.Id) return NotFound();

            var reservaDb = await _repo.ObtenerPorIdAsync(id);
            if (reservaDb == null) return NotFound();

            ModelState.Remove(nameof(Reserva.UsuarioCreadorId));
            ModelState.Remove(nameof(Reserva.Estado));

            ValidarFechas(reserva);

            if (ModelState.IsValid && await _repo.FechaReservadaAsync(reserva.InmuebleId, reserva.FechaInicio, reserva.FechaFin, id))
            {
                ModelState.AddModelError(string.Empty, "Ya existe una reserva vigente para ese inmueble en ese rango de fechas.");
            }

            if (!ModelState.IsValid)
            {
                await CargarCombos();
                return View(reserva);
            }

            reservaDb.InquilinoId = reserva.InquilinoId;
            reservaDb.InmuebleId = reserva.InmuebleId;
            reservaDb.MontoPorDia = reserva.MontoPorDia;
            reservaDb.FechaInicio = reserva.FechaInicio;
            reservaDb.FechaFin = reserva.FechaFin;
            reservaDb.PorcentajeReserva = reserva.PorcentajeReserva;

            await _repo.ActualizarAsync(reservaDb);

            TempData["Mensaje"] = "Reserva actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id, decimal? multa)
        {
            var reserva = await _repo.ObtenerPorIdAsync(id);
            if (reserva == null) return NotFound();

            if (reserva.Estado != "Vigente")
            {
                TempData["Error"] = "Solo se pueden cancelar reservas vigentes.";
                return RedirectToAction(nameof(Index));
            }

            await _repo.CambiarEstadoAsync(id, "Cancelada", multa, fechaFin: null, UsuarioActualId, DateTime.Now);

            TempData["Mensaje"] = "Reserva cancelada.";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Terminar(int id)
        {
            var reserva = await _repo.ObtenerPorIdAsync(id);
            if (reserva == null) return NotFound();

            if (reserva.Estado != "Vigente")
            {
                TempData["Error"] = "Solo se pueden terminar reservas vigentes.";
                return RedirectToAction(nameof(Index));
            }

            var ahora = DateTime.Now;
            await _repo.CambiarEstadoAsync(id, "Terminada", multa: null, fechaFin: ahora, UsuarioActualId, ahora);

            TempData["Mensaje"] = "Reserva terminada.";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Renovar(int id)
        {
            var original = await _repo.ObtenerPorIdAsync(id);
            if (original == null) return NotFound();

            var dias = (original.FechaFin - original.FechaInicio).Days;
            if (dias < 1) dias = 1;

            var nueva = new Reserva
            {
                InquilinoId = original.InquilinoId,
                InmuebleId = original.InmuebleId,
                MontoPorDia = original.MontoPorDia,
                PorcentajeReserva = original.PorcentajeReserva,
                FechaInicio = original.FechaFin,
                FechaFin = original.FechaFin.AddDays(dias),
                ReservaRenovadaDeId = original.Id
            };

            await CargarCombos();
            return View(nueva);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Renovar(int id, Reserva reserva)
        {
            var original = await _repo.ObtenerPorIdAsync(id);
            if (original == null) return NotFound();

            ModelState.Remove(nameof(Reserva.UsuarioCreadorId));
            ModelState.Remove(nameof(Reserva.Estado));

            reserva.ReservaRenovadaDeId = original.Id;
            ValidarFechas(reserva);

            if (ModelState.IsValid && await _repo.FechaReservadaAsync(reserva.InmuebleId, reserva.FechaInicio, reserva.FechaFin))
            {
                ModelState.AddModelError(string.Empty, "Ya existe una reserva vigente para ese inmueble en ese rango de fechas.");
            }

            if (!ModelState.IsValid)
            {
                await CargarCombos();
                return View(reserva);
            }

            reserva.Estado = "Vigente";
            reserva.FechaCreacion = DateTime.Now;
            reserva.UsuarioCreadorId = UsuarioActualId;

            await _repo.CrearAsync(reserva);

            var ahora = DateTime.Now;
            await _repo.CambiarEstadoAsync(original.Id, "Terminada", multa: null, fechaFin: ahora, UsuarioActualId, ahora);

            TempData["Mensaje"] = "Reserva renovada correctamente.";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            var reserva = await _repo.ObtenerPorIdAsync(id);
            if (reserva == null) return NotFound();
            return View(reserva);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.EliminarAsync(id);
            TempData["Mensaje"] = "Reserva eliminada.";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerDatosInmueble(int id)
        {
            var inmueble = await _inmuebleRepo.ObtenerPorIdAsync(id);
            if (inmueble == null) return NotFound();

            return Json(new { inmueble.PrecioPorDia, inmueble.PorcentajeReserva });
        }

        private void ValidarFechas(Reserva reserva)
        {
            if (reserva.FechaFin <= reserva.FechaInicio)
            {
                ModelState.AddModelError(nameof(reserva.FechaFin), "La fecha hasta debe ser posterior a la fecha desde.");
            }
        }

        private async Task CargarCombos()
        {
            ViewBag.Inquilinos = await _inquilinoRepo.ObtenerTodosAsync();
            ViewBag.Inmuebles = await _inmuebleRepo.ObtenerDisponiblesAsync();
        }
    }
}
