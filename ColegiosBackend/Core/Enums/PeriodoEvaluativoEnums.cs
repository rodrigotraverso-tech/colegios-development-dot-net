namespace ColegiosBackend.Core.Enums;

/// <summary>
/// Tipos de períodos evaluativos disponibles en el sistema académico
/// </summary>
public enum TipoPeriodoEvaluativo
{
    /// <summary>
    /// Período bimestral (2 meses aproximadamente)
    /// </summary>
    Bimestre = 1,

    /// <summary>
    /// Período trimestral (3 meses aproximadamente)
    /// </summary>
    Trimestre = 2,

    /// <summary>
    /// Período semestral (6 meses aproximadamente)
    /// </summary>
    Semestre = 3,

    /// <summary>
    /// Período cuatrimestral (4 meses aproximadamente)
    /// </summary>
    Cuatrimestre = 4,

    /// <summary>
    /// Período personalizado definido por el colegio
    /// </summary>
    Personalizado = 5
}

/// <summary>
/// Estados del período evaluativo
/// </summary>
public enum EstadoPeriodoEvaluativo
{
    /// <summary>
    /// Período planificado pero aún no iniciado
    /// </summary>
    Planificado = 1,

    /// <summary>
    /// Período activo, se pueden registrar calificaciones
    /// </summary>
    Activo = 2,

    /// <summary>
    /// Período en proceso de cierre, revisión final
    /// </summary>
    EnCierre = 3,

    /// <summary>
    /// Período cerrado, no se permiten más modificaciones
    /// </summary>
    Cerrado = 4,

    /// <summary>
    /// Período suspendido temporalmente
    /// </summary>
    Suspendido = 5
}
