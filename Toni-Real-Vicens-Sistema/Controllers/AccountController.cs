using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Toni_Real_Vicens_Sistema.Service;

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
                HttpContext.Session.SetString("UsuarioNombre", user.Nombre + " " + user.Apellido);
                HttpContext.Session.SetString("Cargo", user.Cargo);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Credenciales incorrectas";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
