using Microsoft.AspNetCore.Mvc;
using Toni_Real_Vicens_Sistema.Models;
using Toni_Real_Vicens_Sistema.Service;

namespace Toni_Real_Vicens_Sistema.Controllers
{
    public class MatriculasController : Controller
    {
        private readonly AlumnoService _alumnoService;

        public MatriculasController(IConfiguration config)
        {
            _alumnoService = new AlumnoService(config);
        }

        public IActionResult Promocion()
        {
            return View(new List<Alumno>());
        }


        
        [HttpGet]
        public async Task<IActionResult> BuscarAlumnos(string nivel, string grado, string seccion)
        {
            
            var todos = await _alumnoService.GetAllAsync();

            
            var filtrados = todos.Where(a =>
                (string.IsNullOrEmpty(nivel) || a.Nivel == nivel) &&
                (string.IsNullOrEmpty(grado) || a.Grado == grado) &&
                (string.IsNullOrEmpty(seccion) || a.Seccion == seccion)
            ).ToList();

            
            return PartialView("_TablaPromocionPartial", filtrados);
        }

        [HttpPost]
        public async Task<IActionResult> ProcesarPromocion(List<string> alumnosIds, string nuevoGrado, string nuevoNivel, int nuevoAnio)
        {
            try
            {
                if (alumnosIds == null || !alumnosIds.Any())
                    return Json(new { success = false, message = "No se seleccionaron alumnos." });

                foreach (var id in alumnosIds)
                {
                    // Ahora pasamos 4 argumentos al servicio
                    await _alumnoService.PromoverAlumnoAsync(id, nuevoGrado, nuevoNivel, nuevoAnio);
                }

                return Json(new { success = true, message = $"{alumnosIds.Count} alumnos procesados con éxito." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }





    }
}