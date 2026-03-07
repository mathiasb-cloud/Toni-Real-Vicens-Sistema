namespace Toni_Real_Vicens_Sistema.Models
{
    public class Matricula
    {
        public int Anio { get; set; }
        public string Grado { get; set; }
        public string Nivel { get; set; }
        public string Seccion { get; set; }
        public string EstadoMatricula { get; set; } = "Promovido";
        public DateTime FechaInscripcion { get; set; } = DateTime.Now;
    }
}