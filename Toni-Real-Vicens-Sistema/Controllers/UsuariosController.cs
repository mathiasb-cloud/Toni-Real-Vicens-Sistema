using Microsoft.AspNetCore.Mvc;
using Toni_Real_Vicens_Sistema.Models;
using Toni_Real_Vicens_Sistema.Service;

namespace Toni_Real_Vicens_Sistema.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UsuarioService _usuarioService;

        public UsuariosController(IConfiguration config)
        {
            _usuarioService = new UsuarioService(config);
        }

        // Listado de Usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuarioService.GetAllAsync();
            return View(usuarios);
        }

        // Vista de Creación (GET)
        public IActionResult Create()
        {
            return View();
        }

        // Acción de Creación (POST) - AQUÍ ESTABA EL ERROR 405
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                // Por defecto los nuevos usuarios están activos
                usuario.IsActivo = true;

                bool creado = await _usuarioService.AddAsync(usuario);
                if (creado)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "El correo o teléfono ya existen.");
            }
            return View(usuario);
        }

        // Acción para Activar/Desactivar
        [HttpPost]
        public async Task<IActionResult> ToggleEstado(string id)
        {
            var usuario = await _usuarioService.GetByIdAsync(id);
            if (usuario != null)
            {
                usuario.IsActivo = !usuario.IsActivo;
                await _usuarioService.UpdateAsync(usuario);
            }
            return RedirectToAction(nameof(Index));
        }




        
        public async Task<IActionResult> Perfil()
        {
            string userId = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

            var usuario = await _usuarioService.GetByIdAsync(userId);
            return View(usuario);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Perfil(Usuario model)
        {
            string userIdSesion = HttpContext.Session.GetString("UsuarioId");

            
            var usuarioDb = await _usuarioService.GetByIdAsync(userIdSesion);

            if (usuarioDb == null) return NotFound();

            
            usuarioDb.Nombre = model.Nombre;
            usuarioDb.Apellido = model.Apellido;
            usuarioDb.SegundoApellido = model.SegundoApellido;
            usuarioDb.Telefono = model.Telefono;
            usuarioDb.Correo = model.Correo;

            
            if (!string.IsNullOrEmpty(model.Contrasena))
            {
                usuarioDb.Contrasena = model.Contrasena;
            }

            await _usuarioService.UpdateAsync(usuarioDb);

            ViewBag.Success = "Perfil actualizado correctamente";
            return View(usuarioDb);
        }


    }
}