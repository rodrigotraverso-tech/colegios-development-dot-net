using ColegiosBackend.Core.Entities;
using ColegiosBackend.Core.Enums;
using ColegiosBackend.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ColegiosBackend.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio para la gestión de calificaciones
/// </summary>
public class CalificacionRepository : ICalificacionRepository
{
    private readonly DbContext _context;
    private readonly DbSet<Calificacion> _dbSet;

    public CalificacionRepository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<Calificacion>();
    }

    #region Métodos CRUD Básicos

    /// <summary>
    /// Obtiene una calificación por su ID
    /// </summary>
    public async Task<Calificacion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Obtiene una calificación por su ID incluyendo entidades relacionadas
    /// </summary>
    public async Task<Calificacion?> GetByIdWithIncludesAsync(Guid id, string[] includeEntities, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        // Agregar includes dinámicamente
        foreach (var include in includeEntities)
        {
            query = query.Include(include);
        }

        return await query
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Agrega una nueva calificación
    /// </summary>
    public async Task AddAsync(Calificacion calificacion, CancellationToken cancellationToken = default)
    {
        if (calificacion == null)
            throw new ArgumentNullException(nameof(calificacion));

        // Validar que no exista una calificación duplicada
        var existeCalificacion = await ExisteCalificacionAsync(
            calificacion.EstudianteId,
            calificacion.AsignacionId,
            calificacion.TipoEvaluacionId,
            calificacion.PeriodoAcademicoId,
            calificacion.ColegioId!.Value,
            cancellationToken);

        if (existeCalificacion)
        {
            throw new InvalidOperationException("Ya existe una calificación para este estudiante en esta evaluación.");
        }

        await _dbSet.AddAsync(calificacion, cancellationToken);
    }

    /// <summary>
    /// Actualiza una calificación existente
    /// </summary>
    public async Task UpdateAsync(Calificacion calificacion, CancellationToken cancellationToken = default)
    {
        if (calificacion == null)
            throw new ArgumentNullException(nameof(calificacion));

        _dbSet.Update(calificacion);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Elimina una calificación (soft delete)
    /// </summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var calificacion = await GetByIdAsync(id, cancellationToken);
        if (calificacion == null)
            throw new InvalidOperationException($"Calificación con ID {id} no encontrada.");

        if (!calificacion.CanBeDeleted())
        {
            throw new InvalidOperationException("La calificación no puede ser eliminada en su estado actual.");
        }

        calificacion.MarkAsDeleted();
        await UpdateAsync(calificacion, cancellationToken);
    }

    #endregion

    #region Consultas Específicas de Negocio

    /// <summary>
    /// Obtiene todas las calificaciones de un estudiante en un período académico
    /// </summary>
    public async Task<IEnumerable<Calificacion>> GetByEstudianteYPeriodoAsync(Guid estudianteId, Guid periodoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.EstudianteId == estudianteId &&
                       c.PeriodoAcademicoId == periodoAcademicoId &&
                       c.ColegioId == colegioId &&
                       !c.IsDeleted)
            .Include(c => c.Asignacion)
                .ThenInclude(a => a.Materia)
            .Include(c => c.TipoEvaluacion)
            .OrderBy(c => c.FechaCalificacion)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene todas las calificaciones de una asignación específica
    /// </summary>
    public async Task<IEnumerable<Calificacion>> GetByAsignacionYPeriodoAsync(Guid asignacionId, Guid periodoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.AsignacionId == asignacionId &&
                       c.PeriodoAcademicoId == periodoAcademicoId &&
                       c.ColegioId == colegioId &&
                       !c.IsDeleted)
            .Include(c => c.Estudiante)
                .ThenInclude(e => e.Persona)
            .Include(c => c.TipoEvaluacion)
            .OrderBy(c => c.Estudiante.Persona.Apellidos)
                .ThenBy(c => c.Estudiante.Persona.Nombres)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene las calificaciones de un estudiante en una materia específica
    /// </summary>
    public async Task<IEnumerable<Calificacion>> GetByEstudianteMateriaYPeriodoAsync(Guid estudianteId, Guid materiaId, Guid periodoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.EstudianteId == estudianteId &&
                       c.Asignacion.MateriaId == materiaId &&
                       c.PeriodoAcademicoId == periodoAcademicoId &&
                       c.ColegioId == colegioId &&
                       !c.IsDeleted)
            .Include(c => c.TipoEvaluacion)
            .Include(c => c.Asignacion)
                .ThenInclude(a => a.Materia)
            .OrderBy(c => c.FechaCalificacion)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene las calificaciones registradas por un profesor específico
    /// </summary>
    public async Task<IEnumerable<Calificacion>> GetByProfesorAsync(Guid profesorId, Guid? periodoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(c => c.ProfesorId == profesorId &&
                       c.ColegioId == colegioId &&
                       !c.IsDeleted);

        if (periodoAcademicoId.HasValue)
        {
            query = query.Where(c => c.PeriodoAcademicoId == periodoAcademicoId.Value);
        }

        return await query
            .Include(c => c.Estudiante)
                .ThenInclude(e => e.Persona)
            .Include(c => c.Asignacion)
                .ThenInclude(a => a.Materia)
            .Include(c => c.TipoEvaluacion)
            .OrderByDescending(c => c.FechaCalificacion)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Verifica si existe una calificación para un estudiante en una evaluación específica
    /// </summary>
    public async Task<bool> ExisteCalificacionAsync(Guid estudianteId, Guid asignacionId, Guid tipoEvaluacionId, Guid periodoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(c => c.EstudianteId == estudianteId &&
                          c.AsignacionId == asignacionId &&
                          c.TipoEvaluacionId == tipoEvaluacionId &&
                          c.PeriodoAcademicoId == periodoAcademicoId &&
                          c.ColegioId == colegioId &&
                          !c.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Obtiene el promedio de calificaciones de un estudiante en una materia
    /// </summary>
    public async Task<decimal?> GetPromedioEstudianteMateriaAsync(Guid estudianteId, Guid materiaId, Guid periodoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default)
    {
        var calificaciones = await _dbSet
            .Where(c => c.EstudianteId == estudianteId &&
                       c.Asignacion.MateriaId == materiaId &&
                       c.PeriodoAcademicoId == periodoAcademicoId &&
                       c.ColegioId == colegioId &&
                       c.Estado == EstadoCalificacion.Publicada &&
                       !c.IsDeleted)
            .Select(c => c.CalificacionValor)
            .ToListAsync(cancellationToken);

        return calificaciones.Any() ? calificaciones.Average() : null;
    }

    /// <summary>
    /// Obtiene las calificaciones por rango de fechas
    /// </summary>
    public async Task<IEnumerable<Calificacion>> GetByRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin, Guid colegioId, Guid? profesorId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(c => c.FechaCalificacion >= fechaInicio &&
                       c.FechaCalificacion <= fechaFin &&
                       c.ColegioId == colegioId &&
                       !c.IsDeleted);

        if (profesorId.HasValue)
        {
            query = query.Where(c => c.ProfesorId == profesorId.Value);
        }

        return await query
            .Include(c => c.Estudiante)
                .ThenInclude(e => e.Persona)
            .Include(c => c.Asignacion)
                .ThenInclude(a => a.Materia)
            .Include(c => c.TipoEvaluacion)
            .OrderBy(c => c.FechaCalificacion)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene estadísticas de calificaciones por materia en un período
    /// </summary>
    public async Task<(decimal Promedio, decimal Maximo, decimal Minimo, int TotalEstudiantes)> GetEstadisticasMateriaAsync(Guid materiaId, Guid periodoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default)
    {
        var calificaciones = await _dbSet
            .Where(c => c.Asignacion.MateriaId == materiaId &&
                       c.PeriodoAcademicoId == periodoAcademicoId &&
                       c.ColegioId == colegioId &&
                       c.Estado == EstadoCalificacion.Publicada &&
                       !c.IsDeleted)
            .Select(c => c.CalificacionValor)
            .ToListAsync(cancellationToken);

        if (!calificaciones.Any())
        {
            return (0, 0, 0, 0);
        }

        var estudiantesUnicos = await _dbSet
            .Where(c => c.Asignacion.MateriaId == materiaId &&
                       c.PeriodoAcademicoId == periodoAcademicoId &&
                       c.ColegioId == colegioId &&
                       c.Estado == EstadoCalificacion.Publicada &&
                       !c.IsDeleted)
            .Select(c => c.EstudianteId)
            .Distinct()
            .CountAsync(cancellationToken);

        return (
            Promedio: calificaciones.Average(),
            Maximo: calificaciones.Max(),
            Minimo: calificaciones.Min(),
            TotalEstudiantes: estudiantesUnicos
        );
    }

    /// <summary>
    /// Obtiene las calificaciones pendientes de aprobación
    /// </summary>
    public async Task<IEnumerable<Calificacion>> GetCalificacionesPendientesAsync(Guid colegioId, Guid? profesorId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(c => c.ColegioId == colegioId &&
                       c.Estado == EstadoCalificacion.Borrador &&
                       !c.IsDeleted);

        if (profesorId.HasValue)
        {
            query = query.Where(c => c.ProfesorId == profesorId.Value);
        }

        return await query
            .Include(c => c.Estudiante)
                .ThenInclude(e => e.Persona)
            .Include(c => c.Asignacion)
                .ThenInclude(a => a.Materia)
            .Include(c => c.TipoEvaluacion)
            .OrderBy(c => c.FechaCalificacion)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Métodos de Paginación

    /// <summary>
    /// Obtiene calificaciones paginadas con filtros
    /// </summary>
    public async Task<(IEnumerable<Calificacion> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid colegioId,
        Guid? estudianteId = null,
        Guid? profesorId = null,
        Guid? materiaId = null,
        Guid? periodoAcademicoId = null,
        string[]? includeEntities = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(c => c.ColegioId == colegioId && !c.IsDeleted);

        // Aplicar filtros opcionales
        if (estudianteId.HasValue)
            query = query.Where(c => c.EstudianteId == estudianteId.Value);

        if (profesorId.HasValue)
            query = query.Where(c => c.ProfesorId == profesorId.Value);

        if (materiaId.HasValue)
            query = query.Where(c => c.Asignacion.MateriaId == materiaId.Value);

        if (periodoAcademicoId.HasValue)
            query = query.Where(c => c.PeriodoAcademicoId == periodoAcademicoId.Value);

        // Aplicar includes dinámicamente
        if (includeEntities?.Length > 0)
        {
            foreach (var include in includeEntities)
            {
                query = query.Include(include);
            }
        }

        // Contar total de registros antes de aplicar paginación
        var totalCount = await query.CountAsync(cancellationToken);

        // Aplicar paginación y ordenamiento
        var items = await query
            .OrderByDescending(c => c.FechaCalificacion)
                .ThenBy(c => c.Estudiante.Persona.Apellidos)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    #endregion
}
