namespace ColegiosBackend.Core.Enums;

/// <summary>
/// Estados de una calificación en el sistema
/// </summary>
public enum EstadoCalificacion
{
    /// <summary>
    /// Calificación en borrador, puede ser modificada
    /// </summary>
    Borrador = 1,

    /// <summary>
    /// Calificación pendiente de revisión por coordinador
    /// </summary>
    PendienteRevision = 2,

    /// <summary>
    /// Calificación publicada oficialmente, no puede modificarse
    /// </summary>
    Publicada = 3,

    /// <summary>
    /// Calificación anulada por algún motivo
    /// </summary>
    Anulada = 4
}
