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
            
            var taskAlumnos = _alumnoService.GetAllAsync();
            var taskFichas = _fichaService.GetAllAsync(); 
            var taskSeguimientos = _seguimientoService.GetAllAsync(); 

            await Task.WhenAll(taskAlumnos, taskFichas, taskSeguimientos);

            var alumnos = await taskAlumnos;
            var todasLasFichas = await taskFichas;
            var todosLosSeguimientos = await taskSeguimientos;

           
            foreach (var alumno in alumnos)
            {
                alumno.TotalFichasDiagnosticas = todasLasFichas.Count(f => f.AlumnoId == alumno.Id);
                alumno.TotalFichasSeguimiento = todosLosSeguimientos.Count(s => s.AlumnoId == alumno.Id);
            }

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
            // 1. Obtener la cita basada en el ID de la cita.
            var cita = await _citaService.GetByIdAsync(citaId);
            if (cita == null) return NotFound();  // Si no se encuentra la cita, retornar error.

            // 2. Obtener el alumno asociado con la cita.
            var alumno = await _alumnoService.GetByIdAsync(cita.AlumnoId);
            CargarDatosAlumnoEnVista(alumno, cita);  // Cargar información adicional del alumno.

            // 3. Verificar si ya existe una ficha previa para el alumno con la misma cita.
            var fichasExistentes = await _fichaService.GetByAlumnoAsync(cita.AlumnoId);
            var fichaPrevia = fichasExistentes.FirstOrDefault(f => f.CitaId == citaId);

            // 4. Si ya existe una ficha para esa cita, devolverla.
            if (fichaPrevia != null)
            {
                return View(fichaPrevia);
            }

            // 5. Crear una nueva ficha diagnóstica con los valores históricos.
            var ficha = new FichaDiagnostica
            {
                AlumnoId = cita.AlumnoId,
                CitaId = cita.Id,
                Fecha = DateTime.Now,
                AnioAcademico = DateTime.Now.Year,
                GradoAlMomento = alumno.Grado,  // Captura el grado del alumno en el momento de la ficha.
                SeccionAlMomento = alumno.Seccion,  // Captura la sección del alumno en el momento de la ficha.
                FuenteInformacion = cita.Psicologo  // Fuente de información de la cita.
            };

            // 6. Devuelve la vista con la nueva ficha diagnóstica.
            return View(ficha);
        }





        [HttpPost]
        public async Task<IActionResult> Create(FichaDiagnostica ficha)
        {
            try
            {
                ficha.AnioAcademico = DateTime.Now.Year;

                // Guardar o actualizar la ficha
                if (string.IsNullOrEmpty(ficha.Id))
                {
                    ficha.Id = await _fichaService.AddAsync(ficha);
                }
                else
                {
                    await _fichaService.UpdateAsync(ficha);
                }

                // Si la ficha está finalizada, se actualiza el estado de la cita
                if (ficha.EsFinalizada)
                {
                    await _citaService.UpdateEstadoAsync(ficha.CitaId, "Atendida");
                    TempData["Mensaje"] = "Ficha finalizada correctamente";
                    return RedirectToAction("Detalle", "Fichas", new { id = ficha.AlumnoId });
                }
                else
                {
                    await _citaService.UpdateEstadoAsync(ficha.CitaId, "En Proceso");
                    TempData["Mensaje"] = "Ficha actualizada correctamente";
                    return View(ficha);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al guardar la ficha: " + ex.Message;
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

            // Obtener los datos del alumno asociado
            var alumno = await _alumnoService.GetByIdAsync(ficha.AlumnoId);
            ViewBag.NombreAlumno = alumno != null ? $"{alumno.Apellidos}, {alumno.Nombres}" : "Estudiante no encontrado";

            return View(ficha);  // Pasamos toda la ficha a la vista
        }


        public IActionResult VerSeguimientoDetalle(string id)
        {

            return View();
        }


        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> List()
        {
            
            var taskAlumnos = _alumnoService.GetAllAsync();
            var taskFichas = _fichaService.GetAllAsync();
            var taskSeguimientos = _seguimientoService.GetAllAsync();

            await Task.WhenAll(taskAlumnos, taskFichas, taskSeguimientos);

            var alumnos = await taskAlumnos;
            var fichas = await taskFichas;
            var seguimientos = await taskSeguimientos;

            
            var historialCombinado = new List<dynamic>();

            
            foreach (var f in fichas)
            {
                var alu = alumnos.FirstOrDefault(a => a.Id == f.AlumnoId);
                historialCombinado.Add(new
                {
                    Id = f.Id,
                    Fecha = f.Fecha,
                    Alumno = alu != null ? $"{alu.Apellidos}, {alu.Nombres}" : "Desconocido",
                    Tipo = "Diagnóstica",
                    Psicologo = f.FuenteInformacion ?? "No asignado",
                    BadgeColor = "#e7f5ff",
                    TextColor = "#1971c2"
                });
            }

            
            foreach (var s in seguimientos)
            {
                var alu = alumnos.FirstOrDefault(a => a.Id == s.AlumnoId);
                historialCombinado.Add(new
                {
                    Id = s.Id,
                    Fecha = s.Fecha,
                    Alumno = alu != null ? $"{alu.Apellidos}, {alu.Nombres}" : "Desconocido",
                    Tipo = "Seguimiento",
                    Psicologo = s.Psicologo ?? "No asignado",
                    BadgeColor = "#fff4e6",
                    TextColor = "#fd7e14"
                });
            }

            
            ViewBag.Historial = historialCombinado.OrderByDescending(x => x.Fecha).ToList();


            var mensaje = TempData["Mensaje"]?.ToString();
            ViewBag.MensajeAlerta = mensaje;

            return View(); 
        }



        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            
            var ficha = await _fichaService.GetByIdAsync(id);
            if (ficha == null) return NotFound();

            
            var alumno = await _alumnoService.GetByIdAsync(ficha.AlumnoId);
            var cita = await _citaService.GetByIdAsync(ficha.CitaId);

            CargarDatosAlumnoEnVista(alumno, cita);

            
            return View(ficha);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(FichaDiagnostica ficha)
        {
            try
            {
                
                await _fichaService.UpdateAsync(ficha);

                TempData["Mensaje"] = "Ficha actualizada correctamente";
                return RedirectToAction("List"); 
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar: " + ex.Message;
                return View(ficha);
            }
        }


    }
}