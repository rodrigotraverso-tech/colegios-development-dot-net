using ColegiosBackend.Core.Entities;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Interfaces;

/// <summary>
/// Repositorio para la gestión de estudiantes
/// </summary>
public interface IEstudianteRepository
{
    #region Métodos CRUD Básicos

    /// <summary>
    /// Obtiene un estudiante por su ID
    /// </summary>
    /// <param name="id">ID del estudiante</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Estudiante encontrado o null</returns>
    Task<Estudiante?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un estudiante por su ID incluyendo entidades relacionadas
    /// </summary>
    /// <param name="id">ID del estudiante</param>
    /// <param name="includeEntities">Entidades a incluir</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Estudiante con entidades relacionadas</returns>
    Task<Estudiante?> GetByIdWithIncludesAsync(Guid id, string[] includeEntities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega un nuevo estudiante
    /// </summary>
    /// <param name="estudiante">Estudiante a agregar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task AddAsync(Estudiante estudiante, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza un estudiante existente
    /// </summary>
    /// <param name="estudiante">Estudiante a actualizar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task UpdateAsync(Estudiante estudiante, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina un estudiante (soft delete)
    /// </summary>
    /// <param name="id">ID del estudiante a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas Específicas de Negocio

    /// <summary>
    /// Obtiene un estudiante por su PersonaId
    /// </summary>
    /// <param name="personaId">ID de la persona</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Estudiante encontrado o null</returns>
    Task<Estudiante?> GetByPersonaIdAsync(Guid personaId, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un estudiante por su código único
    /// </summary>
    /// <param name="codigoEstudiante">Código del estudiante</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Estudiante encontrado o null</returns>
    Task<Estudiante?> GetByCodigoAsync(string codigoEstudiante, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un estudiante por su número de matrícula actual
    /// </summary>
    /// <param name="numeroMatricula">Número de matrícula</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Estudiante encontrado o null</returns>
    Task<Estudiante?> GetByNumeroMatriculaAsync(string numeroMatricula, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los estudiantes de un colegio
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="estado">Estado específico de estudiante (opcional)</param>
    /// <param name="soloActivos">Si solo incluir estudiantes activos</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de estudiantes del colegio</returns>
    Task<IEnumerable<Estudiante>> GetByColegioAsync(Guid colegioId, EstadoEstudiante? estado = null, bool soloActivos = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene estudiantes por año de ingreso
    /// </summary>
    /// <param name="anoIngreso">Año de ingreso</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de estudiantes del año de ingreso</returns>
    Task<IEnumerable<Estudiante>> GetByAnoIngresoAsync(int anoIngreso, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene estudiantes por rango de edades
    /// </summary>
    /// <param name="edadMinima">Edad mínima</param>
    /// <param name="edadMaxima">Edad máxima</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de estudiantes en el rango de edad</returns>
    Task<IEnumerable<Estudiante>> GetByRangoEdadAsync(int edadMinima, int edadMaxima, Guid colegioId, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas de Acudientes

    /// <summary>
    /// Obtiene todos los acudientes de un estudiante
    /// </summary>
    /// <param name="estudianteId">ID del estudiante</param>
    /// <param name="soloActivos">Si solo incluir relaciones activas</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de relaciones estudiante-acudiente</returns>
    Task<IEnumerable<EstudianteAcudiente>> GetAcudientesAsync(Guid estudianteId, bool soloActivos = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el acudiente principal de un estudiante
    /// </summary>
    /// <param name="estudianteId">ID del estudiante</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Relación del acudiente principal o null</returns>
    Task<EstudianteAcudiente?> GetAcudientePrincipalAsync(Guid estudianteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene estudiantes que tienen a una persona como acudiente
    /// </summary>
    /// <param name="acudienteId">ID de la persona acudiente</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="soloActivos">Si solo incluir relaciones activas</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de estudiantes</returns>
    Task<IEnumerable<Estudiante>> GetEstudiantesByAcudienteAsync(Guid acudienteId, Guid colegioId, bool soloActivos = true, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas de Reportes y Estadísticas

    /// <summary>
    /// Obtiene el conteo de estudiantes por estado
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Diccionario con conteo por estado</returns>
    Task<Dictionary<EstadoEstudiante, int>> GetConteoByEstadoAsync(Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el conteo de estudiantes por año de ingreso
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Diccionario con conteo por año</returns>
    Task<Dictionary<int, int>> GetConteoByAnoIngresoAsync(Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el conteo de estudiantes por género
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Diccionario con conteo por género</returns>
    Task<Dictionary<string, int>> GetConteoByGeneroAsync(Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene estudiantes con información médica relevante
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de estudiantes con información médica</returns>
    Task<IEnumerable<Estudiante>> GetConInformacionMedicaAsync(Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene estudiantes sin acudientes activos
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de estudiantes sin acudientes</returns>
    Task<IEnumerable<Estudiante>> GetSinAcudientesActivosAsync(Guid colegioId, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas de Búsqueda y Filtrado

    /// <summary>
    /// Busca estudiantes por criterios múltiples
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="buscarTexto">Texto a buscar en nombres, apellidos o documento</param>
    /// <param name="estado">Estado específico (opcional)</param>
    /// <param name="anoIngresoDesde">Año de ingreso desde (opcional)</param>
    /// <param name="anoIngresoHasta">Año de ingreso hasta (opcional)</param>
    /// <param name="edadDesde">Edad desde (opcional)</param>
    /// <param name="edadHasta">Edad hasta (opcional)</param>
    /// <param name="genero">Género específico (opcional)</param>
    /// <param name="soloActivos">Si solo incluir estudiantes activos</param>
    /// <param name="pageNumber">Número de página para paginación</param>
    /// <param name="pageSize">Tamaño de página para paginación</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Resultado paginado de estudiantes</returns>
    Task<(IEnumerable<Estudiante> Items, int TotalCount)> SearchAsync(
        Guid colegioId,
        string? buscarTexto = null,
        EstadoEstudiante? estado = null,
        int? anoIngresoDesde = null,
        int? anoIngresoHasta = null,
        int? edadDesde = null,
        int? edadHasta = null,
        char? genero = null,
        bool soloActivos = true,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca estudiantes por número de documento de identidad
    /// </summary>
    /// <param name="numeroDocumento">Número de documento</param>
    /// <param name="colegioId">ID del colegio (opcional para búsqueda global)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de estudiantes que coinciden</returns>
    Task<IEnumerable<Estudiante>> GetByNumeroDocumentoAsync(string numeroDocumento, Guid? colegioId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene estudiantes que cumplen años en un rango de fechas
    /// </summary>
    /// <param name="fechaInicio">Fecha inicio del rango</param>
    /// <param name="fechaFin">Fecha fin del rango</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de estudiantes que cumplen años</returns>
    Task<IEnumerable<Estudiante>> GetCumpleanosAsync(DateTime fechaInicio, DateTime fechaFin, Guid colegioId, CancellationToken cancellationToken = default);

    #endregion

    #region Métodos de Validación

    /// <summary>
    /// Verifica si un código de estudiante ya existe
    /// </summary>
    /// <param name="codigoEstudiante">Código a verificar</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="excludeId">ID de estudiante a excluir de la verificación</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si el código ya existe</returns>
    Task<bool> ExisteCodigoAsync(string codigoEstudiante, Guid colegioId, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si una persona ya es estudiante en el colegio
    /// </summary>
    /// <param name="personaId">ID de la persona</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="excludeId">ID de estudiante a excluir de la verificación</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si la persona ya es estudiante</returns>
    Task<bool> ExistePersonaComoEstudianteAsync(Guid personaId, Guid colegioId, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si se puede eliminar un estudiante
    /// </summary>
    /// <param name="id">ID del estudiante</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si se puede eliminar</returns>
    Task<bool> CanDeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el siguiente código de estudiante disponible
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="anoIngreso">Año de ingreso</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Siguiente código disponible</returns>
    Task<string> GetSiguienteCodigoAsync(Guid colegioId, int anoIngreso, CancellationToken cancellationToken = default);

    #endregion
}