using Microsoft.EntityFrameworkCore;
using ColegiosBackend.Core.Entities;
using ColegiosBackend.Core.Interfaces;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de Asistencia usando Entity Framework Core
/// </summary>
public class AsistenciaRepository : IAsistenciaRepository
{
    private readonly DbContext _context;
    private readonly DbSet<Asistencia> _asistencias;

    public AsistenciaRepository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _asistencias = _context.Set<Asistencia>();
    }

    #region CRUD Operations

    /// <summary>
    /// Obtiene una asistencia por ID con entidades relacionadas
    /// </summary>
    public async Task<Asistencia?> GetByIdAsync(Guid id, string[]? includeEntities = null, CancellationToken cancellationToken = default)
    {
        var query = _asistencias.AsQueryable();

        if (includeEntities?.Length > 0)
        {
            query = includeEntities.Aggregate(query, (current, include) => current.Include(include));
        }

        return await query.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Obtiene todas las asistencias con filtros opcionales
    /// </summary>
    public async Task<IEnumerable<Asistencia>> GetAllAsync(
        Guid? colegioId = null,
        bool includeInactive = false,
        string[]? includeEntities = null,
        CancellationToken cancellationToken = default)
    {
        var query = _asistencias.AsQueryable();

        // Aplicar filtros
        query = query.Where(a => !a.IsDeleted);

        if (colegioId.HasValue)
            query = query.Where(a => a.ColegioId == colegioId);

        if (!includeInactive)
            query = query.Where(a => a.Activo);

        // Incluir entidades relacionadas
        if (includeEntities?.Length > 0)
        {
            query = includeEntities.Aggregate(query, (current, include) => current.Include(include));
        }

        return await query.OrderBy(a => a.FechaClase).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Crea una nueva asistencia
    /// </summary>
    public async Task<Asistencia> CreateAsync(Asistencia asistencia, CancellationToken cancellationToken = default)
    {
        if (asistencia == null)
            throw new ArgumentNullException(nameof(asistencia));

        // Validar duplicados
        var existeAsistencia = await _asistencias
            .AnyAsync(a => a.EstudianteId == asistencia.EstudianteId
                          && a.MateriaId == asistencia.MateriaId
                          && a.GrupoId == asistencia.GrupoId
                          && a.FechaClase.Date == asistencia.FechaClase.Date
                          && !a.IsDeleted, cancellationToken);

        if (existeAsistencia)
            throw new InvalidOperationException("Ya existe un registro de asistencia para este estudiante, materia y fecha");

        _asistencias.Add(asistencia);
        await _context.SaveChangesAsync(cancellationToken);
        return asistencia;
    }

    /// <summary>
    /// Actualiza una asistencia existente
    /// </summary>
    public async Task<Asistencia> UpdateAsync(Asistencia asistencia, CancellationToken cancellationToken = default)
    {
        if (asistencia == null)
            throw new ArgumentNullException(nameof(asistencia));

        var existingAsistencia = await _asistencias
            .FirstOrDefaultAsync(a => a.Id == asistencia.Id && !a.IsDeleted, cancellationToken);

        if (existingAsistencia == null)
            throw new InvalidOperationException("La asistencia no existe o fue eliminada");

        _context.Entry(existingAsistencia).CurrentValues.SetValues(asistencia);
        await _context.SaveChangesAsync(cancellationToken);
        return existingAsistencia;
    }

    /// <summary>
    /// Elimina lógicamente una asistencia
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var asistencia = await _asistencias
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, cancellationToken);

        if (asistencia == null)
            return false;

        if (!asistencia.CanBeDeleted())
            throw new InvalidOperationException("Esta asistencia no puede ser eliminada según las reglas de negocio");

        asistencia.MarkAsDeleted();
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    #endregion

    #region Consultas Específicas de Asistencia

    /// <summary>
    /// Obtiene asistencias por estudiante
    /// </summary>
    public async Task<IEnumerable<Asistencia>> GetByEstudianteAsync(
        Guid estudianteId,
        Guid? colegioId = null,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        string[]? includeEntities = null,
        CancellationToken cancellationToken = default)
    {
        var query = _asistencias.AsQueryable();

        query = query.Where(a => a.EstudianteId == estudianteId && !a.IsDeleted && a.Activo);

        if (colegioId.HasValue)
            query = query.Where(a => a.ColegioId == colegioId);

        if (fechaInicio.HasValue)
            query = query.Where(a => a.FechaClase >= fechaInicio.Value.Date);

        if (fechaFin.HasValue)
            query = query.Where(a => a.FechaClase <= fechaFin.Value.Date);

        if (includeEntities?.Length > 0)
            query = includeEntities.Aggregate(query, (current, include) => current.Include(include));

        return await query.OrderBy(a => a.FechaClase).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene asistencias por materia y grupo
    /// </summary>
    public async Task<IEnumerable<Asistencia>> GetByMateriaGrupoAsync(
        Guid materiaId,
        Guid grupoId,
        Guid? colegioId = null,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        string[]? includeEntities = null,
        CancellationToken cancellationToken = default)
    {
        var query = _asistencias.AsQueryable();

        query = query.Where(a => a.MateriaId == materiaId
                               && a.GrupoId == grupoId
                               && !a.IsDeleted
                               && a.Activo);

        if (colegioId.HasValue)
            query = query.Where(a => a.ColegioId == colegioId);

        if (fechaInicio.HasValue)
            query = query.Where(a => a.FechaClase >= fechaInicio.Value.Date);

        if (fechaFin.HasValue)
            query = query.Where(a => a.FechaClase <= fechaFin.Value.Date);

        if (includeEntities?.Length > 0)
            query = includeEntities.Aggregate(query, (current, include) => current.Include(include));

        return await query
            .OrderBy(a => a.FechaClase)
            .ThenBy(a => a.Estudiante!.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene asistencias por período evaluativo
    /// </summary>
    public async Task<IEnumerable<Asistencia>> GetByPeriodoEvaluativoAsync(
        Guid periodoEvaluativoId,
        Guid? colegioId = null,
        Guid? estudianteId = null,
        Guid? materiaId = null,
        string[]? includeEntities = null,
        CancellationToken cancellationToken = default)
    {
        var query = _asistencias.AsQueryable();

        query = query.Where(a => a.PeriodoEvaluativoId == periodoEvaluativoId && !a.IsDeleted && a.Activo);

        if (colegioId.HasValue)
            query = query.Where(a => a.ColegioId == colegioId);

        if (estudianteId.HasValue)
            query = query.Where(a => a.EstudianteId == estudianteId);

        if (materiaId.HasValue)
            query = query.Where(a => a.MateriaId == materiaId);

        if (includeEntities?.Length > 0)
            query = includeEntities.Aggregate(query, (current, include) => current.Include(include));

        return await query.OrderBy(a => a.FechaClase).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene asistencias por fecha específica
    /// </summary>
    public async Task<IEnumerable<Asistencia>> GetByFechaAsync(
        DateTime fecha,
        Guid? colegioId = null,
        Guid? grupoId = null,
        Guid? materiaId = null,
        string[]? includeEntities = null,
        CancellationToken cancellationToken = default)
    {
        var query = _asistencias.AsQueryable();

        query = query.Where(a => a.FechaClase.Date == fecha.Date && !a.IsDeleted && a.Activo);

        if (colegioId.HasValue)
            query = query.Where(a => a.ColegioId == colegioId);

        if (grupoId.HasValue)
            query = query.Where(a => a.GrupoId == grupoId);

        if (materiaId.HasValue)
            query = query.Where(a => a.MateriaId == materiaId);

        if (includeEntities?.Length > 0)
            query = includeEntities.Aggregate(query, (current, include) => current.Include(include));

        return await query
            .OrderBy(a => a.Grupo!.Nombre)
            .ThenBy(a => a.Materia!.Nombre)
            .ThenBy(a => a.Estudiante!.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Estadísticas y Reportes

    /// <summary>
    /// Obtiene estadísticas de asistencia por estudiante
    /// </summary>
    public async Task<Dictionary<string, int>> GetEstadisticasEstudianteAsync(
        Guid estudianteId,
        Guid? colegioId = null,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        Guid? materiaId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _asistencias.AsQueryable();

        query = query.Where(a => a.EstudianteId == estudianteId && !a.IsDeleted && a.Activo);

        if (colegioId.HasValue)
            query = query.Where(a => a.ColegioId == colegioId);

        if (fechaInicio.HasValue)
            query = query.Where(a => a.FechaClase >= fechaInicio.Value.Date);

        if (fechaFin.HasValue)
            query = query.Where(a => a.FechaClase <= fechaFin.Value.Date);

        if (materiaId.HasValue)
            query = query.Where(a => a.MateriaId == materiaId);

        var asistencias = await query.ToListAsync(cancellationToken);

        return new Dictionary<string, int>
        {
            ["Total"] = asistencias.Count,
            ["Presentes"] = asistencias.Count(a => a.Estado == EstadoAsistencia.Presente),
            ["Ausentes"] = asistencias.Count(a => a.Estado == EstadoAsistencia.Ausente),
            ["Tardanzas"] = asistencias.Count(a => a.Estado == EstadoAsistencia.Tardanza),
            ["AusentesPorSuspension"] = asistencias.Count(a => a.Estado == EstadoAsistencia.AusentePorSuspension),
            ["AusenciasJustificadas"] = asistencias.Count(a => a.Estado == EstadoAsistencia.Ausente && a.EsJustificada),
            ["TardanzasJustificadas"] = asistencias.Count(a => a.Estado == EstadoAsistencia.Tardanza && a.EsJustificada),
            ["AusenciasInjustificadas"] = asistencias.Count(a => a.EsAusenciaInjustificada()),
            ["TardanzasInjustificadas"] = asistencias.Count(a => a.EsTardanzaInjustificada())
        };
    }

    /// <summary>
    /// Obtiene estadísticas de asistencia por materia y grupo
    /// </summary>
    public async Task<Dictionary<string, object>> GetEstadisticasMateriaGrupoAsync(
        Guid materiaId,
        Guid grupoId,
        Guid? colegioId = null,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        CancellationToken cancellationToken = default)
    {
        var query = _asistencias.AsQueryable();

        query = query.Where(a => a.MateriaId == materiaId
                               && a.GrupoId == grupoId
                               && !a.IsDeleted
                               && a.Activo);

        if (colegioId.HasValue)
            query = query.Where(a => a.ColegioId == colegioId);

        if (fechaInicio.HasValue)
            query = query.Where(a => a.FechaClase >= fechaInicio.Value.Date);

        if (fechaFin.HasValue)
            query = query.Where(a => a.FechaClase <= fechaFin.Value.Date);

        var asistencias = await query.ToListAsync(cancellationToken);
        var totalClases = asistencias.Select(a => a.FechaClase.Date).Distinct().Count();
        var totalEstudiantes = asistencias.Select(a => a.EstudianteId).Distinct().Count();

        return new Dictionary<string, object>
        {
            ["TotalRegistros"] = asistencias.Count,
            ["TotalClases"] = totalClases,
            ["TotalEstudiantes"] = totalEstudiantes,
            ["PorcentajeAsistencia"] = asistencias.Count > 0
                ? Math.Round((double)asistencias.Count(a => a.Estado == EstadoAsistencia.Presente) / asistencias.Count * 100, 2)
                : 0,
            ["EstadisticasPorEstado"] = asistencias.GroupBy(a => a.Estado)
                .ToDictionary(g => g.Key.ToString(), g => g.Count()),
            ["ClasesConMayorAusentismo"] = asistencias
                .GroupBy(a => a.FechaClase.Date)
                .Select(g => new {
                    Fecha = g.Key,
                    Ausentes = g.Count(a => a.Estado != EstadoAsistencia.Presente),
                    Total = g.Count()
                })
                .Where(x => x.Total > 0)
                .OrderByDescending(x => (double)x.Ausentes / x.Total)
                .Take(5)
                .ToList()
        };
    }

    /// <summary>
    /// Obtiene reporte de ausentismo por período
    /// </summary>
    public async Task<IEnumerable<object>> GetReporteAusentismoAsync(
        Guid periodoEvaluativoId,
        Guid? colegioId = null,
        Guid? grupoId = null,
        int? minimoAusencias = null,
        CancellationToken cancellationToken = default)
    {
        var query = _asistencias
            .Include(a => a.Estudiante)
                .ThenInclude(e => e!.Persona)
            .Include(a => a.Materia)
            .AsQueryable();

        query = query.Where(a => a.PeriodoEvaluativoId == periodoEvaluativoId
                               && !a.IsDeleted
                               && a.Activo);

        if (colegioId.HasValue)
            query = query.Where(a => a.ColegioId == colegioId);

        if (grupoId.HasValue)
            query = query.Where(a => a.GrupoId == grupoId);

        var asistencias = await query.ToListAsync(cancellationToken);

        var reporteAusentismo = asistencias
            .GroupBy(a => new { a.EstudianteId, a.Estudiante })
            .Select(g => new
            {
                EstudianteId = g.Key.EstudianteId,
                NombreCompleto = $"{g.Key.Estudiante?.Persona?.NombreCompleto}",
                CodigoEstudiante = g.Key.Estudiante?.CodigoEstudiante,
                TotalClases = g.Count(),
                TotalAusencias = g.Count(a => a.Estado == EstadoAsistencia.Ausente),
                AusenciasJustificadas = g.Count(a => a.Estado == EstadoAsistencia.Ausente && a.EsJustificada),
                AusenciasInjustificadas = g.Count(a => a.EsAusenciaInjustificada()),
                TotalTardanzas = g.Count(a => a.Estado == EstadoAsistencia.Tardanza),
                TardanzasJustificadas = g.Count(a => a.Estado == EstadoAsistencia.Tardanza && a.EsJustificada),
                TardanzasInjustificadas = g.Count(a => a.EsTardanzaInjustificada()),
                PorcentajeAsistencia = g.Count() > 0
                    ? Math.Round((double)g.Count(a => a.Estado == EstadoAsistencia.Presente) / g.Count() * 100, 2)
                    : 0,
                MateriasConMasAusencias = g.GroupBy(a => a.Materia)
                    .Where(mg => mg.Count(a => a.Estado == EstadoAsistencia.Ausente) > 0)
                    .OrderByDescending(mg => mg.Count(a => a.Estado == EstadoAsistencia.Ausente))
                    .Take(3)
                    .Select(mg => new {
                        NombreMateria = mg.Key?.Nombre,
                        Ausencias = mg.Count(a => a.Estado == EstadoAsistencia.Ausente)
                    })
                    .ToList()
            })
            .Where(r => !minimoAusencias.HasValue || r.TotalAusencias >= minimoAusencias.Value)
            .OrderByDescending(r => r.TotalAusencias)
            .ToList();

        return reporteAusentismo;
    }

    #endregion

    #region Métodos de Justificación

    /// <summary>
    /// Justifica múltiples ausencias o tardanzas
    /// </summary>
    public async Task<int> JustificarAsistenciasAsync(
        List<Guid> asistenciaIds,
        string motivoJustificacion,
        Guid justificadoPorId,
        CancellationToken cancellationToken = default)
    {
        if (!asistenciaIds?.Any() == true)
            throw new ArgumentException("Debe proporcionar al menos un ID de asistencia");

        if (string.IsNullOrWhiteSpace(motivoJustificacion))
            throw new ArgumentException("El motivo de justificación es requerido");

        var asistencias = await _asistencias
            .Where(a => asistenciaIds.Contains(a.Id) && !a.IsDeleted && a.Activo)
            .ToListAsync(cancellationToken);

        int justificadas = 0;
        foreach (var asistencia in asistencias)
        {
            try
            {
                asistencia.Justificar(motivoJustificacion, justificadoPorId);
                justificadas++;
            }
            catch (InvalidOperationException)
            {
                // Saltar asistencias que no se pueden justificar (ej: presentes)
                continue;
            }
        }

        if (justificadas > 0)
            await _context.SaveChangesAsync(cancellationToken);

        return justificadas;
    }

    /// <summary>
    /// Obtiene asistencias pendientes de justificación
    /// </summary>
    public async Task<IEnumerable<Asistencia>> GetAsistenciasPendientesJustificacionAsync(
        Guid? colegioId = null,
        Guid? estudianteId = null,
        Guid? grupoId = null,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        string[]? includeEntities = null,
        CancellationToken cancellationToken = default)
    {
        var query = _asistencias.AsQueryable();

        query = query.Where(a => (a.Estado == EstadoAsistencia.Ausente || a.Estado == EstadoAsistencia.Tardanza)
                               && !a.EsJustificada
                               && !a.IsDeleted
                               && a.Activo);

        if (colegioId.HasValue)
            query = query.Where(a => a.ColegioId == colegioId);

        if (estudianteId.HasValue)
            query = query.Where(a => a.EstudianteId == estudianteId);

        if (grupoId.HasValue)
            query = query.Where(a => a.GrupoId == grupoId);

        if (fechaInicio.HasValue)
            query = query.Where(a => a.FechaClase >= fechaInicio.Value.Date);

        if (fechaFin.HasValue)
            query = query.Where(a => a.FechaClase <= fechaFin.Value.Date);

        if (includeEntities?.Length > 0)
            query = includeEntities.Aggregate(query, (current, include) => current.Include(include));

        return await query
            .OrderBy(a => a.FechaClase)
            .ThenBy(a => a.Estudiante!.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Paginación

    /// <summary>
    /// Obtiene asistencias con paginación
    /// </summary>
    public async Task<(IEnumerable<Asistencia> Items, int TotalCount)> GetPagedAsync(
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
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 50;
        if (pageSize > 100) pageSize = 100;

        var query = _asistencias.AsQueryable();

        // Aplicar filtros
        query = query.Where(a => !a.IsDeleted && a.Activo);

        if (colegioId.HasValue)
            query = query.Where(a => a.ColegioId == colegioId);

        if (estudianteId.HasValue)
            query = query.Where(a => a.EstudianteId == estudianteId);

        if (grupoId.HasValue)
            query = query.Where(a => a.GrupoId == grupoId);

        if (materiaId.HasValue)
            query = query.Where(a => a.MateriaId == materiaId);

        if (fechaInicio.HasValue)
            query = query.Where(a => a.FechaClase >= fechaInicio.Value.Date);

        if (fechaFin.HasValue)
            query = query.Where(a => a.FechaClase <= fechaFin.Value.Date);

        if (estado.HasValue)
            query = query.Where(a => a.Estado == estado.Value);

        if (soloInjustificadas == true)
        {
            query = query.Where(a => (a.Estado == EstadoAsistencia.Ausente || a.Estado == EstadoAsistencia.Tardanza)
                                   && !a.EsJustificada);
        }

        // Obtener total de registros
        var totalCount = await query.CountAsync(cancellationToken);

        // Aplicar includes
        if (includeEntities?.Length > 0)
            query = includeEntities.Aggregate(query, (current, include) => current.Include(include));

        // Aplicar paginación
        var items = await query
            .OrderByDescending(a => a.FechaClase)
            .ThenBy(a => a.Estudiante!.Persona!.Nombres)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    #endregion

    #region Validaciones y Verificaciones

    /// <summary>
    /// Verifica si existe asistencia para estudiante en fecha y materia específica
    /// </summary>
    public async Task<bool> ExisteAsistenciaAsync(
        Guid estudianteId,
        Guid materiaId,
        Guid grupoId,
        DateTime fechaClase,
        CancellationToken cancellationToken = default)
    {
        return await _asistencias.AnyAsync(
            a => a.EstudianteId == estudianteId
                && a.MateriaId == materiaId
                && a.GrupoId == grupoId
                && a.FechaClase.Date == fechaClase.Date
                && !a.IsDeleted,
            cancellationToken);
    }

    /// <summary>
    /// Obtiene conflictos de asistencia para validación
    /// </summary>
    public async Task<IEnumerable<string>> ValidarConflictosAsync(
        Guid estudianteId,
        DateTime fechaClase,
        Guid? excludeAsistenciaId = null,
        CancellationToken cancellationToken = default)
    {
        var conflictos = new List<string>();

        var asistenciasExistentes = await _asistencias
            .Include(a => a.Materia)
            .Where(a => a.EstudianteId == estudianteId
                       && a.FechaClase.Date == fechaClase.Date
                       && !a.IsDeleted
                       && (excludeAsistenciaId == null || a.Id != excludeAsistenciaId))
            .ToListAsync(cancellationToken);

        foreach (var asistencia in asistenciasExistentes)
        {
            conflictos.Add($"Ya existe asistencia para la materia {asistencia.Materia?.Nombre} en la fecha {fechaClase:dd/MM/yyyy}");
        }

        return conflictos;
    }

    #endregion
}