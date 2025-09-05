using System;
using System.ComponentModel;

namespace ColegiosBackend.Core.Enums;

/// <summary>
/// Tipos de institución educativa
/// </summary>
public enum TipoInstitucion
{
    /// <summary>
    /// Institución educativa pública
    /// </summary>
    [Description("Público")]
    Publico = 1,

    /// <summary>
    /// Institución educativa privada
    /// </summary>
    [Description("Privado")]
    Privado = 2,

    /// <summary>
    /// Institución educativa mixta (público-privado)
    /// </summary>
    [Description("Mixto")]
    Mixto = 3,

    /// <summary>
    /// Institución educativa de convenio
    /// </summary>
    [Description("Convenio")]
    Convenio = 4,

    /// <summary>
    /// Institución educativa cooperativa
    /// </summary>
    [Description("Cooperativo")]
    Cooperativo = 5
}

/// <summary>
/// Niveles educativos que puede ofrecer una institución
/// Se puede combinar usando flags para instituciones que ofrecen múltiples niveles
/// </summary>
[Flags]
public enum NivelesEducativos
{
    /// <summary>
    /// Ningún nivel definido
    /// </summary>
    [Description("Ninguno")]
    Ninguno = 0,

    /// <summary>
    /// Educación preescolar (3-5 años)
    /// </summary>
    [Description("Preescolar")]
    Preescolar = 1,

    /// <summary>
    /// Educación primaria (6-10 años)
    /// </summary>
    [Description("Primaria")]
    Primaria = 2,

    /// <summary>
    /// Educación secundaria básica (11-14 años)
    /// </summary>
    [Description("Secundaria Básica")]
    SecundariaBasica = 4,

    /// <summary>
    /// Educación secundaria media/bachillerato (15-17 años)
    /// </summary>
    [Description("Secundaria Media")]
    SecundariaMedia = 8,

    /// <summary>
    /// Educación técnica
    /// </summary>
    [Description("Técnica")]
    Tecnica = 16,

    /// <summary>
    /// Educación para adultos
    /// </summary>
    [Description("Adultos")]
    Adultos = 32,

    /// <summary>
    /// Educación especial
    /// </summary>
    [Description("Especial")]
    Especial = 64,

    /// <summary>
    /// Combinación común: Preescolar + Primaria
    /// </summary>
    [Description("Preescolar y Primaria")]
    PreescolarPrimaria = Preescolar | Primaria,

    /// <summary>
    /// Combinación común: Primaria + Secundaria
    /// </summary>
    [Description("Primaria y Secundaria")]
    PrimariaSecundaria = Primaria | SecundariaBasica | SecundariaMedia,

    /// <summary>
    /// Educación completa (todos los niveles básicos)
    /// </summary>
    [Description("Completa")]
    Completa = Preescolar | Primaria | SecundariaBasica | SecundariaMedia,

    /// <summary>
    /// Solo niveles de secundaria
    /// </summary>
    [Description("Solo Secundaria")]
    SoloSecundaria = SecundariaBasica | SecundariaMedia
}

/// <summary>
/// Estados del colegio en el sistema
/// </summary>
public enum EstadoColegio
{
    /// <summary>
    /// Colegio activo y operativo
    /// </summary>
    [Description("Activo")]
    Activo = 1,

    /// <summary>
    /// Colegio temporalmente inactivo
    /// </summary>
    [Description("Inactivo")]
    Inactivo = 2,

    /// <summary>
    /// Colegio en proceso de registro
    /// </summary>
    [Description("En Registro")]
    EnRegistro = 3,

    /// <summary>
    /// Colegio suspendido por incumplimientos
    /// </summary>
    [Description("Suspendido")]
    Suspendido = 4,

    /// <summary>
    /// Colegio cerrado permanentemente
    /// </summary>
    [Description("Cerrado")]
    Cerrado = 5
}

/// <summary>
/// Modalidades educativas
/// </summary>
public enum ModalidadEducativa
{
    /// <summary>
    /// Educación presencial tradicional
    /// </summary>
    [Description("Presencial")]
    Presencial = 1,

    /// <summary>
    /// Educación virtual/a distancia
    /// </summary>
    [Description("Virtual")]
    Virtual = 2,

    /// <summary>
    /// Educación mixta (presencial + virtual)
    /// </summary>
    [Description("Mixta")]
    Mixta = 3,

    /// <summary>
    /// Educación semipresencial
    /// </summary>
    [Description("Semipresencial")]
    Semipresencial = 4
}
