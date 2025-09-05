namespace ColegiosBackend.Core.Enums;

/// <summary>
/// Tipos de asignación de profesores
/// </summary>
public enum TipoAsignacion
{
    /// <summary>
    /// Profesor titular de la materia
    /// </summary>
    Titular = 1,

    /// <summary>
    /// Profesor suplente temporal
    /// </summary>
    Suplente = 2,

    /// <summary>
    /// Profesor de apoyo o auxiliar
    /// </summary>
    Apoyo = 3,

    /// <summary>
    /// Co-docente que comparte la materia
    /// </summary>
    CoDocente = 4,

    /// <summary>
    /// Profesor en práctica o entrenamiento
    /// </summary>
    Practicante = 5
}

/// <summary>
/// Estados de la asignación profesor-materia-grupo
/// </summary>
public enum EstadoAsignacion
{
    /// <summary>
    /// Asignación activa y vigente
    /// </summary>
    Activa = 1,

    /// <summary>
    /// Asignación suspendida temporalmente
    /// </summary>
    Suspendida = 2,

    /// <summary>
    /// Asignación finalizada permanentemente
    /// </summary>
    Finalizada = 3,

    /// <summary>
    /// Asignación en revisión o pendiente de aprobación
    /// </summary>
    EnRevision = 4
}