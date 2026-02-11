using System.ComponentModel.DataAnnotations;

namespace Toni_Real_Vicens_Sistema.Models
{
    public class FichaSeguimiento
    {
        public string? Id { get; set; }

        [Required]
        public string AlumnoId { get; set; }

        [Required]
        public string CitaId { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        // --- CONTENIDO DE LA SESIÓN ---

        [Required(ErrorMessage = "Debe registrar la evolución de la sesión")]
        public string Evolucion { get; set; } // ¿Qué se trabajó hoy?

        public string Observaciones { get; set; } // Notas técnicas del psicólogo

        public string AcuerdosTareas { get; set; } // Compromisos del alumno o padres

        public string Recomendaciones { get; set; } // Sugerencias para el hogar/aula

        // --- ESTADO Y PRÓXIMA ACCIÓN ---

        public string EstadoEmocional { get; set; } // Estable, Ansioso, Depresivo, etc.

        public DateTime? FechaProximaSesion { get; set; }

        public string FirmaPsicologo { get; set; }
    }
}