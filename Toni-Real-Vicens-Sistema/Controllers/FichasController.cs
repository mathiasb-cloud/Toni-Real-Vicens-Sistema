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

        // 1. En el INDEX: Para que el contador sea real
        public async Task<IActionResult> Index()
        {
            var alumnos = await _alumnoService.GetAllAsync();
            // Necesitamos cargar el conteo de fichas para cada alumno
            foreach (var alumno in alumnos)
            {
                var fichas = await _fichaService.GetByAlumnoAsync(alumno.Id);
                alumno.TotalFichasDiagnosticas = fichas.Count; // Asegúrate de tener esta propiedad en tu ViewModel o usa un ViewBag
            }
            return View(alumnos);
        }

        // 2. En el DETALLE (Expediente): Para que se vean TODAS las fichas
        public async Task<IActionResult> Detalle(string id)
        {
            var alumno = await _alumnoService.GetByIdAsync(id);
            // Cargamos todas las fichas diagnósticas de este alumno
            ViewBag.FichasDiagnosticas = await _fichaService.GetByAlumnoAsync(id);

            return View(alumno);
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

            // --- LÓGICA DE PROMOCIÓN APLICADA A LA FICHA ---
            // Si la ficha es nueva, capturamos los datos académicos del alumno en este instante
            var ficha = new FichaDiagnostica
            {
                AlumnoId = cita.AlumnoId,
                CitaId = cita.Id,
                Fecha = DateTime.Now,

                // Usamos los nombres exactos de tu modelo de FichaDiagnostica:
                AnioAcademico = DateTime.Now.Year,
                GradoAlMomento = alumno.Grado,
                SeccionAlMomento = alumno.Seccion
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
                
                var alumno = await _alumnoService.GetByIdAsync(ficha.AlumnoId);

                
                ficha.GradoAlMomento = alumno.Grado;
                ficha.SeccionAlMomento = alumno.Seccion;
                ficha.AnioAcademico = DateTime.Now.Year;

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








        public async Task<IActionResult> VerFichaDetalle(string id) // Cambiado de alumnoId a id
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            // 1. Buscamos la ficha específica directamente
            var ficha = await _fichaService.GetByIdAsync(id);

            if (ficha == null)
            {
                TempData["Error"] = "No se encontró la ficha diagnóstica seleccionada.";
                return RedirectToAction("Index");
            }

            // 2. Cargamos los datos del alumno para que el encabezado formal que hicimos tenga nombre
            var alumno = await _alumnoService.GetByIdAsync(ficha.AlumnoId);
            if (alumno != null)
            {
                ViewBag.NombreAlumno = $"{alumno.Apellidos}, {alumno.Nombres}";
            }

            return View(ficha);
        }
    }
}