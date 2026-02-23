using Microsoft.AspNetCore.Mvc;
using Toni_Real_Vicens_Sistema.Models;
using Toni_Real_Vicens_Sistema.Service;

namespace Toni_Real_Vicens_Sistema.Controllers
{
    public class FichasController : Controller
    {
        private readonly FichaService _fichaService;
        private readonly CitaService _citaService;
        private readonly AlumnoService _alumnoService;

        public FichasController(IConfiguration config)
        {
            _fichaService = new FichaService(config);
            _citaService = new CitaService(config);
            _alumnoService = new AlumnoService(config);
        }

        // ======================
        // GET
        // ======================
        public async Task<IActionResult> Create(string citaId)
        {
            var cita = await _citaService.GetByIdAsync(citaId);
            if (cita == null) return NotFound();

            var alumno = await _alumnoService.GetByIdAsync(cita.AlumnoId);
            CargarDatosAlumnoEnVista(alumno, cita);

            
            var fichasExistentes = await _fichaService.GetByAlumnoAsync(cita.AlumnoId);
            var fichaPrevia = fichasExistentes.FirstOrDefault(f => f.CitaId == citaId);

            if (fichaPrevia != null)
            {
                
                return View(fichaPrevia);
            }

            
            var ficha = new FichaDiagnostica
            {
                AlumnoId = cita.AlumnoId,
                CitaId = cita.Id,
                Fecha = DateTime.Now
            };
            return View(ficha);
        }




        // ======================
        // POST
        // ======================

        [HttpPost]
        public async Task<IActionResult> Create(FichaDiagnostica ficha)
        {
            try
            {
                if (string.IsNullOrEmpty(ficha.Id))
                {
                    ficha.Id = await _fichaService.AddAsync(ficha);
                    TempData["Mensaje"] = "creado";
                }
                else
                {
                    await _fichaService.UpdateAsync(ficha);
                    TempData["Mensaje"] = "actualizado";
                }

                if (ficha.EsFinalizada)
                {
                    await _citaService.UpdateEstadoAsync(ficha.CitaId, "Atendida");
                    return RedirectToAction("Index", "Citas");
                }

                
                var cita = await _citaService.GetByIdAsync(ficha.CitaId);
                var alumno = await _alumnoService.GetByIdAsync(ficha.AlumnoId);
                CargarDatosAlumnoEnVista(alumno, cita);

                
                return View(ficha);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return View(ficha);
            }
        }




        // ======================
        // MÉTODO AUXILIAR
        // ======================
        private void CargarDatosAlumnoEnVista(Alumno alumno, Cita cita)
        {
            ViewBag.AlumnoNombre = alumno.Nombres + " " + alumno.Apellidos;
            ViewBag.DNI = alumno.DNI;
            ViewBag.Grado = alumno.Grado + " - " + alumno.Nivel;
            ViewBag.FechaCita = cita.FechaHora?.ToString("dd/MM/yyyy HH:mm");
            ViewBag.Tipo = cita.Tipo;
            ViewBag.Psicologo = cita.Psicologo;

            var edad = DateTime.Now.Year - alumno.FechaNacimiento.Year;
            ViewBag.Edad = edad;
        }
    }
}