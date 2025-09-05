using ColegiosBackend.Core.Entities;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Interfaces;

/// <summary>
/// Interfaz del repositorio de Asistencia
/// Define todas las operaciones disponibles para la gestión de asistencias
/// </summary>
public interface IAsistenciaRepository
{
    #region CRUD Operations

    /// <summary>
    /// Obtiene una asistencia por ID con entidades relacionadas
    /// </summary>
    /// <param name="id">ID de la asistencia</param>
    /// <param name="includeEntities">Entidades relacionadas a incluir</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Asistencia encontrada o null</returns>
    Task<Asistencia?> GetByIdAsync(Guid id, string[]? includeEntities = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las asistencias con filtros opcionales
    /// </summary>
    /// <param name="colegioId">ID del colegio (filtro opcional)</param>
    /// <param name="includeInactive">Incluir asistencias inactivas</param>
    /// <param name="includeEntities">Entidades relacionadas a incluir</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de asistencias</returns>
    Task<IEnumerable<Asistencia>> GetAllAsync(
        Guid? colegioId = null,
        bool includeInactive = false,
        string[]? includeEntities = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Crea una nueva asistencia
    /// </summary>
    /// <param name="asistencia">Asistencia a crear</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Asistencia creada</returns>
    Task<Asistencia> CreateAsync(Asistencia asistencia, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza una asistencia existente
    /// </summary>
    /// <param name="asistencia">Asistencia a actualizar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Asistencia actualizada</returns>
    Task<Asistencia> UpdateAsync(Asistencia asistencia, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina lógicamente una asistencia
    /// </summary>
    /// <param name="id">ID de la asistencia a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si se eliminó correctamente</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas Específicas de Asistencia

    /// <summary>
    /// Obtiene asistencias por estudiante
    /// </summary>
    /// <param name="estudianteId">ID del estudiante</param>
    /// <param name="colegioId">ID del colegio (filtro opcional)</param>
    /// <param name="fechaInicio">Fecha de inicio del rango (opcional)</param>
    /// <param name="fechaFin">Fecha de fin del rango (opcional)</param>
    /// <param name="includeEntities">Entidades relacionadas a incluir</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de asistencias del estudiante</returns>
    Task<IEnumerable<Asistencia>> GetByEstudianteAsync(
        Guid estudianteId,
        Guid? colegioId = null,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        string[]? includeEntities = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene asistencias por materia y grupo
    /// </summary>
    /// <param name="materiaId">ID de la materia</param>
    /// <param name="grupoId">ID del grupo</param>
    /// <param name="colegioId">ID del colegio (filtro opcional)</param>
    /// <param name="fechaInicio">Fecha de inicio del rango (opcional)</param>
    /// <param name="fechaFin">Fecha de fin del rango (opcional)</param>
    /// <param name="includeEntities">Entidades relacionadas a incluir</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de asistencias de la materia y grupo</returns>
    Task<IEnumerable<Asistencia>> GetByMateriaGrupoAsync(
        Guid materiaId,
        Guid grupoId,
        Guid? colegioId = null,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        string[]? includeEntities = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene asistencias por período evaluativo
    /// </summary>
    /// <param name="periodoEvaluativoId">ID del período evaluativo</param>
    /// <param name="colegioId">ID del colegio (filtro opcional)</param>
    /// <param name="estudianteId">ID del estudiante (filtro opcional)</param>
    /// <param name="materiaId">ID de la materia (filtro opcional)</param>
    /// <param name="includeEntities">Entidades relacionadas a incluir</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de asistencias del período</returns>
    Task<IEnumerable<Asistencia>> GetByPeriodoEvaluativoAsync(
        Guid periodoEvaluativoId,
        Guid? colegioId = null,
        Guid? estudianteId = null,
        Guid? materiaId = null,
        string[]? includeEntities = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene asistencias por fecha específica
    /// </summary>
    /// <param name="fecha">Fecha específica</param>
    /// <param name="colegioId">ID del colegio (filtro opcional)</param>
    /// <param name="grupoId">ID del grupo (filtro opcional)</param>
    /// <param name="materiaId">ID de la materia (filtro opcional)</param>
    /// <param name="includeEntities">Entidades relacionadas a incluir</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de asistencias de la fecha</returns>
    Task<IEnumerable<Asistencia>> GetByFechaAsync(
        DateTime fecha,
        Guid? colegioId = null,
        Guid? grupoId = null,
        Guid? materiaId = null,
        string[]? includeEntities = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Estadísticas y Reportes

    /// <summary>
    /// Obtiene estadísticas de asistencia por estudiante
    /// </summary>
    /// <param name="estudianteId">ID del estudiante</param>
    /// <param name="colegioId">ID del colegio (filtro opcional)</param>
    /// <param name="fechaInicio">Fecha de inicio del rango (opcional)</param>
    /// <param name="fechaFin">Fecha de fin del rango (opcional)</param>
    /// <param name="materiaId">ID de la materia (filtro opcional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Diccionario con estadísticas</returns>
    Task<Dictionary<string, int>> GetEstadisticasEstudianteAsync(
        Guid estudianteId,
        Guid? colegioId = null,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        Guid? materiaId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene estadísticas de asistencia por materia y grupo
    /// </summary>
    /// <param name="materiaId">ID de la materia</param>
    /// <param name="grupoId">ID del grupo</param>
    /// <param name="colegioId">ID del colegio (filtro opcional)</param>
    /// <param name="fechaInicio">Fecha de inicio del rango (opcional)</param>
    /// <param name="fechaFin">Fecha de fin del rango (opcional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Diccionario con estadísticas detalladas</returns>
    Task<Dictionary<string, object>> GetEstadisticasMateriaGrupoAsync(
        Guid materiaId,
        Guid grupoId,
        Guid? colegioId = null,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene reporte de ausentismo por período
    /// </summary>
    /// <param name="periodoEvaluativoId">ID del período evaluativo</param>
    /// <param name="colegioId">ID del colegio (filtro opcional)</param>
    /// <param name="grupoId">ID del grupo (filtro opcional)</param>
    /// <param name="minimoAusencias">Mínimo de ausencias para incluir en el reporte</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de objetos con información de ausentismo</returns>
    Task<IEnumerable<object>> GetReporteAusentismoAsync(
        Guid periodoEvaluativoId,
        Guid? colegioId = null,
        Guid? grupoId = null,
        int? minimoAusencias = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Métodos de Justificación

    /// <summary>
    /// Justifica múltiples ausencias o tardanzas
    /// </summary>
    /// <param name="asistenciaIds">Lista de IDs de asistencias a justificar</param>
    /// <param name="motivoJustificacion">Motivo de la justificación</param>
    /// <param name="justificadoPorId">ID del usuario que justifica</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Número de asistencias justificadas exitosamente</returns>
    Task<int> JustificarAsistenciasAsync(
        List<Guid> asistenciaIds,
        string motivoJustificacion,
        Guid justificadoPorId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene asistencias pendientes de justificación
    /// </summary>
    /// <param name="colegioId">ID del colegio (filtro opcional)</param>
    /// <param name="estudianteId">ID del estudiante (filtro opcional)</param>
    /// <param name="grupoId">ID del grupo (filtro opcional)</param>
    /// <param name="fechaInicio">Fecha de inicio del rango (opcional)</param>
    /// <param name="fechaFin">Fecha de fin del rango (opcional)</param>
    /// <param name="includeEntities">Entidades relacionadas a incluir</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de asistencias pendientes de justificación</returns>
    Task<IEnumerable<Asistencia>> GetAsistenciasPendientesJustificacionAsync(
        Guid? colegioId = null,
        Guid? estudianteId = null,
        Guid? grupoId = null,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        string[]? includeEntities = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Paginación

    /// <summary>
    /// Obtiene asistencias con paginación y filtros múltiples
    /// </summary>
    /// <param name="pageNumber">Número de página (base 1)</param>
    /// <param name="pageSize">Tamaño de página (máximo 100)</param>
    /// <param name="colegioId">ID del colegio (filtro opcional)</param>
    /// <param name="estudianteId">ID del estudiante (filtro opcional)</param>
    /// <param name="grupoId">ID del grupo (filtro opcional)</param>
    /// <param name="materiaId">ID de la materia (filtro opcional)</param>
    /// <param name="fechaInicio">Fecha de inicio del rango (opcional)</param>
    /// <param name="fechaFin">Fecha de fin del rango (opcional)</param>
    /// <param name="estado">Estado de asistencia específico (opcional)</param>
    /// <param name="soloInjustificadas">Solo ausencias/tardanzas injustificadas</param>
    /// <param name="includeEntities">Entidades relacionadas a incluir</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Tupla con elementos paginados y total de registros</returns>
    Task<(IEnumerable<Asistencia> Items, int TotalCount)> GetPagedAsync(
        int pageNumber = 1,
        int pageSize = 50,
        Guid? colegioId = null,
        Guid? estudianteId = null,
        Guid? grupoId = null,
        Guid? materiaId = null,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        EstadoAsistencia? estado = null,
        bool? soloInjustificadas = null,
        string[]? includeEntities = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Validaciones y Verificaciones

    /// <summary>
    /// Verifica si existe asistencia para estudiante en fecha y materia específica
    /// </summary>
    /// <param name="estudianteId">ID del estudiante</param>
    /// <param name="materiaId">ID de la materia</param>
    /// <param name="grupoId">ID del grupo</param>
    /// <param name="fechaClase">Fecha de la clase</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si existe la asistencia</returns>
    Task<bool> ExisteAsistenciaAsync(
        Guid estudianteId,
        Guid materiaId,
        Guid grupoId,
        DateTime fechaClase,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene conflictos de asistencia para validación
    /// </summary>
    /// <param name="estudianteId">ID del estudiante</param>
    /// <param name="fechaClase">Fecha de la clase</param>
    /// <param name="excludeAsistenciaId">ID de asistencia a excluir (para edición)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de mensajes de conflicto</returns>
    Task<IEnumerable<string>> ValidarConflictosAsync(
        Guid estudianteId,
        DateTime fechaClase,
        Guid? excludeAsistenciaId = null,
        CancellationToken cancellationToken = default);

    #endregion
}