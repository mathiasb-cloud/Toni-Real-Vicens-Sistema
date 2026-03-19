using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Toni_Real_Vicens_Sistema.Service;
using Toni_Real_Vicens_Sistema.Models;

namespace Toni_Real_Vicens_Sistema.Controllers
{
    public class AccountController : Controller
    {
        private readonly UsuarioService _usuarioService;

        public AccountController(IConfiguration config)
        {
            _usuarioService = new UsuarioService(config);
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string correo, string pass)
        {
            var user = await _usuarioService.LoginAsync(correo, pass);

            if (user != null)
            {
                if (!user.IsActivo)
                {
                    ViewBag.Error = "Esta cuenta ha sido desactivada por el administrador.";
                    return View();
                }

                HttpContext.Session.SetString("UsuarioId", user.Id);
                HttpContext.Session.SetString("UsuarioNombre", user.Nombre + " " + user.Apellido);
                HttpContext.Session.SetString("UsuarioCargo", user.Cargo);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Correo o contraseña incorrectos.";
            return View();
        }

        // --- LÓGICA DE RECUPERACIÓN CON FIREBASE ---

        [HttpGet]
        public IActionResult OlvideContrasena() => View();

        [HttpPost]
        public async Task<IActionResult> VerificarTelefono(string telefono)
        {
            var usuarios = await _usuarioService.GetAllAsync();
            var usuario = usuarios.FirstOrDefault(u => u.Telefono == telefono);

            if (usuario == null)
                return Json(new { success = false, message = "El número telefónico no está registrado." });

            if (!usuario.IsActivo)
                return Json(new { success = false, message = "La cuenta asociada está desactivada." });

            // Guardamos el ID en sesión para que el proceso sea seguro
            HttpContext.Session.SetString("RecoveryUserId", usuario.Id);

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerPassword(string nuevaPassword)
        {
            var userId = HttpContext.Session.GetString("RecoveryUserId");

            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "La sesión ha expirado o no es válida." });

            var usuario = await _usuarioService.GetByIdAsync(userId);
            if (usuario != null)
            {
                usuario.Contrasena = nuevaPassword; // Asegúrate de hashear esto en el Service si es posible
                await _usuarioService.UpdateAsync(usuario);

                // Limpieza de seguridad
                HttpContext.Session.Remove("RecoveryUserId");

                return Json(new { success = true });
            }
            return Json(new { success = false, message = "No se pudo encontrar el usuario para actualizar." });
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}