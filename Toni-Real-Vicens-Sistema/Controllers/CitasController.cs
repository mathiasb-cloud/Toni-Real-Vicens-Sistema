using Microsoft.AspNetCore.Mvc;
using Toni_Real_Vicens_Sistema.Models;
using Toni_Real_Vicens_Sistema.Service;

namespace Toni_Real_Vicens_Sistema.Controllers
{
    public class CitasController : Controller
    {
        private readonly CitaService _citaService;
        private readonly AlumnoService _alumnoService;

        public CitasController(IConfiguration config)
        {
            _citaService = new CitaService(config);
            _alumnoService = new AlumnoService(config);
        }

        public async Task<IActionResult> Index()
        {
            var citas = await _citaService.GetAllAsync();
            return View(citas);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Alumnos = await _alumnoService.GetAllAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Cita cita)
        {
            Console.WriteLine("AlumnoId: " + cita.AlumnoId);
            Console.WriteLine("FechaHora: " + cita.FechaHora);
            Console.WriteLine("Tipo: " + cita.Tipo);
            Console.WriteLine("Psicologo: " + cita.Psicologo);

            if (!ModelState.IsValid)
            {
                Console.WriteLine("MODEL STATE INVALIDO");
                foreach (var error in ModelState)
                {
                    foreach (var e in error.Value.Errors)
                    {
                        Console.WriteLine($"Error en {error.Key}: {e.ErrorMessage}");
                    }
                }
                ViewBag.Alumnos = await _alumnoService.GetAllAsync();
                return View(cita);
            }

            await _citaService.AddAsync(cita);

            Console.WriteLine("GUARDADO OK");



            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Atender(string id)
        {
            var cita = await _citaService.GetByIdAsync(id);
            if (cita == null) return NotFound();

            return View(cita);
        }

        [HttpPost]
        public async Task<IActionResult> IniciarAtencion(string id)
        {
            await _citaService.UpdateEstadoAsync(id, "Atendida");
            return RedirectToAction("Create", "Fichas", new { citaId = id });
        }
    }
}