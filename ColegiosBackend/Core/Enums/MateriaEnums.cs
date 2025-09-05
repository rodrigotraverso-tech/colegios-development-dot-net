using System.ComponentModel;

namespace ColegiosBackend.Core.Enums;

/// <summary>
/// Áreas académicas para clasificar las materias
/// </summary>
public enum AreaAcademica
{
    /// <summary>
    /// Matemáticas y áreas relacionadas
    /// </summary>
    [Description("Matemáticas")]
    Matematicas = 1,

    /// <summary>
    /// Ciencias Naturales (Biología, Química, Física)
    /// </summary>
    [Description("Ciencias Naturales")]
    CienciasNaturales = 2,

    /// <summary>
    /// Ciencias Sociales (Historia, Geografía, Cívica)
    /// </summary>
    [Description("Ciencias Sociales")]
    CienciasSociales = 3,

    /// <summary>
    /// Lenguaje y Literatura
    /// </summary>
    [Description("Lenguaje")]
    Lenguaje = 4,

    /// <summary>
    /// Idiomas Extranjeros
    /// </summary>
    [Description("Idiomas")]
    Idiomas = 5,

    /// <summary>
    /// Educación Artística (Música, Artes Plásticas, Teatro)
    /// </summary>
    [Description("Educación Artística")]
    EducacionArtistica = 6,

    /// <summary>
    /// Educación Física y Deportes
    /// </summary>
    [Description("Educación Física")]
    EducacionFisica = 7,

    /// <summary>
    /// Educación Religiosa
    /// </summary>
    [Description("Educación Religiosa")]
    EducacionReligiosa = 8,

    /// <summary>
    /// Tecnología e Informática
    /// </summary>
    [Description("Tecnología")]
    Tecnologia = 9,

    /// <summary>
    /// Ética y Valores
    /// </summary>
    [Description("Ética y Valores")]
    EticaValores = 10,

    /// <summary>
    /// Filosofía
    /// </summary>
    [Description("Filosofía")]
    Filosofia = 11,

    /// <summary>
    /// Economía y Finanzas
    /// </summary>
    [Description("Economía")]
    Economia = 12,

    /// <summary>
    /// Educación para el Trabajo
    /// </summary>
    [Description("Educación para el Trabajo")]
    EducacionTrabajo = 13,

    /// <summary>
    /// Educación Ambiental
    /// </summary>
    [Description("Educación Ambiental")]
    EducacionAmbiental = 14,

    /// <summary>
    /// Competencias Ciudadanas
    /// </summary>
    [Description("Competencias Ciudadanas")]
    CompetenciasCiudadanas = 15,

    /// <summary>
    /// Educación para la Sexualidad
    /// </summary>
    [Description("Educación Sexual")]
    EducacionSexual = 16,

    /// <summary>
    /// Psicología y Desarrollo Humano
    /// </summary>
    [Description("Psicología")]
    Psicologia = 17,

    /// <summary>
    /// Talleres y Actividades Prácticas
    /// </summary>
    [Description("Talleres")]
    Talleres = 18,

    /// <summary>
    /// Orientación Vocacional
    /// </summary>
    [Description("Orientación Vocacional")]
    OrientacionVocacional = 19,

    /// <summary>
    /// Investigación y Proyectos
    /// </summary>
    [Description("Investigación")]
    Investigacion = 20,

    /// <summary>
    /// Emprendimiento
    /// </summary>
    [Description("Emprendimiento")]
    Emprendimiento = 21,

    /// <summary>
    /// Lógica y Pensamiento Crítico
    /// </summary>
    [Description("Lógica")]
    Logica = 22,

    /// <summary>
    /// Estadística y Probabilidad
    /// </summary>
    [Description("Estadística")]
    Estadistica = 23,

    /// <summary>
    /// Educación Especial
    /// </summary>
    [Description("Educación Especial")]
    EducacionEspecial = 24,

    /// <summary>
    /// Materias Transversales (que cruzan varias áreas)
    /// </summary>
    [Description("Transversal")]
    Transversal = 25,

    /// <summary>
    /// Otras áreas no clasificadas
    /// </summary>
    [Description("Otras")]
    Otras = 99
}

/// <summary>
/// Estados de la materia en el sistema
/// </summary>
public enum EstadoMateria
{
    /// <summary>
    /// Materia activa y disponible para ser asignada
    /// </summary>
    [Description("Activa")]
    Activa = 1,

    /// <summary>
    /// Materia inactiva, no disponible para nuevas asignaciones
    /// </summary>
    [Description("Inactiva")]
    Inactiva = 2,

    /// <summary>
    /// Materia en proceso de revisión curricular
    /// </summary>
    [Description("En Revisión")]
    EnRevision = 3,

    /// <summary>
    /// Materia suspendida temporalmente
    /// </summary>
    [Description("Suspendida")]
    Suspendida = 4,

    /// <summary>
    /// Materia en proceso de aprobación
    /// </summary>
    [Description("Pendiente Aprobación")]
    PendienteAprobacion = 5,

    /// <summary>
    /// Materia archivada (ya no se usa pero se mantiene por historial)
    /// </summary>
    [Description("Archivada")]
    Archivada = 6,

    /// <summary>
    /// Materia eliminada (soft delete)
    /// </summary>
    [Description("Eliminada")]
    Eliminada = 7
}

/// <summary>
/// Tipos de materia según su modalidad de enseñanza
/// </summary>
public enum TipoMateria
{
    /// <summary>
    /// Materia teórica tradicional
    /// </summary>
    [Description("Teórica")]
    Teorica = 1,

    /// <summary>
    /// Materia práctica con laboratorio
    /// </summary>
    [Description("Práctica")]
    Practica = 2,

    /// <summary>
    /// Materia teórico-práctica (combinada)
    /// </summary>
    [Description("Teórico-Práctica")]
    TeoricoPractica = 3,

    /// <summary>
    /// Taller o actividad práctica
    /// </summary>
    [Description("Taller")]
    Taller = 4,

    /// <summary>
    /// Seminario o discusión grupal
    /// </summary>
    [Description("Seminario")]
    Seminario = 5,

    /// <summary>
    /// Proyecto de investigación
    /// </summary>
    [Description("Proyecto")]
    Proyecto = 6,

    /// <summary>
    /// Práctica de campo
    /// </summary>
    [Description("Práctica de Campo")]
    PracticaCampo = 7,

    /// <summary>
    /// Actividad virtual/en línea
    /// </summary>
    [Description("Virtual")]
    Virtual = 8,

    /// <summary>
    /// Actividad deportiva
    /// </summary>
    [Description("Deportiva")]
    Deportiva = 9,

    /// <summary>
    /// Actividad artística
    /// </summary>
    [Description("Artística")]
    Artistica = 10
}

/// <summary>
/// Nivel de dificultad de la materia
/// </summary>
public enum NivelDificultad
{
    /// <summary>
    /// Nivel básico o introductorio
    /// </summary>
    [Description("Básico")]
    Basico = 1,

    /// <summary>
    /// Nivel intermedio
    /// </summary>
    [Description("Intermedio")]
    Intermedio = 2,

    /// <summary>
    /// Nivel avanzado
    /// </summary>
    [Description("Avanzado")]
    Avanzado = 3,

    /// <summary>
    /// Nivel superior o universitario
    /// </summary>
    [Description("Superior")]
    Superior = 4,

    /// <summary>
    /// Nivel especializado
    /// </summary>
    [Description("Especializado")]
    Especializado = 5
}