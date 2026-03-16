using Microsoft.AspNetCore.Mvc;
using Toni_Real_Vicens_Sistema.Models;
using Toni_Real_Vicens_Sistema.Service;
using Microsoft.Extensions.Caching.Memory;


namespace Toni_Real_Vicens_Sistema.Controllers
{
    public class CitasController : Controller
    {
        private readonly CitaService _citaService;
        private readonly AlumnoService _alumnoService;
        private readonly FichaService _fichaService; 
        private readonly SeguimientoService _seguimientoService;
        private readonly IMemoryCache _cache;
        

        public CitasController(IConfiguration config, IMemoryCache cache)
        {
            _citaService = new CitaService(config);
            _alumnoService = new AlumnoService(config);
            _fichaService = new FichaService(config);
            _seguimientoService = new SeguimientoService(config);
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            var citas = await _citaService.GetAllAsync();
            var alumnos = await _alumnoService.GetAllAsync();
            var ahora = DateTime.Now;
            int minutosGracia = 15; 

            var todosLosSeguimientos = await _seguimientoService.GetAllAsync();

            ViewBag.CitasConSeguimiento = todosLosSeguimientos
                .Where(s => !string.IsNullOrEmpty(s.CitaId))
                .Select(s => s.CitaId)
                .ToHashSet();

            ViewBag.NombresAlumnos = alumnos.ToDictionary(a => a.Id, a => a.Nombres + " " + a.Apellidos);

            foreach (var cita in citas)
            {
                
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

                
                if (cita.Estado == "Programada" && cita.FechaHora.HasValue)
                {
                    
                    if (ahora > cita.FechaHora.Value.AddMinutes(minutosGracia))
                    {
                        cita.Estado = "Ausente";
                        
                        await _citaService.UpdateEstadoAsync(cita.Id, "Ausente");
                    }
                }
            }

            return View(citas.OrderByDescending(c => c.FechaHora).ToList());
        }


        [HttpGet]
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

            
            var citasExistentes = await _citaService.GetAllAsync();

            
            bool yaExisteCita = citasExistentes.Any(c =>
                c.AlumnoId == cita.AlumnoId &&
                c.FechaHora.HasValue && 
                cita.FechaHora.HasValue && 
                c.FechaHora.Value.Date == cita.FechaHora.Value.Date);

            if (yaExisteCita)
            {
                ModelState.AddModelError("FechaHora", "El alumno ya tiene una cita programada para este día.");
                TempData["Error"] = "Ya existe una cita para este alumno en la fecha seleccionada.";

                ViewBag.Alumnos = await _alumnoService.GetAllAsync();
                return View(cita);
            }

            
            cita.Estado = "Programada";
            string citaId = await _citaService.AddAsync(cita);

            _cache.Remove($"bloqueo_alumno_{cita.AlumnoId}");

            if (accion == "continuar")
            {
                await _citaService.UpdateEstadoAsync(citaId, "En Proceso");
                return (cita.Tipo == "Evaluación")
                    ? RedirectToAction("Create", "Fichas", new { citaId = citaId })
                    : RedirectToAction("Create", "Seguimiento", new { citaId = citaId });
            }

            TempData["Mensaje"] = "Cita programada con éxito";
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





        [HttpPost]
        public async Task<IActionResult> Reprogramar(string citaId, DateTime nuevaFecha, string motivo)
        {
            try
            {
                var cita = await _citaService.GetByIdAsync(citaId);
                if (cita == null) return Json(new { success = false });

                
                if (!cita.FechaOriginal.HasValue)
                {
                    cita.FechaOriginal = cita.FechaHora;
                }

                cita.FechaHora = nuevaFecha;
                cita.MotivoReprogramacion = motivo;

                await _citaService.UpdateAsync(cita); 

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        [HttpPost]
        public IActionResult BloquearAlumno(string alumnoId)
        {
            if (string.IsNullOrEmpty(alumnoId)) return Json(new { success = true });

            string usuarioActual = HttpContext.Session.GetString("UsuarioNombre") ?? "Otro Psicólogo";
            string key = $"bloqueo_alumno_{alumnoId}";

            
            if (_cache.TryGetValue(key, out string usuarioBloqueador))
            {
                if (usuarioBloqueador != usuarioActual)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Este alumno está siendo atendido actualmente por: {usuarioBloqueador}. Por favor, elige a otro alumno o espera a que termine."
                    });
                }
            }

            
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            _cache.Set(key, usuarioActual, cacheEntryOptions);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult DesbloquearAlumno(string alumnoId)
        {
            if (!string.IsNullOrEmpty(alumnoId))
            {
                _cache.Remove($"bloqueo_alumno_{alumnoId}");
            }
            return Json(new { success = true });
        }


        public async Task<IActionResult> Agenda()
        {
            var todasLasCitas = await _citaService.GetAllAsync();

            
            var citasFiltradas = todasLasCitas
                .Where(c => c.Estado == "Programada" || c.Estado == "Reprogramada")
                .ToList();

            return View(citasFiltradas);
        }


    }
}