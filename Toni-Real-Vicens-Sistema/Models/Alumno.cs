using System.ComponentModel.DataAnnotations;

namespace Toni_Real_Vicens_Sistema.Models
{
    public class Alumno
    {
        public string? Id { get; set; }

        [Required]
        public string? Estado { get; set; } // Estudiante, Retirado, Egresado

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

        // --- CAMPOS ACADÉMICOS ACTUALES (Para compatibilidad con tus vistas actuales) ---
        [Required]
        public string Nivel { get; set; }

        [Required]
        public string Grado { get; set; }

        public string? Seccion { get; set; }

        // --- EL CAMPO CLAVE PARA LA PROMOCIÓN MASIVA ---
        // Este objeto guardará la "foto" del año escolar actual.
        public Matricula? MatriculaActual { get; set; }

        // --- DATOS DE CONTACTO Y UBICACIÓN ---
        public string? Direccion { get; set; }
        public string? Region { get; set; }
        public string? LugarNacimiento { get; set; }
        public string? Institucion { get; set; }
        public string? TelefonoAlumno { get; set; }
        public string? TelefonoEmergencia { get; set; }

        // --- SALUD Y DISCAPACIDAD ---
        public string? Discapacidad { get; set; } // Sí / No
        public string? TipoDiscapacidad { get; set; }
        public string? OtraDiscapacidad { get; set; }
        public string? CondicionDiscapacidad { get; set; }
        public bool TieneConadis { get; set; }

        // --- FAMILIARES ---
        public string? MadreNombre { get; set; }
        public string? MadreDNI { get; set; }
        public string? PadreNombre { get; set; }
        public string? PadreDNI { get; set; }
        public string? ApoderadoNombre { get; set; }
        public string? ApoderadoCelular { get; set; }
        public string? Tutor { get; set; }
        public string? TutorTelefono { get; set; }
        public string? NacimientoRegistrado { get; set; }

        // --- AUDITORÍA (Indispensable para soporte técnico) ---
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public string? UsuarioRegistro { get; set; } 


        public int TotalFichasDiagnosticas { get; set; } = 0;
    }
}