
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using inmobiliaria.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliaria.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly UsuarioRepository _repo;

        public UsuariosController(UsuarioRepository repo)
        {
            _repo = repo;
        }

        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index(string? searchString, int page = 1)
        {
            const int pageSize = 10;
            var resultado = await _repo.ObtenerPaginadosAsync(searchString, page, pageSize);
            return View(resultado);
        }

        public async Task<IActionResult> Details(int id)
        {
            var usuarios = await _repo.ObtenerPorIdAsync(id);
            if (usuarios == null) return NotFound();
            return View(usuarios);
        }


        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (!ModelState.IsValid)
                return View(usuario);

            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash);
            usuario.Rol = "Empleado";
            usuario.FechaCreacion = DateTime.Now;

            await _repo.CrearAsync(usuario);
            return RedirectToAction("Login");

        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);
            var usuario = await _repo.ObtenerPorEmailAsync(dto.Email);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            {
                ModelState.AddModelError("", "Email o contraseña incorrectos");
                return View(dto);
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol)

            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CambiarRol(int id, string nuevoRol)
        {
            var usuario = await _repo.ObtenerPorIdAsync(id);
            if (usuario == null) return NotFound();
            usuario.Rol = nuevoRol;
            await _repo.ActualizarAsync(usuario);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.EliminarAsync(id);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Perfil()
        {

            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var usuario = await _repo.ObtenerPorIdAsync(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }
    }
}