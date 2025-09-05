using ColegiosBackend.Core.Entities;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Interfaces;

/// <summary>
/// Repositorio para la gesti�n de matr�culas de estudiantes
/// </summary>
public interface IMatriculaRepository
{
    #region M�todos CRUD B�sicos

    /// <summary>
    /// Obtiene una matr�cula por su ID
    /// </summary>
    /// <param name="id">ID de la matr�cula</param>
    /// <param name="cancellationToken">Token de cancelaci�n</param>
    /// <returns>Matr�cula encontrada o null</returns>
    Task<Matricula?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una matr�cula por su ID incluyendo entidades relacionadas
    /// </summary>
    /// <param name="id">ID de la matr�cula</param>
    /// <param name="includeEntities">Entidades a incluir</param>
    /// <param name="cancellationToken">Token de cancelaci�n</param>
    /// <returns>Matr�cula con entidades relacionadas</returns>
    Task<Matricula?> GetByIdWithIncludesAsync(Guid id, string[] includeEntities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega una nueva matr�cula
    /// </summary>
    /// <param name="matricula">Matr�cula a agregar</param>
    /// <param name="cancellationToken">Token de cancelaci�n</param>
    Task AddAsync(Matricula matricula, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza una matr�cula existente
    /// </summary>
    /// <param name="matricula">Matr�cula a actualizar</param>
    /// <param name="cancellationToken">Token de cancelaci�n</param>
    Task UpdateAsync(Matricula matricula, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una matr�cula (soft delete)
    /// </summary>
    /// <param name="id">ID de la matr�cula a eliminar</param>
    /// <param name="cancellationToken">Token de cancelaci�n</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas Espec�ficas de Negocio

    /// <summary>
    /// Obtiene todas las matr�culas de un estudiante espec�fico
    /// </summary>
    /// <param name="estudianteId">ID del estudiante</param>
    /// <param name="colegioId">ID del colegio (para multi-tenancy)</param>
    /// <param name="cancellationToken">Token de cancelaci�n</param>
    /// <returns>Lista de matr�culas del estudiante</returns>
    Task<IEnumerable<Matricula>> GetByEstudianteAsync(Guid estudianteId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene la matr�cula activa de un estudiante en un a�o acad�mico espec�fico
    /// </summary>
    /// <param name="estudianteId">ID del estudiante</param>
    /// <param name="anoAcademicoId">ID del a�o acad�mico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelaci�n</param>
    /// <returns>Matr�cula activa o null</returns>
    Task<Matricula?> GetMatriculaActivaAsync(Guid estudianteId, Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las matr�culas de un grupo espec�fico
    /// </summary>
    /// <param name="grupoId">ID del grupo</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="soloActivas">Si solo incluir matr�culas activas</param>
    /// <param name="cancellationToken">Token de cancelaci�n</param>
    /// <returns>Lista de matr�culas del grupo</returns>
    Task<IEnumerable<Matricula>> GetByGrupoAsync(Guid grupoId, Guid colegioId, bool soloActivas = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las matr�culas de un a�o acad�mico
    /// </summary>
    /// <param name="anoAcademicoId">ID del a�o acad�mico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="estado">Estado espec�fico de matr�cula (opcional)</param>
    /// <param name="cancellationToken">Token de cancelaci�n</param>
    /// <returns>Lista de matr�culas del a�o acad�mico</returns>
    Task<IEnumerable<Matricula>> GetByAnoAcademicoAsync(Guid anoAcademicoId, Guid colegioId, EstadoMatricula? estado = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si existe una matr�cula activa para un estudiante en un a�o acad�mico
    /// </summary>
    /// <param name="estudianteId">ID del estudiante</param>
    /// <param name="anoAcademicoId">ID del a�o acad�mico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelaci�n</param>
    /// <returns>True si existe matr�cula activa</returns>
    Task<bool> ExisteMatriculaActivaAsync(Guid estudianteId, Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el n�mero de matr�cula siguiente disponible para un a�o acad�mico
    /// </summary>
    /// <param name="anoAcademicoId">ID del a�o acad�mico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelaci�n</param>
    /// <returns>N�mero de matr�cula siguiente</returns>
    Task<int> GetSiguienteNumeroMatriculaAsync(Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas de Reportes y Estad�sticas

    /// <summary>
    /// Obtiene el conteo de matr�culas por estado en un a�o acad�mico
    /// </summary>
    /// <param name="anoAcademicoId">ID del a�o acad�mico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelaci�n</param>
    /// <returns>Diccionario con conteo por estado</returns>
    Task<Dictionary<EstadoMatricula, int>> GetConteoByEstadoAsync(Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el conteo de matr�culas por tipo en un a�o acad�mico
    /// </summary>
    /// <param name="anoAcademicoId">ID del a�o acad�mico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelaci�n</param>
    /// <returns>Diccionario con conteo por tipo</returns>
    Task<Dictionary<TipoMatricula, int>> GetConteoByTipoAsync(Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene las matr�culas con becas en un a�o acad�mico
    /// </summary>
    /// <param name="anoAcademicoId">ID del a�o acad�mico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="tipoBeca">Tipo espec�fico de beca (opcional)</param>
    /// <param name="cancellationToken">Token de cancelaci�n</param>
    /// <returns>Lista de matr�culas con becas</returns>
    Task<IEnumerable<Matricula>> GetMatriculasConBecasAsync(Guid anoAcademicoId, Guid colegioId, TipoBecaMatricula? tipoBeca = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene las matr�culas con pagos pendientes
    /// </summary>
    /// <param name="anoAcademicoId">ID del a�o acad�mico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="diasVencimiento">D�as de vencimiento para considerar pendiente</param>
    /// <param name="cancellationToken">Token de cancelaci�n</param>
    /// <returns>Lista de matr�culas con pagos pendientes</returns>
    Task<IEnumerable<Matricula>> GetMatriculasConPagosPendientesAsync(Guid anoAcademicoId, Guid colegioId, int diasVencimiento = 30, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas de B�squeda y Filtrado

    /// <summary>
    /// Busca matr�culas por criterios m�ltiples
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="anoAcademicoId">ID del a�o acad�mico (opcional)</param>
    /// <param name="grupoId">ID del grupo (opcional)</param>
    /// <param name="estado">Estado de matr�cula (opcional)</param>
    /// <param name="tipo">Tipo de matr�cula (opcional)</param>
    /// <param name="buscarTexto">Texto a buscar en estudiante (opcional)</param>
    /// <param name="pageNumber">N�mero de p�gina para paginaci�n</param>
    /// <param name="pageSize">Tama�o de p�gina para paginaci�n</param>
    /// <param name="cancellationToken">Token de cancelaci�n</param>
    /// <returns>Resultado paginado de matr�culas</returns>
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
    /// Obtiene matr�culas que vencen en un rango de fechas
    /// </summary>
    /// <param name="fechaInicio">Fecha inicio del rango</param>
    /// <param name="fechaFin">Fecha fin del rango</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelaci�n</param>
    /// <returns>Lista de matr�culas que vencen en el rango</returns>
    Task<IEnumerable<Matricula>> GetMatriculasQueVencenAsync(DateTime fechaInicio, DateTime fechaFin, Guid colegioId, CancellationToken cancellationToken = default);

    #endregion

    #region M�todos de Validaci�n

    /// <summary>
    /// Verifica si un n�mero de matr�cula ya existe en un a�o acad�mico
    /// </summary>
    /// <param name="numeroMatricula">N�mero de matr�cula a verificar</param>
    /// <param name="anoAcademicoId">ID del a�o acad�mico</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="excludeId">ID de matr�cula a excluir de la verificaci�n</param>
    /// <param name="cancellationToken">Token de cancelaci�n</param>
    /// <returns>True si el n�mero ya existe</returns>
    Task<bool> ExisteNumeroMatriculaAsync(int numeroMatricula, Guid anoAcademicoId, Guid colegioId, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si se puede eliminar una matr�cula
    /// </summary>
    /// <param name="id">ID de la matr�cula</param>
    /// <param name="cancellationToken">Token de cancelaci�n</param>
    /// <returns>True si se puede eliminar</returns>
    Task<bool> CanDeleteAsync(Guid id, CancellationToken cancellationToken = default);

    #endregion
}