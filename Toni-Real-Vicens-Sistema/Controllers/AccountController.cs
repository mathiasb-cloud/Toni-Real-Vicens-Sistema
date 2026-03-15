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

                // Guardamos información clave en la sesión
                HttpContext.Session.SetString("UsuarioId", user.Id);
                HttpContext.Session.SetString("UsuarioNombre", user.Nombre + " " + user.Apellido);
                HttpContext.Session.SetString("UsuarioCargo", user.Cargo); // Usamos UsuarioCargo para consistencia

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Correo o contraseña incorrectos.";
            return View();
        }

        // --- LÓGICA DE RECUPERACIÓN ---

        [HttpGet]
        public IActionResult OlvideContrasena() => View();

        [HttpPost]
        public async Task<IActionResult> VerificarTelefono(string telefono)
        {
            var usuarios = await _usuarioService.GetAllAsync();
            // Buscamos si el teléfono existe y la cuenta está activa
            var usuario = usuarios.FirstOrDefault(u => u.Telefono == telefono);

            if (usuario == null)
                return Json(new { success = false, message = "El número telefónico no está registrado." });

            if (!usuario.IsActivo)
                return Json(new { success = false, message = "La cuenta asociada está desactivada." });

            // Retornamos el ID para que el front-end sepa a quién actualizar después
            return Json(new { success = true, userId = usuario.Id });
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerPassword(string userId, string nuevaPassword)
        {
            var usuario = await _usuarioService.GetByIdAsync(userId);
            if (usuario != null)
            {
                usuario.Contrasena = nuevaPassword;
                await _usuarioService.UpdateAsync(usuario);
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Error al identificar al usuario." });
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}