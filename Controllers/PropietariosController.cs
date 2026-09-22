using inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace inmobiliaria.Controllers
{
  [Authorize]
  public class PropietariosController : Controller
  {
    private readonly PropietarioRepository _repo;

    public PropietariosController(PropietarioRepository repo)
    {
      _repo = repo;
    }

    //get propietarios
    public async Task<IActionResult> Index(string? searchString, int page = 1)
    {
      const int pageSize = 10;
      var resultado = await _repo.ObtenerPaginadosAsync(searchString, page, pageSize);
      return View(resultado);
    }

    //get propietarios/details/5 <- id
    public async Task<IActionResult> Details(int id)
    {
      var propietario = await _repo.ObtenerPorIdAsync(id);
      if (propietario == null) return NotFound();
      return View(propietario);
    }

    // get propietarios/create
    public IActionResult Create()
    {
      return View();
    }

    // post propietarios/create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Propietario propietario)
    {
      if (ModelState.IsValid)
      {
        await _repo.CrearAsync(propietario);
        return RedirectToAction(nameof(Index));
      }
      return View(propietario);
    }

    // get propietario edit
    public async Task<IActionResult> Edit(int id)
    {
      var propietario = await _repo.ObtenerPorIdAsync(id);
      if (propietario == null) return NotFound();
      return View(propietario);
    }

    // get propietarios/editpartial
    public async Task<IActionResult> EditPartial(int id)
    {
      var propietario = await _repo.ObtenerPorIdAsync(id);
      if (propietario == null) return NotFound();
      return PartialView("_EditPartial", propietario);
    }

    //post Propietarios edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Propietario propietario)
    {
      if (id != propietario.Id) return NotFound();
      if (ModelState.IsValid)
      {
        await _repo.ActualizarAsync(propietario);
        return RedirectToAction(nameof(Index));
      }
      return View(propietario);
    }

    //get propietario delete
    public async Task<IActionResult> Delete(int id)
    {
      var propietario = await _repo.ObtenerPorIdAsync(id);
      if (propietario == null) return NotFound();
      return View(propietario);
    }

    //post propietario delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      await _repo.EliminarAsync(id);
      return RedirectToAction(nameof(Index));
    }
  }
}