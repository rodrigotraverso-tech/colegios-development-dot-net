using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ColegiosBackend.Core.ValueObjects.Financiero;

/// <summary>
/// Configuración avanzada del sistema financiero del colegio
/// Incluye descuentos, recargos, políticas de pago y configuraciones de facturación
/// </summary>
public class ConfiguracionFinanciera
{
    [JsonPropertyName("descuentos_disponibles")]
    public List<DescuentoDisponible> DescuentosDisponibles { get; set; } = new();

    public List<RecargoConfig> Recargos { get; set; } = new();

    [JsonPropertyName("politicas_pago")]
    public PoliticasPago PoliticasPago { get; set; } = new();

    [JsonPropertyName("configuracion_facturacion")]
    public ConfiguracionFacturacion ConfiguracionFacturacion { get; set; } = new();

    [JsonPropertyName("alertas_financieras")]
    public AlertasFinancieras AlertasFinancieras { get; set; } = new();

    [JsonPropertyName("configuracion_cartera")]
    public ConfiguracionCartera ConfiguracionCartera { get; set; } = new();
}

#region Descuentos

/// <summary>
/// Configuración de descuentos disponibles en el sistema
/// </summary>
public class DescuentoDisponible
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
    /// Tipo de descuento: PORCENTAJE, VALOR_FIJO, ESCALONADO
    /// </summary>
    [MaxLength(20)]
    public string Tipo { get; set; } = "PORCENTAJE";

    [Range(0, 100)]
    public decimal Valor { get; set; }

    [JsonPropertyName("aplicable_a")]
    public List<string> AplicableA { get; set; } = new();

    public CondicionesDescuento Condiciones { get; set; } = new();

    [JsonPropertyName("configuracion_escalonada")]
    public List<EscalonDescuento> ConfiguracionEscalonada { get; set; } = new();

    [JsonPropertyName("vigencia")]
    public VigenciaDescuento Vigencia { get; set; } = new();

    [JsonPropertyName("requiere_aprobacion")]
    public bool RequiereAprobacion { get; set; } = false;

    [JsonPropertyName("nivel_aprobacion")]
    public string NivelAprobacion { get; set; } = "COORDINACION"; // COORDINACION, RECTORIA, CONSEJO

    public bool Activo { get; set; } = true;

    [JsonPropertyName("orden_aplicacion")]
    [Range(1, 100)]
    public int OrdenAplicacion { get; set; } = 1;
}

/// <summary>
/// Condiciones específicas para aplicar un descuento
/// </summary>
public class CondicionesDescuento
{
    [JsonPropertyName("minimo_hermanos")]
    [Range(0, 20)]
    public int MinimoHermanos { get; set; } = 0;

    [JsonPropertyName("maximo_descuento_total")]
    [Range(0, 100)]
    public decimal MaximoDescuentoTotal { get; set; } = 50;

    [JsonPropertyName("aplica_al_menor")]
    public bool AplicaAlMenor { get; set; } = true;

    [JsonPropertyName("requiere_autorizacion")]
    public bool RequiereAutorizacion { get; set; } = false;

    [JsonPropertyName("dias_anticipacion")]
    [Range(0, 365)]
    public int DiasAnticipacion { get; set; } = 0;

    [JsonPropertyName("solo_pension_completa")]
    public bool SoloPensionCompleta { get; set; } = false;

    [JsonPropertyName("minimo_monto")]
    [Range(0, double.MaxValue)]
    public decimal MinimoMonto { get; set; } = 0;

    [JsonPropertyName("maximo_monto")]
    [Range(0, double.MaxValue)]
    public decimal MaximoMonto { get; set; } = 0;

    [JsonPropertyName("promedio_minimo")]
    [Range(0, 5)]
    public decimal PromedioMinimo { get; set; } = 0;

    [JsonPropertyName("sin_observaciones_disciplinarias")]
    public bool SinObservacionesDisciplinarias { get; set; } = false;

    [JsonPropertyName("aplica_trimestre_siguiente")]
    public bool AplicaTrimestreSiguiente { get; set; } = false;

    [JsonPropertyName("antigüedad_minima_meses")]
    [Range(0, 120)]
    public int AntiguedadMinimaMeses { get; set; } = 0;

    [JsonPropertyName("estado_financiero_requerido")]
    public List<string> EstadoFinancieroRequerido { get; set; } = new() { "AL_DIA" };

    [JsonPropertyName("condiciones_personalizadas")]
    public Dictionary<string, object> CondicionesPersonalizadas { get; set; } = new();
}

/// <summary>
/// Configuración para descuentos escalonados
/// </summary>
public class EscalonDescuento
{
    [JsonPropertyName("desde_cantidad")]
    [Range(1, int.MaxValue)]
    public int DesdeCantidad { get; set; }

    [JsonPropertyName("hasta_cantidad")]
    [Range(1, int.MaxValue)]
    public int HastaCantidad { get; set; }

    [JsonPropertyName("porcentaje_descuento")]
    [Range(0, 100)]
    public decimal PorcentajeDescuento { get; set; }

    [JsonPropertyName("valor_fijo_descuento")]
    [Range(0, double.MaxValue)]
    public decimal ValorFijoDescuento { get; set; }
}

/// <summary>
/// Configuración de vigencia de descuentos
/// </summary>
public class VigenciaDescuento
{
    [JsonPropertyName("fecha_inicio")]
    public DateTime? FechaInicio { get; set; }

    [JsonPropertyName("fecha_fin")]
    public DateTime? FechaFin { get; set; }

    [JsonPropertyName("solo_estudiantes_nuevos")]
    public bool SoloEstudiantesNuevos { get; set; } = false;

    [JsonPropertyName("solo_estudiantes_antiguos")]
    public bool SoloEstudiantesAntiguos { get; set; } = false;

    [JsonPropertyName("maximo_usos")]
    [Range(0, int.MaxValue)]
    public int MaximoUsos { get; set; } = 0; // 0 = ilimitado

    [JsonPropertyName("usos_actuales")]
    public int UsosActuales { get; set; } = 0;
}

#endregion

#region Recargos

/// <summary>
/// Configuración de recargos e intereses
/// </summary>
public class RecargoConfig
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
    /// Tipo de recargo: PORCENTAJE_MENSUAL, PORCENTAJE_DIARIO, VALOR_FIJO, ESCALONADO
    /// </summary>
    [MaxLength(30)]
    public string Tipo { get; set; } = "PORCENTAJE_MENSUAL";

    [Range(0, 100)]
    public decimal Valor { get; set; }

    [JsonPropertyName("dias_gracia")]
    [Range(0, 90)]
    public int DiasGracia { get; set; } = 5;

    /// <summary>
    /// Sobre qué se calcula: SALDO_PENDIENTE, VALOR_ORIGINAL, CAPITAL_INTERESES
    /// </summary>
    [MaxLength(30)]
    public string Calculo { get; set; } = "SALDO_PENDIENTE";

    [JsonPropertyName("maximo_acumulable")]
    [Range(0, 200)]
    public decimal MaximoAcumulable { get; set; } = 25;

    [JsonPropertyName("aplicable_a")]
    public List<string> AplicableA { get; set; } = new();

    [JsonPropertyName("requiere_justificacion")]
    public bool RequiereJustificacion { get; set; } = false;

    [JsonPropertyName("configuracion_escalonada")]
    public List<EscalonRecargo> ConfiguracionEscalonada { get; set; } = new();

    [JsonPropertyName("se_capitaliza")]
    public bool SeCapitaliza { get; set; } = false;

    public bool Activo { get; set; } = true;

    [JsonPropertyName("orden_aplicacion")]
    [Range(1, 100)]
    public int OrdenAplicacion { get; set; } = 1;
}

/// <summary>
/// Configuración para recargos escalonados
/// </summary>
public class EscalonRecargo
{
    [JsonPropertyName("desde_dias")]
    [Range(1, 365)]
    public int DesdeDias { get; set; }

    [JsonPropertyName("hasta_dias")]
    [Range(1, 365)]
    public int HastaDias { get; set; }

    [JsonPropertyName("porcentaje_recargo")]
    [Range(0, 100)]
    public decimal PorcentajeRecargo { get; set; }

    [JsonPropertyName("valor_fijo_recargo")]
    [Range(0, double.MaxValue)]
    public decimal ValorFijoRecargo { get; set; }
}

#endregion

#region Políticas de Pago

/// <summary>
/// Configuración de políticas generales de pago
/// </summary>
public class PoliticasPago
{
    [JsonPropertyName("permite_anticipos")]
    public bool PermiteAnticipos { get; set; } = true;

    [JsonPropertyName("genera_saldo_favor")]
    public bool GeneraSaldoFavor { get; set; } = true;

    [JsonPropertyName("aplica_saldo_automaticamente")]
    public bool AplicaSaldoAutomaticamente { get; set; } = true;

    [JsonPropertyName("permite_pago_parcial")]
    public bool PermitePagoParcial { get; set; } = true;

    [JsonPropertyName("minimo_pago_parcial")]
    [Range(0, double.MaxValue)]
    public decimal MinimoPagoParcial { get; set; } = 50000;

    [JsonPropertyName("porcentaje_minimo_pago_parcial")]
    [Range(10, 90)]
    public decimal PorcentajeMinimoPagoParcial { get; set; } = 30;

    [JsonPropertyName("maximo_cuotas")]
    [Range(1, 12)]
    public int MaximoCuotas { get; set; } = 3;

    [JsonPropertyName("interes_financiacion")]
    [Range(0, 10)]
    public decimal InteresFinanciacion { get; set; } = 1.5m;

    [JsonPropertyName("metodos_pago_habilitados")]
    public List<MetodoPagoConfig> MetodosPagoHabilitados { get; set; } = new();

    [JsonPropertyName("politicas_reembolso")]
    public PoliticasReembolso PoliticasReembolso { get; set; } = new();
}

/// <summary>
/// Configuración de métodos de pago
/// </summary>
public class MetodoPagoConfig
{
    [Required]
    [MaxLength(20)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    public bool Habilitado { get; set; } = true;

    [JsonPropertyName("comision_porcentaje")]
    [Range(0, 10)]
    public decimal ComisionPorcentaje { get; set; } = 0;

    [JsonPropertyName("comision_valor_fijo")]
    [Range(0, double.MaxValue)]
    public decimal ComisionValorFijo { get; set; } = 0;

    [JsonPropertyName("minimo_transaccion")]
    [Range(0, double.MaxValue)]
    public decimal MinimoTransaccion { get; set; } = 0;

    [JsonPropertyName("maximo_transaccion")]
    [Range(0, double.MaxValue)]
    public decimal MaximoTransaccion { get; set; } = 0;

    [JsonPropertyName("requiere_validacion")]
    public bool RequiereValidacion { get; set; } = false;

    [JsonPropertyName("tiempo_procesamiento_horas")]
    [Range(0, 72)]
    public int TiempoProcesamiento { get; set; } = 0;
}

/// <summary>
/// Políticas de reembolso
/// </summary>
public class PoliticasReembolso
{
    [JsonPropertyName("permite_reembolsos")]
    public bool PermiteReembolsos { get; set; } = true;

    [JsonPropertyName("dias_limite_reembolso")]
    [Range(1, 365)]
    public int DiasLimiteReembolso { get; set; } = 30;

    [JsonPropertyName("porcentaje_retencion")]
    [Range(0, 30)]
    public decimal PorcentajeRetencion { get; set; } = 10;

    [JsonPropertyName("conceptos_reembolsables")]
    public List<string> ConceptosReembolsables { get; set; } = new();

    [JsonPropertyName("requiere_justificacion")]
    public bool RequiereJustificacion { get; set; } = true;

    [JsonPropertyName("requiere_aprobacion")]
    public bool RequiereAprobacion { get; set; } = true;
}

#endregion

#region Configuración de Facturación

/// <summary>
/// Configuración del sistema de facturación
/// </summary>
public class ConfiguracionFacturacion
{
    [JsonPropertyName("dia_corte_mensual")]
    [Range(1, 31)]
    public int DiaCorteMensual { get; set; } = 30;

    [JsonPropertyName("dias_vencimiento")]
    [Range(1, 90)]
    public int DiasVencimiento { get; set; } = 30;

    [JsonPropertyName("incluir_detalle_hermanos")]
    public bool IncluirDetalleHermanos { get; set; } = true;

    [JsonPropertyName("mostrar_saldo_anterior")]
    public bool MostrarSaldoAnterior { get; set; } = true;

    [JsonPropertyName("formato_numero")]
    [MaxLength(100)]
    public string FormatoNumero { get; set; } = "FC-{YEAR}-{MONTH:00}-{SEQUENCE:0000}";

    [JsonPropertyName("numeracion_por_colegio")]
    public bool NumeracionPorColegio { get; set; } = true;

    [JsonPropertyName("incluir_codigo_barras")]
    public bool IncluirCodigoBarras { get; set; } = true;

    [JsonPropertyName("incluir_qr_pago")]
    public bool IncluirQrPago { get; set; } = false;

    [JsonPropertyName("configuracion_factura_electronica")]
    public FacturaElectronicaConfig ConfiguracionFacturaElectronica { get; set; } = new();

    [JsonPropertyName("plantilla_factura")]
    public PlantillaFacturaConfig PlantillaFactura { get; set; } = new();

    [JsonPropertyName("configuracion_envio")]
    public ConfiguracionEnvioFactura ConfiguracionEnvio { get; set; } = new();
}

/// <summary>
/// Configuración de facturación electrónica
/// </summary>
public class FacturaElectronicaConfig
{
    public bool Habilitada { get; set; } = false;

    [MaxLength(50)]
    public string Proveedor { get; set; } = string.Empty;

    [JsonPropertyName("certificado_digital")]
    public bool CertificadoDigital { get; set; } = false;

    [JsonPropertyName("resolucion_dian")]
    [MaxLength(100)]
    public string ResolucionDian { get; set; } = string.Empty;

    [JsonPropertyName("rango_numeracion")]
    public RangoNumeracion RangoNumeracion { get; set; } = new();

    [JsonPropertyName("configuracion_dian")]
    public Dictionary<string, object> ConfiguracionDian { get; set; } = new();
}

/// <summary>
/// Rango de numeración autorizado por DIAN
/// </summary>
public class RangoNumeracion
{
    [JsonPropertyName("numero_inicial")]
    public long NumeroInicial { get; set; }

    [JsonPropertyName("numero_final")]
    public long NumeroFinal { get; set; }

    [JsonPropertyName("numero_actual")]
    public long NumeroActual { get; set; }

    [JsonPropertyName("fecha_resolucion")]
    public DateTime FechaResolucion { get; set; }

    [JsonPropertyName("vigencia_hasta")]
    public DateTime VigenciaHasta { get; set; }
}

/// <summary>
/// Configuración de plantilla de factura
/// </summary>
public class PlantillaFacturaConfig
{
    [JsonPropertyName("incluir_logo")]
    public bool IncluirLogo { get; set; } = true;

    [JsonPropertyName("incluir_slogan")]
    public bool IncluirSlogan { get; set; } = true;

    [JsonPropertyName("mostrar_descuentos_detallados")]
    public bool MostrarDescuentosDetallados { get; set; } = true;

    [JsonPropertyName("incluir_grafico_estado_cuenta")]
    public bool IncluirGraficoEstadoCuenta { get; set; } = false;

    [JsonPropertyName("texto_pie_factura")]
    [MaxLength(500)]
    public string TextoPieFactura { get; set; } = string.Empty;

    [JsonPropertyName("informacion_bancaria")]
    public List<InformacionBancaria> InformacionBancaria { get; set; } = new();
}

/// <summary>
/// Información bancaria para pagos
/// </summary>
public class InformacionBancaria
{
    [Required]
    [MaxLength(100)]
    public string Banco { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string TipoCuenta { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string NumeroCuenta { get; set; } = string.Empty;

    [JsonPropertyName("titular_cuenta")]
    [MaxLength(200)]
    public string TitularCuenta { get; set; } = string.Empty;

    public bool Principal { get; set; } = false;

    public bool Activa { get; set; } = true;
}

/// <summary>
/// Configuración de envío de facturas
/// </summary>
public class ConfiguracionEnvioFactura
{
    [JsonPropertyName("envio_automatico")]
    public bool EnvioAutomatico { get; set; } = true;

    [JsonPropertyName("enviar_por_email")]
    public bool EnviarPorEmail { get; set; } = true;

    [JsonPropertyName("enviar_por_sms")]
    public bool EnviarPorSms { get; set; } = false;

    [JsonPropertyName("enviar_notificacion_push")]
    public bool EnviarNotificacionPush { get; set; } = true;

    [JsonPropertyName("dias_recordatorio")]
    public List<int> DiasRecordatorio { get; set; } = new() { 15, 7, 3, 1 };

    [JsonPropertyName("plantilla_email")]
    [MaxLength(2000)]
    public string PlantillaEmail { get; set; } = string.Empty;

    [JsonPropertyName("plantilla_sms")]
    [MaxLength(160)]
    public string PlantillaSms { get; set; } = string.Empty;
}

#endregion

#region Alertas Financieras

/// <summary>
/// Configuración de alertas y notificaciones financieras
/// </summary>
public class AlertasFinancieras
{
    [JsonPropertyName("saldo_vencido_dias")]
    public List<int> SaldoVencidoDias { get; set; } = new() { 15, 30, 60, 90 };

    [JsonPropertyName("porcentaje_cartera_alerta")]
    [Range(1, 50)]
    public decimal PorcentajeCarteraAlerta { get; set; } = 15;

    [JsonPropertyName("monto_individual_alerta")]
    [Range(0, double.MaxValue)]
    public decimal MontoIndividualAlerta { get; set; } = 500000;

    [JsonPropertyName("notificar_coordinacion")]
    public bool NotificarCoordinacion { get; set; } = true;

    [JsonPropertyName("notificar_rectoria")]
    public bool NotificarRectoria { get; set; } = false;

    [JsonPropertyName("generar_reporte_automatico")]
    public bool GenerarReporteAutomatico { get; set; } = true;

    [JsonPropertyName("frecuencia_reporte_dias")]
    [Range(1, 30)]
    public int FrecuenciaReporteDias { get; set; } = 7;

    [JsonPropertyName("configuracion_escalamiento")]
    public List<EscalamientoAlerta> ConfiguracionEscalamiento { get; set; } = new();
}

/// <summary>
/// Configuración de escalamiento de alertas financieras
/// </summary>
public class EscalamientoAlerta
{
    [JsonPropertyName("dias_vencido")]
    [Range(1, 365)]
    public int DiasVencido { get; set; }

    [JsonPropertyName("nivel_escalamiento")]
    [MaxLength(20)]
    public string NivelEscalamiento { get; set; } = "COORDINACION"; // COORDINACION, RECTORIA, GERENCIA, JURIDICO

    [JsonPropertyName("accion_automatica")]
    [MaxLength(50)]
    public string AccionAutomatica { get; set; } = "NOTIFICAR"; // NOTIFICAR, SUSPENDER, COBRO_JURIDICO

    [JsonPropertyName("plantilla_mensaje")]
    [MaxLength(1000)]
    public string PlantillaMensaje { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;
}

#endregion

#region Configuración de Cartera

/// <summary>
/// Configuración para gestión de cartera
/// </summary>
public class ConfiguracionCartera
{
    [JsonPropertyName("clasificacion_edades")]
    public List<ClasificacionEdadCartera> ClasificacionEdades { get; set; } = new();

    [JsonPropertyName("configuracion_castigo")]
    public ConfiguracionCastigoCartera ConfiguracionCastigo { get; set; } = new();

    [JsonPropertyName("configuracion_provision")]
    public ConfiguracionProvision ConfiguracionProvision { get; set; } = new();

    [JsonPropertyName("gestores_cobranza")]
    public List<GestorCobranza> GestoresCobranza { get; set; } = new();
}

/// <summary>
/// Clasificación de cartera por edades
/// </summary>
public class ClasificacionEdadCartera
{
    [JsonPropertyName("desde_dias")]
    [Range(0, 1000)]
    public int DesdeDias { get; set; }

    [JsonPropertyName("hasta_dias")]
    [Range(1, 1000)]
    public int HastaDias { get; set; }

    [Required]
    [MaxLength(50)]
    public string Clasificacion { get; set; } = string.Empty;

    [JsonPropertyName("porcentaje_provision")]
    [Range(0, 100)]
    public decimal PorcentajeProvision { get; set; }

    [JsonPropertyName("accion_automatica")]
    [MaxLength(50)]
    public string AccionAutomatica { get; set; } = string.Empty;

    [MaxLength(7)]
    public string Color { get; set; } = "#000000";
}

/// <summary>
/// Configuración para castigo de cartera incobrable
/// </summary>
public class ConfiguracionCastigoCartera
{
    [JsonPropertyName("permite_castigo")]
    public bool PermiteCastigo { get; set; } = false;

    [JsonPropertyName("dias_minimos_castigo")]
    [Range(180, 1095)]
    public int DiasMinimos { get; set; } = 365;

    [JsonPropertyName("requiere_aprobacion_gerencia")]
    public bool RequiereAprobacionGerencia { get; set; } = true;

    [JsonPropertyName("porcentaje_maximo_castigo")]
    [Range(1, 20)]
    public decimal PorcentajeMaximoCastigo { get; set; } = 5;
}

/// <summary>
/// Configuración de provisiones contables
/// </summary>
public class ConfiguracionProvision
{
    [JsonPropertyName("calculo_automatico")]
    public bool CalculoAutomatico { get; set; } = true;

    [JsonPropertyName("metodo_calculo")]
    [MaxLength(30)]
    public string MetodoCalculo { get; set; } = "POR_EDADES"; // POR_EDADES, INDIVIDUAL, PROMEDIO_HISTORICO

    [JsonPropertyName("revision_mensual")]
    public bool RevisionMensual { get; set; } = true;

    [JsonPropertyName("factores_riesgo")]
    public List<FactorRiesgo> FactoresRiesgo { get; set; } = new();
}

/// <summary>
/// Factores de riesgo para cálculo de provisiones
/// </summary>
public class FactorRiesgo
{
    [Required]
    [MaxLength(50)]
    public string Factor { get; set; } = string.Empty;

    [JsonPropertyName("peso_porcentual")]
    [Range(0, 100)]
    public decimal PesoPorcentual { get; set; }

    [JsonPropertyName("valor_maximo")]
    [Range(0, 100)]
    public decimal ValorMaximo { get; set; }

    public bool Activo { get; set; } = true;
}

/// <summary>
/// Configuración de gestores de cobranza
/// </summary>
public class GestorCobranza
{
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [JsonPropertyName("usuario_id")]
    public Guid? UsuarioId { get; set; }

    [JsonPropertyName("email")]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("telefono")]
    [MaxLength(20)]
    public string Telefono { get; set; } = string.Empty;

    [JsonPropertyName("cartera_asignada_maxima")]
    [Range(0, double.MaxValue)]
    public decimal CarteraAsignadaMaxima { get; set; } = 0;

    [JsonPropertyName("comision_recuperacion")]
    [Range(0, 30)]
    public decimal ComisionRecuperacion { get; set; } = 0;
        
    [JsonPropertyName("estrategias_cobranza")]
    public List<EstrategiaCobranza> EstrategiasCobranza { get; set; } = new();

    public bool Activo { get; set; } = true;
}

#endregion
