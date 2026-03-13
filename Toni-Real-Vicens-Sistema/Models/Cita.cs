using System.ComponentModel.DataAnnotations;

namespace Toni_Real_Vicens_Sistema.Models
{
    public class Cita
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "El alumno es obligatorio")]
        public string AlumnoId { get; set; }

        [Required(ErrorMessage = "La fecha y hora son obligatorias")]
        public DateTime? FechaHora { get; set; }

        [Required(ErrorMessage = "El tipo de cita es obligatorio")]
        public string Tipo { get; set; } 

        [Required(ErrorMessage = "El psicólogo es obligatorio")]
        public string Psicologo { get; set; }

        public string Estado { get; set; } = "Programada";

        
        public DateTime? FechaOriginal { get; set; }
        public string? MotivoReprogramacion { get; set; }

        
        public bool FueReprogramada => FechaOriginal.HasValue && FechaOriginal != FechaHora;
    }
}