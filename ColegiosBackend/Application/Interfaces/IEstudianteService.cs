using ColegiosBackend.Application.DTOs.Estudiante;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Application.Interfaces;

/// <summary>
/// Interface para el servicio de gestión de estudiantes
/// </summary>
public interface IEstudianteService
{
    #region Consultas

    /// <summary>
    /// Obtiene un estudiante por su ID
    /// </summary>
    Task<EstudianteDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un estudiante por su ID con información detallada
    /// </summary>
    Task<EstudianteDetalleDto?> GetDetalleByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un estudiante por su código en un colegio específico
    /// </summary>
    Task<EstudianteDto?> GetByCodigoAsync(string codigoEstudiante, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los estudiantes de un colegio con paginación
    /// </summary>
    Task<(IEnumerable<EstudianteDto> Items, int TotalCount)> GetAllByColegioAsync(
        Guid colegioId,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca estudiantes por criterios múltiples
    /// </summary>
    Task<(IEnumerable<EstudianteDto> Items, int TotalCount)> BuscarEstudiantesAsync(
        Guid colegioId,
        string? filtroTexto = null,
        Guid? gradoId = null,
        Guid? grupoId = null,
        EstadoMatricula? estadoMatricula = null,
        DateTime? fechaMatriculaDesde = null,
        DateTime? fechaMatriculaHasta = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene estudiantes por grupo
    /// </summary>
    Task<IEnumerable<EstudianteDto>> GetByGrupoAsync(Guid grupoId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene hermanos de un estudiante
    /// </summary>
    Task<IEnumerable<EstudianteDto>> GetHermanosAsync(Guid estudianteId, CancellationToken cancellationToken = default);

    #endregion

    #region Operaciones

    /// <summary>
    /// Crea un nuevo estudiante
    /// </summary>
    Task<EstudianteDto> CrearAsync(CrearEstudianteDto crearEstudianteDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza un estudiante existente
    /// </summary>
    Task<EstudianteDto> ActualizarAsync(Guid id, ActualizarEstudianteDto actualizarEstudianteDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina un estudiante (soft delete)
    /// </summary>
    Task<bool> EliminarAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Transfiere un estudiante a otro grupo
    /// </summary>
    Task<bool> TransferirGrupoAsync(Guid estudianteId, Guid nuevoGrupoId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cambia el estado de matrícula de un estudiante
    /// </summary>
    Task<bool> CambiarEstadoMatriculaAsync(Guid estudianteId, EstadoMatricula nuevoEstado, string? observaciones = null, CancellationToken cancellationToken = default);

    #endregion

    #region Reportes y Estadísticas

    /// <summary>
    /// Obtiene estadísticas de estudiantes por colegio
    /// </summary>
    Task<EstadisticasEstudiantesDto> GetEstadisticasAsync(Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el reporte de estudiantes por grado
    /// </summary>
    Task<IEnumerable<ReporteEstudiantesPorGradoDto>> GetReporteEstudiantesPorGradoAsync(Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Genera reporte de estudiantes con información familiar
    /// </summary>
    Task<IEnumerable<ReporteEstudianteFamiliaDto>> GetReporteEstudiantesConFamiliaAsync(
        Guid colegioId,
        Guid? gradoId = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Validaciones

    /// <summary>
    /// Verifica si un código de estudiante está disponible
    /// </summary>
    Task<bool> CodigoDisponibleAsync(string codigoEstudiante, Guid colegioId, Guid? estudianteId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si un estudiante puede ser eliminado
    /// </summary>
    Task<bool> PuedeSerEliminadoAsync(Guid estudianteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene las dependencias que impiden eliminar un estudiante
    /// </summary>
    Task<IEnumerable<string>> GetDependenciasEliminacionAsync(Guid estudianteId, CancellationToken cancellationToken = default);

    #endregion
}