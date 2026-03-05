using Microsoft.AspNetCore.Mvc;
using Toni_Real_Vicens_Sistema.Models;
using Toni_Real_Vicens_Sistema.Service;

namespace Toni_Real_Vicens_Sistema.Controllers
{
    public class AlumnosController : Controller
    {
        private readonly AlumnoService _service;

        public AlumnosController(IConfiguration config)
        {
            _service = new AlumnoService(config);
        }

        public async Task<IActionResult> Index()
        {
            var alumnos = await _service.GetAllAsync();
            return View(alumnos);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Alumno alumno)
        {
            
            ModelState.Remove("Id");
            ModelState.Remove("Seccion");
            ModelState.Remove("Tutor");
            ModelState.Remove("Direccion");

            if (!ModelState.IsValid)
            {
                
                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                foreach (var error in errores)
                {
                    System.Diagnostics.Debug.WriteLine("Validación falló en: " + error);
                }
                return View(alumno);
            }

            var existente = await _service.GetByDniAsync(alumno.DNI);
            if (existente != null)
            {
                ModelState.AddModelError("DNI", "Este alumno ya está registrado.");
                return View(alumno);
            }

            await _service.AddAsync(alumno);

            
            TempData["SuccessMessage"] = "¡Alumno " + alumno.Nombres + " registrado correctamente!";

            return RedirectToAction("Index");
        }
    }
}
