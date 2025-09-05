using System.ComponentModel;

namespace ColegiosBackend.Core.Enums;

/// <summary>
/// Estados de la matrícula en el sistema
/// </summary>
public enum EstadoMatricula
{
    /// <summary>
    /// Matrícula activa y vigente
    /// </summary>
    [Description("Activa")]
    Activa = 1,

    /// <summary>
    /// Matrícula suspendida temporalmente
    /// </summary>
    [Description("Suspendida")]
    Suspendida = 2,

    /// <summary>
    /// Matrícula retirada (estudiante se retiró del colegio)
    /// </summary>
    [Description("Retirada")]
    Retirada = 3,

    /// <summary>
    /// Matrícula graduada (estudiante completó el nivel)
    /// </summary>
    [Description("Graduada")]
    Graduada = 4,

    /// <summary>
    /// Matrícula trasladada (a otro grupo o colegio)
    /// </summary>
    [Description("Trasladada")]
    Trasladada = 5,

    /// <summary>
    /// Matrícula con condicional académico
    /// </summary>
    [Description("Condicional Académica")]
    CondicionalAcademica = 6,

    /// <summary>
    /// Matrícula con condicional disciplinaria
    /// </summary>
    [Description("Condicional Disciplinaria")]
    CondicionalDisciplinaria = 7,

    /// <summary>
    /// Matrícula cancelada (antes de iniciar clases)
    /// </summary>
    [Description("Cancelada")]
    Cancelada = 8,

    /// <summary>
    /// Matrícula pendiente de documentación
    /// </summary>
    [Description("Pendiente Documentos")]
    PendienteDocumentos = 9,

    /// <summary>
    /// Matrícula pendiente de pago
    /// </summary>
    [Description("Pendiente Pago")]
    PendientePago = 10
}

/// <summary>
/// Tipos de matrícula según las circunstancias del ingreso
/// </summary>
public enum TipoMatricula
{
    /// <summary>
    /// Matrícula regular ordinaria
    /// </summary>
    [Description("Regular")]
    Regular = 1,

    /// <summary>
    /// Matrícula de estudiante nuevo (primer ingreso al colegio)
    /// </summary>
    [Description("Nuevo Ingreso")]
    NuevoIngreso = 2,

    /// <summary>
    /// Matrícula de estudiante que repite el grado
    /// </summary>
    [Description("Repitente")]
    Repitente = 3,

    /// <summary>
    /// Matrícula por transferencia de otro colegio
    /// </summary>
    [Description("Transferencia")]
    Transferencia = 4,

    /// <summary>
    /// Matrícula extraordinaria (fuera de fechas normales)
    /// </summary>
    [Description("Extraordinaria")]
    Extraordinaria = 5,

    /// <summary>
    /// Matrícula condicional (período de prueba)
    /// </summary>
    [Description("Condicional")]
    Condicional = 6,

    /// <summary>
    /// Matrícula de reingreso (estudiante que regresa tras retiro)
    /// </summary>
    [Description("Reingreso")]
    Reingreso = 7,

    /// <summary>
    /// Matrícula por intercambio estudiantil
    /// </summary>
    [Description("Intercambio")]
    Intercambio = 8,

    /// <summary>
    /// Matrícula temporal (por tiempo limitado)
    /// </summary>
    [Description("Temporal")]
    Temporal = 9,

    /// <summary>
    /// Matrícula de validación de estudios
    /// </summary>
    [Description("Validación")]
    Validacion = 10
}

/// <summary>
/// Tipos de beca disponibles en el sistema
/// </summary>
public enum TipoBecaMatricula
{
    /// <summary>
    /// Beca por excelencia académica
    /// </summary>
    [Description("Excelencia Académica")]
    ExcelenciaAcademica = 1,

    /// <summary>
    /// Beca por situación socioeconómica
    /// </summary>
    [Description("Socioeconómica")]
    Socioeconomica = 2,

    /// <summary>
    /// Beca deportiva
    /// </summary>
    [Description("Deportiva")]
    Deportiva = 3,

    /// <summary>
    /// Beca cultural o artística
    /// </summary>
    [Description("Cultural")]
    Cultural = 4,

    /// <summary>
    /// Beca para hermanos (descuento familiar)
    /// </summary>
    [Description("Hermanos")]
    Hermanos = 5,

    /// <summary>
    /// Beca para hijos de empleados
    /// </summary>
    [Description("Empleados")]
    Empleados = 6,

    /// <summary>
    /// Beca por liderazgo estudiantil
    /// </summary>
    [Description("Liderazgo")]
    Liderazgo = 7,

    /// <summary>
    /// Beca por necesidad especial o discapacidad
    /// </summary>
    [Description("Necesidad Especial")]
    NecesidadEspecial = 8,

    /// <summary>
    /// Beca por orfandad
    /// </summary>
    [Description("Orfandad")]
    Orfandad = 9,

    /// <summary>
    /// Beca por convenio institucional
    /// </summary>
    [Description("Convenio")]
    Convenio = 10,

    /// <summary>
    /// Beca por mérito científico o investigación
    /// </summary>
    [Description("Mérito Científico")]
    MeritoCientifico = 11,

    /// <summary>
    /// Beca de emergencia (situaciones imprevistas)
    /// </summary>
    [Description("Emergencia")]
    Emergencia = 12,

    /// <summary>
    /// Beca por desplazamiento forzado
    /// </summary>
    [Description("Desplazamiento")]
    Desplazamiento = 13,

    /// <summary>
    /// Beca integral (cubre varios conceptos)
    /// </summary>
    [Description("Integral")]
    Integral = 14,

    /// <summary>
    /// Otro tipo de beca no clasificada
    /// </summary>
    [Description("Otra")]
    Otra = 99
}