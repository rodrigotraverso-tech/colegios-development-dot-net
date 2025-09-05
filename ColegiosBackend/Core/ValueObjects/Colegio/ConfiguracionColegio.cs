using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ColegiosBackend.Core.ValueObjects.Colegio;

/// <summary>
/// Configuración integral del colegio almacenada en campo JSONB
/// Contiene todas las configuraciones académicas, financieras y operativas
/// </summary>
public class ConfiguracionColegio
{
    [JsonPropertyName("calendario_academico")]
    public CalendarioAcademico CalendarioAcademico { get; set; } = new();
    
    [JsonPropertyName("sistema_calificacion")]
    public SistemaCalificacion SistemaCalificacion { get; set; } = new();
    
    public HorariosConfig Horarios { get; set; } = new();
    
    public AsistenciaConfig Asistencia { get; set; } = new();
    
    public DisciplinaConfig Disciplina { get; set; } = new();
    
    public FinancieroConfig Financiero { get; set; } = new();
    
    public ComunicacionesConfig Comunicaciones { get; set; } = new();
    
    public AcademicoConfig Academico { get; set; } = new();
    
    public SeguridadConfig Seguridad { get; set; } = new();
    
    public PersonalizacionConfig Personalizacion { get; set; } = new();
    
    [JsonPropertyName("modulos_activos")]
    public ModulosActivosConfig ModulosActivos { get; set; } = new();
    
    public IntegracionesConfig Integraciones { get; set; } = new();
    
    public ReportesConfig Reportes { get; set; } = new();
}

#region Calendario Académico

public class CalendarioAcademico
{
    [JsonPropertyName("inicio_clases")]
    public DateTime InicioClases { get; set; }
    
    [JsonPropertyName("fin_clases")]
    public DateTime FinClases { get; set; }
    
    public List<PeriodoConfig> Periodos { get; set; } = new();
    
    public List<VacacionConfig> Vacaciones { get; set; } = new();
}

public class PeriodoConfig
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
}

public class VacacionConfig
{
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;
    
    [JsonPropertyName("fecha_inicio")]
    public DateTime FechaInicio { get; set; }
    
    [JsonPropertyName("fecha_fin")]
    public DateTime FechaFin { get; set; }
}

#endregion

#region Sistema de Calificación

public class SistemaCalificacion
{
    [JsonPropertyName("escala_minima")]
    [Range(0, 10)]
    public decimal EscalaMinima { get; set; } = 1.0m;
    
    [JsonPropertyName("escala_maxima")]
    [Range(1, 10)]
    public decimal EscalaMaxima { get; set; } = 5.0m;
    
    [JsonPropertyName("nota_minima_aprobacion")]
    public decimal NotaMinimaAprobacion { get; set; } = 3.0m;
    
    [Range(0, 3)]
    public int Decimales { get; set; } = 1;
    
    [JsonPropertyName("permite_recuperacion")]
    public bool PermiteRecuperacion { get; set; } = true;
    
    [JsonPropertyName("nota_maxima_recuperacion")]
    public decimal NotaMaximaRecuperacion { get; set; } = 3.5m;
    
    [JsonPropertyName("escalas_cualitativas")]
    public Dictionary<string, string> EscalasCualitativas { get; set; } = new();
    
    [JsonPropertyName("tipos_evaluacion")]
    public List<TipoEvaluacionConfig> TiposEvaluacion { get; set; } = new();
}

public class TipoEvaluacionConfig
{
    [Required]
    [MaxLength(20)]
    public string Codigo { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;
    
    [JsonPropertyName("porcentaje_maximo")]
    [Range(0, 100)]
    public decimal PorcentajeMaximo { get; set; }
}

#endregion

#region Horarios

public class HorariosConfig
{
    [JsonPropertyName("hora_inicio_jornada")]
    public string HoraInicioJornada { get; set; } = "07:00";
    
    [JsonPropertyName("hora_fin_jornada")]
    public string HoraFinJornada { get; set; } = "15:00";
    
    [JsonPropertyName("duracion_clase_minutos")]
    [Range(30, 120)]
    public int DuracionClaseMinutos { get; set; } = 45;
    
    [JsonPropertyName("duracion_descanso_minutos")]
    [Range(5, 60)]
    public int DuracionDescansoMinutos { get; set; } = 15;
    
    public List<DescansoConfig> Descansos { get; set; } = new();
    
    public List<JornadaConfig> Jornadas { get; set; } = new();
}

public class DescansoConfig
{
    [Required]
    [MaxLength(50)]
    public string Nombre { get; set; } = string.Empty;
    
    [JsonPropertyName("hora_inicio")]
    public string HoraInicio { get; set; } = string.Empty;
    
    [JsonPropertyName("hora_fin")]
    public string HoraFin { get; set; } = string.Empty;
}

public class JornadaConfig
{
    [Required]
    [MaxLength(20)]
    public string Codigo { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;
    
    [JsonPropertyName("hora_inicio")]
    public string HoraInicio { get; set; } = string.Empty;
    
    [JsonPropertyName("hora_fin")]
    public string HoraFin { get; set; } = string.Empty;
}

#endregion

#region Asistencia

public class AsistenciaConfig
{
    [JsonPropertyName("tolerancia_llegada_tarde_minutos")]
    [Range(0, 60)]
    public int ToleranciaLlegadaTardeMinutos { get; set; } = 15;
    
    [JsonPropertyName("requiere_justificacion_falta")]
    public bool RequiereJustificacionFalta { get; set; } = true;
    
    [JsonPropertyName("dias_maximos_justificar")]
    [Range(1, 30)]
    public int DiasMaximosJustificar { get; set; } = 3;
    
    [JsonPropertyName("porcentaje_minimo_asistencia")]
    [Range(50, 100)]
    public int PorcentajeMinimoAsistencia { get; set; } = 85;
    
    [JsonPropertyName("notificar_falta_automaticamente")]
    public bool NotificarFaltaAutomaticamente { get; set; } = true;
    
    [JsonPropertyName("estados_permitidos")]
    public List<string> EstadosPermitidos { get; set; } = new() 
    { 
        "PRESENTE", "AUSENTE", "TARDE", "EXCUSADO", "PERMISO" 
    };
}

#endregion

#region Disciplina

public class DisciplinaConfig
{
    [JsonPropertyName("tipos_observaciones")]
    public List<TipoObservacionConfig> TiposObservaciones { get; set; } = new();
    
    [JsonPropertyName("requiere_firma_acudiente")]
    public List<string> RequiereFirmaAcudiente { get; set; } = new();
    
    [JsonPropertyName("notificar_acudiente_automaticamente")]
    public bool NotificarAcudienteAutomaticamente { get; set; } = true;
}

public class TipoObservacionConfig
{
    [Required]
    [MaxLength(20)]
    public string Codigo { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;
    
    [MaxLength(7)]
    public string Color { get; set; } = "#000000";
}

#endregion

#region Financiero

public class FinancieroConfig
{
    [MaxLength(3)]
    public string Moneda { get; set; } = "COP";
    
    [JsonPropertyName("formato_factura")]
    [MaxLength(50)]
    public string FormatoFactura { get; set; } = "FC-{YEAR}-{NUMERO}";
    
    [JsonPropertyName("dias_vencimiento_factura")]
    [Range(1, 365)]
    public int DiasVencimientoFactura { get; set; } = 30;
    
    [JsonPropertyName("permite_pago_parcial")]
    public bool PermitePagoParcial { get; set; } = true;
    
    [JsonPropertyName("descuento_hermanos")]
    public DescuentoHermanosConfig DescuentoHermanos { get; set; } = new();
    
    [JsonPropertyName("intereses_mora")]
    public InteresesMoraConfig InteresesMora { get; set; } = new();
    
    [JsonPropertyName("metodos_pago_permitidos")]
    public List<string> MetodosPagoPermitidos { get; set; } = new() 
    { 
        "EFECTIVO", "TARJETA_CREDITO", "TARJETA_DEBITO", "TRANSFERENCIA", "PSE" 
    };
}

public class DescuentoHermanosConfig
{
    public bool Activo { get; set; } = true;
    
    [JsonPropertyName("segundo_hermano")]
    [Range(0, 50)]
    public decimal SegundoHermano { get; set; } = 10;
    
    [JsonPropertyName("tercer_hermano")]
    [Range(0, 50)]
    public decimal TercerHermano { get; set; } = 15;
    
    [JsonPropertyName("cuarto_hermano_en_adelante")]
    [Range(0, 50)]
    public decimal CuartoHermanoEnAdelante { get; set; } = 20;
}

public class InteresesMoraConfig
{
    public bool Activo { get; set; } = true;
    
    [JsonPropertyName("porcentaje_mensual")]
    [Range(0, 10)]
    public decimal PorcentajeMensual { get; set; } = 2.5m;
    
    [JsonPropertyName("dias_gracia")]
    [Range(0, 30)]
    public int DiasGracia { get; set; } = 5;
}

#endregion

#region Comunicaciones

public class ComunicacionesConfig
{
    [JsonPropertyName("email_automatico")]
    public EmailAutomaticoConfig EmailAutomatico { get; set; } = new();
    
    [JsonPropertyName("sms_automatico")]
    public SmsAutomaticoConfig SmsAutomatico { get; set; } = new();
    
    [JsonPropertyName("notificaciones_push")]
    public NotificacionesPushConfig NotificacionesPush { get; set; } = new();
}

public class EmailAutomaticoConfig
{
    [JsonPropertyName("notas_publicadas")]
    public bool NotasPublicadas { get; set; } = true;
    
    [JsonPropertyName("faltas_estudiante")]
    public bool FaltasEstudiante { get; set; } = true;
    
    [JsonPropertyName("vencimiento_facturas")]
    public bool VencimientoFacturas { get; set; } = true;
    
    [JsonPropertyName("eventos_importantes")]
    public bool EventosImportantes { get; set; } = true;
}

public class SmsAutomaticoConfig
{
    public bool Emergencias { get; set; } = true;
    
    [JsonPropertyName("llegada_tarde")]
    public bool LlegadaTarde { get; set; } = false;
    
    [JsonPropertyName("facturas_vencidas")]
    public bool FacturasVencidas { get; set; } = true;
}

public class NotificacionesPushConfig
{
    [JsonPropertyName("todas_habilitadas")]
    public bool TodasHabilitadas { get; set; } = true;
    
    [JsonPropertyName("horario_silencio")]
    public HorarioSilencioConfig HorarioSilencio { get; set; } = new();
}

public class HorarioSilencioConfig
{
    public string Inicio { get; set; } = "22:00";
    public string Fin { get; set; } = "06:00";
}

#endregion

#region Académico

public class AcademicoConfig
{
    [JsonPropertyName("permite_calificaciones_decimales")]
    public bool PermiteCalificacionesDecimales { get; set; } = true;
    
    public string Redondeo { get; set; } = "MATEMATICO"; // MATEMATICO, HACIA_ARRIBA, HACIA_ABAJO
    
    [JsonPropertyName("muestra_puesto_estudiante")]
    public bool MuestraPuestoEstudiante { get; set; } = true;
    
    [JsonPropertyName("calcula_promedio_acumulado")]
    public bool CalculaPromedioAcumulado { get; set; } = true;
    
    [JsonPropertyName("requiere_observaciones_nota_baja")]
    public bool RequiereObservacionesNotaBaja { get; set; } = true;
    
    [JsonPropertyName("limite_nota_baja")]
    public decimal LimiteNotaBaja { get; set; } = 3.0m;
    
    [JsonPropertyName("permite_edicion_notas")]
    public EdicionNotasConfig PermiteEdicionNotas { get; set; } = new();
}

public class EdicionNotasConfig
{
    public bool Activo { get; set; } = true;
    
    [JsonPropertyName("dias_limite")]
    [Range(1, 90)]
    public int DiasLimite { get; set; } = 7;
    
    [JsonPropertyName("requiere_autorizacion")]
    public bool RequiereAutorizacion { get; set; } = true;
}

#endregion

#region Seguridad

public class SeguridadConfig
{
    [JsonPropertyName("sesion_expira_minutos")]
    [Range(30, 1440)]
    public int SesionExpiraMinutos { get; set; } = 480;
    
    [JsonPropertyName("intentos_login_maximos")]
    [Range(3, 10)]
    public int IntentosLoginMaximos { get; set; } = 3;
    
    [JsonPropertyName("bloqueo_cuenta_minutos")]
    [Range(5, 120)]
    public int BloqueoCuentaMinutos { get; set; } = 30;
    
    [JsonPropertyName("requiere_cambio_password_dias")]
    [Range(30, 365)]
    public int RequiereCambioPasswordDias { get; set; } = 90;
    
    [JsonPropertyName("longitud_minima_password")]
    [Range(6, 20)]
    public int LongitudMinimaPassword { get; set; } = 8;
    
    [JsonPropertyName("requiere_caracteres_especiales")]
    public bool RequiereCaracteresEspeciales { get; set; } = true;
    
    [JsonPropertyName("backup_automatico")]
    public BackupAutomaticoConfig BackupAutomatico { get; set; } = new();
}

public class BackupAutomaticoConfig
{
    public bool Activo { get; set; } = true;
    
    [JsonPropertyName("frecuencia_horas")]
    [Range(1, 168)]
    public int FrecuenciaHoras { get; set; } = 24;
    
    [JsonPropertyName("retener_dias")]
    [Range(7, 365)]
    public int RetenerDias { get; set; } = 30;
}

#endregion

#region Personalización

public class PersonalizacionConfig
{
    [JsonPropertyName("logo_principal")]
    [MaxLength(500)]
    public string LogoPrincipal { get; set; } = string.Empty;
    
    [JsonPropertyName("logo_reportes")]
    [MaxLength(500)]
    public string LogoReportes { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Favicon { get; set; } = string.Empty;
    
    public ColoresPersonalizacionConfig Colores { get; set; } = new();
    
    public FuentesConfig Fuentes { get; set; } = new();
    
    [JsonPropertyName("mostrar_marca_agua")]
    public bool MostrarMarcaAgua { get; set; } = true;
    
    [JsonPropertyName("pie_pagina_reportes")]
    [MaxLength(200)]
    public string PiePaginaReportes { get; set; } = string.Empty;
}

public class ColoresPersonalizacionConfig
{
    [MaxLength(7)]
    public string Primario { get; set; } = "#1565C0";
    
    [MaxLength(7)]
    public string Secundario { get; set; } = "#FFA726";
    
    [MaxLength(7)]
    public string Acento { get; set; } = "#43A047";
    
    [MaxLength(7)]
    public string Fondo { get; set; } = "#F5F5F5";
}

public class FuentesConfig
{
    [MaxLength(50)]
    public string Principal { get; set; } = "Roboto";
    
    [MaxLength(50)]
    public string Titulos { get; set; } = "Montserrat";
}

#endregion

#region Módulos Activos

public class ModulosActivosConfig
{
    [JsonPropertyName("gestion_estudiantes")]
    public bool GestionEstudiantes { get; set; } = true;
    
    [JsonPropertyName("gestion_profesores")]
    public bool GestionProfesores { get; set; } = true;
    
    public bool Calificaciones { get; set; } = true;
    
    public bool Asistencia { get; set; } = true;
    
    public bool Disciplina { get; set; } = true;
    
    public bool Financiero { get; set; } = true;
    
    public bool Reportes { get; set; } = true;
    
    public bool Comunicaciones { get; set; } = true;
    
    public bool Biblioteca { get; set; } = false;
    
    public bool Transporte { get; set; } = false;
    
    public bool Inventario { get; set; } = false;
    
    public bool Nomina { get; set; } = false;
}

#endregion

#region Integraciones

public class IntegracionesConfig
{
    [JsonPropertyName("ministerio_educacion")]
    public MinisterioEducacionConfig MinisterioEducacion { get; set; } = new();
    
    [JsonPropertyName("plataforma_pagos")]
    public PlataformaPagosConfig PlataformaPagos { get; set; } = new();
    
    [JsonPropertyName("sms_gateway")]
    public SmsGatewayConfig SmsGateway { get; set; } = new();
}

public class MinisterioEducacionConfig
{
    public bool Activo { get; set; } = false;
    
    [JsonPropertyName("codigo_dane")]
    [MaxLength(20)]
    public string CodigoDane { get; set; } = string.Empty;
    
    [JsonPropertyName("reporte_automatico")]
    public bool ReporteAutomatico { get; set; } = false;
}

public class PlataformaPagosConfig
{
    public bool Activo { get; set; } = false;
    
    [MaxLength(50)]
    public string Proveedor { get; set; } = string.Empty;
    
    public Dictionary<string, object> Configuracion { get; set; } = new();
}

public class SmsGatewayConfig
{
    public bool Activo { get; set; } = false;
    
    [MaxLength(50)]
    public string Proveedor { get; set; } = string.Empty;
    
    public Dictionary<string, object> Configuracion { get; set; } = new();
}

#endregion

#region Reportes

public class ReportesConfig
{
    [JsonPropertyName("formatos_disponibles")]
    public List<string> FormatosDisponibles { get; set; } = new() { "PDF", "EXCEL", "CSV" };
    
    [JsonPropertyName("incluir_logo")]
    public bool IncluirLogo { get; set; } = true;
    
    [JsonPropertyName("incluir_firma_digital")]
    public bool IncluirFirmaDigital { get; set; } = false;
    
    [JsonPropertyName("plantillas_personalizadas")]
    public PlantillasPersonalizadasConfig PlantillasPersonalizadas { get; set; } = new();
}

public class PlantillasPersonalizadasConfig
{
    [MaxLength(100)]
    public string Boletin { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string Certificado { get; set; } = string.Empty;
    
    [JsonPropertyName("paz_y_salvo")]
    [MaxLength(100)]
    public string PazYSalvo { get; set; } = string.Empty;
}

#endregion