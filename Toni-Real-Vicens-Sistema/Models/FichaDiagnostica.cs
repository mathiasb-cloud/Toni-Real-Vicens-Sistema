using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace Toni_Real_Vicens_Sistema.Models
{
    public class FichaDiagnostica
    {
        public string Id { get; set; } = string.Empty;
        public string AlumnoId { get; set; } = string.Empty;
        public string CitaId { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
        public bool EsFinalizada { get; set; } = false;


        
        public int AnioAcademico { get; set; } 
        public string GradoAlMomento { get; set; } 
        public string SeccionAlMomento { get; set; } 


        // --- II. ASPECTO FAMILIAR ---

        // 2.1 Red familiar (Tabla dinámica)
        public List<RedFamiliar> RedFamiliar { get; set; } = new();

        // 2.2 ¿Con quién vive el estudiante? 
        // (Sugerencia: Usar Checkboxes en la vista para estos bool)
        public bool ViveAmbosPadres { get; set; }
        public bool ViveSoloMadre { get; set; }
        public bool ViveSoloPadre { get; set; }
        public bool ViveSolo { get; set; }
        public bool ViveHermanos { get; set; }
        public int? NumeroHermanos { get; set; }
        public bool ViveOtros { get; set; }
        public string? DetalleOtrosVive { get; set; }

        // 2.3 Relación con familiares (Ideal para Radio Buttons)
        public string? RelacionFamiliar { get; set; }

        // 2.4 Problemas en el hogar
        public bool ProblemaViolencia { get; set; }
        public bool ProblemaComunicacion { get; set; }
        public bool ProblemaEconomico { get; set; }
        public bool ProblemaSalud { get; set; }
        public string? ProblemaOtros { get; set; }

        // 2.5 Influencia en la comunicación
        public bool InfluenciaFaltaTiempo { get; set; }
        public bool InfluenciaDesinteres { get; set; }
        public bool InfluenciaFaltaComprension { get; set; }
        public bool InfluenciaTemor { get; set; }
        public bool InfluenciaNinguna { get; set; }

        // 2.6 Apoyo en tareas
        public bool ApoyoMama { get; set; }
        public bool ApoyoPapa { get; set; }
        public bool ApoyoHermanos { get; set; }
        public bool ApoyoTios { get; set; }
        public bool ApoyoAbuelos { get; set; }
        public bool ApoyoSolo { get; set; }

        // --- III. ASPECTO LABORAL Y ECONÓMICO ---

        public bool PadreTrabaja { get; set; }
        public string? PadreDondeTrabaja { get; set; }
        public string? PadreQueTrabaja { get; set; }

        public bool MadreTrabaja { get; set; }
        public string? MadreDondeTrabaja { get; set; }
        public string? MadreQueTrabaja { get; set; }

        public string? IngresoFamiliarMensual { get; set; }

        public bool GastoAlimentos { get; set; }
        public bool GastoEducacion { get; set; }
        public bool GastoRopa { get; set; }
        public bool GastoArtefactos { get; set; }
        public bool GastoServicios { get; set; }
        public string? GastoOtros { get; set; }

        public bool EstudianteTrabaja { get; set; }
        public string? EstudianteDondeTrabaja { get; set; }
        public string? EstudianteQueTrabaja { get; set; }

        public bool DestinoDoyPadres { get; set; }
        public bool DestinoEducacion { get; set; }
        public bool DestinoRopa { get; set; }
        public bool DestinoArtefactos { get; set; }
        public bool DestinoInternet { get; set; }
        public string? DestinoOtros { get; set; }

        // --- IV. ASPECTOS DE VIVIENDA ---
        public string? TipoVivienda { get; set; }
        public string? MaterialVivienda { get; set; }
        public string? TipoPiso { get; set; }

        // Cantidades como Nullable para evitar ceros automáticos
        public int? CantCocina { get; set; }
        public int? CantDormitorios { get; set; }
        public int? CantComedor { get; set; }
        public int? CantSala { get; set; }
        public int? CantBano { get; set; }

        public bool ServicioAgua { get; set; }
        public bool ServicioLuz { get; set; }
        public bool ServicioDesague { get; set; }
        public bool ServicioCable { get; set; }
        public bool ServicioInternet { get; set; }
        public bool ServicioPlataformas { get; set; }

        public bool TieneTelevisor { get; set; }
        public bool TieneComputadora { get; set; }
        public bool TieneEquipoSonido { get; set; }
        public bool TieneCocinaEquip { get; set; }
        public bool TieneLavadora { get; set; }
        public bool TieneRefrigeradora { get; set; }

        public string? MedioTraslado { get; set; }

        // --- V. ASPECTOS DE SALUD ---
        public bool SaludDificultadVisual { get; set; }
        public bool SaludDificultadAuditiva { get; set; }
        public bool SaludDificultadMotriz { get; set; }
        public bool SaludDificultadOral { get; set; }

        public bool SaludProblemaAprendizaje { get; set; }
        public bool SaludProblemaNeurodesarrollo { get; set; }
        public bool SaludProblemaConducta { get; set; }
        public bool SaludProblemaAfectivo { get; set; }

        public bool TieneSeguroSalud { get; set; }
        public string? TipoSeguro { get; set; }

        public bool SufreEnfermedad { get; set; }
        public string? EspecificarEnfermedad { get; set; }

        public bool RecibeTratamiento { get; set; }
        public string? EspecificarTratamiento { get; set; }

        // --- VI. ASPECTOS DE ALIMENTACIÓN ---
        public bool AlimentoDiaDesayuno { get; set; }
        public bool AlimentoDiaRefrigerio { get; set; }
        public bool AlimentoDiaAlmuerzo { get; set; }
        public bool AlimentoDiaCena { get; set; }

        public bool AlimentoCasaDesayuno { get; set; }
        public bool AlimentoCasaRefrigerio { get; set; }
        public bool AlimentoCasaAlmuerzo { get; set; }
        public bool AlimentoCasaCena { get; set; }

        public bool LlevaDineroRefrigerio { get; set; }

        public bool CompraGalletas { get; set; }
        public bool CompraManzana { get; set; }
        public bool CompraPan { get; set; }
        public bool CompraMarciano { get; set; }
        public bool CompraEmpanadas { get; set; }
        public string? CompraOtros { get; set; }

        public bool ConsumeMasArroz { get; set; }
        public bool ConsumeMasMenestra { get; set; }
        public bool ConsumeMasAvena { get; set; }
        public bool ConsumeMasCarne { get; set; }
        public string? ConsumeMasOtros { get; set; }

        // --- VII. ASPECTO ACADÉMICO ---
        public bool RepitioAnio { get; set; }
        public string? CualAnioRepitio { get; set; }
        public string? MotivoRepitencia { get; set; }

        public bool TieneHorarioEstudio { get; set; }
        public int? HorasTareas { get; set; }
        public int? HorasLectura { get; set; }
        public int? HorasTV { get; set; }
        public int? HorasJuego { get; set; }

        public string? FuenteInformacion { get; set; }

        public bool EspacioAdecuadoCasa { get; set; }
        public bool FacilIniciarTareas { get; set; }
        public bool NoSeLevantaHastaTerminar { get; set; }

        public bool FacilComprenderDocentes { get; set; }
        public bool AulaAdecuada { get; set; }
        public bool FacilExpresarOpiniones { get; set; }

        public bool ComprendeLoQueLee { get; set; }
        public bool FacilRetenerInformacion { get; set; }
        public bool LograConcentrarse { get; set; }
        public string? CursosFaciles { get; set; }
        public string? CursosDificiles { get; set; }

        // --- VIII. ASPECTO EMOCIONAL ---

        public string ObservacionEmocional { get; set; }
        public bool EmoTimidez { get; set; }
        public bool EmoNerviosismo { get; set; }
        public bool EmoCumpleNormas { get; set; }
        public bool EmoHablaCompaneros { get; set; }
        public bool EmoTieneTemor { get; set; }
        public bool EmoProblemaCompanero { get; set; }
        public bool EmoAgredidoAula { get; set; }
        public bool EmoConsumeSustancias { get; set; }
        public bool EmoControlaImpulsos { get; set; }
        public bool EmoSienteTristeza { get; set; }
        public bool EmoInfluyeFamilia { get; set; }
        public bool EmoPensamientoSuicida { get; set; }

        // --- IX. ASPECTO SOCIAL ---
        public bool SocPrefiereAmigos { get; set; }
        public bool SocAmigosMayores { get; set; }
        public bool SocAmigosMenores { get; set; }
        public bool SocAmigosConsumen { get; set; }
        public bool SocAsisteEventos { get; set; }
        public bool SocPresionAmigos { get; set; }
        public bool SocAsisteIglesia { get; set; }
        public bool SocAsisteTalleres { get; set; }

        // --- X. RECURSOS PERSONALES ---
        public string? RecPersMotivacion { get; set; }
        public string? RecPersPorqueColegio { get; set; }
        public string? RecPersActividadesDisfruta { get; set; }
        public string? RecPersCualidadesHabilidades { get; set; }
    }
}