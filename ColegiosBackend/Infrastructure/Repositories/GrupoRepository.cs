using ColegiosBackend.Core.Entities;
using ColegiosBackend.Core.Enums;
using ColegiosBackend.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ColegiosBackend.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio para la gestión de grupos/secciones académicas
/// </summary>
public class GrupoRepository : IGrupoRepository
{
    private readonly DbContext _context;
    private readonly DbSet<Grupo> _dbSet;

    public GrupoRepository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<Grupo>();
    }

    #region Métodos CRUD Básicos

    /// <summary>
    /// Obtiene un grupo por su ID
    /// </summary>
    public async Task<Grupo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(g => g.Id == id && !g.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Obtiene un grupo por su ID incluyendo entidades relacionadas
    /// </summary>
    public async Task<Grupo?> GetByIdWithIncludesAsync(Guid id, string[] includeEntities, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        // Agregar includes dinámicamente
        foreach (var include in includeEntities)
        {
            query = query.Include(include);
        }

        return await query
            .FirstOrDefaultAsync(g => g.Id == id && !g.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Agrega un nuevo grupo
    /// </summary>
    public async Task AddAsync(Grupo grupo, CancellationToken cancellationToken = default)
    {
        if (grupo == null)
            throw new ArgumentNullException(nameof(grupo));

        // Validar duplicados
        var existeDuplicado = await ExisteDuplicadoAsync(
            grupo.GradoId,
            grupo.AnoAcademicoId,
            grupo.Codigo,
            grupo.ColegioId!.Value,
            null,
            cancellationToken);

        if (existeDuplicado)
            throw new InvalidOperationException($"Ya existe un grupo con código '{grupo.Codigo}' en el grado y año académico especificado.");

        await _dbSet.AddAsync(grupo, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Actualiza un grupo existente
    /// </summary>
    public async Task UpdateAsync(Grupo grupo, CancellationToken cancellationToken = default)
    {
        if (grupo == null)
            throw new ArgumentNullException(nameof(grupo));

        // Validar duplicados excluyendo el grupo actual
        var existeDuplicado = await ExisteDuplicadoAsync(
            grupo.GradoId,
            grupo.AnoAcademicoId,
            grupo.Codigo,
            grupo.ColegioId!.Value,
            grupo.Id,
            cancellationToken);

        if (existeDuplicado)
            throw new InvalidOperationException($"Ya existe otro grupo con código '{grupo.Codigo}' en el grado y año académico especificado.");

        _dbSet.Update(grupo);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Elimina un grupo (soft delete)
    /// </summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var grupo = await GetByIdAsync(id, cancellationToken);
        if (grupo == null)
            throw new InvalidOperationException("El grupo no existe.");

        if (!await CanBeDeletedAsync(id, cancellationToken))
            throw new InvalidOperationException("El grupo no puede ser eliminado porque tiene estudiantes matriculados o depende de otros registros.");

        grupo.MarkAsDeleted();
        await _context.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Consultas Específicas de Negocio

    /// <summary>
    /// Obtiene un grupo por su código dentro de un grado y año académico
    /// </summary>
    public async Task<Grupo?> GetByCodigoAsync(Guid gradoId, Guid anoAcademicoId, string codigo, Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(g =>
                g.GradoId == gradoId &&
                g.AnoAcademicoId == anoAcademicoId &&
                g.Codigo == codigo &&
                g.ColegioId == colegioId &&
                !g.IsDeleted,
                cancellationToken);
    }

    /// <summary>
    /// Obtiene todos los grupos de un grado específico en un año académico
    /// </summary>
    public async Task<IEnumerable<Grupo>> GetByGradoAsync(Guid gradoId, Guid anoAcademicoId, Guid colegioId, bool soloActivos = true, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(g =>
                g.GradoId == gradoId &&
                g.AnoAcademicoId == anoAcademicoId &&
                g.ColegioId == colegioId &&
                !g.IsDeleted);

        if (soloActivos)
            query = query.Where(g => g.Activo);

        return await query
            .OrderBy(g => g.OrdenPresentacion)
            .ThenBy(g => g.Codigo)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene todos los grupos de un año académico específico
    /// </summary>
    public async Task<IEnumerable<Grupo>> GetByAnoAcademicoAsync(Guid anoAcademicoId, Guid colegioId, bool soloActivos = true, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(g => g.Grado)
            .Where(g =>
                g.AnoAcademicoId == anoAcademicoId &&
                g.ColegioId == colegioId &&
                !g.IsDeleted);

        if (soloActivos)
            query = query.Where(g => g.Activo);

        return await query
            .OrderBy(g => g.Grado!.OrdenPresentacion)
            .ThenBy(g => g.OrdenPresentacion)
            .ThenBy(g => g.Codigo)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene grupos asignados a un profesor como director de grupo
    /// </summary>
    public async Task<IEnumerable<Grupo>> GetByDirectorGrupoAsync(Guid profesorId, Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(g => g.Grado)
            .Where(g =>
                g.DirectorGrupoId == profesorId &&
                g.AnoAcademicoId == anoAcademicoId &&
                g.ColegioId == colegioId &&
                g.Activo &&
                !g.IsDeleted)
            .OrderBy(g => g.Grado!.OrdenPresentacion)
            .ThenBy(g => g.Codigo)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene todos los grupos de un colegio con filtros opcionales
    /// </summary>
    public async Task<IEnumerable<Grupo>> GetAllAsync(
        Guid colegioId,
        Guid? anoAcademicoId = null,
        JornadaAcademica? jornada = null,
        bool soloActivos = true,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(g => g.Grado)
            .Where(g =>
                g.ColegioId == colegioId &&
                !g.IsDeleted);

        if (anoAcademicoId.HasValue)
            query = query.Where(g => g.AnoAcademicoId == anoAcademicoId);

        if (jornada.HasValue)
            query = query.Where(g => g.Jornada == jornada);

        if (soloActivos)
            query = query.Where(g => g.Activo);

        return await query
            .OrderBy(g => g.Grado!.OrdenPresentacion)
            .ThenBy(g => g.OrdenPresentacion)
            .ThenBy(g => g.Codigo)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene grupos con capacidad disponible para nuevas matrículas
    /// </summary>
    public async Task<IEnumerable<Grupo>> GetConCapacidadDisponibleAsync(Guid gradoId, Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default)
    {
        var query = from g in _dbSet
                    where g.GradoId == gradoId &&
                          g.AnoAcademicoId == anoAcademicoId &&
                          g.ColegioId == colegioId &&
                          g.Activo &&
                          !g.IsDeleted
                    let matriculasActivas = _context.Set<Matricula>()
                        .Count(m => m.GrupoId == g.Id &&
                                   m.Estado == EstadoMatricula.Activa)
                    where matriculasActivas < g.CapacidadMaxima
                    orderby g.OrdenPresentacion, g.Codigo
                    select g;

        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene grupos sin director asignado
    /// </summary>
    public async Task<IEnumerable<Grupo>> GetSinDirectorAsync(Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(g => g.Grado)
            .Where(g =>
                g.AnoAcademicoId == anoAcademicoId &&
                g.ColegioId == colegioId &&
                g.DirectorGrupoId == null &&
                g.Activo &&
                !g.IsDeleted)
            .OrderBy(g => g.Grado!.OrdenPresentacion)
            .ThenBy(g => g.Codigo)
            .ToListAsync(cancellationToken);
    }

    #endregion
    #region Consultas con Paginación

    /// <summary>
    /// Obtiene grupos paginados con filtros
    /// </summary>
    public async Task<(IEnumerable<Grupo> Items, int TotalCount)> GetPagedAsync(
        Guid colegioId,
        int pageNumber,
        int pageSize,
        Guid? gradoId = null,
        Guid? anoAcademicoId = null,
        string? searchTerm = null,
        bool soloActivos = true,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(g => g.Grado)
            .Include(g => g.AnoAcademico)
            .Include(g => g.DirectorGrupo)
            .ThenInclude(p => p!.Persona)
            .Where(g =>
                g.ColegioId == colegioId &&
                !g.IsDeleted);

        if (gradoId.HasValue)
            query = query.Where(g => g.GradoId == gradoId);

        if (anoAcademicoId.HasValue)
            query = query.Where(g => g.AnoAcademicoId == anoAcademicoId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(g =>
                g.Nombre.ToLower().Contains(searchTerm) ||
                g.Codigo.ToLower().Contains(searchTerm) ||
                (g.Aula != null && g.Aula.ToLower().Contains(searchTerm)));
        }

        if (soloActivos)
            query = query.Where(g => g.Activo);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(g => g.Grado!.OrdenPresentacion)
            .ThenBy(g => g.OrdenPresentacion)
            .ThenBy(g => g.Codigo)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    #endregion

    #region Validaciones de Negocio

    /// <summary>
    /// Verifica si existe un grupo con el mismo código en el grado y año académico
    /// </summary>
    public async Task<bool> ExisteDuplicadoAsync(Guid gradoId, Guid anoAcademicoId, string codigo, Guid colegioId, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(g =>
                g.GradoId == gradoId &&
                g.AnoAcademicoId == anoAcademicoId &&
                g.Codigo == codigo &&
                g.ColegioId == colegioId &&
                !g.IsDeleted);

        if (excludeId.HasValue)
            query = query.Where(g => g.Id != excludeId);

        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Verifica si un grupo puede ser eliminado
    /// </summary>
    public async Task<bool> CanBeDeletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Verificar si tiene estudiantes matriculados
        var tieneEstudiantes = await _context.Set<Matricula>()
            .AnyAsync(m => m.GrupoId == id && m.Estado == EstadoMatricula.Activa, cancellationToken);

        if (tieneEstudiantes)
            return false;

        // Verificar si tiene horarios asignados
        var tieneHorarios = await _context.Set<Horario>()
            .AnyAsync(h => h.GrupoId == id && h.Activo, cancellationToken);

        if (tieneHorarios)
            return false;

        // Verificar si tiene asignaciones de profesores-materias
        var tieneAsignaciones = await _context.Set<Asignacion>()
            .AnyAsync(a => a.GrupoId == id && a.Estado == EstadoAsignacion.Activa, cancellationToken);

        if (tieneAsignaciones)
            return false;

        // Verificar si tiene calificaciones registradas (a través de Asignacion)
        var tieneCalificaciones = await _context.Set<Calificacion>()
            .Include(c => c.Asignacion)
            .AnyAsync(c => c.Asignacion!.GrupoId == id, cancellationToken);

        if (tieneCalificaciones)
            return false;

        // Verificar si tiene asistencias registradas
        var tieneAsistencias = await _context.Set<Asistencia>()
            .AnyAsync(a => a.GrupoId == id, cancellationToken);

        return !tieneAsistencias;
    }

    /// <summary>
    /// Obtiene el número total de estudiantes matriculados en un grupo
    /// </summary>
    public async Task<int> GetConteoEstudiantesAsync(Guid grupoId, bool soloActivos = true, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Matricula>()
            .Where(m => m.GrupoId == grupoId);

        if (soloActivos)
            query = query.Where(m => m.Estado == EstadoMatricula.Activa);

        return await query.CountAsync(cancellationToken);
    }

    #endregion

    #region Consultas de Reportes y Estadísticas

    /// <summary>
    /// Obtiene el conteo de grupos por grado en un año académico
    /// </summary>
    public async Task<Dictionary<string, int>> GetConteoByGradoAsync(Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(g => g.Grado)
            .Where(g =>
                g.AnoAcademicoId == anoAcademicoId &&
                g.ColegioId == colegioId &&
                g.Activo &&
                !g.IsDeleted)
            .GroupBy(g => g.Grado!.Nombre)
            .Select(group => new { Grado = group.Key, Conteo = group.Count() })
            .ToDictionaryAsync(x => x.Grado, x => x.Conteo, cancellationToken);
    }

    /// <summary>
    /// Obtiene el conteo de grupos por jornada
    /// </summary>
    public async Task<Dictionary<JornadaAcademica, int>> GetConteoByJornadaAsync(Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(g =>
                g.AnoAcademicoId == anoAcademicoId &&
                g.ColegioId == colegioId &&
                g.Activo &&
                g.Jornada.HasValue &&
                !g.IsDeleted)
            .GroupBy(g => g.Jornada!.Value)
            .Select(group => new { Jornada = group.Key, Conteo = group.Count() })
            .ToDictionaryAsync(x => x.Jornada, x => x.Conteo, cancellationToken);
    }

    /// <summary>
    /// Obtiene estadísticas de ocupación de grupos
    /// </summary>
    public async Task<IEnumerable<(Guid GrupoId, string NombreGrupo, int CapacidadMaxima, int EstudiantesMatriculados, decimal PorcentajeOcupacion)>> GetEstadisticasOcupacionAsync(
        Guid anoAcademicoId,
        Guid colegioId,
        CancellationToken cancellationToken = default)
    {
        var query = from g in _dbSet
                    where g.AnoAcademicoId == anoAcademicoId &&
                          g.ColegioId == colegioId &&
                          g.Activo &&
                          !g.IsDeleted
                    let estudiantesMatriculados = _context.Set<Matricula>()
                        .Count(m => m.GrupoId == g.Id && m.Estado == EstadoMatricula.Activa)
                    select new
                    {
                        GrupoId = g.Id,
                        NombreGrupo = g.Nombre,
                        CapacidadMaxima = g.CapacidadMaxima,
                        EstudiantesMatriculados = estudiantesMatriculados,
                        PorcentajeOcupacion = g.CapacidadMaxima > 0
                            ? (decimal)estudiantesMatriculados / g.CapacidadMaxima * 100
                            : 0
                    };

        var resultado = await query.ToListAsync(cancellationToken);

        return resultado.Select(x => (
            x.GrupoId,
            x.NombreGrupo,
            x.CapacidadMaxima,
            x.EstudiantesMatriculados,
            x.PorcentajeOcupacion
        ));
    }

    #endregion
}