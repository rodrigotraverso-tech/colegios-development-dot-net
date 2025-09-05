namespace ColegiosBackend.Core.Enums;

/// <summary>
/// Días de la semana para horarios académicos
/// </summary>
public enum DiaSemana
{
    /// <summary>
    /// Lunes
    /// </summary>
    Lunes = 1,

    /// <summary>
    /// Martes
    /// </summary>
    Martes = 2,

    /// <summary>
    /// Miércoles
    /// </summary>
    Miercoles = 3,

    /// <summary>
    /// Jueves
    /// </summary>
    Jueves = 4,

    /// <summary>
    /// Viernes
    /// </summary>
    Viernes = 5,

    /// <summary>
    /// Sábado
    /// </summary>
    Sabado = 6,

    /// <summary>
    /// Domingo
    /// </summary>
    Domingo = 7
}

/// <summary>
/// Estados posibles de un horario
/// </summary>
public enum EstadoHorario
{
    /// <summary>
    /// Horario activo y en funcionamiento
    /// </summary>
    Activo = 1,

    /// <summary>
    /// Horario temporalmente suspendido
    /// </summary>
    Suspendido = 2,

    /// <summary>
    /// Horario inactivo o finalizado
    /// </summary>
    Inactivo = 3
}

/// <summary>
/// Tipos de clase según su naturaleza
/// </summary>
public enum TipoClase
{
    /// <summary>
    /// Clase regular teórica
    /// </summary>
    Regular = 1,

    /// <summary>
    /// Clase de laboratorio o práctica
    /// </summary>
    Laboratorio = 2,

    /// <summary>
    /// Clase de educación física o deportes
    /// </summary>
    EducacionFisica = 3,

    /// <summary>
    /// Clase de taller o manualidades
    /// </summary>
    Taller = 4,

    /// <summary>
    /// Actividad cultural o artística
    /// </summary>
    Cultural = 5,

    /// <summary>
    /// Actividad extracurricular
    /// </summary>
    Extracurricular = 6,

    /// <summary>
    /// Clase de refuerzo o apoyo académico
    /// </summary>
    Refuerzo = 7,

    /// <summary>
    /// Evaluación o examen
    /// </summary>
    Evaluacion = 8
}