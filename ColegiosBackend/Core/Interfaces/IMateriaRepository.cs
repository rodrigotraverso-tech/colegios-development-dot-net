using ColegiosBackend.Core.Entities;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Interfaces;

/// <summary>
/// Repositorio para la gestión de materias/asignaturas académicas
/// </summary>
public interface IMateriaRepository
{
    #region Métodos CRUD Básicos

    /// <summary>
    /// Obtiene una materia por su ID
    /// </summary>
    /// <param name="id">ID de la materia</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Materia encontrada o null</returns>
    Task<Materia?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una materia por su ID incluyendo entidades relacionadas
    /// </summary>
    /// <param name="id">ID de la materia</param>
    /// <param name="includeEntities">Entidades a incluir</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Materia con entidades relacionadas</returns>
    Task<Materia?> GetByIdWithIncludesAsync(Guid id, string[] includeEntities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega una nueva materia
    /// </summary>
    /// <param name="materia">Materia a agregar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task AddAsync(Materia materia, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza una materia existente
    /// </summary>
    /// <param name="materia">Materia a actualizar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task UpdateAsync(Materia materia, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una materia (soft delete)
    /// </summary>
    /// <param name="id">ID de la materia a eliminar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas Específicas de Negocio

    /// <summary>
    /// Obtiene una materia por su código dentro del colegio
    /// </summary>
    /// <param name="codigoMateria">Código de la materia</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Materia encontrada o null</returns>
    Task<Materia?> GetByCodigoAsync(string codigoMateria, Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las materias de un área académica específica
    /// </summary>
    /// <param name="area">Área académica</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="soloActivas">Si solo incluir materias activas</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de materias del área</returns>
    Task<IEnumerable<Materia>> GetByAreaAcademicaAsync(AreaAcademica area, Guid colegioId, bool soloActivas = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene materias por nivel educativo permitido
    /// </summary>
    /// <param name="nivel">Nivel educativo</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="soloActivas">Si solo incluir materias activas</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de materias para el nivel</returns>
    Task<IEnumerable<Materia>> GetByNivelEducativoAsync(NivelesEducativos nivel, Guid colegioId, bool soloActivas = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene materias obligatorias o electivas
    /// </summary>
    /// <param name="esObligatoria">Si buscar materias obligatorias o electivas</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="area">Área académica (opcional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de materias según tipo</returns>
    Task<IEnumerable<Materia>> GetByTipoObligatoriaAsync(bool esObligatoria, Guid colegioId, AreaAcademica? area = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene materias prácticas (laboratorios, talleres)
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="area">Área académica (opcional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de materias prácticas</returns>
    Task<IEnumerable<Materia>> GetMateriasPracticasAsync(Guid colegioId, AreaAcademica? area = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene materias que requieren materiales especiales
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de materias que requieren materiales</returns>
    Task<IEnumerable<Materia>> GetConMaterialesEspecialesAsync(Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las materias de un colegio con filtros opcionales
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="estado">Estado específico (opcional)</param>
    /// <param name="area">Área académica (opcional)</param>
    /// <param name="nivel">Nivel educativo (opcional)</param>
    /// <param name="soloActivas">Si solo incluir materias activas</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de materias</returns>
    Task<IEnumerable<Materia>> GetAllAsync(
        Guid colegioId,
        EstadoMateria? estado = null,
        AreaAcademica? area = null,
        NivelesEducativos? nivel = null,
        bool soloActivas = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca materias por término de búsqueda (nombre, código, descripción)
    /// </summary>
    /// <param name="searchTerm">Término de búsqueda</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="area">Área académica (opcional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de materias que coinciden con la búsqueda</returns>
    Task<IEnumerable<Materia>> SearchAsync(string searchTerm, Guid colegioId, AreaAcademica? area = null, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas con Paginación

    /// <summary>
    /// Obtiene materias paginadas con filtros
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="pageNumber">Número de página</param>
    /// <param name="pageSize">Tamaño de página</param>
    /// <param name="area">Área académica (opcional)</param>
    /// <param name="estado">Estado de la materia (opcional)</param>
    /// <param name="searchTerm">Término de búsqueda (opcional)</param>
    /// <param name="soloActivas">Si solo incluir materias activas</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Tupla con materias paginadas y conteo total</returns>
    Task<(IEnumerable<Materia> Items, int TotalCount)> GetPagedAsync(
        Guid colegioId,
        int pageNumber,
        int pageSize,
        AreaAcademica? area = null,
        EstadoMateria? estado = null,
        string? searchTerm = null,
        bool soloActivas = true,
        CancellationToken cancellationToken = default);

    #endregion

    #region Validaciones de Negocio

    /// <summary>
    /// Verifica si existe una materia con el mismo código
    /// </summary>
    /// <param name="codigoMateria">Código de la materia</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="excludeId">ID de la materia a excluir (para edición)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si existe un duplicado</returns>
    Task<bool> ExisteDuplicadoAsync(string codigoMateria, Guid colegioId, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si una materia puede ser eliminada
    /// </summary>
    /// <param name="id">ID de la materia</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>True si puede ser eliminada</returns>
    Task<bool> CanBeDeletedAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el número de profesores asignados a una materia
    /// </summary>
    /// <param name="materiaId">ID de la materia</param>
    /// <param name="anoAcademicoId">ID del año académico (opcional)</param>
    /// <param name="soloActivos">Si solo contar asignaciones activas</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Número de profesores asignados</returns>
    Task<int> GetConteoProfesoresAsignadosAsync(Guid materiaId, Guid? anoAcademicoId = null, bool soloActivos = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el número de grupos donde se enseña una materia
    /// </summary>
    /// <param name="materiaId">ID de la materia</param>
    /// <param name="anoAcademicoId">ID del año académico (opcional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Número de grupos</returns>
    Task<int> GetConteoGruposAsync(Guid materiaId, Guid? anoAcademicoId = null, CancellationToken cancellationToken = default);

    #endregion

    #region Consultas de Reportes y Estadísticas

    /// <summary>
    /// Obtiene el conteo de materias por área académica
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="soloActivas">Si solo incluir materias activas</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Diccionario con conteo por área</returns>
    Task<Dictionary<AreaAcademica, int>> GetConteoByAreaAsync(Guid colegioId, bool soloActivas = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el conteo de materias por estado
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Diccionario con conteo por estado</returns>
    Task<Dictionary<EstadoMateria, int>> GetConteoByEstadoAsync(Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el conteo de materias por tipo (obligatoria/electiva)
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Diccionario con conteo por tipo</returns>
    Task<Dictionary<string, int>> GetConteoByTipoAsync(Guid colegioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene estadísticas de intensidad horaria por área
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista con estadísticas de horas por área</returns>
    Task<IEnumerable<(AreaAcademica Area, int TotalMaterias, int TotalHorasSemanales, decimal PromedioHoras)>> GetEstadisticasIntensidadHorariaAsync(
        Guid colegioId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene materias sin asignaciones en un año académico
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="anoAcademicoId">ID del año académico</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de materias sin asignaciones</returns>
    Task<IEnumerable<Materia>> GetSinAsignacionesAsync(Guid colegioId, Guid anoAcademicoId, CancellationToken cancellationToken = default);

    #endregion
}