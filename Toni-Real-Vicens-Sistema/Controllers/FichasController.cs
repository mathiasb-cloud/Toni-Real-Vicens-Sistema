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

            
            if (cita.Tipo == "Seguimiento")
            {
                var fichaSeg = new FichaSeguimiento
                {
                    AlumnoId = cita.AlumnoId,
                    CitaId = cita.Id,
                    Fecha = DateTime.Now
                };
                return View("CreateSeguimiento", fichaSeg);
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
           
            if (!ModelState.IsValid)
            {
               
                var cita = await _citaService.GetByIdAsync(ficha.CitaId);
                var alumno = await _alumnoService.GetByIdAsync(ficha.AlumnoId);
                CargarDatosAlumnoEnVista(alumno, cita);
                return View(ficha);
            }

            
            await _fichaService.AddAsync(ficha);

            
            await _citaService.UpdateEstadoAsync(ficha.CitaId, "Atendida");

            return RedirectToAction("Index", "Alumnos");
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