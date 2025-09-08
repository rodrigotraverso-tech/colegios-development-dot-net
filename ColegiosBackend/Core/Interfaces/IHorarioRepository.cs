using ColegiosBackend.Core.Entities;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Interfaces;

/// <summary>
/// Repositorio para la gestión de horarios académicos
/// </summary>
public interface IHorarioRepository
{
    #region Métodos CRUD Básicos

    /// <summary>
    /// Obtiene un horario por su ID
    /// </summary>
    /// <param name="id">ID del horario</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Horario encontrado o null</returns>
    Task<Horario?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un horario por su ID incluyendo entidades relacionadas
    /// </summary>
    /// <param name="id">ID del horario</param>
    /// <param name="includeEntities">Entidades a incluir</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Horario con entidades relacionadas</returns>
    Task<Horario?> GetByIdWithIncludesAsync(Guid id, string[] includeEntities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega un nuevo horario
    /// </summary>
    /// <param name="horario">Horario a agregar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task AddAsync(Horario horario, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza un horario existente
    /// </summary>
    /// <param name="horario">Horario a actualizar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task UpdateAsync(Horario horario, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina un horario (soft delete)
    /// </summary>
    /// <param name="id">ID del horario a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas por Entidades Relacionadas

    /// <summary>
    /// Obtiene todos los horarios de un grupo específico
    /// </summary>
    /// <param name="grupoId">ID del grupo</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="soloActivos">Si solo incluir horarios activos</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de horarios del grupo</returns>
    Task<IEnumerable<Horario>> GetByGrupoAsync(Guid grupoId, Guid anoAcademicoId, bool soloActivos = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los horarios de un profesor específico
    /// </summary>
    /// <param name="profesorId">ID del profesor</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="diaSemana">Día específico (opcional)</param>
    /// <param name="soloActivos">Si solo incluir horarios activos</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de horarios del profesor</returns>
    Task<IEnumerable<Horario>> GetByProfesorAsync(Guid profesorId, Guid anoAcademicoId, DiaSemana? diaSemana = null, bool soloActivos = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los horarios de una materia específica
    /// </summary>
    /// <param name="materiaId">ID de la materia</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="soloActivos">Si solo incluir horarios activos</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de horarios de la materia</returns>
    Task<IEnumerable<Horario>> GetByMateriaAsync(Guid materiaId, Guid anoAcademicoId, bool soloActivos = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene horarios por día de la semana
    /// </summary>
    /// <param name="diaSemana">Día de la semana</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="soloActivos">Si solo incluir horarios activos</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de horarios del día</returns>
    Task<IEnumerable<Horario>> GetByDiaSemanaAsync(DiaSemana diaSemana, Guid colegioId, Guid anoAcademicoId, bool soloActivos = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene horarios por aula o salón
    /// </summary>
    /// <param name="aula">Nombre del aula</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="diaSemana">Día específico (opcional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de horarios del aula</returns>
    Task<IEnumerable<Horario>> GetByAulaAsync(string aula, Guid colegioId, Guid anoAcademicoId, DiaSemana? diaSemana = null, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas por Filtros Específicos

    /// <summary>
    /// Obtiene horarios por tipo de clase
    /// </summary>
    /// <param name="tipoClase">Tipo de clase</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="soloActivos">Si solo incluir horarios activos</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de horarios del tipo especificado</returns>
    Task<IEnumerable<Horario>> GetByTipoClaseAsync(TipoClase tipoClase, Guid colegioId, Guid anoAcademicoId, bool soloActivos = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene horarios por estado
    /// </summary>
    /// <param name="estado">Estado del horario</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de horarios con el estado especificado</returns>
    Task<IEnumerable<Horario>> GetByEstadoAsync(EstadoHorario estado, Guid colegioId, Guid anoAcademicoId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene horarios en un rango de tiempo específico
    /// </summary>
    /// <param name="horaInicio">Hora de inicio del rango</param>
    /// <param name="horaFin">Hora de fin del rango</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="diaSemana">Día específico (opcional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de horarios en el rango especificado</returns>
    Task<IEnumerable<Horario>> GetByRangoHorarioAsync(TimeOnly horaInicio, TimeOnly horaFin, Guid colegioId, Guid anoAcademicoId, DiaSemana? diaSemana = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene horarios vigentes en una fecha específica
    /// </summary>
    /// <param name="fecha">Fecha a consultar</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de horarios vigentes</returns>
    Task<IEnumerable<Horario>> GetVigentesEnFechaAsync(DateOnly fecha, Guid colegioId, Guid anoAcademicoId, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas de Disponibilidad y Conflictos

    /// <summary>
    /// Verifica si hay conflictos de horario para un profesor
    /// </summary>
    /// <param name="profesorId">ID del profesor</param>
    /// <param name="diaSemana">Día de la semana</param>
    /// <param name="horaInicio">Hora de inicio</param>
    /// <param name="horaFin">Hora de fin</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="excludeHorarioId">ID del horario a excluir (para edición)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si hay conflictos</returns>
    Task<bool> TieneConflictoProfesorAsync(Guid profesorId, DiaSemana diaSemana, TimeOnly horaInicio, TimeOnly horaFin, Guid anoAcademicoId, Guid? excludeHorarioId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si hay conflictos de horario para un grupo
    /// </summary>
    /// <param name="grupoId">ID del grupo</param>
    /// <param name="diaSemana">Día de la semana</param>
    /// <param name="horaInicio">Hora de inicio</param>
    /// <param name="horaFin">Hora de fin</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="excludeHorarioId">ID del horario a excluir (para edición)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si hay conflictos</returns>
    Task<bool> TieneConflictoGrupoAsync(Guid grupoId, DiaSemana diaSemana, TimeOnly horaInicio, TimeOnly horaFin, Guid anoAcademicoId, Guid? excludeHorarioId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si hay conflictos de aula
    /// </summary>
    /// <param name="aula">Nombre del aula</param>
    /// <param name="diaSemana">Día de la semana</param>
    /// <param name="horaInicio">Hora de inicio</param>
    /// <param name="horaFin">Hora de fin</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="excludeHorarioId">ID del horario a excluir (para edición)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si hay conflictos</returns>
    Task<bool> TieneConflictoAulaAsync(string aula, DiaSemana diaSemana, TimeOnly horaInicio, TimeOnly horaFin, Guid colegioId, Guid anoAcademicoId, Guid? excludeHorarioId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene horarios con conflictos en el sistema
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de horarios con conflictos</returns>
    Task<IEnumerable<Horario>> GetConflictosAsync(Guid colegioId, Guid anoAcademicoId, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas con Paginación

    /// <summary>
    /// Obtiene horarios paginados con filtros
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="pageNumber">Número de página</param>
    /// <param name="pageSize">Tamaño de página</param>
    /// <param name="grupoId">ID del grupo (opcional)</param>
    /// <param name="profesorId">ID del profesor (opcional)</param>
    /// <param name="diaSemana">Día de la semana (opcional)</param>
    /// <param name="soloActivos">Si solo incluir horarios activos</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Tupla con horarios paginados y conteo total</returns>
    Task<(IEnumerable<Horario> Items, int TotalCount)> GetPagedAsync(
        Guid colegioId,
        Guid anoAcademicoId,
        int pageNumber,
        int pageSize,
        Guid? grupoId = null,
        Guid? profesorId = null,
        DiaSemana? diaSemana = null,
        bool soloActivos = true,
        CancellationToken cancellationToken = default);

    #endregion

    #region Validaciones de Negocio

    /// <summary>
    /// Verifica si un horario puede ser eliminado
    /// </summary>
    /// <param name="id">ID del horario</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si puede ser eliminado</returns>
    Task<bool> CanBeDeletedAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el número total de horarios de un grupo
    /// </summary>
    /// <param name="grupoId">ID del grupo</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="soloActivos">Si solo contar horarios activos</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Número de horarios</returns>
    Task<int> GetConteoHorariosGrupoAsync(Guid grupoId, Guid anoAcademicoId, bool soloActivos = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el número total de horas semanales de un profesor
    /// </summary>
    /// <param name="profesorId">ID del profesor</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Número de horas semanales</returns>
    Task<int> GetHorasSemanalesProfesorAsync(Guid profesorId, Guid anoAcademicoId, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas de Reportes y Estadísticas

    /// <summary>
    /// Obtiene estadísticas de uso por día de la semana
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Diccionario con conteo por día</returns>
    Task<Dictionary<DiaSemana, int>> GetEstadisticasPorDiaAsync(Guid colegioId, Guid anoAcademicoId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene estadísticas de uso por tipo de clase
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Diccionario con conteo por tipo</returns>
    Task<Dictionary<TipoClase, int>> GetEstadisticasPorTipoClaseAsync(Guid colegioId, Guid anoAcademicoId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene estadísticas de ocupación de aulas
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista con estadísticas por aula</returns>
    Task<IEnumerable<(string Aula, int TotalHorarios, int HorasSemanales)>> GetEstadisticasOcupacionAulasAsync(Guid colegioId, Guid anoAcademicoId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene profesores con mayor carga horaria
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="limite">Número máximo de profesores a retornar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de profesores con su carga horaria</returns>
    Task<IEnumerable<(Guid ProfesorId, string NombreProfesor, int HorasSemanales, int TotalClases)>> GetProfesoresMayorCargaAsync(Guid colegioId, Guid anoAcademicoId, int limite = 10, CancellationToken cancellationToken = default);

    #endregion
}
