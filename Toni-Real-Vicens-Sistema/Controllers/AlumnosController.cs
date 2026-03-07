using Microsoft.AspNetCore.Mvc;
using Toni_Real_Vicens_Sistema.Models;
using Toni_Real_Vicens_Sistema.Service;

namespace Toni_Real_Vicens_Sistema.Controllers
{
    public class AlumnosController : Controller
    {
        private readonly AlumnoService _service;
        private readonly FichaService _fichaService;

        
        public AlumnosController(IConfiguration config)
        {
            _service = new AlumnoService(config);
            _fichaService = new FichaService(config); 
        }

        public async Task<IActionResult> Index()
        {
            var alumnos = await _service.GetAllAsync();

            
            foreach (var alumno in alumnos)
            {
                var fichas = await _fichaService.GetByAlumnoAsync(alumno.Id);
                alumno.TotalFichasDiagnosticas = fichas.Count;
            }

            return View(alumnos);
        }

        public async Task<IActionResult> Detalle(string id)
        {
            var alumno = await _service.GetByIdAsync(id);
            if (alumno == null) return NotFound();

            
            ViewBag.FichasDiagnosticas = await _fichaService.GetByAlumnoAsync(id);

            return View(alumno);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Alumno alumno)
        {
            // Limpiamos validaciones automáticas que podrían bloquear el registro
            ModelState.Clear();

            if (string.IsNullOrEmpty(alumno.DNI) || string.IsNullOrEmpty(alumno.Nombres))
            {
                TempData["Error"] = "DNI y Nombres son campos obligatorios.";
                return View(alumno);
            }

            try
            {
                var existente = await _service.GetByDniAsync(alumno.DNI);
                if (existente != null)
                {
                    TempData["Error"] = "Error: Ya existe un alumno registrado con ese DNI.";
                    return View(alumno);
                }

                // Inicializamos la matrícula del 2026
                alumno.MatriculaActual = new Matricula
                {
                    Anio = 2026,
                    Grado = alumno.Grado,
                    Nivel = alumno.Nivel,
                    Seccion = alumno.Seccion ?? "A",
                    EstadoMatricula = "Regular",
                    FechaInscripcion = DateTime.Now
                };

                await _service.AddAsync(alumno);

                TempData["SuccessMessage"] = $"¡Registro Exitoso! El alumno {alumno.Nombres} ha sido guardado.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error crítico al registrar: " + ex.Message;
                return View(alumno);
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            var alumno = await _service.GetByIdAsync(id);
            if (alumno == null) return NotFound();
            return View(alumno);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, Alumno alumno)
        {
            ModelState.Clear();

            try
            {
                var alumnoDb = await _service.GetByIdAsync(id);
                if (alumnoDb == null) return NotFound();

                // Mantenemos la estructura de matrícula actualizada
                alumno.MatriculaActual = new Matricula
                {
                    Anio = 2026,
                    Grado = alumno.Grado,
                    Nivel = alumno.Nivel,
                    Seccion = alumno.Seccion ?? "A",
                    EstadoMatricula = alumno.Estado ?? "Regular",
                    FechaInscripcion = alumnoDb.MatriculaActual?.FechaInscripcion ?? DateTime.Now
                };

                await _service.UpdateAsync(id, alumno);
                TempData["SuccessMessage"] = "Datos actualizados correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al editar datos: " + ex.Message;
                return View(alumno);
            }
        }

        [HttpPost]
        public async Task<JsonResult> CambiarEstado(string id, string estado)
        {
            var alumno = await _service.GetByIdAsync(id);
            if (alumno != null)
            {
                alumno.Estado = estado;
                if (alumno.MatriculaActual != null)
                {
                    // CAMBIO: Usa EstadoMatricula en lugar de EstadoAlumno
                    alumno.MatriculaActual.EstadoMatricula = estado;
                }
                await _service.UpdateAsync(id, alumno);
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }




        public async Task<IActionResult> GetFiltradosParaPromocion(string nivel, string grado)
        {
            
            var alumnos = await _service.GetAllAsync();

            var filtrados = alumnos.Where(a =>
                (string.IsNullOrEmpty(nivel) || a.Nivel == nivel) &&
                (string.IsNullOrEmpty(grado) || a.Grado == grado)
            ).ToList();

            
            return PartialView("_AlumnosRows", filtrados);
        }

        
        [HttpPost]
        public async Task<JsonResult> ProcesarPromocionMasiva(List<string> alumnosIds, string nuevoGrado, string nuevoNivel, int nuevoAnio)
        {
            try
            {
                foreach (var id in alumnosIds)
                {
                    // Pasamos también el nuevoNivel al servicio
                    await _service.PromoverAlumnoAsync(id, nuevoGrado, nuevoNivel, nuevoAnio);
                }
                return Json(new { success = true, message = "Promoción procesada correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        // ASEGÚRATE DE QUE SE LLAME "id" EN MINÚSCULAS
        // Cambia tu método Details por este:
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Cambiamos _alumnoService por _service que es como lo declaraste arriba
            var alumno = await _service.GetByIdAsync(id);

            if (alumno == null)
            {
                return NotFound();
            }

            // IMPORTANTE: Si tu vista de detalles necesita ver las fichas, 
            // agrega esta línea que ya usabas en el otro método:
            ViewBag.FichasDiagnosticas = await _fichaService.GetByAlumnoAsync(id);

            return View(alumno);
        }




    }
}