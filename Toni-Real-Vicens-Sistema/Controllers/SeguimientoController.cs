using Microsoft.AspNetCore.Mvc;
using Toni_Real_Vicens_Sistema.Models;
using Toni_Real_Vicens_Sistema.Service;

namespace Toni_Real_Vicens_Sistema.Controllers
{
    public class SeguimientoController : Controller
    {
        private readonly SeguimientoService _seguimientoService; // Nombre unificado
        private readonly AlumnoService _alumnoService;
        private readonly CitaService _citaService;

        public SeguimientoController(IConfiguration config)
        {
            _seguimientoService = new SeguimientoService(config);
            _alumnoService = new AlumnoService(config);
            _citaService = new CitaService(config);
        }

        public async Task<IActionResult> Create(string citaId)
        {
            if (string.IsNullOrEmpty(citaId)) return RedirectToAction("Index", "Citas");

            var cita = await _citaService.GetByIdAsync(citaId);
            if (cita == null) return NotFound();

            var alumno = await _alumnoService.GetByIdAsync(cita.AlumnoId);
            if (alumno == null) return NotFound();

            ViewBag.AlumnoNombre = $"{alumno.Nombres} {alumno.Apellidos}";

            var seguimientos = await _seguimientoService.GetByAlumnoAsync(cita.AlumnoId);
            var existente = seguimientos.FirstOrDefault(s => s.CitaId == citaId);

            if (existente != null) return View(existente);

            return View(new FichaSeguimiento
            {
                AlumnoId = alumno.Id,
                CitaId = citaId,
                Psicologo = cita.Psicologo,
                Fecha = DateTime.Now,
                Motivo = "Seguimiento Psicológico"
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(FichaSeguimiento ficha) // <-- AQUÍ estaba el error, antes decía FichaDiagnostica
        {
            try
            {
                if (string.IsNullOrEmpty(ficha.Psicologo))
                    ficha.Psicologo = HttpContext.Session.GetString("UsuarioNombre") ?? "Sistema";

                // Guardar o Actualizar usando el servicio de SEGUIMIENTO
                if (string.IsNullOrEmpty(ficha.Id))
                {
                    ficha.Fecha = DateTime.Now;
                    // El servicio debe recibir FichaSeguimiento
                    ficha.Id = await _seguimientoService.AddAsync(ficha);
                }
                else
                {
                    await _seguimientoService.UpdateAsync(ficha.Id, ficha);
                }

                // Manejo de estados (Recuerda que en Seguimiento es IsFinalizada con 'I')
                if (ficha.IsFinalizada)
                {
                    await _citaService.UpdateEstadoAsync(ficha.CitaId, "Atendida");
                    TempData["Mensaje"] = "actualizado";
                    return RedirectToAction("Detalle", "Fichas", new { id = ficha.AlumnoId });
                }
                else
                {
                    await _citaService.UpdateEstadoAsync(ficha.CitaId, "En Proceso");

                    var alumno = await _alumnoService.GetByIdAsync(ficha.AlumnoId);
                    ViewBag.AlumnoNombre = alumno != null ? $"{alumno.Nombres} {alumno.Apellidos}" : "Alumno";

                    TempData["Mensaje"] = "actualizado";
                    return View(ficha);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(ficha);
            }
        }






    }
}