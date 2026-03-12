using System;
using System.Collections.Generic;

namespace Toni_Real_Vicens_Sistema.Models
{
    public class FichaSeguimiento
    {
        public string Id { get; set; }
        public string AlumnoId { get; set; }
        public string CitaId { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        // --- SECCIÓN: CONSULTA / RESPONSABLE ---
        public string FormasIngreso { get; set; } // Formas de Ingreso
        public string Motivo { get; set; }        // Motivo
        public string Psicologo { get; set; }     // Psicólogo responsable
        public string DxNosologico { get; set; }  // Dx Nosológico
        public string DxCodigo { get; set; }      // Dx Código
        public string DxNivel { get; set; }       // Dx Nivel

        // --- SECCIÓN: TRATAMIENTO ---
        public DateTime? Psicoeducacion1 { get; set; }
        public DateTime? Psicoeducacion2 { get; set; }
        public DateTime? Psicoeducacion3 { get; set; }

        // Evaluaciones (1-5)
        public DateTime? Evaluacion1 { get; set; }
        public DateTime? Evaluacion2 { get; set; }
        public DateTime? Evaluacion3 { get; set; }
        public DateTime? Evaluacion4 { get; set; }
        public DateTime? Evaluacion5 { get; set; }

        // Terapia Individual (1-8)
        public DateTime? TerapiaInd1 { get; set; }
        public DateTime? TerapiaInd2 { get; set; }
        public DateTime? TerapiaInd3 { get; set; }
        public DateTime? TerapiaInd4 { get; set; }
        public DateTime? TerapiaInd5 { get; set; }
        public DateTime? TerapiaInd6 { get; set; }
        public DateTime? TerapiaInd7 { get; set; }
        public DateTime? TerapiaInd8 { get; set; }

        // Terapia Familiar (1-5)
        public DateTime? TerapiaFam1 { get; set; }
        public DateTime? TerapiaFam2 { get; set; }
        public DateTime? TerapiaFam3 { get; set; }
        public DateTime? TerapiaFam4 { get; set; }
        public DateTime? TerapiaFam5 { get; set; }

        // Visita Domiciliaria (1-3)
        public DateTime? VisitaDom1 { get; set; }
        public DateTime? VisitaDom2 { get; set; }
        public DateTime? VisitaDom3 { get; set; }

        // --- SECCIÓN: INTERCONSULTA ---
        public string InstitucionDerivacion { get; set; }
        public DateTime? FechaDerivacion { get; set; }
        public string MotivoDerivacion { get; set; }

        public bool IsFinalizada { get; set; }
    }
}