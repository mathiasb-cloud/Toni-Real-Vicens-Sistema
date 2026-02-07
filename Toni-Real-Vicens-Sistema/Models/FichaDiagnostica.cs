using System.ComponentModel.DataAnnotations;

namespace Toni_Real_Vicens_Sistema.Models
{
    public class FichaDiagnostica
    {
        public string Id { get; set; }
        public string AlumnoId { get; set; }
        public string CitaId { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        // II. Aspecto Familiar
        public string ConQuienVive { get; set; }
        public string RelacionFamiliar { get; set; }
        public string ProblemasHogar { get; set; }
        public string ComunicacionHogar { get; set; }
        public string ApoyoTareas { get; set; }

        // III. Aspecto Laboral
        public bool PapaTrabaja { get; set; }
        public string TrabajoPapa { get; set; }
        public bool MamaTrabaja { get; set; }
        public string TrabajoMama { get; set; }
        public string IngresoMensual { get; set; }

        // IV. Vivienda
        public string TipoVivienda { get; set; }
        public string MaterialVivienda { get; set; }
        public string PisoVivienda { get; set; }
        public string Servicios { get; set; }

        // V. Salud
        public string DificultadFisica { get; set; }
        public string DificultadMental { get; set; }
        public bool TieneSeguro { get; set; }
        public string Enfermedad { get; set; }

        // VI. Alimentación
        public string AlimentacionDiaria { get; set; }

        // VII. Académico
        public bool RepitioAnio { get; set; }
        public string CursosFaciles { get; set; }
        public string CursosDificiles { get; set; }

        // VIII. Emocional
        public bool EsTimido { get; set; }
        public bool SeSienteTriste { get; set; }

        // IX. Social
        public bool PrefiereAmigosQueColegio { get; set; }

        // X. Recursos personales
        public string Motivacion { get; set; }
        public string PorqueAsiste { get; set; }
        public string ActividadesDisfruta { get; set; }
        public string Habilidades { get; set; }
    }
}