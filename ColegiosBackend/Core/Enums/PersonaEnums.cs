using System.ComponentModel;

namespace ColegiosBackend.Core.Enums;

/// <summary>
/// Géneros disponibles en el sistema
/// </summary>
public enum Genero
{
    /// <summary>
    /// Género masculino
    /// </summary>
    [Description("Masculino")]
    Masculino = 1,

    /// <summary>
    /// Género femenino
    /// </summary>
    [Description("Femenino")]
    Femenino = 2,

    /// <summary>
    /// Otro género
    /// </summary>
    [Description("Otro")]
    Otro = 3,

    /// <summary>
    /// Prefiere no especificar
    /// </summary>
    [Description("No especifica")]
    NoEspecifica = 4
}

/// <summary>
/// Estados civiles disponibles
/// </summary>
public enum EstadoCivil
{
    /// <summary>
    /// Persona soltera
    /// </summary>
    [Description("Soltero/a")]
    Soltero = 1,

    /// <summary>
    /// Persona casada
    /// </summary>
    [Description("Casado/a")]
    Casado = 2,

    /// <summary>
    /// Persona en unión libre
    /// </summary>
    [Description("Unión Libre")]
    UnionLibre = 3,

    /// <summary>
    /// Persona divorciada
    /// </summary>
    [Description("Divorciado/a")]
    Divorciado = 4,

    /// <summary>
    /// Persona viuda
    /// </summary>
    [Description("Viudo/a")]
    Viudo = 5,

    /// <summary>
    /// Persona separada
    /// </summary>
    [Description("Separado/a")]
    Separado = 6
}

/// <summary>
/// Tipos de documento de identidad
/// </summary>
public enum TipoDocumento
{
    /// <summary>
    /// Cédula de ciudadanía
    /// </summary>
    [Description("Cédula de Ciudadanía")]
    CedulaCiudadania = 1,

    /// <summary>
    /// Tarjeta de identidad
    /// </summary>
    [Description("Tarjeta de Identidad")]
    TarjetaIdentidad = 2,

    /// <summary>
    /// Registro civil de nacimiento
    /// </summary>
    [Description("Registro Civil")]
    RegistroCivil = 3,

    /// <summary>
    /// Cédula de extranjería
    /// </summary>
    [Description("Cédula de Extranjería")]
    CedulaExtranjeria = 4,

    /// <summary>
    /// Pasaporte
    /// </summary>
    [Description("Pasaporte")]
    Pasaporte = 5,

    /// <summary>
    /// Permiso especial de permanencia
    /// </summary>
    [Description("Permiso Especial")]
    PermisoEspecial = 6,

    /// <summary>
    /// Documento nacional de identidad (para otros países)
    /// </summary>
    [Description("DNI")]
    DNI = 7,

    /// <summary>
    /// Otro tipo de documento
    /// </summary>
    [Description("Otro")]
    Otro = 99
}

/// <summary>
/// Tipos de contacto telefónico
/// </summary>
public enum TipoTelefono
{
    /// <summary>
    /// Teléfono móvil/celular
    /// </summary>
    [Description("Móvil")]
    Movil = 1,

    /// <summary>
    /// Teléfono fijo/casa
    /// </summary>
    [Description("Fijo")]
    Fijo = 2,

    /// <summary>
    /// Teléfono del trabajo
    /// </summary>
    [Description("Trabajo")]
    Trabajo = 3,

    /// <summary>
    /// Otro tipo de teléfono
    /// </summary>
    [Description("Otro")]
    Otro = 4
}

/// <summary>
/// Tipos de email
/// </summary>
public enum TipoEmail
{
    /// <summary>
    /// Email personal
    /// </summary>
    [Description("Personal")]
    Personal = 1,

    /// <summary>
    /// Email institucional del colegio
    /// </summary>
    [Description("Institucional")]
    Institucional = 2,

    /// <summary>
    /// Email de trabajo
    /// </summary>
    [Description("Trabajo")]
    Trabajo = 3,

    /// <summary>
    /// Email académico
    /// </summary>
    [Description("Académico")]
    Academico = 4,

    /// <summary>
    /// Otro tipo de email
    /// </summary>
    [Description("Otro")]
    Otro = 5
}