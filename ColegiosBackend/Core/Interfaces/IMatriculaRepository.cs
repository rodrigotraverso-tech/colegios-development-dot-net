using ColegiosBackend.Core.Entities;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Interfaces;

/// <summary>
/// Repositorio para la gestión de matrículas de estudiantes
/// </summary>
public interface IMatriculaRepository
{
    #region Métodos CRUD Básicos

    /// <summary>
    /// Obtiene una matrícula por su ID
    /// </summary>
    /// <param name="id">ID de la matrícula</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Matrícula encontrada o null</returns>
    Task<Matricula?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una matrícula por su ID incluyendo entidades relacionadas
    /// </summary>
    /// <param name="id">ID de la matrícula</param>
    /// <param name="includeEntities">Entidades a incluir</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Matrícula con entidades relacionadas</returns>
    Task<Matricula?> GetByIdWithIncludesAsync(Guid id, string[] includeEntities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega una nueva matrícula
    /// </summary>
    /// <param name="matricula">Matrícula a agregar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task AddAsync(Matricula matricula, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza una matrícula existente
    /// </summary>
    /// <param name="matricula">Matrícula a actualizar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task UpdateAsync(Matricula matricula, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una matrícula (soft delete)
    /// </summary>
    /// <param name="id">ID de la matrícula a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas Específicas de Negocio

    /// <summary>
    /// Obtiene todas las matrículas de un estudiante específico
    /// </summary>
    /// <param name="estudianteId">ID del estudiante</param>
    /// <param name="colegioId">ID del colegio (para multi-tenancy)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de matrículas del estudiante</returns>
    Task<IEnumerable<Matricula>> GetByEstudianteAsync(Guid estudianteId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene la matrícula activa de un estudiante en un año académico específico
    /// </summary>
    /// <param name="estudianteId">ID del estudiante</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Matrícula activa o null</returns>
    Task<Matricula?> GetMatriculaActivaAsync(Guid estudianteId, Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las matrículas de un grupo específico
    /// </summary>
    /// <param name="grupoId">ID del grupo</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="soloActivas">Si solo incluir matrículas activas</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de matrículas del grupo</returns>
    Task<IEnumerable<Matricula>> GetByGrupoAsync(Guid grupoId, Guid colegioId, bool soloActivas = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las matrículas de un año académico
    /// </summary>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="estado">Estado específico de matrícula (opcional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de matrículas del año académico</returns>
    Task<IEnumerable<Matricula>> GetByAnoAcademicoAsync(Guid anoAcademicoId, Guid colegioId, EstadoMatricula? estado = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si existe una matrícula activa para un estudiante en un año académico
    /// </summary>
    /// <param name="estudianteId">ID del estudiante</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si existe matrícula activa</returns>
    Task<bool> ExisteMatriculaActivaAsync(Guid estudianteId, Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el número de matrícula siguiente disponible para un año académico
    /// </summary>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Número de matrícula siguiente</returns>
    Task<int> GetSiguienteNumeroMatriculaAsync(Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas de Reportes y Estadísticas

    /// <summary>
    /// Obtiene el conteo de matrículas por estado en un año académico
    /// </summary>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Diccionario con conteo por estado</returns>
    Task<Dictionary<EstadoMatricula, int>> GetConteoByEstadoAsync(Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el conteo de matrículas por tipo en un año académico
    /// </summary>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Diccionario con conteo por tipo</returns>
    Task<Dictionary<TipoMatricula, int>> GetConteoByTipoAsync(Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene las matrículas con becas en un año académico
    /// </summary>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="tipoBeca">Tipo específico de beca (opcional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de matrículas con becas</returns>
    Task<IEnumerable<Matricula>> GetMatriculasConBecasAsync(Guid anoAcademicoId, Guid colegioId, TipoBecaMatricula? tipoBeca = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene las matrículas con pagos pendientes
    /// </summary>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="diasVencimiento">Días de vencimiento para considerar pendiente</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de matrículas con pagos pendientes</returns>
    Task<IEnumerable<Matricula>> GetMatriculasConPagosPendientesAsync(Guid anoAcademicoId, Guid colegioId, int diasVencimiento = 30, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas de Búsqueda y Filtrado

    /// <summary>
    /// Busca matrículas por criterios múltiples
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="anoAcademicoId">ID del año académico (opcional)</param>
    /// <param name="grupoId">ID del grupo (opcional)</param>
    /// <param name="estado">Estado de matrícula (opcional)</param>
    /// <param name="tipo">Tipo de matrícula (opcional)</param>
    /// <param name="buscarTexto">Texto a buscar en estudiante (opcional)</param>
    /// <param name="pageNumber">Número de página para paginación</param>
    /// <param name="pageSize">Tamaño de página para paginación</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Resultado paginado de matrículas</returns>
    Task<(IEnumerable<Matricula> Items, int TotalCount)> SearchAsync(
        Guid colegioId,
        Guid? anoAcademicoId = null,
        Guid? grupoId = null,
        EstadoMatricula? estado = null,
        TipoMatricula? tipo = null,
        string? buscarTexto = null,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene matrículas que vencen en un rango de fechas
    /// </summary>
    /// <param name="fechaInicio">Fecha inicio del rango</param>
    /// <param name="fechaFin">Fecha fin del rango</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de matrículas que vencen en el rango</returns>
    Task<IEnumerable<Matricula>> GetMatriculasQueVencenAsync(DateTime fechaInicio, DateTime fechaFin, Guid colegioId, CancellationToken cancellationToken = default);

    #endregion

    #region Métodos de Validación

    /// <summary>
    /// Verifica si un número de matrícula ya existe en un año académico
    /// </summary>
    /// <param name="numeroMatricula">Número de matrícula a verificar</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="excludeId">ID de matrícula a excluir de la verificación</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si el número ya existe</returns>
    Task<bool> ExisteNumeroMatriculaAsync(int numeroMatricula, Guid anoAcademicoId, Guid colegioId, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si se puede eliminar una matrícula
    /// </summary>
    /// <param name="id">ID de la matrícula</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si se puede eliminar</returns>
    Task<bool> CanDeleteAsync(Guid id, CancellationToken cancellationToken = default);

    #endregion
}