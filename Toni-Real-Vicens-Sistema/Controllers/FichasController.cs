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

               
                await _citaService.UpdateEstadoAsync(ficha.CitaId, "Pendiente");

               
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



        public async Task<IActionResult> CreateSeguimiento(string citaId)
        {
            
            var cita = await _citaService.GetByIdAsync(citaId);
            if (cita == null) return NotFound();

            
            ViewBag.CitaId = citaId;
            ViewBag.AlumnoId = cita.AlumnoId;

            return View();
        }




        // ======================
        // MÉTODO AUXILIAR
        // ======================
        private void CargarDatosAlumnoEnVista(Alumno alumno, Cita cita)
        {
            ViewBag.AlumnoNombre = $"{alumno.Nombres} {alumno.Apellidos}";
            ViewBag.DNI = alumno.DNI;
            ViewBag.Grado = $"{alumno.Grado} - {alumno.Nivel}";
            ViewBag.FechaCita = cita.FechaHora?.ToString("dd/MM/yyyy HH:mm") ?? "No programada";
            ViewBag.Tipo = cita.Tipo;
            ViewBag.Psicologo = cita.Psicologo;

            // Manejo seguro de DateTime? para la edad
            int edad = 0;
            if (alumno.FechaNacimiento.HasValue) // Verificamos si tiene fecha
            {
                var hoy = DateTime.Today;
                var fechaNac = alumno.FechaNacimiento.Value; // Extraemos el valor real

                edad = hoy.Year - fechaNac.Year;

                // Ajuste por si aún no llega su día de cumpleaños
                if (fechaNac.Date > hoy.AddYears(-edad))
                {
                    edad--;
                }
            }

            ViewBag.Edad = alumno.FechaNacimiento.HasValue ? edad.ToString() : "N/E";
        }
    }
}