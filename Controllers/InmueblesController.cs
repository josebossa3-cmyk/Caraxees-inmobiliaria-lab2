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

    private readonly ImagenInmuebleRepository _imagenInmuebleRepo;

    private readonly IWebHostEnvironment _environment;

    public InmueblesController(
      InmuebleRepository repo,
      PropietarioRepository propietarioRepo,
      TipoInmuebleRepository tipoInmuebleRepo,
      ImagenInmuebleRepository imagenInmuebleRepo,
      IWebHostEnvironment environment)
    {
      _repo = repo;
      _propietarioRepo = propietarioRepo;
      _tipoInmuebleRepo = tipoInmuebleRepo;
      _imagenInmuebleRepo = imagenInmuebleRepo;
      _environment = environment;
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

      var imagenes = await _imagenInmuebleRepo.ObtenerPorInmuebleAsync(id);
      ViewBag.Imagenes = imagenes;
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
    public async Task<IActionResult> Create(Inmueble inmueble, List<IFormFile> archivos)
    {

      if (ModelState.IsValid)
      {
        inmueble.FechaAlta = DateTime.Now;
        await _repo.CrearAsync(inmueble); // el inmueble tiene Id

        if (archivos != null && archivos.Count > 0)
        {
          int orden = 0;
          foreach (var archivo in archivos)
          {
            if (archivo.Length > 0)
            {
              string url = await GuardarImagenAsync(archivo);
              bool esPortada = (orden == 0);
              await _imagenInmuebleRepo.CrearAsync(new ImagenInmueble
              {
                InmuebleId = inmueble.Id,
                Url = url,
                EsPortada = esPortada,
                Orden = orden
              });

              // actualiza el imagenPortada del inmueble
              if (esPortada)
              {
                inmueble.ImagenPortada = url;
                await _repo.ActualizarAsync(inmueble);
              }
              orden++;
            }
          }
        }
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
    public async Task<IActionResult> Edit(int id, Inmueble inmueble, List<IFormFile> archivos)
    {
      if (id != inmueble.Id) return NotFound();

      if (ModelState.IsValid)
      {
        await _repo.ActualizarAsync(inmueble);

        //vemos los archivos subidos
        if (archivos != null && archivos.Count > 0)
        {
          //borramos las previas
          await _imagenInmuebleRepo.EliminarPorInmuebleAsync(inmueble.Id);

          int orden = 0;
          foreach (var archivo in archivos)
          {
            if (archivo.Length > 0)
            {
              string url = await GuardarImagenAsync(archivo);
              bool esPortada = (orden == 0);
              await _imagenInmuebleRepo.CrearAsync(new ImagenInmueble
              {
                InmuebleId = inmueble.Id,
                Url = url,
                EsPortada = esPortada,
                Orden = orden
              });

              if (esPortada)
              {
                inmueble.ImagenPortada = url;
                await _repo.ActualizarAsync(inmueble);
              }
              orden++;
            }
          }
        }

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

    private async Task<string> GuardarImagenAsync(IFormFile archivo)
    {
      string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "inmuebles");
      if (!Directory.Exists(uploadsFolder))
        Directory.CreateDirectory(uploadsFolder);

      // generar nombre unico
      string nombreUnico = Guid.NewGuid().ToString() + Path.GetExtension(archivo.FileName);
      string filePath = Path.Combine(uploadsFolder, nombreUnico);

      using (var fileStream = new FileStream(filePath, FileMode.Create))
      {
        await archivo.CopyToAsync(fileStream);
      }

      return $"/uploads/inmuebles/{nombreUnico}";
    }
  }
}