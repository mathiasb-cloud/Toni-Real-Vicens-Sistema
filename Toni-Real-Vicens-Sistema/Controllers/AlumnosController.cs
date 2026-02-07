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
            if (!ModelState.IsValid)
                return View(alumno);

            var existente = await _service.GetByDniAsync(alumno.DNI);
            if (existente != null)
            {
                ModelState.AddModelError("DNI", "Este alumno ya está registrado.");
                return View(alumno);
            }

            await _service.AddAsync(alumno);
            return RedirectToAction("Index");
        }
    }
}
