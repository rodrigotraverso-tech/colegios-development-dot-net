using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ColegiosBackend.Core.ValueObjects.Permisos;

/// <summary>
/// Sistema completo de permisos granulares por rol
/// Almacenado en campo JSONB de la tabla roles
/// </summary>
public class PermisosRol
{
    public ModulosPermisos Modulos { get; set; } = new();

    public RestriccionesRol Restricciones { get; set; } = new();

    [JsonPropertyName("configuracion_especial")]
    public ConfiguracionEspecialRol ConfiguracionEspecial { get; set; } = new();
}

#region Módulos de Permisos

/// <summary>
/// Permisos organizados por módulos del sistema
/// </summary>
public class ModulosPermisos
{
    public PermisoModuloEstudiantes Estudiantes { get; set; } = new();

    public PermisoModuloProfesores Profesores { get; set; } = new();

    public PermisoModuloCalificaciones Calificaciones { get; set; } = new();

    public PermisoModuloAsistencia Asistencia { get; set; } = new();

    public PermisoModuloFinanciero Financiero { get; set; } = new();

    public PermisoModuloReportes Reportes { get; set; } = new();

    public PermisoModuloComunicaciones Comunicaciones { get; set; } = new();

    public PermisoModuloAdministracion Administracion { get; set; } = new();

    public PermisoModuloDisciplina Disciplina { get; set; } = new();

    public PermisoModuloBiblioteca Biblioteca { get; set; } = new();
}

#endregion

#region Permisos Base

/// <summary>
/// Permisos básicos CRUD aplicables a la mayoría de módulos
/// </summary>
public class PermisoModuloBase
{
    public bool Ver { get; set; } = false;

    public bool Crear { get; set; } = false;

    public bool Editar { get; set; } = false;

    public bool Eliminar { get; set; } = false;

    public bool Exportar { get; set; } = false;
}

#endregion

#region Permisos por Módulo

/// <summary>
/// Permisos específicos para gestión de estudiantes
/// </summary>
public class PermisoModuloEstudiantes : PermisoModuloBase
{
    [JsonPropertyName("matricular_estudiantes")]
    public bool MatricularEstudiantes { get; set; } = false;

    [JsonPropertyName("cambiar_grupo")]
    public bool CambiarGrupo { get; set; } = false;

    [JsonPropertyName("ver_informacion_familiar")]
    public bool VerInformacionFamiliar { get; set; } = false;

    [JsonPropertyName("editar_datos_personales")]
    public bool EditarDatosPersonales { get; set; } = false;

    [JsonPropertyName("generar_certificados")]
    public bool GenerarCertificados { get; set; } = false;

    [JsonPropertyName("ver_historial_academico")]
    public bool VerHistorialAcademico { get; set; } = false;

    [JsonPropertyName("gestionar_documentos")]
    public bool GestionarDocumentos { get; set; } = false;
}

/// <summary>
/// Permisos específicos para gestión de profesores
/// </summary>
public class PermisoModuloProfesores : PermisoModuloBase
{
    [JsonPropertyName("asignar_materias")]
    public bool AsignarMaterias { get; set; } = false;

    [JsonPropertyName("asignar_grupos")]
    public bool AsignarGrupos { get; set; } = false;

    [JsonPropertyName("ver_horarios")]
    public bool VerHorarios { get; set; } = false;

    [JsonPropertyName("editar_horarios")]
    public bool EditarHorarios { get; set; } = false;

    [JsonPropertyName("evaluar_desempeño")]
    public bool EvaluarDesempeño { get; set; } = false;

    [JsonPropertyName("gestionar_permisos")]
    public bool GestionarPermisos { get; set; } = false;
}

/// <summary>
/// Permisos específicos para calificaciones
/// </summary>
public class PermisoModuloCalificaciones : PermisoModuloBase
{
    public bool Publicar { get; set; } = false;

    [JsonPropertyName("ver_otras_materias")]
    public bool VerOtrasMaterias { get; set; } = false;

    [JsonPropertyName("editar_despues_publicacion")]
    public bool EditarDespuesPublicacion { get; set; } = false;

    [JsonPropertyName("crear_tipos_evaluacion")]
    public bool CrearTiposEvaluacion { get; set; } = false;

    [JsonPropertyName("configurar_escalas")]
    public bool ConfigurarEscalas { get; set; } = false;

    [JsonPropertyName("generar_boletines")]
    public bool GenerarBoletines { get; set; } = false;

    [JsonPropertyName("ver_estadisticas")]
    public bool VerEstadisticas { get; set; } = false;

    [JsonPropertyName("recuperaciones")]
    public bool Recuperaciones { get; set; } = false;
}

/// <summary>
/// Permisos específicos para asistencia
/// </summary>
public class PermisoModuloAsistencia : PermisoModuloBase
{
    public bool Registrar { get; set; } = false;

    public bool Justificar { get; set; } = false;

    [JsonPropertyName("editar_registros")]
    public bool EditarRegistros { get; set; } = false;

    [JsonPropertyName("ver_estadisticas")]
    public bool VerEstadisticas { get; set; } = false;

    [JsonPropertyName("notificar_ausencias")]
    public bool NotificarAusencias { get; set; } = false;

    [JsonPropertyName("generar_reportes")]
    public bool GenerarReportes { get; set; } = false;

    [JsonPropertyName("configurar_estados")]
    public bool ConfigurarEstados { get; set; } = false;
}

/// <summary>
/// Permisos específicos para módulo financiero
/// </summary>
public class PermisoModuloFinanciero
{
    [JsonPropertyName("ver_facturas")]
    public bool VerFacturas { get; set; } = false;

    [JsonPropertyName("crear_facturas")]
    public bool CrearFacturas { get; set; } = false;

    [JsonPropertyName("editar_facturas")]
    public bool EditarFacturas { get; set; } = false;

    [JsonPropertyName("anular_facturas")]
    public bool AnularFacturas { get; set; } = false;

    [JsonPropertyName("recibir_pagos")]
    public bool RecibirPagos { get; set; } = false;

    [JsonPropertyName("aplicar_descuentos")]
    public bool AplicarDescuentos { get; set; } = false;

    [JsonPropertyName("generar_recibos")]
    public bool GenerarRecibos { get; set; } = false;

    [JsonPropertyName("ver_cartera")]
    public bool VerCartera { get; set; } = false;

    [JsonPropertyName("reportes_financieros")]
    public bool ReportesFinancieros { get; set; } = false;

    [JsonPropertyName("configurar_conceptos")]
    public bool ConfigurarConceptos { get; set; } = false;

    [JsonPropertyName("gestionar_becas")]
    public bool GestionarBecas { get; set; } = false;

    [JsonPropertyName("estados_cuenta")]
    public bool EstadosCuenta { get; set; } = false;
}

/// <summary>
/// Permisos específicos para reportes
/// </summary>
public class PermisoModuloReportes
{
    public bool Boletines { get; set; } = false;

    public bool Certificados { get; set; } = false;

    [JsonPropertyName("listas_estudiantes")]
    public bool ListasEstudiantes { get; set; } = false;

    [JsonPropertyName("reportes_academicos")]
    public bool ReportesAcademicos { get; set; } = false;

    [JsonPropertyName("reportes_financieros")]
    public bool ReportesFinancieros { get; set; } = false;

    [JsonPropertyName("reportes_disciplinarios")]
    public bool ReportesDisciplinarios { get; set; } = false;

    [JsonPropertyName("reportes_asistencia")]
    public bool ReportesAsistencia { get; set; } = false;

    [JsonPropertyName("estadisticas_generales")]
    public bool EstadisticasGenerales { get; set; } = false;

    [JsonPropertyName("exportar_datos")]
    public bool ExportarDatos { get; set; } = false;

    [JsonPropertyName("personalizar_plantillas")]
    public bool PersonalizarPlantillas { get; set; } = false;

    [JsonPropertyName("reportes_gobierno")]
    public bool ReportesGobierno { get; set; } = false;
}

/// <summary>
/// Permisos específicos para comunicaciones
/// </summary>
public class PermisoModuloComunicaciones
{
    [JsonPropertyName("enviar_notificaciones")]
    public bool EnviarNotificaciones { get; set; } = false;

    [JsonPropertyName("mensajes_acudientes")]
    public bool MensajesAcudientes { get; set; } = false;

    [JsonPropertyName("mensajes_masivos")]
    public bool MensajesMasivos { get; set; } = false;

    [JsonPropertyName("crear_circulares")]
    public bool CrearCirculares { get; set; } = false;

    [JsonPropertyName("configurar_plantillas")]
    public bool ConfigurarPlantillas { get; set; } = false;

    [JsonPropertyName("historial_mensajes")]
    public bool HistorialMensajes { get; set; } = false;

    [JsonPropertyName("notificaciones_automaticas")]
    public bool NotificacionesAutomaticas { get; set; } = false;

    [JsonPropertyName("emergency_alerts")]
    public bool EmergencyAlerts { get; set; } = false;
}

/// <summary>
/// Permisos específicos para administración del sistema
/// </summary>
public class PermisoModuloAdministracion
{
    [JsonPropertyName("gestionar_usuarios")]
    public bool GestionarUsuarios { get; set; } = false;

    [JsonPropertyName("configurar_colegio")]
    public bool ConfigurarColegio { get; set; } = false;

    [JsonPropertyName("gestionar_roles")]
    public bool GestionarRoles { get; set; } = false;

    [JsonPropertyName("logs_auditoria")]
    public bool LogsAuditoria { get; set; } = false;

    [JsonPropertyName("backup_restaurar")]
    public bool BackupRestaurar { get; set; } = false;

    [JsonPropertyName("configurar_periodos")]
    public bool ConfigurarPeriodos { get; set; } = false;

    [JsonPropertyName("gestionar_grados")]
    public bool GestionarGrados { get; set; } = false;

    [JsonPropertyName("configurar_horarios")]
    public bool ConfigurarHorarios { get; set; } = false;

    [JsonPropertyName("integraciones")]
    public bool Integraciones { get; set; } = false;

    [JsonPropertyName("configuracion_avanzada")]
    public bool ConfiguracionAvanzada { get; set; } = false;
}

/// <summary>
/// Permisos específicos para disciplina
/// </summary>
public class PermisoModuloDisciplina : PermisoModuloBase
{
    [JsonPropertyName("crear_observaciones")]
    public bool CrearObservaciones { get; set; } = false;

    [JsonPropertyName("editar_observaciones")]
    public bool EditarObservaciones { get; set; } = false;

    [JsonPropertyName("aprobar_sanciones")]
    public bool AprobarSanciones { get; set; } = false;

    [JsonPropertyName("seguimientos")]
    public bool Seguimientos { get; set; } = false;

    [JsonPropertyName("comite_convivencia")]
    public bool ComiteConvivencia { get; set; } = false;
}

/// <summary>
/// Permisos específicos para biblioteca
/// </summary>
public class PermisoModuloBiblioteca : PermisoModuloBase
{
    [JsonPropertyName("gestionar_prestamos")]
    public bool GestionarPrestamos { get; set; } = false;

    [JsonPropertyName("catalogar_libros")]
    public bool CatalogarLibros { get; set; } = false;

    [JsonPropertyName("reservar_recursos")]
    public bool ReservarRecursos { get; set; } = false;

    [JsonPropertyName("multas_retrasos")]
    public bool MultasRetrasos { get; set; } = false;
}

#endregion

#region Restricciones

/// <summary>
/// Restricciones y limitaciones del rol
/// </summary>
public class RestriccionesRol
{
    [JsonPropertyName("solo_sus_estudiantes")]
    public bool SoloSusEstudiantes { get; set; } = true;

    [JsonPropertyName("solo_sus_materias")]
    public bool SoloSusMaterias { get; set; } = true;

    [JsonPropertyName("solo_su_grupo")]
    public bool SoloSuGrupo { get; set; } = false;

    [JsonPropertyName("solo_su_colegio")]
    public bool SoloSuColegio { get; set; } = true;

    [JsonPropertyName("horario_acceso")]
    public HorarioAcceso HorarioAcceso { get; set; } = new();

    [JsonPropertyName("ip_permitidas")]
    public List<string> IpPermitidas { get; set; } = new();

    [JsonPropertyName("solo_horario_laboral")]
    public bool SoloHorarioLaboral { get; set; } = false;

    [JsonPropertyName("dispositivos_permitidos")]
    public List<string> DispositivosPermitidos { get; set; } = new();

    [JsonPropertyName("ubicaciones_permitidas")]
    public List<string> UbicacionesPermitidas { get; set; } = new();
}

/// <summary>
/// Configuración de horario de acceso
/// </summary>
public class HorarioAcceso
{
    public bool Activo { get; set; } = false;

    [JsonPropertyName("hora_inicio")]
    public string HoraInicio { get; set; } = "06:00";

    [JsonPropertyName("hora_fin")]
    public string HoraFin { get; set; } = "18:00";

    [JsonPropertyName("dias_semana")]
    public List<string> DiasSemana { get; set; } = new()
    {
        "lunes", "martes", "miercoles", "jueves", "viernes"
    };

    [JsonPropertyName("excepciones_fechas")]
    public List<DateTime> ExcepcionesFechas { get; set; } = new();
}

#endregion

#region Configuración Especial

/// <summary>
/// Configuraciones especiales y avanzadas del rol
/// </summary>
public class ConfiguracionEspecialRol
{
    [JsonPropertyName("puede_ver_notas_finales")]
    public bool PuedeVerNotasFinales { get; set; } = true;

    [JsonPropertyName("puede_editar_despues_publicacion")]
    public bool PuedeEditarDespuesPublicacion { get; set; } = false;

    [JsonPropertyName("requiere_autorizacion_cambios")]
    public bool RequiereAutorizacionCambios { get; set; } = true;

    [JsonPropertyName("notificar_cambios_importantes")]
    public bool NotificarCambiosImportantes { get; set; } = true;

    [JsonPropertyName("limite_estudiantes_consulta")]
    [Range(0, 10000)]
    public int LimiteEstudiantesConsulta { get; set; } = 100;

    [JsonPropertyName("sesion_multiple")]
    public bool SesionMultiple { get; set; } = false;

    [JsonPropertyName("acceso_modo_offline")]
    public bool AccesoModoOffline { get; set; } = false;

    [JsonPropertyName("nivel_auditoria")]
    public string NivelAuditoria { get; set; } = "BASICO"; // BASICO, MEDIO, COMPLETO

    [JsonPropertyName("puede_delegar_permisos")]
    public bool PuedeDelegarPermisos { get; set; } = false;

    [JsonPropertyName("acceso_datos_historicos")]
    public bool AccesoDatosHistoricos { get; set; } = true;

    [JsonPropertyName("limite_descarga_archivos")]
    [Range(0, 1000)]
    public int LimiteDescargaArchivos { get; set; } = 10;

    [JsonPropertyName("puede_ejecutar_reportes_pesados")]
    public bool PuedeEjecutarReportesPesados { get; set; } = false;

    [JsonPropertyName("notificaciones_tiempo_real")]
    public bool NotificacionesTiempoReal { get; set; } = true;

    [JsonPropertyName("configuracion_personalizada")]
    public Dictionary<string, object> ConfiguracionPersonalizada { get; set; } = new();

    [JsonPropertyName("acceso_todos_colegios")]
    public bool AccesoTodosColegios { get; set; } = false;
}

#endregion

#region Métodos de Extensión y Utilidades

/// <summary>
/// Métodos de extensión para facilitar el trabajo con permisos
/// </summary>
public static class PermisosExtensions
{
    /// <summary>
    /// Verifica si el rol tiene un permiso específico
    /// </summary>
    public static bool TienePermiso(this PermisosRol permisos, string modulo, string accion)
    {
        return modulo.ToLowerInvariant() switch
        {
            "estudiantes" => VerificarPermisoModulo(permisos.Modulos.Estudiantes, accion),
            "profesores" => VerificarPermisoModulo(permisos.Modulos.Profesores, accion),
            "calificaciones" => VerificarPermisoModulo(permisos.Modulos.Calificaciones, accion),
            "asistencia" => VerificarPermisoModulo(permisos.Modulos.Asistencia, accion),
            "financiero" => VerificarPermisoFinanciero(permisos.Modulos.Financiero, accion),
            "reportes" => VerificarPermisoReportes(permisos.Modulos.Reportes, accion),
            "comunicaciones" => VerificarPermisoComunicaciones(permisos.Modulos.Comunicaciones, accion),
            "administracion" => VerificarPermisoAdministracion(permisos.Modulos.Administracion, accion),
            "disciplina" => VerificarPermisoModulo(permisos.Modulos.Disciplina, accion),
            "biblioteca" => VerificarPermisoModulo(permisos.Modulos.Biblioteca, accion),
            _ => false
        };
    }

    /// <summary>
    /// Verifica si está en horario permitido
    /// </summary>
    public static bool EstaEnHorarioPermitido(this RestriccionesRol restricciones, DateTime fechaHora)
    {
        if (!restricciones.HorarioAcceso.Activo)
            return true;

        var diaSemana = fechaHora.DayOfWeek.ToString().ToLowerInvariant();
        var hora = fechaHora.TimeOfDay;

        if (!restricciones.HorarioAcceso.DiasSemana.Contains(diaSemana, StringComparer.OrdinalIgnoreCase))
            return false;

        if (restricciones.HorarioAcceso.ExcepcionesFechas.Any(f => f.Date == fechaHora.Date))
            return false;

        if (TimeSpan.TryParse(restricciones.HorarioAcceso.HoraInicio, out var horaInicio) &&
            TimeSpan.TryParse(restricciones.HorarioAcceso.HoraFin, out var horaFin))
        {
            return hora >= horaInicio && hora <= horaFin;
        }

        return true;
    }

    /// <summary>
    /// Verifica si la IP está permitida
    /// </summary>
    public static bool EsIpPermitida(this RestriccionesRol restricciones, string ipAddress)
    {
        if (!restricciones.IpPermitidas.Any())
            return true; // Si no hay restricciones de IP, permitir todas

        return restricciones.IpPermitidas.Contains(ipAddress);
    }

    /// <summary>
    /// Obtiene el nivel de acceso como entero para comparaciones
    /// </summary>
    public static int ObtenerNivelAcceso(this PermisosRol permisos)
    {
        var puntuacion = 0;

        // Contar permisos activos (usando reflexión simplificada)
        if (permisos.Modulos.Estudiantes.Ver) puntuacion += 1;
        if (permisos.Modulos.Estudiantes.Crear) puntuacion += 2;
        if (permisos.Modulos.Estudiantes.Editar) puntuacion += 2;
        if (permisos.Modulos.Estudiantes.Eliminar) puntuacion += 3;

        if (permisos.Modulos.Administracion.GestionarUsuarios) puntuacion += 10;
        if (permisos.Modulos.Administracion.ConfigurarColegio) puntuacion += 10;
        if (permisos.Modulos.Administracion.GestionarRoles) puntuacion += 15;

        return puntuacion;
    }

    private static bool VerificarPermisoModulo(PermisoModuloBase modulo, string accion)
    {
        return accion.ToLowerInvariant() switch
        {
            "ver" => modulo.Ver,
            "crear" => modulo.Crear,
            "editar" => modulo.Editar,
            "eliminar" => modulo.Eliminar,
            "exportar" => modulo.Exportar,
            _ => false
        };
    }

    private static bool VerificarPermisoFinanciero(PermisoModuloFinanciero modulo, string accion)
    {
        return accion.ToLowerInvariant() switch
        {
            "ver_facturas" => modulo.VerFacturas,
            "crear_facturas" => modulo.CrearFacturas,
            "recibir_pagos" => modulo.RecibirPagos,
            "anular_facturas" => modulo.AnularFacturas,
            "aplicar_descuentos" => modulo.AplicarDescuentos,
            "reportes_financieros" => modulo.ReportesFinancieros,
            _ => false
        };
    }

    private static bool VerificarPermisoReportes(PermisoModuloReportes modulo, string accion)
    {
        return accion.ToLowerInvariant() switch
        {
            "boletines" => modulo.Boletines,
            "certificados" => modulo.Certificados,
            "listas_estudiantes" => modulo.ListasEstudiantes,
            "reportes_academicos" => modulo.ReportesAcademicos,
            "reportes_financieros" => modulo.ReportesFinancieros,
            _ => false
        };
    }

    private static bool VerificarPermisoComunicaciones(PermisoModuloComunicaciones modulo, string accion)
    {
        return accion.ToLowerInvariant() switch
        {
            "enviar_notificaciones" => modulo.EnviarNotificaciones,
            "mensajes_acudientes" => modulo.MensajesAcudientes,
            "mensajes_masivos" => modulo.MensajesMasivos,
            "crear_circulares" => modulo.CrearCirculares,
            _ => false
        };
    }

    private static bool VerificarPermisoAdministracion(PermisoModuloAdministracion modulo, string accion)
    {
        return accion.ToLowerInvariant() switch
        {
            "gestionar_usuarios" => modulo.GestionarUsuarios,
            "configurar_colegio" => modulo.ConfigurarColegio,
            "gestionar_roles" => modulo.GestionarRoles,
            "logs_auditoria" => modulo.LogsAuditoria,
            "backup_restaurar" => modulo.BackupRestaurar,
            _ => false
        };
    }
}

#endregion
