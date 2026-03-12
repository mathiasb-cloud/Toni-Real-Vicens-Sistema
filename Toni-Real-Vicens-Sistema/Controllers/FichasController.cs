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
        private readonly SeguimientoService _seguimientoService;

        public FichasController(IConfiguration config)
        {
            _fichaService = new FichaService(config);
            _citaService = new CitaService(config);
            _alumnoService = new AlumnoService(config);
            _seguimientoService = new SeguimientoService(config);
        }

        public async Task<IActionResult> Index()
        {
            // 1. Obtenemos todos los alumnos
            var alumnos = await _alumnoService.GetAllAsync();

            // 2. Obtenemos todas las fichas para poder contar cuántas tiene cada alumno
            // Nota: Esto es opcional, pero hará que los contadores de las "tarjetas" funcionen
            foreach (var alumno in alumnos)
            {
                var fichas = await _fichaService.GetByAlumnoAsync(alumno.Id);
                var seguimientos = await _seguimientoService.GetByAlumnoAsync(alumno.Id);

                // Asignamos los conteos (Asegúrate de que estas propiedades existan en tu modelo Alumno)
                alumno.TotalFichasDiagnosticas = fichas.Count;
                // Si no tienes una propiedad para seguimiento, puedes usar un ViewBag o una propiedad temporal
            }

            // 3. Devolvemos la lista de alumnos (ordenada por apellido)
            return View(alumnos.OrderBy(a => a.Apellidos).ToList());
        }



        public async Task<IActionResult> Detalle(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            // 1. Obtener los datos del alumno
            var alumno = await _alumnoService.GetByIdAsync(id);
            if (alumno == null) return NotFound();

            // 2. Obtener TODAS las fichas diagnósticas
            var fichasDiagnosticas = await _fichaService.GetByAlumnoAsync(id);

            
            var seguimientos = await _seguimientoService.GetByAlumnoAsync(id);

            // 4. Pasamos ambas listas a la vista mediante ViewBag
            // Las listamos todas sin excepción
            ViewBag.FichasDiagnosticas = fichasDiagnosticas ?? new List<FichaDiagnostica>();
            ViewBag.FichasSeguimiento = seguimientos ?? new List<FichaSeguimiento>();

            return View(alumno);
        }

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
                Fecha = DateTime.Now,
                AnioAcademico = DateTime.Now.Year,
                GradoAlMomento = alumno.Grado,
                SeccionAlMomento = alumno.Seccion,
                FuenteInformacion = cita.Psicologo
            };

            return View(ficha);
        }

        [HttpPost]
        public async Task<IActionResult> Create(FichaDiagnostica ficha)
        {
            // Obtenemos los datos necesarios para recargar la vista SIEMPRE
            var alumno = await _alumnoService.GetByIdAsync(ficha.AlumnoId);
            var citaActual = await _citaService.GetByIdAsync(ficha.CitaId);

            try
            {
                ficha.AnioAcademico = DateTime.Now.Year;

                if (string.IsNullOrEmpty(ficha.FuenteInformacion))
                {
                    ficha.FuenteInformacion = HttpContext.Session.GetString("UsuarioNombre") ?? "Psicólogo";
                }

                // Guardar o Actualizar
                if (string.IsNullOrEmpty(ficha.Id))
                {
                    ficha.Id = await _fichaService.AddAsync(ficha);
                }
                else
                {
                    await _fichaService.UpdateAsync(ficha);
                }

                if (ficha.EsFinalizada)
                {
                    await _citaService.UpdateEstadoAsync(ficha.CitaId, "Atendida");
                    TempData["Mensaje"] = "Ficha finalizada correctamente";
                    
                    return RedirectToAction("Detalle", "Fichas", new { id = ficha.AlumnoId });
                }
                
                else
                {
                    await _citaService.UpdateEstadoAsync(ficha.CitaId, "En Proceso");

                    
                    CargarDatosAlumnoEnVista(alumno, citaActual);

                    
                    TempData["Mensaje"] = "actualizado"; 

                    return View(ficha);
                }
            }
            catch (Exception ex)
            {
                // En caso de error también recargamos el ViewBag para que no se vea feo
                CargarDatosAlumnoEnVista(alumno, citaActual);
                TempData["Error"] = "Error: " + ex.Message;
                return View(ficha);
            }
        }





        private void CargarDatosAlumnoEnVista(Alumno alumno, Cita cita)
        {
            ViewBag.AlumnoNombre = $"{alumno.Nombres} {alumno.Apellidos}";
            ViewBag.DNI = alumno.DNI;
            ViewBag.Grado = $"{alumno.Grado} - {alumno.Nivel}";
            ViewBag.FechaCita = cita.FechaHora?.ToString("dd/MM/yyyy HH:mm") ?? "No programada";
            ViewBag.Tipo = cita.Tipo;
            ViewBag.Psicologo = cita.Psicologo;

            int edad = 0;
            if (alumno.FechaNacimiento.HasValue)
            {
                var hoy = DateTime.Today;
                var fechaNac = alumno.FechaNacimiento.Value;
                edad = hoy.Year - fechaNac.Year;
                if (fechaNac.Date > hoy.AddYears(-edad)) edad--;
            }
            ViewBag.Edad = alumno.FechaNacimiento.HasValue ? edad.ToString() : "N/E";
        }

        public async Task<IActionResult> VerFichaDetalle(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var ficha = await _fichaService.GetByIdAsync(id);
            if (ficha == null) return NotFound();

            
            var alumno = await _alumnoService.GetByIdAsync(ficha.AlumnoId);
            ViewBag.NombreAlumno = alumno != null ? $"{alumno.Apellidos}, {alumno.Nombres}" : "Estudiante no encontrado";

            return View(ficha);
        }
    }
}