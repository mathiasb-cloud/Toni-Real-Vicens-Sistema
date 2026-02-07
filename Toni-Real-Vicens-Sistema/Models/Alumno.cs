using System.ComponentModel.DataAnnotations;

namespace Toni_Real_Vicens_Sistema.Models
{
    public class Alumno
    {
        public string Id { get; set; }

        [Required]
        public string Nombres { get; set; }

        [Required]
        public string Apellidos { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 8)]
        public string DNI { get; set; }

        [Required]
        public string Sexo { get; set; }

        [Required]
        public DateTime FechaNacimiento { get; set; }

        public string Direccion { get; set; }
        public string Region { get; set; }

        [Required]
        public string Nivel { get; set; }

        [Required]
        public string Grado { get; set; }

        public string Institucion { get; set; }

        public string CondicionDiscapacidad { get; set; }
        public bool TieneConadis { get; set; }

        public string MadreNombre { get; set; }
        public string MadreDNI { get; set; }

        public string PadreNombre { get; set; }
        public string PadreDNI { get; set; }

        public string ApoderadoNombre { get; set; }
        public string ApoderadoCelular { get; set; }

        public string Seccion { get; set; }

        public string LugarNacimiento { get; set; }

        public string TelefonoAlumno { get; set; }

        public string TelefonoEmergencia { get; set; }

        public string Tutor { get; set; }

        public string TutorTelefono { get; set; }
    }
}