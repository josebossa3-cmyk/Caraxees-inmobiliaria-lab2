using System.Threading.Tasks;
using inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;

namespace inmobiliaria.Controllers
{
  public class InmueblesController : Controller
  {
    private readonly InmuebleRepository _repo;
    private readonly PropietarioRepository _propietarioRepo;
    private readonly TipoInmuebleRepository _tipoInmuebleRepo;

    public InmueblesController(
      InmuebleRepository repo,
      PropietarioRepository propietarioRepo,
      TipoInmuebleRepository tipoInmuebleRepo)
    {
      _repo = repo;
      _propietarioRepo = propietarioRepo;
      _tipoInmuebleRepo = tipoInmuebleRepo;
    }

    // get de inmuebles
    public async Task<IActionResult> Index()
    {
      var inmuebles = await _repo.ObtenerTodosAsync();
      return View(inmuebles);
    }

    //get inmueble details
    public async Task<IActionResult> Details(int id)
    {
      var inmueble = await _repo.ObtenerPorIdAsync(id);
      if (inmueble == null) return NotFound();
      return View(inmueble);
    }

    // get inmueble create
    public async Task<IActionResult> Create()
    {
      await CargarCombos();
      return View();
    }

    //post create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Inmueble inmueble)
    {

      if (ModelState.IsValid)
      {
        inmueble.FechaAlta = DateTime.Now;
        await _repo.CrearAsync(inmueble);
        return RedirectToAction(nameof(Index));
      }
      await CargarCombos();
      return View(inmueble);
    }

    // get inmueble edit
    public async Task<IActionResult> Edit(int id)
    {
      var inmueble = await _repo.ObtenerPorIdAsync(id);
      if (inmueble == null) return NotFound();
      await CargarCombos();
      return View(inmueble);
    }
    // post edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Inmueble inmueble)
    {
      if (id != inmueble.Id) return NotFound();

      if (ModelState.IsValid)
      {
        await _repo.ActualizarAsync(inmueble);
        return RedirectToAction(nameof(Index));
      }
      await CargarCombos();
      return View(inmueble);
    }

    // get delete
    public async Task<IActionResult> Delete(int id)
    {
      var inmueble = await _repo.ObtenerPorIdAsync(id);
      if (inmueble == null) return NotFound();
      return View(inmueble);
    }

    //post delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      await _repo.EliminarAsync(id);
      return RedirectToAction(nameof(Index));
    }

    private async Task CargarCombos()
    {
      var propietarios = await _propietarioRepo.ObtenerTodosAsync();
      var tipos = await _tipoInmuebleRepo.ObtenerTodosAsync();

      ViewBag.Propietario = new SelectList(propietarios, "Id", "NombreCompleto");
      ViewBag.TiposInmueble = new SelectList(tipos, "Id", "Nombre");
    }
  }
}