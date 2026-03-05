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
            var alumnos = await _alumnoService.GetAllAsync();

            
            ViewBag.NombresAlumnos = alumnos.ToDictionary(a => a.Id, a => a.Nombres + " " + a.Apellidos);

            return View(citas);
        }

        public async Task<IActionResult> Create()
        {
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
                
                await _citaService.UpdateEstadoAsync(citaId, "Pendiente");

                if (cita.Tipo == "Evaluación")
                {
                    return RedirectToAction("Create", "Fichas", new { citaId = citaId });
                }
                else if (cita.Tipo == "Seguimiento")
                {
                    return RedirectToAction("CreateSeguimiento", "Fichas", new { citaId = citaId });
                }
            }

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





        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { success = false, message = "El ID de la cita no es válido." });
                }

                
                bool eliminado = await _citaService.DeleteCitaCompletaAsync(id);

                if (eliminado)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudo eliminar el registro de Firebase." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error de servidor: " + ex.Message });
            }
        }




    }
}