using ColegiosBackend.Core.Entities;
using ColegiosBackend.Core.Enums;
using ColegiosBackend.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ColegiosBackend.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio para la gestión de materias/asignaturas académicas
/// </summary>
public class MateriaRepository : IMateriaRepository
{
    private readonly DbContext _context;
    private readonly DbSet<Materia> _dbSet;

    public MateriaRepository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<Materia>();
    }

    #region Métodos CRUD Básicos

    /// <summary>
    /// Obtiene una materia por su ID
    /// </summary>
    public async Task<Materia?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Obtiene una materia por su ID incluyendo entidades relacionadas
    /// </summary>
    public async Task<Materia?> GetByIdWithIncludesAsync(Guid id, string[] includeEntities, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        // Agregar includes dinámicamente
        foreach (var include in includeEntities)
        {
            query = query.Include(include);
        }

        return await query
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Agrega una nueva materia
    /// </summary>
    public async Task AddAsync(Materia materia, CancellationToken cancellationToken = default)
    {
        if (materia == null)
            throw new ArgumentNullException(nameof(materia));

        // Validar duplicados
        var existeDuplicado = await ExisteDuplicadoAsync(
            materia.CodigoMateria,
            materia.ColegioId!.Value,
            null,
            cancellationToken);

        if (existeDuplicado)
            throw new InvalidOperationException($"Ya existe una materia con código '{materia.CodigoMateria}' en el colegio especificado.");

        await _dbSet.AddAsync(materia, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Actualiza una materia existente
    /// </summary>
    public async Task UpdateAsync(Materia materia, CancellationToken cancellationToken = default)
    {
        if (materia == null)
            throw new ArgumentNullException(nameof(materia));

        // Validar duplicados excluyendo la materia actual
        var existeDuplicado = await ExisteDuplicadoAsync(
            materia.CodigoMateria,
            materia.ColegioId!.Value,
            materia.Id,
            cancellationToken);

        if (existeDuplicado)
            throw new InvalidOperationException($"Ya existe otra materia con código '{materia.CodigoMateria}' en el colegio especificado.");

        _dbSet.Update(materia);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Elimina una materia (soft delete)
    /// </summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var materia = await GetByIdAsync(id, cancellationToken);
        if (materia == null)
            throw new InvalidOperationException("La materia no existe.");

        if (!await CanBeDeletedAsync(id, cancellationToken))
            throw new InvalidOperationException("La materia no puede ser eliminada porque tiene asignaciones, horarios o calificaciones asociadas.");

        materia.MarkAsDeleted();
        await _context.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Consultas Específicas de Negocio

    /// <summary>
    /// Obtiene una materia por su código dentro del colegio
    /// </summary>
    public async Task<Materia?> GetByCodigoAsync(string codigoMateria, Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(m =>
                m.CodigoMateria == codigoMateria &&
                m.ColegioId == colegioId &&
                !m.IsDeleted,
                cancellationToken);
    }

    /// <summary>
    /// Obtiene todas las materias de un área académica específica
    /// </summary>
    public async Task<IEnumerable<Materia>> GetByAreaAcademicaAsync(AreaAcademica area, Guid colegioId, bool soloActivas = true, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(m =>
                m.Area == area &&
                m.ColegioId == colegioId &&
                !m.IsDeleted);

        if (soloActivas)
            query = query.Where(m => m.Estado == EstadoMateria.Activa);

        return await query
            .OrderBy(m => m.OrdenPresentacion)
            .ThenBy(m => m.Nombre)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene materias por nivel educativo permitido
    /// </summary>
    public async Task<IEnumerable<Materia>> GetByNivelEducativoAsync(NivelesEducativos nivel, Guid colegioId, bool soloActivas = true, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(m =>
                (m.NivelesPermitidos & nivel) == nivel &&
                m.ColegioId == colegioId &&
                !m.IsDeleted);

        if (soloActivas)
            query = query.Where(m => m.Estado == EstadoMateria.Activa);

        return await query
            .OrderBy(m => m.Area)
            .ThenBy(m => m.OrdenPresentacion)
            .ThenBy(m => m.Nombre)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene materias obligatorias o electivas
    /// </summary>
    public async Task<IEnumerable<Materia>> GetByTipoObligatoriaAsync(bool esObligatoria, Guid colegioId, AreaAcademica? area = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(m =>
                m.EsObligatoria == esObligatoria &&
                m.ColegioId == colegioId &&
                m.Estado == EstadoMateria.Activa &&
                !m.IsDeleted);

        if (area.HasValue)
            query = query.Where(m => m.Area == area);

        return await query
            .OrderBy(m => m.Area)
            .ThenBy(m => m.OrdenPresentacion)
            .ThenBy(m => m.Nombre)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene materias prácticas (laboratorios, talleres)
    /// </summary>
    public async Task<IEnumerable<Materia>> GetMateriasPracticasAsync(Guid colegioId, AreaAcademica? area = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(m =>
                m.EsPractica &&
                m.ColegioId == colegioId &&
                m.Estado == EstadoMateria.Activa &&
                !m.IsDeleted);

        if (area.HasValue)
            query = query.Where(m => m.Area == area);

        return await query
            .OrderBy(m => m.Area)
            .ThenBy(m => m.Nombre)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene materias que requieren materiales especiales
    /// </summary>
    public async Task<IEnumerable<Materia>> GetConMaterialesEspecialesAsync(Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m =>
                m.RequiereMateriales &&
                m.ColegioId == colegioId &&
                m.Estado == EstadoMateria.Activa &&
                !m.IsDeleted)
            .OrderBy(m => m.Area)
            .ThenBy(m => m.Nombre)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene todas las materias de un colegio con filtros opcionales
    /// </summary>
    public async Task<IEnumerable<Materia>> GetAllAsync(
        Guid colegioId,
        EstadoMateria? estado = null,
        AreaAcademica? area = null,
        NivelesEducativos? nivel = null,
        bool soloActivas = true,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(m =>
                m.ColegioId == colegioId &&
                !m.IsDeleted);

        if (estado.HasValue)
            query = query.Where(m => m.Estado == estado);
        else if (soloActivas)
            query = query.Where(m => m.Estado == EstadoMateria.Activa);

        if (area.HasValue)
            query = query.Where(m => m.Area == area);

        if (nivel.HasValue)
            query = query.Where(m => (m.NivelesPermitidos & nivel) == nivel);

        return await query
            .OrderBy(m => m.Area)
            .ThenBy(m => m.OrdenPresentacion)
            .ThenBy(m => m.Nombre)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca materias por término de búsqueda (nombre, código, descripción)
    /// </summary>
    public async Task<IEnumerable<Materia>> SearchAsync(string searchTerm, Guid colegioId, AreaAcademica? area = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync(colegioId, area: area, cancellationToken: cancellationToken);

        searchTerm = searchTerm.ToLower();

        var query = _dbSet
            .Where(m =>
                m.ColegioId == colegioId &&
                m.Estado == EstadoMateria.Activa &&
                !m.IsDeleted &&
                (m.Nombre.ToLower().Contains(searchTerm) ||
                 m.CodigoMateria.ToLower().Contains(searchTerm) ||
                 m.NombreCorto.ToLower().Contains(searchTerm) ||
                 (m.Descripcion != null && m.Descripcion.ToLower().Contains(searchTerm))));

        if (area.HasValue)
            query = query.Where(m => m.Area == area);

        return await query
            .OrderBy(m => m.Area)
            .ThenBy(m => m.OrdenPresentacion)
            .ThenBy(m => m.Nombre)
            .ToListAsync(cancellationToken);
    }

    #endregion
    #region Consultas con Paginación

    /// <summary>
    /// Obtiene materias paginadas con filtros
    /// </summary>
    public async Task<(IEnumerable<Materia> Items, int TotalCount)> GetPagedAsync(
        Guid colegioId,
        int pageNumber,
        int pageSize,
        AreaAcademica? area = null,
        EstadoMateria? estado = null,
        string? searchTerm = null,
        bool soloActivas = true,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(m =>
                m.ColegioId == colegioId &&
                !m.IsDeleted);

        if (area.HasValue)
            query = query.Where(m => m.Area == area);

        if (estado.HasValue)
            query = query.Where(m => m.Estado == estado);
        else if (soloActivas)
            query = query.Where(m => m.Estado == EstadoMateria.Activa);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(m =>
                m.Nombre.ToLower().Contains(searchTerm) ||
                m.CodigoMateria.ToLower().Contains(searchTerm) ||
                m.NombreCorto.ToLower().Contains(searchTerm) ||
                (m.Descripcion != null && m.Descripcion.ToLower().Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(m => m.Area)
            .ThenBy(m => m.OrdenPresentacion)
            .ThenBy(m => m.Nombre)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    #endregion

    #region Validaciones de Negocio

    /// <summary>
    /// Verifica si existe una materia con el mismo código
    /// </summary>
    public async Task<bool> ExisteDuplicadoAsync(string codigoMateria, Guid colegioId, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(m =>
                m.CodigoMateria == codigoMateria &&
                m.ColegioId == colegioId &&
                !m.IsDeleted);

        if (excludeId.HasValue)
            query = query.Where(m => m.Id != excludeId);

        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Verifica si una materia puede ser eliminada
    /// </summary>
    public async Task<bool> CanBeDeletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Verificar si tiene asignaciones de profesores
        var tieneAsignaciones = await _context.Set<Asignacion>()
            .AnyAsync(a => a.MateriaId == id && a.Estado == EstadoAsignacion.Activa, cancellationToken);

        if (tieneAsignaciones)
            return false;

        // Verificar si tiene horarios programados
        var tieneHorarios = await _context.Set<Horario>()
            .AnyAsync(h => h.MateriaId == id && h.Activo, cancellationToken);

        if (tieneHorarios)
            return false;

        // Verificar si tiene calificaciones registradas (a través de Asignacion)
        var tieneCalificaciones = await _context.Set<Calificacion>()
            .Include(c => c.Asignacion)
            .AnyAsync(c => c.Asignacion!.MateriaId == id, cancellationToken);

        if (tieneCalificaciones)
            return false;

        // Verificar si tiene asistencias registradas
        var tieneAsistencias = await _context.Set<Asistencia>()
            .AnyAsync(a => a.MateriaId == id, cancellationToken);

        return !tieneAsistencias;
    }

    /// <summary>
    /// Obtiene el número de profesores asignados a una materia
    /// </summary>
    public async Task<int> GetConteoProfesoresAsignadosAsync(Guid materiaId, Guid? anoAcademicoId = null, bool soloActivos = true, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Asignacion>()
            .Where(a => a.MateriaId == materiaId);

        if (anoAcademicoId.HasValue)
            query = query.Where(a => a.AnoAcademicoId == anoAcademicoId);

        if (soloActivos)
            query = query.Where(a => a.Estado == EstadoAsignacion.Activa);

        return await query.CountAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene el número de grupos donde se enseña una materia
    /// </summary>
    public async Task<int> GetConteoGruposAsync(Guid materiaId, Guid? anoAcademicoId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Asignacion>()
            .Where(a => a.MateriaId == materiaId && a.Estado == EstadoAsignacion.Activa);

        if (anoAcademicoId.HasValue)
            query = query.Where(a => a.AnoAcademicoId == anoAcademicoId);

        return await query
            .Select(a => a.GrupoId)
            .Distinct()
            .CountAsync(cancellationToken);
    }

    #endregion

    #region Consultas de Reportes y Estadísticas

    /// <summary>
    /// Obtiene el conteo de materias por área académica
    /// </summary>
    public async Task<Dictionary<AreaAcademica, int>> GetConteoByAreaAsync(Guid colegioId, bool soloActivas = true, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(m =>
                m.ColegioId == colegioId &&
                !m.IsDeleted);

        if (soloActivas)
            query = query.Where(m => m.Estado == EstadoMateria.Activa);

        return await query
            .GroupBy(m => m.Area)
            .Select(group => new { Area = group.Key, Conteo = group.Count() })
            .ToDictionaryAsync(x => x.Area, x => x.Conteo, cancellationToken);
    }

    /// <summary>
    /// Obtiene el conteo de materias por estado
    /// </summary>
    public async Task<Dictionary<EstadoMateria, int>> GetConteoByEstadoAsync(Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m =>
                m.ColegioId == colegioId &&
                !m.IsDeleted)
            .GroupBy(m => m.Estado)
            .Select(group => new { Estado = group.Key, Conteo = group.Count() })
            .ToDictionaryAsync(x => x.Estado, x => x.Conteo, cancellationToken);
    }

    /// <summary>
    /// Obtiene el conteo de materias por tipo (obligatoria/electiva)
    /// </summary>
    public async Task<Dictionary<string, int>> GetConteoByTipoAsync(Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m =>
                m.ColegioId == colegioId &&
                m.Estado == EstadoMateria.Activa &&
                !m.IsDeleted)
            .GroupBy(m => m.EsObligatoria)
            .Select(group => new {
                Tipo = group.Key ? "Obligatoria" : "Electiva",
                Conteo = group.Count()
            })
            .ToDictionaryAsync(x => x.Tipo, x => x.Conteo, cancellationToken);
    }

    /// <summary>
    /// Obtiene estadísticas de intensidad horaria por área
    /// </summary>
    public async Task<IEnumerable<(AreaAcademica Area, int TotalMaterias, int TotalHorasSemanales, decimal PromedioHoras)>> GetEstadisticasIntensidadHorariaAsync(
        Guid colegioId,
        CancellationToken cancellationToken = default)
    {
        var query = from m in _dbSet
                    where m.ColegioId == colegioId &&
                          m.Estado == EstadoMateria.Activa &&
                          !m.IsDeleted
                    group m by m.Area into g
                    select new
                    {
                        Area = g.Key,
                        TotalMaterias = g.Count(),
                        TotalHorasSemanales = g.Sum(x => x.IntensidadHorariaSemanal),
                        PromedioHoras = g.Average(x => (decimal)x.IntensidadHorariaSemanal)
                    };

        var resultado = await query.ToListAsync(cancellationToken);

        return resultado.Select(x => (
            x.Area,
            x.TotalMaterias,
            x.TotalHorasSemanales,
            Math.Round(x.PromedioHoras, 2)
        ));
    }

    /// <summary>
    /// Obtiene materias sin asignaciones en un año académico
    /// </summary>
    public async Task<IEnumerable<Materia>> GetSinAsignacionesAsync(Guid colegioId, Guid anoAcademicoId, CancellationToken cancellationToken = default)
    {
        var materiasConAsignacion = _context.Set<Asignacion>()
            .Where(a => a.ColegioId == colegioId &&
                       a.AnoAcademicoId == anoAcademicoId &&
                       a.Estado == EstadoAsignacion.Activa)
            .Select(a => a.MateriaId);

        return await _dbSet
            .Where(m =>
                m.ColegioId == colegioId &&
                m.Estado == EstadoMateria.Activa &&
                !m.IsDeleted &&
                !materiasConAsignacion.Contains(m.Id))
            .OrderBy(m => m.Area)
            .ThenBy(m => m.Nombre)
            .ToListAsync(cancellationToken);
    }

    #endregion
}