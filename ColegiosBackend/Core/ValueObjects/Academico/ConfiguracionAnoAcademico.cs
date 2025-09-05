using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ColegiosBackend.Core.ValueObjects.Academico;

/// <summary>
/// Configuración integral del año académico almacenada en campo JSONB
/// Incluye períodos, escalas de calificación, calendario y configuración de evaluaciones
/// </summary>
public class ConfiguracionAnoAcademico
{
    [JsonPropertyName("periodos_academicos")]
    public List<PeriodoAcademicoConfig> PeriodosAcademicos { get; set; } = new();

    [JsonPropertyName("escala_calificacion")]
    public EscalaCalificacion EscalaCalificacion { get; set; } = new();

    [JsonPropertyName("calendario_especial")]
    public CalendarioEspecial CalendarioEspecial { get; set; } = new();

    [JsonPropertyName("configuracion_evaluacion")]
    public ConfiguracionEvaluacion ConfiguracionEvaluacion { get; set; } = new();

    [JsonPropertyName("configuracion_promocion")]
    public ConfiguracionPromocion ConfiguracionPromocion { get; set; } = new();

    [JsonPropertyName("configuracion_recuperaciones")]
    public ConfiguracionRecuperaciones ConfiguracionRecuperaciones { get; set; } = new();
}

#region Períodos Académicos

/// <summary>
/// Configuración de un período académico específico
/// </summary>
public class PeriodoAcademicoConfig
{
    public int Numero { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("fecha_inicio")]
    public DateTime FechaInicio { get; set; }

    [JsonPropertyName("fecha_fin")]
    public DateTime FechaFin { get; set; }

    [JsonPropertyName("peso_porcentual")]
    [Range(0, 100)]
    public decimal PesoPorcentual { get; set; }

    [JsonPropertyName("permite_recuperacion")]
    public bool PermiteRecuperacion { get; set; } = true;

    [JsonPropertyName("fecha_limite_notas")]
    public DateTime FechaLimiteNotas { get; set; }

    [JsonPropertyName("fecha_limite_modificaciones")]
    public DateTime FechaLimiteModificaciones { get; set; }

    [JsonPropertyName("requiere_aprobacion_director")]
    public bool RequiereAprobacionDirector { get; set; } = false;

    [JsonPropertyName("activo")]
    public bool Activo { get; set; } = true;

    /// <summary>
    /// Configuración específica de evaluaciones para este período
    /// </summary>
    [JsonPropertyName("tipos_evaluacion_periodo")]
    public List<TipoEvaluacionPeriodo> TiposEvaluacionPeriodo { get; set; } = new();
}

/// <summary>
/// Configuración de tipos de evaluación específicos por período
/// </summary>
public class TipoEvaluacionPeriodo
{
    [Required]
    [MaxLength(20)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("porcentaje_minimo")]
    [Range(0, 100)]
    public decimal PorcentajeMinimo { get; set; }

    [JsonPropertyName("porcentaje_maximo")]
    [Range(0, 100)]
    public decimal PorcentajeMaximo { get; set; }

    [JsonPropertyName("es_obligatorio")]
    public bool EsObligatorio { get; set; } = true;

    [JsonPropertyName("permite_multiple")]
    public bool PermiteMultiple { get; set; } = true;

    [JsonPropertyName("requiere_observaciones")]
    public bool RequiereObservaciones { get; set; } = false;

    [JsonPropertyName("fecha_limite")]
    public DateTime? FechaLimite { get; set; }
}

#endregion

#region Escala de Calificación

/// <summary>
/// Configuración completa del sistema de calificaciones
/// </summary>
public class EscalaCalificacion
{
    [Range(0, 10)]
    public decimal Minima { get; set; } = 1.0m;

    [Range(1, 10)]
    public decimal Maxima { get; set; } = 5.0m;

    [Range(0, 3)]
    public int Decimales { get; set; } = 1;

    [JsonPropertyName("nota_aprobacion")]
    public decimal NotaAprobacion { get; set; } = 3.0m;

    [JsonPropertyName("nota_maxima_recuperacion")]
    public decimal NotaMaximaRecuperacion { get; set; } = 3.5m;

    /// <summary>
    /// Equivalencias cualitativas por rangos de notas
    /// Formato: "min-max": "descripción"
    /// </summary>
    public Dictionary<string, string> Equivalencias { get; set; } = new()
    {
        ["4.6-5.0"] = "Superior",
        ["4.0-4.5"] = "Alto",
        ["3.0-3.9"] = "Básico",
        ["1.0-2.9"] = "Bajo"
    };

    /// <summary>
    /// Configuración de redondeo de calificaciones
    /// </summary>
    [JsonPropertyName("configuracion_redondeo")]
    public ConfiguracionRedondeo ConfiguracionRedondeo { get; set; } = new();

    /// <summary>
    /// Configuración de conversiones entre diferentes escalas
    /// </summary>
    [JsonPropertyName("conversiones_escala")]
    public List<ConversionEscala> ConversionesEscala { get; set; } = new();
}

/// <summary>
/// Configuración de cómo redondear las calificaciones
/// </summary>
public class ConfiguracionRedondeo
{
    /// <summary>
    /// Tipo de redondeo: MATEMATICO, HACIA_ARRIBA, HACIA_ABAJO, TRUNCAR
    /// </summary>
    [MaxLength(20)]
    public string Tipo { get; set; } = "MATEMATICO";

    /// <summary>
    /// Número de decimales para el redondeo
    /// </summary>
    [Range(0, 3)]
    public int Decimales { get; set; } = 1;

    /// <summary>
    /// Si aplica redondeo solo al mostrar o también al almacenar
    /// </summary>
    [JsonPropertyName("solo_visualizacion")]
    public bool SoloVisualizacion { get; set; } = false;
}

/// <summary>
/// Configuración para conversión entre diferentes escalas de calificación
/// </summary>
public class ConversionEscala
{
    [Required]
    [MaxLength(50)]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("escala_origen_min")]
    public decimal EscalaOrigenMin { get; set; }

    [JsonPropertyName("escala_origen_max")]
    public decimal EscalaOrigenMax { get; set; }

    [JsonPropertyName("escala_destino_min")]
    public decimal EscalaDestinoMin { get; set; }

    [JsonPropertyName("escala_destino_max")]
    public decimal EscalaDestinoMax { get; set; }

    /// <summary>
    /// Fórmula de conversión personalizada si es necesaria
    /// </summary>
    [MaxLength(200)]
    public string Formula { get; set; } = string.Empty;

    public bool Activa { get; set; } = true;
}

#endregion

#region Calendario Especial

/// <summary>
/// Configuración del calendario académico especial (vacaciones, eventos, fechas importantes)
/// </summary>
public class CalendarioEspecial
{
    public List<VacacionConfig> Vacaciones { get; set; } = new();

    [JsonPropertyName("fechas_especiales")]
    public List<FechaEspecialConfig> FechasEspeciales { get; set; } = new();

    [JsonPropertyName("dias_no_laborales")]
    public List<DiaNoLaboral> DiasNoLaborales { get; set; } = new();

    [JsonPropertyName("configuracion_semanas")]
    public ConfiguracionSemanas ConfiguracionSemanas { get; set; } = new();
}

/// <summary>
/// Configuración de un período de vacaciones
/// </summary>
public class VacacionConfig
{
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("fecha_inicio")]
    public DateTime FechaInicio { get; set; }

    [JsonPropertyName("fecha_fin")]
    public DateTime FechaFin { get; set; }

    public string Tipo { get; set; } = "ACADEMICA"; // ACADEMICA, RELIGIOSA, NACIONAL, INSTITUCIONAL

    [JsonPropertyName("suspende_clases")]
    public bool SuspendeClases { get; set; } = true;

    [JsonPropertyName("suspende_actividades_administrativas")]
    public bool SuspendeActividadesAdministrativas { get; set; } = false;

    [MaxLength(500)]
    public string Descripcion { get; set; } = string.Empty;

    public bool Activa { get; set; } = true;
}

/// <summary>
/// Configuración de fechas especiales y eventos
/// </summary>
public class FechaEspecialConfig
{
    public DateTime Fecha { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    public string Tipo { get; set; } = "EVENTO_ACADEMICO"; // EVENTO_ACADEMICO, INSTITUCIONAL, CELEBRACION, EVALUACION

    [MaxLength(500)]
    public string Descripcion { get; set; } = string.Empty;

    [JsonPropertyName("es_fecha_limite")]
    public bool EsFechaLimite { get; set; } = false;

    [JsonPropertyName("requiere_preparacion")]
    public bool RequierePreparacion { get; set; } = false;

    [JsonPropertyName("dias_preparacion")]
    [Range(0, 365)]
    public int DiasPreparacion { get; set; } = 0;

    [JsonPropertyName("notificar_acudientes")]
    public bool NotificarAcudientes { get; set; } = false;

    public bool Activa { get; set; } = true;
}

/// <summary>
/// Configuración de días no laborales recurrentes
/// </summary>
public class DiaNoLaboral
{
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de recurrencia: SEMANAL, MENSUAL, ANUAL, FECHA_FIJA
    /// </summary>
    [MaxLength(20)]
    public string TipoRecurrencia { get; set; } = "SEMANAL";

    /// <summary>
    /// Valor de recurrencia (ej: "LUNES" para semanal, "15" para día 15 mensual)
    /// </summary>
    [MaxLength(20)]
    public string ValorRecurrencia { get; set; } = string.Empty;

    [JsonPropertyName("suspende_clases")]
    public bool SuspendeClases { get; set; } = true;

    public bool Activo { get; set; } = true;
}

/// <summary>
/// Configuración de semanas especiales (semana de exámenes, etc.)
/// </summary>
public class ConfiguracionSemanas
{
    [JsonPropertyName("semanas_examenes")]
    public List<SemanaEspecial> SemanasExamenes { get; set; } = new();

    [JsonPropertyName("semanas_recuperacion")]
    public List<SemanaEspecial> SemanasRecuperacion { get; set; } = new();

    [JsonPropertyName("semanas_actividades_especiales")]
    public List<SemanaEspecial> SemanasActividadesEspeciales { get; set; } = new();
}

/// <summary>
/// Configuración de una semana especial
/// </summary>
public class SemanaEspecial
{
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("fecha_inicio")]
    public DateTime FechaInicio { get; set; }

    [JsonPropertyName("fecha_fin")]
    public DateTime FechaFin { get; set; }

    [MaxLength(500)]
    public string Descripcion { get; set; } = string.Empty;

    [JsonPropertyName("horario_especial")]
    public bool HorarioEspecial { get; set; } = false;

    [JsonPropertyName("configuracion_horario")]
    public Dictionary<string, object> ConfiguracionHorario { get; set; } = new();
}

#endregion

#region Configuración de Evaluación

/// <summary>
/// Configuración general del sistema de evaluaciones
/// </summary>
public class ConfiguracionEvaluacion
{
    [JsonPropertyName("permite_extemporaneas")]
    public bool PermiteExtemporaneas { get; set; } = true;

    [JsonPropertyName("dias_limite_extemporaneas")]
    [Range(1, 30)]
    public int DiasLimiteExtemporaneas { get; set; } = 7;

    [JsonPropertyName("requiere_justificacion")]
    public bool RequiereJustificacion { get; set; } = true;

    [JsonPropertyName("calificacion_maxima_extemporanea")]
    public decimal CalificacionMaximaExtemporanea { get; set; } = 3.5m;

    [JsonPropertyName("permite_autoevaluacion")]
    public bool PermiteAutoevaluacion { get; set; } = false;

    [JsonPropertyName("peso_autoevaluacion")]
    [Range(0, 30)]
    public decimal PesoAutoevaluacion { get; set; } = 0;

    [JsonPropertyName("permite_coevaluacion")]
    public bool PermiteCoevaluacion { get; set; } = false;

    [JsonPropertyName("peso_coevaluacion")]
    [Range(0, 30)]
    public decimal PesoCoevaluacion { get; set; } = 0;

    [JsonPropertyName("evaluacion_continua")]
    public bool EvaluacionContinua { get; set; } = false;

    [JsonPropertyName("numero_minimo_evaluaciones")]
    [Range(1, 20)]
    public int NumeroMinimoEvaluaciones { get; set; } = 3;

    [JsonPropertyName("permite_eliminar_peor_nota")]
    public bool PermiteEliminarPeorNota { get; set; } = false;

    [JsonPropertyName("configuracion_observaciones")]
    public ConfiguracionObservaciones ConfiguracionObservaciones { get; set; } = new();
}

/// <summary>
/// Configuración de observaciones en evaluaciones
/// </summary>
public class ConfiguracionObservaciones
{
    [JsonPropertyName("obligatorias_nota_baja")]
    public bool ObligatoriasNotaBaja { get; set; } = true;

    [JsonPropertyName("limite_nota_baja")]
    public decimal LimiteNotaBaja { get; set; } = 3.0m;

    [JsonPropertyName("obligatorias_nota_alta")]
    public bool ObligatoriasNotaAlta { get; set; } = false;

    [JsonPropertyName("limite_nota_alta")]
    public decimal LimiteNotaAlta { get; set; } = 4.5m;

    [JsonPropertyName("longitud_maxima")]
    [Range(100, 2000)]
    public int LongitudMaxima { get; set; } = 500;

    [JsonPropertyName("plantillas_predefinidas")]
    public List<string> PlantillasPredefinidas { get; set; } = new();
}

#endregion

#region Configuración de Promoción

/// <summary>
/// Configuración de reglas de promoción de estudiantes
/// </summary>
public class ConfiguracionPromocion
{
    [JsonPropertyName("nota_minima_promocion")]
    public decimal NotaMinimaPromocion { get; set; } = 3.0m;

    [JsonPropertyName("porcentaje_minimo_asistencia")]
    [Range(50, 100)]
    public decimal PorcentajeMinimoAsistencia { get; set; } = 80m;

    [JsonPropertyName("maximo_areas_perdidas")]
    [Range(0, 10)]
    public int MaximoAreasPerdidas { get; set; } = 2;

    [JsonPropertyName("areas_fundamentales")]
    public List<string> AreasFundamentales { get; set; } = new()
    {
        "MATEMATICAS", "LENGUAJE", "CIENCIAS", "SOCIALES"
    };

    [JsonPropertyName("permite_promocion_anticipada")]
    public bool PermitePromocionAnticipada { get; set; } = false;

    [JsonPropertyName("criterios_promocion_anticipada")]
    public CriteriosPromocionAnticipada CriteriosPromocionAnticipada { get; set; } = new();

    [JsonPropertyName("permite_promocion_con_areas_pendientes")]
    public bool PermitePromocionConAreasPendientes { get; set; } = true;

    [JsonPropertyName("plan_mejoramiento_obligatorio")]
    public bool PlanMejoramientoObligatorio { get; set; } = true;
}

/// <summary>
/// Criterios para promoción anticipada
/// </summary>
public class CriteriosPromocionAnticipada
{
    [JsonPropertyName("promedio_minimo")]
    public decimal PromedioMinimo { get; set; } = 4.5m;

    [JsonPropertyName("sin_areas_perdidas")]
    public bool SinAreasPerdidas { get; set; } = true;

    [JsonPropertyName("asistencia_minima")]
    [Range(85, 100)]
    public decimal AsistenciaMinima { get; set; } = 95m;

    [JsonPropertyName("sin_observaciones_disciplinarias")]
    public bool SinObservacionesDisciplinarias { get; set; } = true;

    [JsonPropertyName("requiere_evaluacion_psicologica")]
    public bool RequiereEvaluacionPsicologica { get; set; } = true;

    [JsonPropertyName("requiere_aprobacion_consejo")]
    public bool RequiereAprobacionConsejo { get; set; } = true;
}

#endregion

#region Configuración de Recuperaciones

/// <summary>
/// Configuración del sistema de recuperaciones y habilitaciones
/// </summary>
public class ConfiguracionRecuperaciones
{
    [JsonPropertyName("permite_recuperaciones")]
    public bool PermiteRecuperaciones { get; set; } = true;

    [JsonPropertyName("numero_maximo_recuperaciones")]
    [Range(1, 5)]
    public int NumeroMaximoRecuperaciones { get; set; } = 2;

    [JsonPropertyName("dias_despues_periodo")]
    [Range(1, 90)]
    public int DiasDespuesPeriodo { get; set; } = 15;

    [JsonPropertyName("nota_minima_para_recuperar")]
    public decimal NotaMinimaParaRecuperar { get; set; } = 1.0m;

    [JsonPropertyName("nota_maxima_para_recuperar")]
    public decimal NotaMaximaParaRecuperar { get; set; } = 2.9m;

    [JsonPropertyName("nota_maxima_recuperacion")]
    public decimal NotaMaximaRecuperacion { get; set; } = 3.0m;

    [JsonPropertyName("requiere_plan_mejoramiento")]
    public bool RequierePlanMejoramiento { get; set; } = true;

    [JsonPropertyName("requiere_autorizacion_acudiente")]
    public bool RequiereAutorizacionAcudiente { get; set; } = true;

    [JsonPropertyName("costo_recuperacion")]
    public decimal CostoRecuperacion { get; set; } = 0;

    [JsonPropertyName("configuracion_habilitaciones")]
    public ConfiguracionHabilitaciones ConfiguracionHabilitaciones { get; set; } = new();
}

/// <summary>
/// Configuración específica para habilitaciones de fin de año
/// </summary>
public class ConfiguracionHabilitaciones
{
    [JsonPropertyName("permite_habilitaciones")]
    public bool PermiteHabilitaciones { get; set; } = true;

    [JsonPropertyName("maximo_areas_habilitar")]
    [Range(1, 5)]
    public int MaximoAreasHabilitar { get; set; } = 3;

    [JsonPropertyName("promedio_minimo_otras_areas")]
    public decimal PromedioMinimoOtrasAreas { get; set; } = 3.5m;

    [JsonPropertyName("nota_minima_habilitacion")]
    public decimal NotaMinimaHabilitacion { get; set; } = 1.0m;

    [JsonPropertyName("nota_maxima_habilitacion")]
    public decimal NotaMaximaHabilitacion { get; set; } = 2.9m;

    [JsonPropertyName("nota_aprobatoria_habilitacion")]
    public decimal NotaAprobatoriaHabilitacion { get; set; } = 3.0m;

    [JsonPropertyName("fecha_limite_inscripcion")]
    public DateTime? FechaLimiteInscripcion { get; set; }

    [JsonPropertyName("fechas_examenes")]
    public List<FechaExamenHabilitacion> FechasExamenes { get; set; } = new();

    [JsonPropertyName("costo_habilitacion")]
    public decimal CostoHabilitacion { get; set; } = 0;
}

/// <summary>
/// Configuración de fechas de exámenes de habilitación
/// </summary>
public class FechaExamenHabilitacion
{
    [Required]
    [MaxLength(100)]
    public string Area { get; set; } = string.Empty;

    [JsonPropertyName("fecha_examen")]
    public DateTime FechaExamen { get; set; }

    [JsonPropertyName("hora_inicio")]
    public string HoraInicio { get; set; } = string.Empty;

    [JsonPropertyName("hora_fin")]
    public string HoraFin { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Aula { get; set; } = string.Empty;

    public bool Activa { get; set; } = true;
}

#endregion

#region Métodos de Extensión y Utilidades

/// <summary>
/// Métodos de extensión para trabajar con ConfiguracionAnoAcademico
/// </summary>
public static class ConfiguracionAnoAcademicoExtensions
{
    /// <summary>
    /// Valida si la configuración académica es consistente
    /// </summary>
    public static List<string> ValidarConsistencia(this ConfiguracionAnoAcademico config)
    {
        var errores = new List<string>();

        // Validar que los períodos suman 100%
        var sumaPesos = config.PeriodosAcademicos.Sum(p => p.PesoPorcentual);
        if (Math.Abs(sumaPesos - 100) > 0.01m)
        {
            errores.Add($"La suma de pesos porcentuales debe ser 100%. Actual: {sumaPesos}%");
        }

        // Validar fechas de períodos no se solapan
        for (int i = 0; i < config.PeriodosAcademicos.Count - 1; i++)
        {
            var periodoActual = config.PeriodosAcademicos[i];
            var siguientePeriodo = config.PeriodosAcademicos[i + 1];

            if (periodoActual.FechaFin >= siguientePeriodo.FechaInicio)
            {
                errores.Add($"Los períodos {periodoActual.Nombre} y {siguientePeriodo.Nombre} tienen fechas que se solapan");
            }
        }

        // Validar escala de calificación
        if (config.EscalaCalificacion.Minima >= config.EscalaCalificacion.Maxima)
        {
            errores.Add("La escala mínima debe ser menor que la máxima");
        }

        if (config.EscalaCalificacion.NotaAprobacion < config.EscalaCalificacion.Minima ||
            config.EscalaCalificacion.NotaAprobacion > config.EscalaCalificacion.Maxima)
        {
            errores.Add("La nota de aprobación debe estar dentro de la escala de calificaciones");
        }

        return errores;
    }

    /// <summary>
    /// Obtiene el período académico actual basado en la fecha
    /// </summary>
    public static PeriodoAcademicoConfig? ObtenerPeriodoActual(this ConfiguracionAnoAcademico config, DateTime fecha)
    {
        return config.PeriodosAcademicos
            .Where(p => p.Activo)
            .FirstOrDefault(p => fecha >= p.FechaInicio && fecha <= p.FechaFin);
    }

    /// <summary>
    /// Obtiene el siguiente período académico
    /// </summary>
    public static PeriodoAcademicoConfig? ObtenerSiguientePeriodo(this ConfiguracionAnoAcademico config, DateTime fecha)
    {
        return config.PeriodosAcademicos
            .Where(p => p.Activo && p.FechaInicio > fecha)
            .OrderBy(p => p.FechaInicio)
            .FirstOrDefault();
    }

    /// <summary>
    /// Verifica si una fecha está en período de vacaciones
    /// </summary>
    public static bool EstaEnVacaciones(this ConfiguracionAnoAcademico config, DateTime fecha)
    {
        return config.CalendarioEspecial.Vacaciones
            .Any(v => v.Activa && v.SuspendeClases && fecha >= v.FechaInicio && fecha <= v.FechaFin);
    }

    /// <summary>
    /// Obtiene eventos del día especificado
    /// </summary>
    public static List<FechaEspecialConfig> ObtenerEventosDelDia(this ConfiguracionAnoAcademico config, DateTime fecha)
    {
        return config.CalendarioEspecial.FechasEspeciales
            .Where(f => f.Activa && f.Fecha.Date == fecha.Date)
            .OrderBy(f => f.Fecha)
            .ToList();
    }

    /// <summary>
    /// Verifica si un estudiante puede ser promovido
    /// </summary>
    public static bool PuedeSerPromovido(this ConfiguracionAnoAcademico config,
        decimal promedioGeneral, decimal porcentajeAsistencia, int areasPerdidasCount,
        List<string> areasPerdidas)
    {
        var configPromocion = config.ConfiguracionPromocion;

        // Verificar promedio general
        if (promedioGeneral < configPromocion.NotaMinimaPromocion)
            return false;

        // Verificar asistencia
        if (porcentajeAsistencia < configPromocion.PorcentajeMinimoAsistencia)
            return false;

        // Verificar número de áreas perdidas
        if (areasPerdidasCount > configPromocion.MaximoAreasPerdidas)
            return false;

        // Verificar áreas fundamentales
        var areasFundamentalesPerdidas = areasPerdidas
            .Intersect(configPromocion.AreasFundamentales, StringComparer.OrdinalIgnoreCase)
            .Count();

        if (areasFundamentalesPerdidas > 0)
            return false;

        return true;
    }

    /// <summary>
    /// Calcula la nota final del año con los pesos de períodos
    /// </summary>
    public static decimal CalcularNotaFinal(this ConfiguracionAnoAcademico config,
        Dictionary<int, decimal> notasPorPeriodo)
    {
        decimal notaFinal = 0;
        decimal sumaPresosUsados = 0;

        foreach (var periodo in config.PeriodosAcademicos.Where(p => p.Activo))
        {
            if (notasPorPeriodo.ContainsKey(periodo.Numero))
            {
                notaFinal += notasPorPeriodo[periodo.Numero] * (periodo.PesoPorcentual / 100);
                sumaPresosUsados += periodo.PesoPorcentual;
            }
        }

        // Si no todos los períodos tienen nota, ajustar proporcionalmente
        if (sumaPresosUsados > 0 && sumaPresosUsados < 100)
        {
            notaFinal = notaFinal * (100 / sumaPresosUsados);
        }

        return RedondearCalificacion(config.EscalaCalificacion, notaFinal);
    }

    /// <summary>
    /// Redondea una calificación según la configuración establecida
    /// </summary>
    public static decimal RedondearCalificacion(this EscalaCalificacion escala, decimal calificacion)
    {
        var config = escala.ConfiguracionRedondeo;

        return config.Tipo.ToUpperInvariant() switch
        {
            "MATEMATICO" => Math.Round(calificacion, config.Decimales, MidpointRounding.ToEven),
            "HACIA_ARRIBA" => Math.Ceiling(calificacion * (decimal)Math.Pow(10, config.Decimales)) / (decimal)Math.Pow(10, config.Decimales),
            "HACIA_ABAJO" => Math.Floor(calificacion * (decimal)Math.Pow(10, config.Decimales)) / (decimal)Math.Pow(10, config.Decimales),
            "TRUNCAR" => Math.Truncate(calificacion * (decimal)Math.Pow(10, config.Decimales)) / (decimal)Math.Pow(10, config.Decimales),
            _ => Math.Round(calificacion, config.Decimales)
        };
    }

    /// <summary>
    /// Obtiene la equivalencia cualitativa de una calificación
    /// </summary>
    public static string ObtenerEquivalenciaCualitativa(this EscalaCalificacion escala, decimal calificacion)
    {
        foreach (var equivalencia in escala.Equivalencias)
        {
            var rango = equivalencia.Key.Split('-');
            if (rango.Length == 2 &&
                decimal.TryParse(rango[0], out var min) &&
                decimal.TryParse(rango[1], out var max))
            {
                if (calificacion >= min && calificacion <= max)
                {
                    return equivalencia.Value;
                }
            }
        }
        return "Sin Equivalencia";
    }

    /// <summary>
    /// Convierte una calificación a otra escala
    /// </summary>
    public static decimal ConvertirEscala(this ConfiguracionAnoAcademico config,
        decimal calificacion, string nombreConversion)
    {
        var conversion = config.EscalaCalificacion.ConversionesEscala
            .FirstOrDefault(c => c.Activa && c.Nombre.Equals(nombreConversion, StringComparison.OrdinalIgnoreCase));

        if (conversion == null)
            return calificacion;

        // Si hay fórmula personalizada, aquí se implementaría
        if (!string.IsNullOrEmpty(conversion.Formula))
        {
            // Implementar evaluador de fórmulas personalizado si es necesario
            return calificacion;
        }

        // Conversión lineal simple
        var rangoOrigen = conversion.EscalaOrigenMax - conversion.EscalaOrigenMin;
        var rangoDestino = conversion.EscalaDestinoMax - conversion.EscalaDestinoMin;

        var porcentaje = (calificacion - conversion.EscalaOrigenMin) / rangoOrigen;
        var resultado = conversion.EscalaDestinoMin + (porcentaje * rangoDestino);

        return Math.Max(conversion.EscalaDestinoMin, Math.Min(conversion.EscalaDestinoMax, resultado));
    }

    /// <summary>
    /// Verifica si una fecha es válida para modificar calificaciones
    /// </summary>
    public static bool PuedeModificarCalificaciones(this ConfiguracionAnoAcademico config,
        DateTime fecha, int numeroPeriodo)
    {
        var periodo = config.PeriodosAcademicos
            .FirstOrDefault(p => p.Numero == numeroPeriodo && p.Activo);

        if (periodo == null)
            return false;

        return fecha <= periodo.FechaLimiteModificaciones;
    }

    /// <summary>
    /// Obtiene la configuración de recuperaciones para un estudiante
    /// </summary>
    public static bool PuedeRecuperar(this ConfiguracionAnoAcademico config,
        decimal calificacion, int numeroRecuperacionesPrevias, DateTime fechaActual, int numeroPeriodo)
    {
        var configRecup = config.ConfiguracionRecuperaciones;

        if (!configRecup.PermiteRecuperaciones)
            return false;

        if (numeroRecuperacionesPrevias >= configRecup.NumeroMaximoRecuperaciones)
            return false;

        if (calificacion < configRecup.NotaMinimaParaRecuperar ||
            calificacion > configRecup.NotaMaximaParaRecuperar)
            return false;

        var periodo = config.PeriodosAcademicos
            .FirstOrDefault(p => p.Numero == numeroPeriodo && p.Activo);

        if (periodo == null || !periodo.PermiteRecuperacion)
            return false;

        var diasTranscurridos = (fechaActual - periodo.FechaFin).Days;
        if (diasTranscurridos > configRecup.DiasDespuesPeriodo)
            return false;

        return true;
    }

    /// <summary>
    /// Verifica si un estudiante puede habilitar áreas
    /// </summary>
    public static bool PuedeHabilitar(this ConfiguracionAnoAcademico config,
        List<string> areasPerdidas, decimal promedioOtrasAreas)
    {
        var configHab = config.ConfiguracionRecuperaciones.ConfiguracionHabilitaciones;

        if (!configHab.PermiteHabilitaciones)
            return false;

        if (areasPerdidas.Count > configHab.MaximoAreasHabilitar)
            return false;

        if (promedioOtrasAreas < configHab.PromedioMinimoOtrasAreas)
            return false;

        return true;
    }

    /// <summary>
    /// Obtiene todas las fechas importantes del año académico
    /// </summary>
    public static List<DateTime> ObtenerFechasImportantes(this ConfiguracionAnoAcademico config)
    {
        var fechas = new List<DateTime>();

        // Fechas de períodos
        fechas.AddRange(config.PeriodosAcademicos.SelectMany(p => new[]
        {
            p.FechaInicio, p.FechaFin, p.FechaLimiteNotas, p.FechaLimiteModificaciones
        }));

        // Fechas de vacaciones
        fechas.AddRange(config.CalendarioEspecial.Vacaciones.SelectMany(v => new[]
        {
            v.FechaInicio, v.FechaFin
        }));

        // Fechas especiales
        fechas.AddRange(config.CalendarioEspecial.FechasEspeciales.Select(f => f.Fecha));

        // Fechas de habilitaciones
        if (config.ConfiguracionRecuperaciones.ConfiguracionHabilitaciones.FechaLimiteInscripcion.HasValue)
        {
            fechas.Add(config.ConfiguracionRecuperaciones.ConfiguracionHabilitaciones.FechaLimiteInscripcion.Value);
        }

        fechas.AddRange(config.ConfiguracionRecuperaciones.ConfiguracionHabilitaciones.FechasExamenes
            .Select(f => f.FechaExamen));

        return fechas.Distinct().OrderBy(f => f).ToList();
    }

    /// <summary>
    /// Valida si la distribución de tipos de evaluación es correcta para un período
    /// </summary>
    public static bool ValidarDistribucionEvaluaciones(this PeriodoAcademicoConfig periodo,
        List<(string Tipo, decimal Porcentaje)> evaluacionesAsignadas)
    {
        foreach (var tipoConfig in periodo.TiposEvaluacionPeriodo)
        {
            var porcentajeAsignado = evaluacionesAsignadas
                .Where(e => e.Tipo.Equals(tipoConfig.Codigo, StringComparison.OrdinalIgnoreCase))
                .Sum(e => e.Porcentaje);

            if (tipoConfig.EsObligatorio && porcentajeAsignado == 0)
                return false;

            if (porcentajeAsignado < tipoConfig.PorcentajeMinimo ||
                porcentajeAsignado > tipoConfig.PorcentajeMaximo)
                return false;
        }

        // Verificar que la suma total sea 100%
        var sumaTotal = evaluacionesAsignadas.Sum(e => e.Porcentaje);
        return Math.Abs(sumaTotal - 100) <= 0.01m;
    }

    /// <summary>
    /// Genera un reporte de configuración del año académico
    /// </summary>
    public static Dictionary<string, object> GenerarReporteConfiguracion(this ConfiguracionAnoAcademico config)
    {
        return new Dictionary<string, object>
        {
            ["total_periodos"] = config.PeriodosAcademicos.Count(p => p.Activo),
            ["escala_calificacion"] = $"{config.EscalaCalificacion.Minima} - {config.EscalaCalificacion.Maxima}",
            ["nota_aprobacion"] = config.EscalaCalificacion.NotaAprobacion,
            ["total_vacaciones"] = config.CalendarioEspecial.Vacaciones.Count(v => v.Activa),
            ["total_eventos"] = config.CalendarioEspecial.FechasEspeciales.Count(f => f.Activa),
            ["permite_recuperaciones"] = config.ConfiguracionRecuperaciones.PermiteRecuperaciones,
            ["permite_habilitaciones"] = config.ConfiguracionRecuperaciones.ConfiguracionHabilitaciones.PermiteHabilitaciones,
            ["evaluaciones_extemporaneas"] = config.ConfiguracionEvaluacion.PermiteExtemporaneas,
            ["promocion_anticipada"] = config.ConfiguracionPromocion.PermitePromocionAnticipada,
            ["fechas_importantes"] = config.ObtenerFechasImportantes().Count,
            ["errores_configuracion"] = config.ValidarConsistencia()
        };
    }
}

#endregion