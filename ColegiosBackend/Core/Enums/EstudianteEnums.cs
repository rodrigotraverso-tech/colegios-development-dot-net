using System.ComponentModel;

namespace ColegiosBackend.Core.Enums;

/// <summary>
/// Estados del estudiante en el sistema
/// </summary>
public enum EstadoEstudiante
{
    /// <summary>
    /// Estudiante en proceso de pre-matrícula
    /// </summary>
    [Description("Pre-Matrícula")]
    PreMatricula = 1,

    /// <summary>
    /// Estudiante activo y cursando
    /// </summary>
    [Description("Activo")]
    Activo = 2,

    /// <summary>
    /// Estudiante temporalmente suspendido
    /// </summary>
    [Description("Suspendido")]
    Suspendido = 3,

    /// <summary>
    /// Estudiante retirado del colegio
    /// </summary>
    [Description("Retirado")]
    Retirado = 4,

    /// <summary>
    /// Estudiante graduado
    /// </summary>
    [Description("Graduado")]
    Graduado = 5,

    /// <summary>
    /// Estudiante trasladado a otro colegio
    /// </summary>
    [Description("Trasladado")]
    Trasladado = 6,

    /// <summary>
    /// Estudiante expulsado
    /// </summary>
    [Description("Expulsado")]
    Expulsado = 7
}

/// <summary>
/// Tipos de relación del acudiente con el estudiante
/// </summary>
public enum TipoRelacionAcudiente
{
    /// <summary>
    /// Padre biológico o adoptivo
    /// </summary>
    [Description("Padre")]
    Padre = 1,

    /// <summary>
    /// Madre biológica o adoptiva
    /// </summary>
    [Description("Madre")]
    Madre = 2,

    /// <summary>
    /// Tutor legal
    /// </summary>
    [Description("Tutor Legal")]
    TutorLegal = 3,

    /// <summary>
    /// Abuelo/abuela
    /// </summary>
    [Description("Abuelo/a")]
    Abuelo = 4,

    /// <summary>
    /// Tío/tía
    /// </summary>
    [Description("Tío/a")]
    Tio = 5,

    /// <summary>
    /// Hermano/hermana mayor
    /// </summary>
    [Description("Hermano/a")]
    Hermano = 6,

    /// <summary>
    /// Padrastro
    /// </summary>
    [Description("Padrastro")]
    Padrastro = 7,

    /// <summary>
    /// Madrastra
    /// </summary>
    [Description("Madrastra")]
    Madrastra = 8,

    /// <summary>
    /// Otro familiar
    /// </summary>
    [Description("Otro Familiar")]
    OtroFamiliar = 9,

    /// <summary>
    /// Acudiente no familiar
    /// </summary>
    [Description("Acudiente")]
    Acudiente = 10
}