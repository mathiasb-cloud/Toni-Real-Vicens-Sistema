using System.ComponentModel.DataAnnotations;

namespace Toni_Real_Vicens_Sistema.Models
{
    public class FichaDiagnostica
    {
        public string Id { get; set; }
        public string AlumnoId { get; set; }
        public string CitaId { get; set; }
        public DateTime Fecha { get; set; }

        // --- II. ASPECTO FAMILIAR (Psicólogo) ---

        // 2.1 Red familiar (Tabla)
        public List<RedFamiliar> RedFamiliar { get; set; } = new();

        // 2.2 ¿Con quién vive el estudiante?
        public bool ViveAmbosPadres { get; set; }
        public bool ViveSoloMadre { get; set; }
        public bool ViveSoloPadre { get; set; }
        public bool ViveSolo { get; set; }
        public bool ViveHermanos { get; set; }
        public int? NumeroHermanos { get; set; }
        public bool ViveOtros { get; set; }
        public string? DetalleOtrosVive { get; set; }

        // 2.3 Relación con familiares
        public string? RelacionFamiliar { get; set; } 

        // 2.4 Problemas en el hogar
        public bool ProblemaViolencia { get; set; }
        public bool ProblemaComunicacion { get; set; }
        public bool ProblemaEconomico { get; set; }
        public bool ProblemaSalud { get; set; }
        public string? ProblemaOtros { get; set; }

        // 2.5 Influencia en la comunicación (Dificultades)
        public bool InfluenciaFaltaTiempo { get; set; }
        public bool InfluenciaDesinteres { get; set; }
        public bool InfluenciaFaltaComprension { get; set; }
        public bool InfluenciaTemor { get; set; }
        public bool InfluenciaNinguna { get; set; }

        // 2.6 Apoyo en tareas escolares
        public bool ApoyoMama { get; set; }
        public bool ApoyoPapa { get; set; }
        public bool ApoyoHermanos { get; set; }
        public bool ApoyoTios { get; set; }
        public bool ApoyoAbuelos { get; set; }
        public bool ApoyoSolo { get; set; }






    }
}