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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.IsActivo = true;
                bool creado = await _usuarioService.AddAsync(usuario);
                if (creado)
                {
                    TempData["Accion"] = "creado"; // <--- Añadido para animación
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "El correo o teléfono ya existen.");
            }
            return View(usuario);
        }


        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var usuario = await _usuarioService.GetByIdAsync(id);
            if (usuario == null) return NotFound();

            
            if (usuario.Cargo == "Administrador")
            {
                TempData["Error"] = "La cuenta de Administrador está protegida, no se puede editar.";
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Usuario usuario)
        {
            
            var dbUser = await _usuarioService.GetByIdAsync(usuario.Id);
            if (dbUser.Cargo == "Administrador") return RedirectToAction(nameof(Index));

            if (ModelState.IsValid)
            {
                await _usuarioService.UpdateAsync(usuario);
                TempData["Accion"] = "editado";
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }


        [HttpPost]
        public async Task<IActionResult> ToggleEstado(string id)
        {
            var usuario = await _usuarioService.GetByIdAsync(id);

            
            if (usuario != null && usuario.Cargo != "Administrador")
            {
                usuario.IsActivo = !usuario.IsActivo;
                await _usuarioService.UpdateAsync(usuario);
                return Json(new { success = true, nuevoEstado = usuario.IsActivo });
            }
            return Json(new { success = false, message = "Acción no permitida para Administradores" });
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