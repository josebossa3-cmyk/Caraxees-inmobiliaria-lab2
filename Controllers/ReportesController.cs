using System;
using System.Linq;
using System.Threading.Tasks;
using inmobiliaria.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria.Controllers
{
  [Authorize]
  public class ReportesController : Controller
  {
    private readonly InmuebleRepository _inmuebleRepo;
    private readonly PropietarioRepository _propietarioRepo;
    private readonly ReservaRepository _reservaRepo;

    public ReportesController(
      InmuebleRepository inmuebleRepo,
      PropietarioRepository propietarioRepo,
      ReservaRepository reservaRepo)
    {
      _inmuebleRepo = inmuebleRepo;
      _propietarioRepo = propietarioRepo;
      _reservaRepo = reservaRepo;
    }

    //reporte 1
    public async Task<IActionResult> InmueblesPorDueno(bool? disponible)
    {
      var inmuebles = await _inmuebleRepo.ObtenerTodosAsync();

      if (disponible.HasValue)
        inmuebles = inmuebles.Where(i => i.Estado == disponible.Value).ToList();

      ViewBag.Disponible = disponible;
      return View(inmuebles);
    }

    //reporte 2
    public async Task<IActionResult> InmueblesPorPropietario(int? propietarioId)
    {
      ViewBag.Propietarios = await _propietarioRepo.ObtenerTodosAsync();

      if (!propietarioId.HasValue)
        return View(Enumerable.Empty<Inmueble>());

      var inmuebles = await _inmuebleRepo.ObtenerTodosAsync();
      inmuebles = inmuebles.Where(i => i.PropietarioId == propietarioId.Value).ToList();
      ViewBag.PropietarioId = propietarioId;
      return View(inmuebles);
    }

    //reporte 3
    public async Task<IActionResult> InmueblesMasReservados()
    {
      var reservas = await _reservaRepo.ObtenerTodosAsync();
      var desde = DateTime.Now.AddDays(-365);

      var resultado = reservas
        .Where(r => r.FechaCreacion >= desde)
        .GroupBy(r => r.InmuebleId)
        .Select(g => new
        {
          InmuebleId = g.Key,
          Direccion = g.First().Inmueble?.Direccion ?? "Sin dirección",
          Cantidad = g.Count()
        })
        .OrderByDescending(x => x.Cantidad)
        .Take(10)
        .ToList();

      return View(resultado);
    }

    //reporte 4
    public async Task<IActionResult> InmueblesSinReserva(int dias = 30)
    {
      var inmuebles = await _inmuebleRepo.ObtenerTodosAsync();
      var reservas = await _reservaRepo.ObtenerTodosAsync();
      var desde = DateTime.Now.AddDays(-dias);

      var inmueblesConReservas = reservas
        .Where(r => r.FechaCreacion >= desde)
        .Select(r => r.InmuebleId)
        .Distinct()
        .ToList();

      var resultado = inmuebles
        .Where(i => !inmueblesConReservas.Contains(i.Id))
        .ToList();

      ViewBag.Dias = dias;
      return View(resultado);
    }

    //reporte 5
    public async Task<IActionResult> ReservasVigentes()
    {
      var reservas = await _reservaRepo.ObtenerTodosAsync();
      var hoy = DateTime.Today;

      var vigentes = reservas
      .Where(r => r.Estado == "Vigente" && r.FechaInicio <= hoy && r.FechaFin >= hoy)
      .ToList();

      return View(vigentes);
    }

    //reporte 6
    public async Task<IActionResult> ReservasPorVencer(int dias = 7)
    {
      var reservas = await _reservaRepo.ObtenerTodosAsync();
      var hoy = DateTime.Today;
      var limite = hoy.AddDays(dias);

      var porVencer = reservas
        .Where(r => r.Estado == "Vigente" && r.FechaFin >= hoy && r.FechaFin <= limite)
        .OrderBy(r => r.FechaFin)
        .ToList();

      ViewBag.Dias = dias;
      return View(porVencer);
    }

    //reporte 7
    public async Task<IActionResult> InmueblesDisponibles(DateTime? desde, DateTime? hasta)
    {
      if (!desde.HasValue || !hasta.HasValue)
        return View(Enumerable.Empty<Inmueble>());

      var inmuebles = await _inmuebleRepo.ObtenerDisponiblesAsync();
      var reservas = await _reservaRepo.ObtenerTodosAsync();

      var ocupados = reservas
        .Where(r => r.Estado == "Vigente" && r.FechaInicio < hasta.Value && r.FechaFin > desde.Value)
        .Select(r => r.InmuebleId)
        .Distinct()
        .ToList();

      var resultado = inmuebles.Where(i => !ocupados.Contains(i.Id)).ToList();

      ViewBag.Desde = desde;
      ViewBag.Hasta = hasta;

      return View(resultado);
    }
  }
}