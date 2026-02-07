using System.ComponentModel.DataAnnotations;

namespace Toni_Real_Vicens_Sistema.Models
{
    public class Cita
    {
        public string? Id { get; set; }

        [Required]
        public string AlumnoId { get; set; }

        [Required]
        public DateTime? FechaHora { get; set; }

        [Required]
        public string Tipo { get; set; } // Evaluación / Seguimiento

        [Required]
        public string Psicologo { get; set; }

        public string Estado { get; set; } = "Programada";
    }
}