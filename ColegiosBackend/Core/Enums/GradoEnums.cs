using System.ComponentModel;

namespace ColegiosBackend.Core.Enums;

/// <summary>
/// Estados del grado académico en el sistema
/// </summary>
public enum EstadoGrado
{
    /// <summary>
    /// Grado activo y disponible para matrículas
    /// </summary>
    [Description("Activo")]
    Activo = 1,

    /// <summary>
    /// Grado inactivo, no disponible para nuevas matrículas
    /// </summary>
    [Description("Inactivo")]
    Inactivo = 2,

    /// <summary>
    /// Grado suspendido temporalmente
    /// </summary>
    [Description("Suspendido")]
    Suspendido = 3,

    /// <summary>
    /// Grado en planificación (próximo a abrir)
    /// </summary>
    [Description("En Planificación")]
    EnPlanificacion = 4,

    /// <summary>
    /// Grado cerrado por falta de estudiantes
    /// </summary>
    [Description("Cerrado")]
    Cerrado = 5,

    /// <summary>
    /// Grado en proceso de reestructuración
    /// </summary>
    [Description("En Reestructuración")]
    EnReestructuracion = 6,

    /// <summary>
    /// Grado archivado (histórico, ya no se usa)
    /// </summary>
    [Description("Archivado")]
    Archivado = 7
}
/// <summary>
/// Jornadas académicas disponibles para los grupos
/// </summary>
public enum JornadaAcademica
{
    /// <summary>
    /// Jornada matutina (generalmente 7:00 AM - 12:00 PM)
    /// </summary>
    [Description("Mañana")]
    Manana = 1,

    /// <summary>
    /// Jornada vespertina (generalmente 1:00 PM - 6:00 PM)
    /// </summary>
    [Description("Tarde")]
    Tarde = 2,

    /// <summary>
    /// Jornada nocturna (generalmente 6:00 PM - 10:00 PM)
    /// </summary>
    [Description("Nocturna")]
    Nocturna = 3,

    /// <summary>
    /// Jornada única o completa (mañana y tarde)
    /// </summary>
    [Description("Única")]
    Unica = 4,

    /// <summary>
    /// Jornada de fin de semana (sábados/domingos)
    /// </summary>
    [Description("Fin de Semana")]
    FinDeSemana = 5,

    /// <summary>
    /// Jornada flexible o mixta
    /// </summary>
    [Description("Flexible")]
    Flexible = 6
}