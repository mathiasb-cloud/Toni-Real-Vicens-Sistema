using System.ComponentModel.DataAnnotations;

namespace Toni_Real_Vicens_Sistema.Models
{
    public class Alumno
    {
        public string? Id { get; set; } // Opcional porque Firebase lo genera

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener 8 dígitos")]
        public string DNI { get; set; }

        [Required]
        public string Sexo { get; set; }

        [Required]
        public DateTime? FechaNacimiento { get; set; }

        // CAMPOS QUE DEBEN SER OPCIONALES (Añadido ?)
        public string? Direccion { get; set; }
        public string? Region { get; set; }
        public string? LugarNacimiento { get; set; }
        public string? Institucion { get; set; }
        public string? CondicionDiscapacidad { get; set; }
        public bool TieneConadis { get; set; }

        // Datos de familiares (Opcionales por ahora)
        public string? MadreNombre { get; set; }
        public string? MadreDNI { get; set; }
        public string? PadreNombre { get; set; }
        public string? PadreDNI { get; set; }
        public string? ApoderadoNombre { get; set; }
        public string? ApoderadoCelular { get; set; }

        [Required]
        public string Nivel { get; set; }

        [Required]
        public string Grado { get; set; }

        public string? Seccion { get; set; }
        public string? TelefonoAlumno { get; set; }
        public string? TelefonoEmergencia { get; set; }

        // Tutor y nuevos campos
        public string? Tutor { get; set; }
        public string? TutorTelefono { get; set; }
        public string? NacimientoRegistrado { get; set; }
        public string? Discapacidad { get; set; }
        public string? TipoDiscapacidad { get; set; }
        public string? OtraDiscapacidad { get; set; }
    }
}