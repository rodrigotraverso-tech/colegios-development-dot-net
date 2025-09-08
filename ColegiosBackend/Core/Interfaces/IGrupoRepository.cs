using ColegiosBackend.Core.Entities;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Interfaces;

/// <summary>
/// Repositorio para la gestión de grupos/secciones académicas
/// </summary>
public interface IGrupoRepository
{
    #region Métodos CRUD Básicos

    /// <summary>
    /// Obtiene un grupo por su ID
    /// </summary>
    /// <param name="id">ID del grupo</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Grupo encontrado o null</returns>
    Task<Grupo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un grupo por su ID incluyendo entidades relacionadas
    /// </summary>
    /// <param name="id">ID del grupo</param>
    /// <param name="includeEntities">Entidades a incluir</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Grupo con entidades relacionadas</returns>
    Task<Grupo?> GetByIdWithIncludesAsync(Guid id, string[] includeEntities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega un nuevo grupo
    /// </summary>
    /// <param name="grupo">Grupo a agregar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task AddAsync(Grupo grupo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza un grupo existente
    /// </summary>
    /// <param name="grupo">Grupo a actualizar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task UpdateAsync(Grupo grupo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina un grupo (soft delete)
    /// </summary>
    /// <param name="id">ID del grupo a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas Específicas de Negocio

    /// <summary>
    /// Obtiene un grupo por su código dentro de un grado y año académico
    /// </summary>
    /// <param name="gradoId">ID del grado</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="codigo">Código del grupo</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Grupo encontrado o null</returns>
    Task<Grupo?> GetByCodigoAsync(Guid gradoId, Guid anoAcademicoId, string codigo, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los grupos de un grado específico en un año académico
    /// </summary>
    /// <param name="gradoId">ID del grado</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="soloActivos">Si solo incluir grupos activos</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de grupos del grado</returns>
    Task<IEnumerable<Grupo>> GetByGradoAsync(Guid gradoId, Guid anoAcademicoId, Guid colegioId, bool soloActivos = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los grupos de un año académico específico
    /// </summary>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="soloActivos">Si solo incluir grupos activos</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de grupos del año académico</returns>
    Task<IEnumerable<Grupo>> GetByAnoAcademicoAsync(Guid anoAcademicoId, Guid colegioId, bool soloActivos = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene grupos asignados a un profesor como director de grupo
    /// </summary>
    /// <param name="profesorId">ID del profesor</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de grupos dirigidos por el profesor</returns>
    Task<IEnumerable<Grupo>> GetByDirectorGrupoAsync(Guid profesorId, Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los grupos de un colegio con filtros opcionales
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="anoAcademicoId">ID del año académico (opcional)</param>
    /// <param name="jornada">Jornada específica (opcional)</param>
    /// <param name="soloActivos">Si solo incluir grupos activos</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de grupos</returns>
    Task<IEnumerable<Grupo>> GetAllAsync(
        Guid colegioId,
        Guid? anoAcademicoId = null,
        JornadaAcademica? jornada = null,
        bool soloActivos = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene grupos con capacidad disponible para nuevas matrículas
    /// </summary>
    /// <param name="gradoId">ID del grado</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de grupos con capacidad disponible</returns>
    Task<IEnumerable<Grupo>> GetConCapacidadDisponibleAsync(Guid gradoId, Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene grupos sin director asignado
    /// </summary>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de grupos sin director</returns>
    Task<IEnumerable<Grupo>> GetSinDirectorAsync(Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas con Paginación

    /// <summary>
    /// Obtiene grupos paginados con filtros
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="pageNumber">Número de página</param>
    /// <param name="pageSize">Tamaño de página</param>
    /// <param name="gradoId">ID del grado (opcional)</param>
    /// <param name="anoAcademicoId">ID del año académico (opcional)</param>
    /// <param name="searchTerm">Término de búsqueda (opcional)</param>
    /// <param name="soloActivos">Si solo incluir grupos activos</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Tupla con grupos paginados y conteo total</returns>
    Task<(IEnumerable<Grupo> Items, int TotalCount)> GetPagedAsync(
        Guid colegioId,
        int pageNumber,
        int pageSize,
        Guid? gradoId = null,
        Guid? anoAcademicoId = null,
        string? searchTerm = null,
        bool soloActivos = true,
        CancellationToken cancellationToken = default);

    #endregion

    #region Validaciones de Negocio

    /// <summary>
    /// Verifica si existe un grupo con el mismo código en el grado y año académico
    /// </summary>
    /// <param name="gradoId">ID del grado</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="codigo">Código del grupo</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="excludeId">ID del grupo a excluir (para edición)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si existe un duplicado</returns>
    Task<bool> ExisteDuplicadoAsync(Guid gradoId, Guid anoAcademicoId, string codigo, Guid colegioId, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si un grupo puede ser eliminado
    /// </summary>
    /// <param name="id">ID del grupo</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si puede ser eliminado</returns>
    Task<bool> CanBeDeletedAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el número total de estudiantes matriculados en un grupo
    /// </summary>
    /// <param name="grupoId">ID del grupo</param>
    /// <param name="soloActivos">Si solo contar matrículas activas</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Número de estudiantes matriculados</returns>
    Task<int> GetConteoEstudiantesAsync(Guid grupoId, bool soloActivos = true, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas de Reportes y Estadísticas

    /// <summary>
    /// Obtiene el conteo de grupos por grado en un año académico
    /// </summary>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Diccionario con conteo por grado</returns>
    Task<Dictionary<string, int>> GetConteoByGradoAsync(Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el conteo de grupos por jornada
    /// </summary>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Diccionario con conteo por jornada</returns>
    Task<Dictionary<JornadaAcademica, int>> GetConteoByJornadaAsync(Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene estadísticas de ocupación de grupos
    /// </summary>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista con estadísticas de ocupación por grupo</returns>
    Task<IEnumerable<(Guid GrupoId, string NombreGrupo, int CapacidadMaxima, int EstudiantesMatriculados, decimal PorcentajeOcupacion)>> GetEstadisticasOcupacionAsync(
        Guid anoAcademicoId,
        Guid colegioId,
        CancellationToken cancellationToken = default);

    #endregion
}