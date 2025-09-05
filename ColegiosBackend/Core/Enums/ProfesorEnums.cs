using System.ComponentModel;

namespace ColegiosBackend.Core.Enums;

/// <summary>
/// Estados del profesor en el sistema
/// </summary>
public enum EstadoProfesor
{
    /// <summary>
    /// Profesor en período de prueba
    /// </summary>
    [Description("Período de Prueba")]
    PeriodoPrueba = 1,

    /// <summary>
    /// Profesor activo y en ejercicio
    /// </summary>
    [Description("Activo")]
    Activo = 2,

    /// <summary>
    /// Profesor temporalmente suspendido
    /// </summary>
    [Description("Suspendido")]
    Suspendido = 3,

    /// <summary>
    /// Profesor en licencia (médica, maternidad, etc.)
    /// </summary>
    [Description("En Licencia")]
    EnLicencia = 4,

    /// <summary>
    /// Profesor retirado del colegio
    /// </summary>
    [Description("Retirado")]
    Retirado = 5,

    /// <summary>
    /// Profesor despedido
    /// </summary>
    [Description("Despedido")]
    Despedido = 6,

    /// <summary>
    /// Profesor pensionado/jubilado
    /// </summary>
    [Description("Pensionado")]
    Pensionado = 7,

    /// <summary>
    /// Profesor inactivo temporalmente
    /// </summary>
    [Description("Inactivo")]
    Inactivo = 8
}

/// <summary>
/// Tipos de contrato del profesor
/// </summary>
public enum TipoContratoProfesor
{
    /// <summary>
    /// Contrato indefinido a término fijo
    /// </summary>
    [Description("Indefinido")]
    Indefinido = 1,

    /// <summary>
    /// Contrato a término fijo (temporal)
    /// </summary>
    [Description("Término Fijo")]
    TerminoFijo = 2,

    /// <summary>
    /// Contrato por obra o labor específica
    /// </summary>
    [Description("Obra o Labor")]
    ObraLabor = 3,

    /// <summary>
    /// Contrato de prestación de servicios
    /// </summary>
    [Description("Prestación de Servicios")]
    PrestacionServicios = 4,

    /// <summary>
    /// Contrato de medio tiempo
    /// </summary>
    [Description("Medio Tiempo")]
    MedioTiempo = 5,

    /// <summary>
    /// Contrato por horas cátedra
    /// </summary>
    [Description("Por Horas")]
    PorHoras = 6,

    /// <summary>
    /// Contrato de reemplazo/suplencia
    /// </summary>
    [Description("Reemplazo")]
    Reemplazo = 7,

    /// <summary>
    /// Contrato de práctica profesional
    /// </summary>
    [Description("Práctica")]
    Practica = 8,

    /// <summary>
    /// Contrato voluntario (sin remuneración)
    /// </summary>
    [Description("Voluntario")]
    Voluntario = 9
}

/// <summary>
/// Cargos que puede ocupar un profesor
/// </summary>
public enum CargoProfesor
{
    /// <summary>
    /// Profesor de aula regular
    /// </summary>
    [Description("Profesor")]
    Profesor = 1,

    /// <summary>
    /// Profesor titular de materia
    /// </summary>
    [Description("Profesor Titular")]
    ProfesorTitular = 2,

    /// <summary>
    /// Profesor auxiliar o asistente
    /// </summary>
    [Description("Profesor Auxiliar")]
    ProfesorAuxiliar = 3,

    /// <summary>
    /// Director de grupo/curso
    /// </summary>
    [Description("Director de Grupo")]
    DirectorGrupo = 4,

    /// <summary>
    /// Coordinador académico
    /// </summary>
    [Description("Coordinador Académico")]
    CoordinadorAcademico = 5,

    /// <summary>
    /// Coordinador de área específica
    /// </summary>
    [Description("Coordinador de Área")]
    CoordinadorArea = 6,

    /// <summary>
    /// Coordinador de disciplina/convivencia
    /// </summary>
    [Description("Coordinador de Disciplina")]
    CoordinadorDisciplina = 7,

    /// <summary>
    /// Jefe de departamento
    /// </summary>
    [Description("Jefe de Departamento")]
    JefeDepartamento = 8,

    /// <summary>
    /// Vicedirector o subdirector
    /// </summary>
    [Description("Vicedirector")]
    Vicedirector = 9,

    /// <summary>
    /// Director del colegio
    /// </summary>
    [Description("Director")]
    Director = 10,

    /// <summary>
    /// Psicólogo educativo
    /// </summary>
    [Description("Psicólogo")]
    Psicologo = 11,

    /// <summary>
    /// Orientador vocacional
    /// </summary>
    [Description("Orientador")]
    Orientador = 12,

    /// <summary>
    /// Profesor de educación especial
    /// </summary>
    [Description("Educación Especial")]
    EducacionEspecial = 13,

    /// <summary>
    /// Bibliotecario
    /// </summary>
    [Description("Bibliotecario")]
    Bibliotecario = 14,

    /// <summary>
    /// Profesor de laboratorio
    /// </summary>
    [Description("Profesor de Laboratorio")]
    ProfesorLaboratorio = 15,

    /// <summary>
    /// Instructor de talleres
    /// </summary>
    [Description("Instructor")]
    Instructor = 16,

    /// <summary>
    /// Tutor personalizado
    /// </summary>
    [Description("Tutor")]
    Tutor = 17,

    /// <summary>
    /// Profesor sustituto/reemplazo
    /// </summary>
    [Description("Sustituto")]
    Sustituto = 18,

    /// <summary>
    /// Asesor metodológico
    /// </summary>
    [Description("Asesor")]
    Asesor = 19,

    /// <summary>
    /// Secretario académico
    /// </summary>
    [Description("Secretario Académico")]
    SecretarioAcademico = 20
}