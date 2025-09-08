using ColegiosBackend.Core.Entities;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Interfaces;

/// <summary>
/// Repositorio para la gestión de calificaciones
/// </summary>
public interface ICalificacionRepository
{
    #region Métodos CRUD Básicos

    /// <summary>
    /// Obtiene una calificación por su ID
    /// </summary>
    /// <param name="id">ID de la calificación</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Calificación encontrada o null</returns>
    Task<Calificacion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una calificación por su ID incluyendo entidades relacionadas
    /// </summary>
    /// <param name="id">ID de la calificación</param>
    /// <param name="includeEntities">Entidades a incluir</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Calificación con entidades relacionadas</returns>
    Task<Calificacion?> GetByIdWithIncludesAsync(Guid id, string[] includeEntities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega una nueva calificación
    /// </summary>
    /// <param name="calificacion">Calificación a agregar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task AddAsync(Calificacion calificacion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza una calificación existente
    /// </summary>
    /// <param name="calificacion">Calificación a actualizar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task UpdateAsync(Calificacion calificacion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una calificación (soft delete)
    /// </summary>
    /// <param name="id">ID de la calificación a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas Específicas de Negocio

    /// <summary>
    /// Obtiene todas las calificaciones de un estudiante en un período académico
    /// </summary>
    /// <param name="estudianteId">ID del estudiante</param>
    /// <param name="periodoAcademicoId">ID del período académico</param>
    /// <param name="colegioId">ID del colegio para multi-tenancy</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de calificaciones del estudiante</returns>
    Task<IEnumerable<Calificacion>> GetByEstudianteYPeriodoAsync(Guid estudianteId, Guid periodoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las calificaciones de una asignación específica
    /// </summary>
    /// <param name="asignacionId">ID de la asignación (profesor-materia-grupo)</param>
    /// <param name="periodoAcademicoId">ID del período académico</param>
    /// <param name="colegioId">ID del colegio para multi-tenancy</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de calificaciones de la asignación</returns>
    Task<IEnumerable<Calificacion>> GetByAsignacionYPeriodoAsync(Guid asignacionId, Guid periodoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene las calificaciones de un estudiante en una materia específica
    /// </summary>
    /// <param name="estudianteId">ID del estudiante</param>
    /// <param name="materiaId">ID de la materia</param>
    /// <param name="periodoAcademicoId">ID del período académico</param>
    /// <param name="colegioId">ID del colegio para multi-tenancy</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de calificaciones del estudiante en la materia</returns>
    Task<IEnumerable<Calificacion>> GetByEstudianteMateriaYPeriodoAsync(Guid estudianteId, Guid materiaId, Guid periodoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene las calificaciones registradas por un profesor específico
    /// </summary>
    /// <param name="profesorId">ID del profesor</param>
    /// <param name="periodoAcademicoId">ID del período académico (opcional)</param>
    /// <param name="colegioId">ID del colegio para multi-tenancy</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de calificaciones registradas por el profesor</returns>
    Task<IEnumerable<Calificacion>> GetByProfesorAsync(Guid profesorId, Guid? periodoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si existe una calificación para un estudiante en una evaluación específica
    /// </summary>
    /// <param name="estudianteId">ID del estudiante</param>
    /// <param name="asignacionId">ID de la asignación</param>
    /// <param name="tipoEvaluacionId">ID del tipo de evaluación</param>
    /// <param name="periodoAcademicoId">ID del período académico</param>
    /// <param name="colegioId">ID del colegio para multi-tenancy</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si existe la calificación</returns>
    Task<bool> ExisteCalificacionAsync(Guid estudianteId, Guid asignacionId, Guid tipoEvaluacionId, Guid periodoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el promedio de calificaciones de un estudiante en una materia
    /// </summary>
    /// <param name="estudianteId">ID del estudiante</param>
    /// <param name="materiaId">ID de la materia</param>
    /// <param name="periodoAcademicoId">ID del período académico</param>
    /// <param name="colegioId">ID del colegio para multi-tenancy</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Promedio de calificaciones o null si no hay calificaciones</returns>
    Task<decimal?> GetPromedioEstudianteMateriaAsync(Guid estudianteId, Guid materiaId, Guid periodoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene las calificaciones por rango de fechas
    /// </summary>
    /// <param name="fechaInicio">Fecha inicio del rango</param>
    /// <param name="fechaFin">Fecha fin del rango</param>
    /// <param name="colegioId">ID del colegio para multi-tenancy</param>
    /// <param name="profesorId">ID del profesor (opcional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de calificaciones en el rango de fechas</returns>
    Task<IEnumerable<Calificacion>> GetByRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin, Guid colegioId, Guid? profesorId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene estadísticas de calificaciones por materia en un período
    /// </summary>
    /// <param name="materiaId">ID de la materia</param>
    /// <param name="periodoAcademicoId">ID del período académico</param>
    /// <param name="colegioId">ID del colegio para multi-tenancy</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Estadísticas de la materia (promedio, máximo, mínimo, total estudiantes)</returns>
    Task<(decimal Promedio, decimal Maximo, decimal Minimo, int TotalEstudiantes)> GetEstadisticasMateriaAsync(Guid materiaId, Guid periodoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene las calificaciones pendientes de aprobación
    /// </summary>
    /// <param name="colegioId">ID del colegio para multi-tenancy</param>
    /// <param name="profesorId">ID del profesor (opcional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de calificaciones pendientes</returns>
    Task<IEnumerable<Calificacion>> GetCalificacionesPendientesAsync(Guid colegioId, Guid? profesorId = null, CancellationToken cancellationToken = default);

    #endregion

    #region Métodos de Paginación

    /// <summary>
    /// Obtiene calificaciones paginadas con filtros
    /// </summary>
    /// <param name="pageNumber">Número de página</param>
    /// <param name="pageSize">Tamaño de página</param>
    /// <param name="colegioId">ID del colegio para multi-tenancy</param>
    /// <param name="estudianteId">ID del estudiante (filtro opcional)</param>
    /// <param name="profesorId">ID del profesor (filtro opcional)</param>
    /// <param name="materiaId">ID de la materia (filtro opcional)</param>
    /// <param name="periodoAcademicoId">ID del período académico (filtro opcional)</param>
    /// <param name="includeEntities">Entidades a incluir</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Tupla con calificaciones y total de registros</returns>
    Task<(IEnumerable<Calificacion> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid colegioId,
        Guid? estudianteId = null,
        Guid? profesorId = null,
        Guid? materiaId = null,
        Guid? periodoAcademicoId = null,
        string[]? includeEntities = null,
        CancellationToken cancellationToken = default);

    #endregion
}