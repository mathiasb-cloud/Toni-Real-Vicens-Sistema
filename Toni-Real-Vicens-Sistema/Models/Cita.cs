namespace Toni_Real_Vicens_Sistema.Models
{
    public class Cita
    {
        public string Id { get; set; }
        public string AlumnoId { get; set; }

        public DateTime Fecha { get; set; }
        public string Tipo { get; set; }
        public string Psicologo { get; set; }
        public string Estado { get; set; }
    }

}
