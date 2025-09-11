using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Application.DTOs.Estudiante;

/// <summary>
/// DTO básico para mostrar información de un estudiante
/// </summary>
public class EstudianteDto
{
    public Guid Id { get; set; }
    public Guid ColegioId { get; set; }
    public Guid PersonaId { get; set; }
    public string CodigoEstudiante { get; set; } = string.Empty;
    public EstadoEstudiante Estado { get; set; }
    public DateTime FechaIngreso { get; set; }
    public DateTime? FechaEgreso { get; set; }
    public string? MotivoEgreso { get; set; }
    public string? NumeroMatricula { get; set; }
    public int AnoIngreso { get; set; }
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Datos de la persona asociada
    public string NombreCompleto { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public DateTime? FechaNacimiento { get; set; }
    public char? Genero { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Celular { get; set; }
    public string? FotoUrl { get; set; }

    // Información del colegio
    public string NombreColegio { get; set; } = string.Empty;
    public string CodigoColegio { get; set; } = string.Empty;

    // Información académica actual (si aplica)
    public string? GrupoActual { get; set; }
    public string? GradoActual { get; set; }
    public string? JornadaActual { get; set; }
}

/// <summary>
/// DTO detallado para mostrar información completa de un estudiante
/// </summary>
public class EstudianteDetalleDto : EstudianteDto
{
    // Información médica
    public string? InformacionMedica { get; set; }

    // Contacto de emergencia
    public string? ContactoEmergenciaNombre { get; set; }
    public string? ContactoEmergenciaTelefono { get; set; }
    public string? ContactoEmergenciaRelacion { get; set; }

    // Observaciones
    public string? Observaciones { get; set; }

    // Datos completos de la persona
    public string? SegundoNombre { get; set; }
    public string? SegundoApellido { get; set; }
    public string? Direccion { get; set; }
    public int TipoDocumentoId { get; set; }
    public string TipoDocumentoNombre { get; set; } = string.Empty;

    // Acudientes/Responsables
    public List<EstudianteAcudienteDto> Acudientes { get; set; } = new();

    // Información académica histórica
    public List<MatriculaEstudianteDto> HistorialMatriculas { get; set; } = new();

    // Estadísticas académicas del período actual
    public EstadisticasAcademicasEstudianteDto? EstadisticasActuales { get; set; }
}

/// <summary>
/// DTO para crear un nuevo estudiante
/// </summary>
public class CrearEstudianteDto
{
    public Guid ColegioId { get; set; }
    public Guid PersonaId { get; set; }
    public string CodigoEstudiante { get; set; } = string.Empty;
    public DateTime FechaIngreso { get; set; }
    public int AnoIngreso { get; set; }
    public string? NumeroMatricula { get; set; }
    public string? InformacionMedica { get; set; }
    public string? ContactoEmergenciaNombre { get; set; }
    public string? ContactoEmergenciaTelefono { get; set; }
    public string? ContactoEmergenciaRelacion { get; set; }
    public string? Observaciones { get; set; }
}

/// <summary>
/// DTO para actualizar un estudiante existente
/// </summary>
public class ActualizarEstudianteDto
{
    public string? NumeroMatricula { get; set; }
    public string? InformacionMedica { get; set; }
    public string? ContactoEmergenciaNombre { get; set; }
    public string? ContactoEmergenciaTelefono { get; set; }
    public string? ContactoEmergenciaRelacion { get; set; }
    public string? Observaciones { get; set; }
    public bool Activo { get; set; } = true;
}

/// <summary>
/// DTO para mostrar información del acudiente/responsable de un estudiante
/// </summary>
public class EstudianteAcudienteDto
{
    public Guid Id { get; set; }
    public Guid PersonaId { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Celular { get; set; }
    public TipoRelacionAcudiente TipoRelacion { get; set; }
    public string TipoRelacionNombre { get; set; } = string.Empty;
    public bool EsContactoPrincipal { get; set; }
    public bool EsAutorizadoRecoger { get; set; }
    public bool RecibeNotificaciones { get; set; }
    public string? Ocupacion { get; set; }
    public string? LugarTrabajo { get; set; }
}

/// <summary>
/// DTO para mostrar información de matrícula del estudiante
/// </summary>
public class MatriculaEstudianteDto
{
    public Guid Id { get; set; }
    public Guid AnoAcademicoId { get; set; }
    public string AnoAcademico { get; set; } = string.Empty;
    public Guid GradoId { get; set; }
    public string GradoNombre { get; set; } = string.Empty;
    public Guid GrupoId { get; set; }
    public string GrupoNombre { get; set; } = string.Empty;
    public EstadoMatricula Estado { get; set; }
    public string EstadoNombre { get; set; } = string.Empty;
    public DateTime FechaMatricula { get; set; }
    public DateTime? FechaRetiro { get; set; }
    public string? MotivoRetiro { get; set; }
    public bool EsRepitente { get; set; }
    public string? ObservacionesMatricula { get; set; }
}

/// <summary>
/// DTO para estadísticas académicas del estudiante
/// </summary>
public class EstadisticasAcademicasEstudianteDto
{
    public decimal PromedioGeneral { get; set; }
    public int NumeroMaterias { get; set; }
    public int MateriasAprobadas { get; set; }
    public int MateriasReprobadas { get; set; }
    public int MateriasEnRiesgo { get; set; }
    public decimal PorcentajeAsistencia { get; set; }
    public int FaltasInjustificadas { get; set; }
    public int FaltasJustificadas { get; set; }
    public int Tardanzas { get; set; }
    public string PeriodoEvaluativo { get; set; } = string.Empty;
}

/// <summary>
/// DTO para estadísticas generales de estudiantes
/// </summary>
public class EstadisticasEstudiantesDto
{
    public int TotalEstudiantes { get; set; }
    public int EstudiantesActivos { get; set; }
    public int EstudiantesInactivos { get; set; }
    public int EstudiantesRetirados { get; set; }
    public int EstudiantesNuevos { get; set; }
    public Dictionary<string, int> EstudiantesPorGrado { get; set; } = new();
    public Dictionary<string, int> EstudiantesPorJornada { get; set; } = new();
    public Dictionary<EstadoEstudiante, int> EstudiantesPorEstado { get; set; } = new();
}

/// <summary>
/// DTO para reporte de estudiantes por grado
/// </summary>
public class ReporteEstudiantesPorGradoDto
{
    public Guid GradoId { get; set; }
    public string GradoNombre { get; set; } = string.Empty;
    public int OrdenGrado { get; set; }
    public int TotalEstudiantes { get; set; }
    public int EstudiantesHombres { get; set; }
    public int EstudiantesMujeres { get; set; }
    public List<GrupoEstudiantesDto> Grupos { get; set; } = new();
}

/// <summary>
/// DTO para mostrar información de grupo con cantidad de estudiantes
/// </summary>
public class GrupoEstudiantesDto
{
    public Guid GrupoId { get; set; }
    public string GrupoNombre { get; set; } = string.Empty;
    public int TotalEstudiantes { get; set; }
    public int EstudiantesHombres { get; set; }
    public int EstudiantesMujeres { get; set; }
    public string? DirectorGrupo { get; set; }
}

/// <summary>
/// DTO para reporte de estudiantes con información familiar
/// </summary>
public class ReporteEstudianteFamiliaDto
{
    public Guid EstudianteId { get; set; }
    public string CodigoEstudiante { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string GradoGrupo { get; set; } = string.Empty;
    public string? ContactoPrincipal { get; set; }
    public string? TelefonoContacto { get; set; }
    public string? EmailContacto { get; set; }
    public int NumeroHermanos { get; set; }
    public List<string> HermanosEnColegio { get; set; } = new();
}