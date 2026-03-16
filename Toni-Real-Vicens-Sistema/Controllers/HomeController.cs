using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Toni_Real_Vicens_Sistema.Models;
using Toni_Real_Vicens_Sistema.Service;

namespace Toni_Real_Vicens_Sistema.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AlumnoService _alumnoService; // Ajusta el nombre según tu servicio
        private readonly CitaService _citaService;     // Ajusta el nombre según tu servicio
        private readonly FichaService _fichaService;   // Ajusta el nombre según tu servicio

        public HomeController(ILogger<HomeController> logger, AlumnoService alumnoService, CitaService citaService, FichaService fichaService)
        {
            _logger = logger;
            _alumnoService = alumnoService;
            _citaService = citaService;
            _fichaService = fichaService;
        }

        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioNombre")))
            {
                return RedirectToAction("Login", "Account");
            }

            
            var alumnos = await _alumnoService.GetAllAsync();
            var citas = await _citaService.GetAllAsync();
            var fichas = await _fichaService.GetAllAsync();

            
            ViewBag.TotalAlumnos = alumnos.Count();
            var hoy = DateTime.Today;
            ViewBag.CitasHoy = citas.Count(c => c.FechaHora.HasValue && c.FechaHora.Value.Date == hoy);
            ViewBag.TotalFichas = fichas.Count();
            ViewBag.TotalTalleres = alumnos.Count(a => !string.IsNullOrEmpty(a.AulaComplementaria));

            
            var conteoSemanal = new int[7];
            for (int i = 0; i < 7; i++)
            {
                var fechaObjetivo = hoy.AddDays(i);
                conteoSemanal[i] = citas.Count(c => c.FechaHora.HasValue && c.FechaHora.Value.Date == fechaObjetivo);
            }
            ViewBag.ConteoSemanal = conteoSemanal;

            
            ViewBag.DataNiveles = new int[] {
        alumnos.Count(a => a.Nivel == "Inicial"),
        alumnos.Count(a => a.Nivel == "Primaria"),
        alumnos.Count(a => a.Nivel == "Secundaria")
    };

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}