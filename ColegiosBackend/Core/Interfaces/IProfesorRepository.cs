using ColegiosBackend.Core.Entities;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Interfaces;

/// <summary>
/// Repositorio para la gestión de profesores
/// </summary>
public interface IProfesorRepository
{
    #region Métodos CRUD Básicos

    /// <summary>
    /// Obtiene un profesor por su ID
    /// </summary>
    /// <param name="id">ID del profesor</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Profesor encontrado o null</returns>
    Task<Profesor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un profesor por su ID incluyendo entidades relacionadas
    /// </summary>
    /// <param name="id">ID del profesor</param>
    /// <param name="includeEntities">Entidades a incluir</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Profesor con entidades relacionadas</returns>
    Task<Profesor?> GetByIdWithIncludesAsync(Guid id, string[] includeEntities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega un nuevo profesor
    /// </summary>
    /// <param name="profesor">Profesor a agregar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task AddAsync(Profesor profesor, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza un profesor existente
    /// </summary>
    /// <param name="profesor">Profesor a actualizar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task UpdateAsync(Profesor profesor, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina un profesor (soft delete)
    /// </summary>
    /// <param name="id">ID del profesor a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas Específicas de Negocio

    /// <summary>
    /// Obtiene un profesor por su código dentro del colegio
    /// </summary>
    /// <param name="codigoProfesor">Código del profesor</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Profesor encontrado o null</returns>
    Task<Profesor?> GetByCodigoAsync(string codigoProfesor, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un profesor por su PersonaId dentro del colegio
    /// </summary>
    /// <param name="personaId">ID de la persona</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Profesor encontrado o null</returns>
    Task<Profesor?> GetByPersonaIdAsync(Guid personaId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene profesores por estado específico
    /// </summary>
    /// <param name="estado">Estado del profesor</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de profesores con el estado especificado</returns>
    Task<IEnumerable<Profesor>> GetByEstadoAsync(EstadoProfesor estado, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene profesores por tipo de contrato
    /// </summary>
    /// <param name="tipoContrato">Tipo de contrato</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="soloActivos">Si solo incluir profesores activos</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de profesores con el tipo de contrato especificado</returns>
    Task<IEnumerable<Profesor>> GetByTipoContratoAsync(TipoContratoProfesor tipoContrato, Guid colegioId, bool soloActivos = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene profesores por cargo
    /// </summary>
    /// <param name="cargo">Cargo del profesor</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="soloActivos">Si solo incluir profesores activos</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de profesores con el cargo especificado</returns>
    Task<IEnumerable<Profesor>> GetByCargoAsync(CargoProfesor cargo, Guid colegioId, bool soloActivos = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene profesores por especialidad académica
    /// </summary>
    /// <param name="especialidad">Especialidad a buscar</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="soloActivos">Si solo incluir profesores activos</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de profesores con la especialidad especificada</returns>
    Task<IEnumerable<Profesor>> GetByEspecialidadAsync(string especialidad, Guid colegioId, bool soloActivos = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene profesores disponibles para ser coordinadores
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de profesores que pueden ser coordinadores</returns>
    Task<IEnumerable<Profesor>> GetDisponiblesParaCoordinacionAsync(Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene profesores disponibles para ser directores de grupo
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="anoAcademicoId">ID del año académico (opcional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de profesores disponibles para dirigir grupos</returns>
    Task<IEnumerable<Profesor>> GetDisponiblesParaDireccionGrupoAsync(Guid colegioId, Guid? anoAcademicoId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene profesores disponibles para reemplazos
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="especialidad">Especialidad requerida (opcional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de profesores disponibles para reemplazos</returns>
    Task<IEnumerable<Profesor>> GetDisponiblesParaReemplazosAsync(Guid colegioId, string? especialidad = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas los profesores de un colegio con filtros opcionales
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="estado">Estado específico (opcional)</param>
    /// <param name="cargo">Cargo específico (opcional)</param>
    /// <param name="tipoContrato">Tipo de contrato (opcional)</param>
    /// <param name="soloActivos">Si solo incluir profesores activos</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de profesores</returns>
    Task<IEnumerable<Profesor>> GetAllAsync(
        Guid colegioId,
        EstadoProfesor? estado = null,
        CargoProfesor? cargo = null,
        TipoContratoProfesor? tipoContrato = null,
        bool soloActivos = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca profesores por término de búsqueda (código, nombres, especialidades)
    /// </summary>
    /// <param name="searchTerm">Término de búsqueda</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cargo">Cargo específico (opcional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de profesores que coinciden con la búsqueda</returns>
    Task<IEnumerable<Profesor>> SearchAsync(string searchTerm, Guid colegioId, CargoProfesor? cargo = null, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas de Asignaciones y Horarios

    /// <summary>
    /// Obtiene profesores con asignaciones en un año académico específico
    /// </summary>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="soloActivos">Si solo incluir asignaciones activas</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de profesores con asignaciones</returns>
    Task<IEnumerable<Profesor>> GetConAsignacionesAsync(Guid anoAcademicoId, Guid colegioId, bool soloActivos = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene profesores sin asignaciones en un año académico
    /// </summary>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de profesores sin asignaciones</returns>
    Task<IEnumerable<Profesor>> GetSinAsignacionesAsync(Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene directores de grupo activos
    /// </summary>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de profesores que son directores de grupo</returns>
    Task<IEnumerable<Profesor>> GetDirectoresGrupoAsync(Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas con Paginación

    /// <summary>
    /// Obtiene profesores paginados con filtros
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="pageNumber">Número de página</param>
    /// <param name="pageSize">Tamaño de página</param>
    /// <param name="estado">Estado del profesor (opcional)</param>
    /// <param name="cargo">Cargo específico (opcional)</param>
    /// <param name="searchTerm">Término de búsqueda (opcional)</param>
    /// <param name="soloActivos">Si solo incluir profesores activos</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Tupla con profesores paginados y conteo total</returns>
    Task<(IEnumerable<Profesor> Items, int TotalCount)> GetPagedAsync(
        Guid colegioId,
        int pageNumber,
        int pageSize,
        EstadoProfesor? estado = null,
        CargoProfesor? cargo = null,
        string? searchTerm = null,
        bool soloActivos = true,
        CancellationToken cancellationToken = default);

    #endregion

    #region Validaciones de Negocio

    /// <summary>
    /// Verifica si existe un profesor con el mismo código
    /// </summary>
    /// <param name="codigoProfesor">Código del profesor</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="excludeId">ID del profesor a excluir (para edición)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si existe un duplicado</returns>
    Task<bool> ExisteDuplicadoAsync(string codigoProfesor, Guid colegioId, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si una persona ya es profesor en el colegio
    /// </summary>
    /// <param name="personaId">ID de la persona</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="excludeId">ID del profesor a excluir (para edición)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si la persona ya es profesor</returns>
    Task<bool> PersonaYaEsProfesorAsync(Guid personaId, Guid colegioId, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si un profesor puede ser eliminado
    /// </summary>
    /// <param name="id">ID del profesor</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si puede ser eliminado</returns>
    Task<bool> CanBeDeletedAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el número de asignaciones de un profesor
    /// </summary>
    /// <param name="profesorId">ID del profesor</param>
    /// <param name="anoAcademicoId">ID del año académico (opcional)</param>
    /// <param name="soloActivas">Si solo contar asignaciones activas</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Número de asignaciones</returns>
    Task<int> GetConteoAsignacionesAsync(Guid profesorId, Guid? anoAcademicoId = null, bool soloActivas = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el número de grupos dirigidos por un profesor
    /// </summary>
    /// <param name="profesorId">ID del profesor</param>
    /// <param name="anoAcademicoId">ID del año académico (opcional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Número de grupos dirigidos</returns>
    Task<int> GetConteoGruposDirigidosAsync(Guid profesorId, Guid? anoAcademicoId = null, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas de Reportes y Estadísticas

    /// <summary>
    /// Obtiene el conteo de profesores por estado
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Diccionario con conteo por estado</returns>
    Task<Dictionary<EstadoProfesor, int>> GetConteoByEstadoAsync(Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el conteo de profesores por cargo
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="soloActivos">Si solo incluir profesores activos</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Diccionario con conteo por cargo</returns>
    Task<Dictionary<CargoProfesor, int>> GetConteoByCargoAsync(Guid colegioId, bool soloActivos = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el conteo de profesores por tipo de contrato
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="soloActivos">Si solo incluir profesores activos</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Diccionario con conteo por tipo de contrato</returns>
    Task<Dictionary<TipoContratoProfesor, int>> GetConteoByTipoContratoAsync(Guid colegioId, bool soloActivos = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene estadísticas de experiencia de profesores
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Estadísticas de años de experiencia</returns>
    Task<(int TotalProfesores, decimal PromedioExperiencia, int MaximaExperiencia, int MinimaExperiencia)> GetEstadisticasExperienciaAsync(
        Guid colegioId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene profesores próximos a jubilarse
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="edadJubilacion">Edad de jubilación (por defecto 65)</param>
    /// <param name="anosAnticipacion">Años de anticipación (por defecto 5)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de profesores próximos a jubilarse</returns>
    Task<IEnumerable<Profesor>> GetProximosJubilacionAsync(Guid colegioId, int edadJubilacion = 65, int anosAnticipacion = 5, CancellationToken cancellationToken = default);

    #endregion
}