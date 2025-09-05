namespace ColegiosBackend.Core.Enums;

/// <summary>
/// Estados posibles de asistencia de un estudiante
/// </summary>
public enum EstadoAsistencia
{
    /// <summary>
    /// El estudiante estuvo presente en clase
    /// </summary>
    Presente = 1,

    /// <summary>
    /// El estudiante faltó a clase
    /// </summary>
    Ausente = 2,

    /// <summary>
    /// El estudiante llegó tarde a clase
    /// </summary>
    Tardanza = 3,

    /// <summary>
    /// El estudiante faltó por suspensión disciplinaria
    /// </summary>
    AusentePorSuspension = 4
}
