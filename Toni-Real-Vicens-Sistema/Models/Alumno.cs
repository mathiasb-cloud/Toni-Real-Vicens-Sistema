namespace Toni_Real_Vicens_Sistema.Models
{
    public class Alumno
    {
        public string Id { get; set; }

        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string DNI { get; set; }
        public DateTime FechaNacimiento { get; set; }

        public string Direccion { get; set; }
        public string Region { get; set; }

        public string Grado { get; set; }
        public string Nivel { get; set; }
        public string Institucion { get; set; }

        public string CondicionDiscapacidad { get; set; }
        public bool TieneConadis { get; set; }

        public string MadreNombre { get; set; }
        public string MadreDNI { get; set; }

        public string PadreNombre { get; set; }
        public string PadreDNI { get; set; }

        public string ApoderadoNombre { get; set; }
        public string ApoderadoCelular { get; set; }
    }

}
