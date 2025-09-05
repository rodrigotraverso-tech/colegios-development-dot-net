using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ColegiosBackend.Core.ValueObjects.Financiero;

#region Configuración de Becas

/// <summary>
/// Configuración completa del sistema de becas y ayudas económicas
/// </summary>
public class ConfiguracionBecas
{
    [JsonPropertyName("permite_becas")]
    public bool PermiteBecas { get; set; } = true;

    [JsonPropertyName("tipos_beca")]
    public List<TipoBeca> TiposBeca { get; set; } = new();

    [JsonPropertyName("proceso_solicitud")]
    public ProcesoSolicitudBeca ProcesoSolicitud { get; set; } = new();

    [JsonPropertyName("criterios_evaluacion")]
    public CriteriosEvaluacionBeca CriteriosEvaluacion { get; set; } = new();

    [JsonPropertyName("renovacion_becas")]
    public RenovacionBecas RenovacionBecas { get; set; } = new();
}

/// <summary>
/// Configuración de tipos de becas disponibles
/// </summary>
public class TipoBeca
{
    [Required]
    [MaxLength(20)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de beca: ACADEMICA, DEPORTIVA, ARTISTICA, SOCIOECONOMICA, INSTITUCIONAL
    /// </summary>
    [MaxLength(30)]
    public string Categoria { get; set; } = "ACADEMICA";

    [JsonPropertyName("porcentaje_cobertura")]
    [Range(10, 100)]
    public decimal PorcentajeCobertura { get; set; }

    [JsonPropertyName("conceptos_cubiertos")]
    public List<string> ConceptosCubiertos { get; set; } = new();

    [JsonPropertyName("numero_maximo_beneficiarios")]
    [Range(1, 1000)]
    public int NumeroMaximoBeneficiarios { get; set; } = 10;

    [JsonPropertyName("beneficiarios_actuales")]
    public int BeneficiariosActuales { get; set; } = 0;

    [JsonPropertyName("duracion_meses")]
    [Range(1, 60)]
    public int DuracionMeses { get; set; } = 12;

    [JsonPropertyName("renovable")]
    public bool Renovable { get; set; } = true;

    [JsonPropertyName("requiere_mantenimiento")]
    public bool RequiereMantenimiento { get; set; } = true;

    public bool Activa { get; set; } = true;
}

/// <summary>
/// Configuración del proceso de solicitud de becas
/// </summary>
public class ProcesoSolicitudBeca
{
    [JsonPropertyName("fecha_inicio_convocatoria")]
    public DateTime? FechaInicioConvocatoria { get; set; }

    [JsonPropertyName("fecha_fin_convocatoria")]
    public DateTime? FechaFinConvocatoria { get; set; }

    [JsonPropertyName("documentos_requeridos")]
    public List<DocumentoRequerido> DocumentosRequeridos { get; set; } = new();

    [JsonPropertyName("requiere_entrevista")]
    public bool RequiereEntrevista { get; set; } = false;

    [JsonPropertyName("requiere_visita_domiciliaria")]
    public bool RequiereVisitaDomiciliaria { get; set; } = false;

    [JsonPropertyName("comite_evaluacion")]
    public List<string> ComiteEvaluacion { get; set; } = new();

    [JsonPropertyName("tiempo_respuesta_dias")]
    [Range(5, 90)]
    public int TiempoRespuestaDias { get; set; } = 30;
}

/// <summary>
/// Documento requerido para solicitud de beca
/// </summary>
public class DocumentoRequerido
{
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Descripcion { get; set; } = string.Empty;

    public bool Obligatorio { get; set; } = true;

    [JsonPropertyName("formato_permitido")]
    public List<string> FormatoPermitido { get; set; } = new() { "PDF", "JPG", "PNG" };

    [JsonPropertyName("tamaño_maximo_mb")]
    [Range(1, 50)]
    public int TamañoMaximoMb { get; set; } = 5;
}

/// <summary>
/// Criterios de evaluación para becas
/// </summary>
public class CriteriosEvaluacionBeca
{
    [JsonPropertyName("promedio_academico")]
    public CriterioAcademico PromedioAcademico { get; set; } = new();

    [JsonPropertyName("situacion_socioeconomica")]
    public CriterioSocioeconomico SituacionSocioeconomica { get; set; } = new();

    [JsonPropertyName("disciplina")]
    public CriterioDisciplina Disciplina { get; set; } = new();

    [JsonPropertyName("actividades_extracurriculares")]
    public CriterioExtracurricular ActividadesExtracurriculares { get; set; } = new();
}

/// <summary>
/// Criterio académico para evaluación de becas
/// </summary>
public class CriterioAcademico
{
    [JsonPropertyName("peso_porcentual")]
    [Range(0, 100)]
    public decimal PesoPorcentual { get; set; } = 40;

    [JsonPropertyName("promedio_minimo")]
    [Range(3.0, 5.0)]
    public decimal PromedioMinimo { get; set; } = 4.0m;

    [JsonPropertyName("considera_ultimos_periodos")]
    [Range(1, 8)]
    public int ConsideraUltimosPeriodos { get; set; } = 4;

    [JsonPropertyName("areas_peso_especial")]
    public Dictionary<string, decimal> AreasPesoEspecial { get; set; } = new();
}

/// <summary>
/// Criterio socioeconómico para evaluación de becas
/// </summary>
public class CriterioSocioeconomico
{
    [JsonPropertyName("peso_porcentual")]
    [Range(0, 100)]
    public decimal PesoPorcentual { get; set; } = 30;

    [JsonPropertyName("ingresos_maximos_smmlv")]
    [Range(1, 10)]
    public decimal IngresosMaximosSmmlv { get; set; } = 3;

    [JsonPropertyName("considera_numero_hermanos")]
    public bool ConsideraNumeroHermanos { get; set; } = true;

    [JsonPropertyName("considera_situacion_laboral")]
    public bool ConsideraSituacionLaboral { get; set; } = true;

    [JsonPropertyName("requiere_sisben")]
    public bool RequiereSisben { get; set; } = false;

    [JsonPropertyName("nivel_sisben_maximo")]
    [Range(1, 4)]
    public int NivelSisbenMaximo { get; set; } = 3;
}

/// <summary>
/// Criterio disciplinario para evaluación de becas
/// </summary>
public class CriterioDisciplina
{
    [JsonPropertyName("peso_porcentual")]
    [Range(0, 100)]
    public decimal PesoPorcentual { get; set; } = 20;

    [JsonPropertyName("maximo_observaciones_graves")]
    [Range(0, 5)]
    public int MaximoObservacionesGraves { get; set; } = 0;

    [JsonPropertyName("considera_asistencia")]
    public bool ConsideraAsistencia { get; set; } = true;

    [JsonPropertyName("asistencia_minima")]
    [Range(80, 100)]
    public decimal AsistenciaMinima { get; set; } = 90;
}

/// <summary>
/// Criterio extracurricular para evaluación de becas
/// </summary>
public class CriterioExtracurricular
{
    [JsonPropertyName("peso_porcentual")]
    [Range(0, 100)]
    public decimal PesoPorcentual { get; set; } = 10;

    [JsonPropertyName("actividades_valoradas")]
    public List<string> ActividadesValoradas { get; set; } = new()
    {
        "DEPORTES", "ARTE", "MUSICA", "LIDERAZGO", "VOLUNTARIADO"
    };

    [JsonPropertyName("minimo_participaciones")]
    [Range(0, 10)]
    public int MinimoParticipaciones { get; set; } = 1;

    [JsonPropertyName("considera_logros")]
    public bool ConsideraLogros { get; set; } = true;
}

/// <summary>
/// Configuración de renovación de becas
/// </summary>
public class RenovacionBecas
{
    [JsonPropertyName("evaluacion_automatica")]
    public bool EvaluacionAutomatica { get; set; } = true;

    [JsonPropertyName("frecuencia_evaluacion")]
    [MaxLength(20)]
    public string FrecuenciaEvaluacion { get; set; } = "ANUAL"; // SEMESTRAL, ANUAL, BIANUAL

    [JsonPropertyName("criterios_mantenimiento")]
    public CriteriosMantenimientoBeca CriteriosMantenimiento { get; set; } = new();

    [JsonPropertyName("proceso_apelacion")]
    public ProcesoApelacion ProcesoApelacion { get; set; } = new();
}

/// <summary>
/// Criterios para mantener una beca
/// </summary>
public class CriteriosMantenimientoBeca
{
    [JsonPropertyName("promedio_minimo_mantenimiento")]
    [Range(3.0, 5.0)]
    public decimal PromedioMinimoMantenimiento { get; set; } = 3.8m;

    [JsonPropertyName("asistencia_minima_mantenimiento")]
    [Range(80, 100)]
    public decimal AsistenciaMinimaMantenimiento { get; set; } = 85;

    [JsonPropertyName("disciplina_impecable")]
    public bool DisciplinaImpecable { get; set; } = true;

    [JsonPropertyName("participacion_actividades")]
    public bool ParticipacionActividades { get; set; } = false;

    [JsonPropertyName("servicio_social")]
    [Range(0, 100)]
    public int HorasServicioSocial { get; set; } = 0;
}

/// <summary>
/// Proceso de apelación para becas
/// </summary>
public class ProcesoApelacion
{
    [JsonPropertyName("permite_apelaciones")]
    public bool PermiteApelaciones { get; set; } = true;

    [JsonPropertyName("dias_limite_apelacion")]
    [Range(5, 30)]
    public int DiasLimiteApelacion { get; set; } = 15;

    [JsonPropertyName("instancias_apelacion")]
    public List<string> InstanciasApelacion { get; set; } = new()
    {
        "COORDINACION_ACADEMICA", "RECTORIA", "CONSEJO_DIRECTIVO"
    };

    [JsonPropertyName("requiere_nuevos_documentos")]
    public bool RequiereNuevosDocumentos { get; set; } = false;

    [JsonPropertyName("tiempo_respuesta_dias")]
    [Range(5, 60)]
    public int TiempoRespuestaDias { get; set; } = 20;
}

#endregion

#region Estrategias de Cobranza

/// <summary>
/// Estrategias de cobranza configurables
/// </summary>
public class EstrategiaCobranza
{
    [Required]
    [MaxLength(50)]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("dias_activacion")]
    [Range(1, 365)]
    public int DiasActivacion { get; set; }

    [JsonPropertyName("canales_comunicacion")]
    public List<string> CanalesComunicacion { get; set; } = new() { "EMAIL", "TELEFONO", "SMS" };

    [JsonPropertyName("frecuencia_contacto_dias")]
    [Range(1, 30)]
    public int FrecuenciaContactoDias { get; set; } = 7;

    [JsonPropertyName("plantilla_mensaje")]
    [MaxLength(1000)]
    public string PlantillaMensaje { get; set; } = string.Empty;

    [JsonPropertyName("escala_siguiente")]
    public bool EscalaSiguiente { get; set; } = true;

    public bool Activa { get; set; } = true;
}

#endregion

#region Métodos de Extensión y Utilidades

/// <summary>
/// Métodos de extensión para trabajar con ConfiguracionFinanciera
/// </summary>
public static class ConfiguracionFinancieraExtensions
{
    /// <summary>
    /// Calcula el descuento aplicable para un estudiante
    /// </summary>
    public static decimal CalcularDescuento(this DescuentoDisponible descuento,
        decimal montoBase, Dictionary<string, object> contexto)
    {
        if (!ValidarCondicionesDescuento(descuento.Condiciones, contexto))
            return 0;

        if (!EstaVigente(descuento.Vigencia))
            return 0;

        return descuento.Tipo.ToUpperInvariant() switch
        {
            "PORCENTAJE" => CalcularDescuentoPorcentaje(descuento, montoBase),
            "VALOR_FIJO" => Math.Min(descuento.Valor, montoBase),
            "ESCALONADO" => CalcularDescuentoEscalonado(descuento, montoBase, contexto),
            _ => 0
        };
    }

    /// <summary>
    /// Calcula el recargo aplicable por mora
    /// </summary>
    public static decimal CalcularRecargo(this RecargoConfig recargo,
        decimal saldoPendiente, int diasVencidos, decimal interesesAcumulados = 0)
    {
        if (diasVencidos <= recargo.DiasGracia)
            return 0;

        var diasFacturables = diasVencidos - recargo.DiasGracia;
        var baseCalculo = recargo.Calculo.ToUpperInvariant() switch
        {
            "SALDO_PENDIENTE" => saldoPendiente,
            "VALOR_ORIGINAL" => saldoPendiente + interesesAcumulados,
            "CAPITAL_INTERESES" => saldoPendiente,
            _ => saldoPendiente
        };

        var recargoCalculado = recargo.Tipo.ToUpperInvariant() switch
        {
            "PORCENTAJE_MENSUAL" => CalcularRecargoMensual(recargo, baseCalculo, diasFacturables),
            "PORCENTAJE_DIARIO" => CalcularRecargoDiario(recargo, baseCalculo, diasFacturables),
            "VALOR_FIJO" => recargo.Valor,
            "ESCALONADO" => CalcularRecargoEscalonado(recargo, baseCalculo, diasVencidos),
            _ => 0
        };

        // Aplicar máximo acumulable
        if (recargo.MaximoAcumulable > 0)
        {
            var maximoPermitido = saldoPendiente * (recargo.MaximoAcumulable / 100);
            recargoCalculado = Math.Min(recargoCalculado, maximoPermitido);
        }

        return Math.Round(recargoCalculado, 0);
    }

    /// <summary>
    /// Genera el número de factura según el formato configurado
    /// </summary>
    public static string GenerarNumeroFactura(this ConfiguracionFacturacion config,
        DateTime fecha, int secuencia, string? codigoColegio = null)
    {
        var formato = config.FormatoNumero;

        formato = formato.Replace("{YEAR}", fecha.Year.ToString());
        formato = formato.Replace("{MONTH:00}", fecha.Month.ToString("00"));
        formato = formato.Replace("{MONTH}", fecha.Month.ToString());
        formato = formato.Replace("{DAY:00}", fecha.Day.ToString("00"));
        formato = formato.Replace("{DAY}", fecha.Day.ToString());
        formato = formato.Replace("{SEQUENCE:0000}", secuencia.ToString("0000"));
        formato = formato.Replace("{SEQUENCE:000}", secuencia.ToString("000"));
        formato = formato.Replace("{SEQUENCE}", secuencia.ToString());

        if (!string.IsNullOrEmpty(codigoColegio))
        {
            formato = formato.Replace("{COLEGIO}", codigoColegio);
        }

        return formato;
    }

    /// <summary>
    /// Determina si se debe generar una alerta financiera
    /// </summary>
    public static bool RequiereAlerta(this AlertasFinancieras alertas,
        decimal saldoVencido, int diasVencidos, decimal porcentajeCartera)
    {
        return saldoVencido >= alertas.MontoIndividualAlerta ||
               alertas.SaldoVencidoDias.Contains(diasVencidos) ||
               porcentajeCartera >= alertas.PorcentajeCarteraAlerta;
    }

    /// <summary>
    /// Obtiene la clasificación de cartera por edad
    /// </summary>
    public static ClasificacionEdadCartera? ObtenerClasificacionCartera(
        this ConfiguracionCartera config, int diasVencidos)
    {
        return config.ClasificacionEdades
            .FirstOrDefault(c => diasVencidos >= c.DesdeDias && diasVencidos <= c.HastaDias);
    }

    /// <summary>
    /// Evalúa si un estudiante cumple criterios para una beca específica
    /// </summary>
    public static (bool Cumple, decimal Puntaje, List<string> Observaciones) EvaluarCriteriosBeca(
        this TipoBeca tipoBeca, CriteriosEvaluacionBeca criterios, Dictionary<string, object> datosEstudiante)
    {
        var observaciones = new List<string>();
        decimal puntajeTotal = 0;

        // Evaluar criterio académico
        if (datosEstudiante.ContainsKey("promedio") && datosEstudiante["promedio"] is decimal promedio)
        {
            if (promedio >= criterios.PromedioAcademico.PromedioMinimo)
            {
                var puntajeAcademico = (criterios.PromedioAcademico.PesoPorcentual / 100) *
                                     ((promedio - criterios.PromedioAcademico.PromedioMinimo) /
                                      (5.0m - criterios.PromedioAcademico.PromedioMinimo)) * 100;
                puntajeTotal += puntajeAcademico;
                observaciones.Add($"Promedio académico: {promedio:F1} (✓)");
            }
            else
            {
                observaciones.Add($"Promedio académico: {promedio:F1} - No cumple mínimo {criterios.PromedioAcademico.PromedioMinimo:F1} (✗)");
            }
        }

        // Evaluar criterio socioeconómico
        if (datosEstudiante.ContainsKey("ingresos_smmlv") && datosEstudiante["ingresos_smmlv"] is decimal ingresos)
        {
            if (ingresos <= criterios.SituacionSocioeconomica.IngresosMaximosSmmlv)
            {
                var puntajeSocio = criterios.SituacionSocioeconomica.PesoPorcentual;
                puntajeTotal += puntajeSocio;
                observaciones.Add($"Situación socioeconómica: {ingresos:F1} SMMLV (✓)");
            }
            else
            {
                observaciones.Add($"Ingresos familiares: {ingresos:F1} SMMLV - Excede máximo {criterios.SituacionSocioeconomica.IngresosMaximosSmmlv:F1} (✗)");
            }
        }

        // Evaluar disciplina
        if (datosEstudiante.ContainsKey("observaciones_graves") && datosEstudiante["observaciones_graves"] is int obsGraves)
        {
            if (obsGraves <= criterios.Disciplina.MaximoObservacionesGraves)
            {
                var puntajeDisciplina = criterios.Disciplina.PesoPorcentual;
                puntajeTotal += puntajeDisciplina;
                observaciones.Add($"Disciplina: {obsGraves} observaciones graves (✓)");
            }
            else
            {
                observaciones.Add($"Disciplina: {obsGraves} observaciones graves - Excede máximo {criterios.Disciplina.MaximoObservacionesGraves} (✗)");
            }
        }

        var cumple = puntajeTotal >= 70; // Puntaje mínimo de 70%

        return (cumple, puntajeTotal, observaciones);
    }

    /// <summary>
    /// Calcula el costo total de una beca por año
    /// </summary>
    public static decimal CalcularCostoBeca(this TipoBeca tipoBeca,
        List<(string Concepto, decimal Valor)> conceptosEstudiante)
    {
        decimal costoTotal = 0;

        foreach (var concepto in conceptosEstudiante)
        {
            if (tipoBeca.ConceptosCubiertos.Contains(concepto.Concepto, StringComparer.OrdinalIgnoreCase))
            {
                costoTotal += concepto.Valor * (tipoBeca.PorcentajeCobertura / 100);
            }
        }

        return Math.Round(costoTotal, 0);
    }

    /// <summary>
    /// Valida si se puede otorgar una nueva beca del tipo especificado
    /// </summary>
    public static bool PuedeOtorgarNuevaBeca(this TipoBeca tipoBeca)
    {
        return tipoBeca.Activa &&
               tipoBeca.BeneficiariosActuales < tipoBeca.NumeroMaximoBeneficiarios;
    }

    /// <summary>
    /// Obtiene la estrategia de cobranza apropiada según los días vencidos
    /// </summary>
    public static EstrategiaCobranza? ObtenerEstrategiaCobranza(this ConfiguracionCartera config,
        int diasVencidos)
    {
        return config.GestoresCobranza
            .SelectMany(g => g.EstrategiasCobranza)
            .Where(e => e.Activa && diasVencidos >= e.DiasActivacion)
            .OrderByDescending(e => e.DiasActivacion)
            .FirstOrDefault();
    }

    /// <summary>
    /// Valida la configuración financiera completa
    /// </summary>
    public static List<string> ValidarConfiguracion(this ConfiguracionFinanciera config)
    {
        var errores = new List<string>();

        // Validar descuentos
        var codigosDescuentos = config.DescuentosDisponibles.GroupBy(d => d.Codigo)
            .Where(g => g.Count() > 1);
        foreach (var grupo in codigosDescuentos)
        {
            errores.Add($"Código de descuento duplicado: {grupo.Key}");
        }

        // Validar recargos
        var codigosRecargos = config.Recargos.GroupBy(r => r.Codigo)
            .Where(g => g.Count() > 1);
        foreach (var grupo in codigosRecargos)
        {
            errores.Add($"Código de recargo duplicado: {grupo.Key}");
        }

        // Validar configuración de facturación
        if (config.ConfiguracionFacturacion.DiaCorteMensual < 1 ||
            config.ConfiguracionFacturacion.DiaCorteMensual > 31)
        {
            errores.Add("El día de corte mensual debe estar entre 1 y 31");
        }

        // Validar métodos de pago
        if (!config.PoliticasPago.MetodosPagoHabilitados.Any(m => m.Habilitado))
        {
            errores.Add("Debe haber al menos un método de pago habilitado");
        }

        return errores;
    }

    #region Métodos Privados de Cálculo

    private static bool ValidarCondicionesDescuento(CondicionesDescuento condiciones,
        Dictionary<string, object> contexto)
    {
        // Validar hermanos
        if (condiciones.MinimoHermanos > 0 &&
            contexto.ContainsKey("numeroHermanos") &&
            contexto["numeroHermanos"] is int hermanos)
        {
            if (hermanos < condiciones.MinimoHermanos)
                return false;
        }

        // Validar promedio académico
        if (condiciones.PromedioMinimo > 0 &&
            contexto.ContainsKey("promedio") &&
            contexto["promedio"] is decimal promedio)
        {
            if (promedio < condiciones.PromedioMinimo)
                return false;
        }

        // Validar monto
        if (condiciones.MinimoMonto > 0 &&
            contexto.ContainsKey("monto") &&
            contexto["monto"] is decimal monto)
        {
            if (monto < condiciones.MinimoMonto)
                return false;

            if (condiciones.MaximoMonto > 0 && monto > condiciones.MaximoMonto)
                return false;
        }

        return true;
    }

    private static bool EstaVigente(VigenciaDescuento vigencia)
    {
        var fechaActual = DateTime.Now.Date;

        if (vigencia.FechaInicio.HasValue && fechaActual < vigencia.FechaInicio.Value.Date)
            return false;

        if (vigencia.FechaFin.HasValue && fechaActual > vigencia.FechaFin.Value.Date)
            return false;

        if (vigencia.MaximoUsos > 0 && vigencia.UsosActuales >= vigencia.MaximoUsos)
            return false;

        return true;
    }

    private static decimal CalcularDescuentoPorcentaje(DescuentoDisponible descuento, decimal montoBase)
    {
        var descuentoCalculado = montoBase * (descuento.Valor / 100);

        if (descuento.Condiciones.MaximoDescuentoTotal > 0)
        {
            var maximoPermitido = montoBase * (descuento.Condiciones.MaximoDescuentoTotal / 100);
            descuentoCalculado = Math.Min(descuentoCalculado, maximoPermitido);
        }

        return Math.Round(descuentoCalculado, 0);
    }

    private static decimal CalcularDescuentoEscalonado(DescuentoDisponible descuento,
        decimal montoBase, Dictionary<string, object> contexto)
    {
        if (!contexto.ContainsKey("cantidad") || contexto["cantidad"] is not int cantidad)
            return 0;

        var escalon = descuento.ConfiguracionEscalonada
            .FirstOrDefault(e => cantidad >= e.DesdeCantidad && cantidad <= e.HastaCantidad);

        if (escalon == null)
            return 0;

        return escalon.PorcentajeDescuento > 0
            ? montoBase * (escalon.PorcentajeDescuento / 100)
            : escalon.ValorFijoDescuento;
    }

    private static decimal CalcularRecargoMensual(RecargoConfig recargo, decimal baseCalculo, int diasFacturables)
    {
        var mesesFacturables = diasFacturables / 30.0m;
        return baseCalculo * (recargo.Valor / 100) * mesesFacturables;
    }

    private static decimal CalcularRecargoDiario(RecargoConfig recargo, decimal baseCalculo, int diasFacturables)
    {
        return baseCalculo * (recargo.Valor / 100) * diasFacturables;
    }

    private static decimal CalcularRecargoEscalonado(RecargoConfig recargo, decimal baseCalculo, int diasVencidos)
    {
        var escalon = recargo.ConfiguracionEscalonada
            .FirstOrDefault(e => diasVencidos >= e.DesdeDias && diasVencidos <= e.HastaDias);

        if (escalon == null)
            return 0;

        return escalon.PorcentajeRecargo > 0
            ? baseCalculo * (escalon.PorcentajeRecargo / 100)
            : escalon.ValorFijoRecargo;
    }

    #endregion
}

#endregion
