using Microsoft.AspNetCore.Mvc;
using Toni_Real_Vicens_Sistema.Models;
using Toni_Real_Vicens_Sistema.Service;

namespace Toni_Real_Vicens_Sistema.Controllers
{
    public class CitasController : Controller
    {
        private readonly CitaService _citaService;
        private readonly AlumnoService _alumnoService;
        private readonly FichaService _fichaService; // Agregado
        private readonly SeguimientoService _seguimientoService; // Agregado

        public CitasController(IConfiguration config)
        {
            _citaService = new CitaService(config);
            _alumnoService = new AlumnoService(config);
            _fichaService = new FichaService(config); // Inicializado
            _seguimientoService = new SeguimientoService(config); // Inicializado
        }

        public async Task<IActionResult> Index()
        {
            var citas = await _citaService.GetAllAsync();
            var alumnos = await _alumnoService.GetAllAsync();

            ViewBag.NombresAlumnos = alumnos.ToDictionary(a => a.Id, a => a.Nombres + " " + a.Apellidos);

            foreach (var cita in citas)
            {
                // 1. Lógica para Seguimiento
                if (cita.Tipo == "Seguimiento")
                {
                    
                    List<FichaSeguimiento> seguimientos = await _seguimientoService.GetByAlumnoAsync(cita.AlumnoId);
                    var seg = seguimientos.FirstOrDefault(s => s.CitaId == cita.Id);
                    if (seg != null)
                    {
                        cita.Estado = seg.IsFinalizada ? "Atendida" : "En Proceso";
                    }
                }
               
                else if (cita.Tipo == "Evaluación")
                {
                    
                    List<FichaDiagnostica> fichas = await _fichaService.GetByAlumnoAsync(cita.AlumnoId);
                    var ficha = fichas.FirstOrDefault(f => f.CitaId == cita.Id);
                    if (ficha != null)
                    {
                        cita.Estado = ficha.EsFinalizada ? "Atendida" : "En Proceso";
                    }
                }
            }

            return View(citas.OrderByDescending(c => c.FechaHora).ToList());
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Necesitamos cargar la lista de alumnos para que el dropdown del HTML funcione
            ViewBag.Alumnos = await _alumnoService.GetAllAsync();
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Create(Cita cita, string accion)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Alumnos = await _alumnoService.GetAllAsync();
                return View(cita);
            }

            cita.Estado = "Programada";
            string citaId = await _citaService.AddAsync(cita);

            if (accion == "continuar")
            {
                await _citaService.UpdateEstadoAsync(citaId, "En Proceso");
                return (cita.Tipo == "Evaluación")
                    ? RedirectToAction("Create", "Fichas", new { citaId = citaId })
                    : RedirectToAction("Create", "Seguimiento", new { citaId = citaId });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) return Json(new { success = false, message = "ID no válido" });
                bool eliminado = await _citaService.DeleteCitaCompletaAsync(id);
                return Json(new { success = eliminado });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }
    }
}