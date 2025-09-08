using ColegiosBackend.Core.Entities;
using ColegiosBackend.Core.Enums;
using ColegiosBackend.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ColegiosBackend.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio para la gestión de profesores
/// </summary>
public class ProfesorRepository : IProfesorRepository
{
    private readonly DbContext _context;
    private readonly DbSet<Profesor> _dbSet;

    public ProfesorRepository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<Profesor>();
    }

    #region Métodos CRUD Básicos

    /// <summary>
    /// Obtiene un profesor por su ID
    /// </summary>
    public async Task<Profesor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Obtiene un profesor por su ID incluyendo entidades relacionadas
    /// </summary>
    public async Task<Profesor?> GetByIdWithIncludesAsync(Guid id, string[] includeEntities, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        // Agregar includes dinámicamente
        foreach (var include in includeEntities)
        {
            query = query.Include(include);
        }

        return await query
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Agrega un nuevo profesor
    /// </summary>
    public async Task AddAsync(Profesor profesor, CancellationToken cancellationToken = default)
    {
        if (profesor == null)
            throw new ArgumentNullException(nameof(profesor));

        // Validar duplicados por código
        var existeDuplicadoCodigo = await ExisteDuplicadoAsync(
            profesor.CodigoProfesor,
            profesor.ColegioId!.Value,
            null,
            cancellationToken);

        if (existeDuplicadoCodigo)
            throw new InvalidOperationException($"Ya existe un profesor con código '{profesor.CodigoProfesor}' en el colegio especificado.");

        // Validar que la persona no sea ya profesor en el colegio
        var personaYaEsProfesor = await PersonaYaEsProfesorAsync(
            profesor.PersonaId,
            profesor.ColegioId!.Value,
            null,
            cancellationToken);

        if (personaYaEsProfesor)
            throw new InvalidOperationException("Esta persona ya es profesor en el colegio especificado.");

        await _dbSet.AddAsync(profesor, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Actualiza un profesor existente
    /// </summary>
    public async Task UpdateAsync(Profesor profesor, CancellationToken cancellationToken = default)
    {
        if (profesor == null)
            throw new ArgumentNullException(nameof(profesor));

        // Validar duplicados excluyendo el profesor actual
        var existeDuplicadoCodigo = await ExisteDuplicadoAsync(
            profesor.CodigoProfesor,
            profesor.ColegioId!.Value,
            profesor.Id,
            cancellationToken);

        if (existeDuplicadoCodigo)
            throw new InvalidOperationException($"Ya existe otro profesor con código '{profesor.CodigoProfesor}' en el colegio especificado.");

        // Validar que la persona no sea ya profesor en el colegio (excluyendo actual)
        var personaYaEsProfesor = await PersonaYaEsProfesorAsync(
            profesor.PersonaId,
            profesor.ColegioId!.Value,
            profesor.Id,
            cancellationToken);

        if (personaYaEsProfesor)
            throw new InvalidOperationException("Esta persona ya es profesor en el colegio especificado.");

        _dbSet.Update(profesor);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Elimina un profesor (soft delete)
    /// </summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var profesor = await GetByIdAsync(id, cancellationToken);
        if (profesor == null)
            throw new InvalidOperationException("El profesor no existe.");

        if (!await CanBeDeletedAsync(id, cancellationToken))
            throw new InvalidOperationException("El profesor no puede ser eliminado porque tiene asignaciones, horarios o registros asociados.");

        profesor.MarkAsDeleted();
        await _context.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Consultas Específicas de Negocio

    /// <summary>
    /// Obtiene un profesor por su código dentro del colegio
    /// </summary>
    public async Task<Profesor?> GetByCodigoAsync(string codigoProfesor, Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p =>
                p.CodigoProfesor == codigoProfesor &&
                p.ColegioId == colegioId &&
                !p.IsDeleted,
                cancellationToken);
    }

    /// <summary>
    /// Obtiene un profesor por su PersonaId dentro del colegio
    /// </summary>
    public async Task<Profesor?> GetByPersonaIdAsync(Guid personaId, Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p =>
                p.PersonaId == personaId &&
                p.ColegioId == colegioId &&
                !p.IsDeleted,
                cancellationToken);
    }

    /// <summary>
    /// Obtiene profesores por estado específico
    /// </summary>
    public async Task<IEnumerable<Profesor>> GetByEstadoAsync(EstadoProfesor estado, Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Persona)
            .Where(p =>
                p.Estado == estado &&
                p.ColegioId == colegioId &&
                !p.IsDeleted)
            .OrderBy(p => p.Persona!.Apellidos)
            .ThenBy(p => p.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene profesores por tipo de contrato
    /// </summary>
    public async Task<IEnumerable<Profesor>> GetByTipoContratoAsync(TipoContratoProfesor tipoContrato, Guid colegioId, bool soloActivos = true, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(p => p.Persona)
            .Where(p =>
                p.TipoContrato == tipoContrato &&
                p.ColegioId == colegioId &&
                !p.IsDeleted);

        if (soloActivos)
            query = query.Where(p => p.Estado == EstadoProfesor.Activo);

        return await query
            .OrderBy(p => p.Persona!.Apellidos)
            .ThenBy(p => p.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene profesores por cargo
    /// </summary>
    public async Task<IEnumerable<Profesor>> GetByCargoAsync(CargoProfesor cargo, Guid colegioId, bool soloActivos = true, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(p => p.Persona)
            .Where(p =>
                p.Cargo == cargo &&
                p.ColegioId == colegioId &&
                !p.IsDeleted);

        if (soloActivos)
            query = query.Where(p => p.Estado == EstadoProfesor.Activo);

        return await query
            .OrderBy(p => p.Persona!.Apellidos)
            .ThenBy(p => p.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene profesores por especialidad académica
    /// </summary>
    public async Task<IEnumerable<Profesor>> GetByEspecialidadAsync(string especialidad, Guid colegioId, bool soloActivos = true, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(p => p.Persona)
            .Where(p =>
                p.ColegioId == colegioId &&
                p.Especialidades != null &&
                p.Especialidades.Contains(especialidad) &&
                !p.IsDeleted);

        if (soloActivos)
            query = query.Where(p => p.Estado == EstadoProfesor.Activo);

        return await query
            .OrderBy(p => p.Persona!.Apellidos)
            .ThenBy(p => p.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene profesores disponibles para ser coordinadores
    /// </summary>
    public async Task<IEnumerable<Profesor>> GetDisponiblesParaCoordinacionAsync(Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Persona)
            .Where(p =>
                p.ColegioId == colegioId &&
                p.Estado == EstadoProfesor.Activo &&
                p.PuedeSerCoordinador &&
                !p.IsDeleted)
            .OrderBy(p => p.Persona!.Apellidos)
            .ThenBy(p => p.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene profesores disponibles para ser directores de grupo
    /// </summary>
    public async Task<IEnumerable<Profesor>> GetDisponiblesParaDireccionGrupoAsync(Guid colegioId, Guid? anoAcademicoId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(p => p.Persona)
            .Where(p =>
                p.ColegioId == colegioId &&
                p.Estado == EstadoProfesor.Activo &&
                p.PuedeSerDirectorGrupo &&
                !p.IsDeleted);

        // Si se especifica año académico, excluir profesores que ya son directores de grupo
        if (anoAcademicoId.HasValue)
        {
            var profesoresConGrupo = _context.Set<Grupo>()
                .Where(g => g.AnoAcademicoId == anoAcademicoId &&
                           g.DirectorGrupoId != null &&
                           g.Activo)
                .Select(g => g.DirectorGrupoId!.Value);

            query = query.Where(p => !profesoresConGrupo.Contains(p.Id));
        }

        return await query
            .OrderBy(p => p.Persona!.Apellidos)
            .ThenBy(p => p.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene profesores disponibles para reemplazos
    /// </summary>
    public async Task<IEnumerable<Profesor>> GetDisponiblesParaReemplazosAsync(Guid colegioId, string? especialidad = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(p => p.Persona)
            .Where(p =>
                p.ColegioId == colegioId &&
                p.Estado == EstadoProfesor.Activo &&
                p.DisponibleReemplazos &&
                !p.IsDeleted);

        if (!string.IsNullOrWhiteSpace(especialidad))
        {
            query = query.Where(p =>
                p.Especialidades != null &&
                p.Especialidades.Contains(especialidad));
        }

        return await query
            .OrderBy(p => p.Persona!.Apellidos)
            .ThenBy(p => p.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene todas los profesores de un colegio con filtros opcionales
    /// </summary>
    public async Task<IEnumerable<Profesor>> GetAllAsync(
        Guid colegioId,
        EstadoProfesor? estado = null,
        CargoProfesor? cargo = null,
        TipoContratoProfesor? tipoContrato = null,
        bool soloActivos = true,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(p => p.Persona)
            .Where(p =>
                p.ColegioId == colegioId &&
                !p.IsDeleted);

        if (estado.HasValue)
            query = query.Where(p => p.Estado == estado);
        else if (soloActivos)
            query = query.Where(p => p.Estado == EstadoProfesor.Activo);

        if (cargo.HasValue)
            query = query.Where(p => p.Cargo == cargo);

        if (tipoContrato.HasValue)
            query = query.Where(p => p.TipoContrato == tipoContrato);

        return await query
            .OrderBy(p => p.Persona!.Apellidos)
            .ThenBy(p => p.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca profesores por término de búsqueda (código, nombres, especialidades)
    /// </summary>
    public async Task<IEnumerable<Profesor>> SearchAsync(string searchTerm, Guid colegioId, CargoProfesor? cargo = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync(colegioId, cargo: cargo, cancellationToken: cancellationToken);

        searchTerm = searchTerm.ToLower();

        var query = _dbSet
            .Include(p => p.Persona)
            .Where(p =>
                p.ColegioId == colegioId &&
                p.Estado == EstadoProfesor.Activo &&
                !p.IsDeleted &&
                (p.CodigoProfesor.ToLower().Contains(searchTerm) ||
                 p.Persona!.Nombres.ToLower().Contains(searchTerm) ||
                 p.Persona!.Apellidos.ToLower().Contains(searchTerm) ||
                 (p.Especialidades != null && p.Especialidades.ToLower().Contains(searchTerm)) ||
                 (p.TitulosAcademicos != null && p.TitulosAcademicos.ToLower().Contains(searchTerm))));

        if (cargo.HasValue)
            query = query.Where(p => p.Cargo == cargo);

        return await query
            .OrderBy(p => p.Persona!.Apellidos)
            .ThenBy(p => p.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    #endregion
    #region Consultas de Asignaciones y Horarios

    /// <summary>
    /// Obtiene profesores con asignaciones en un año académico específico
    /// </summary>
    public async Task<IEnumerable<Profesor>> GetConAsignacionesAsync(Guid anoAcademicoId, Guid colegioId, bool soloActivos = true, CancellationToken cancellationToken = default)
    {
        var profesoresConAsignacion = _context.Set<Asignacion>()
            .Where(a => a.AnoAcademicoId == anoAcademicoId &&
                       a.ColegioId == colegioId)
            .Where(a => !soloActivos || a.Estado == EstadoAsignacion.Activa)
            .Select(a => a.ProfesorId);

        return await _dbSet
            .Include(p => p.Persona)
            .Where(p =>
                p.ColegioId == colegioId &&
                p.Estado == EstadoProfesor.Activo &&
                !p.IsDeleted &&
                profesoresConAsignacion.Contains(p.Id))
            .OrderBy(p => p.Persona!.Apellidos)
            .ThenBy(p => p.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene profesores sin asignaciones en un año académico
    /// </summary>
    public async Task<IEnumerable<Profesor>> GetSinAsignacionesAsync(Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default)
    {
        var profesoresConAsignacion = _context.Set<Asignacion>()
            .Where(a => a.AnoAcademicoId == anoAcademicoId &&
                       a.ColegioId == colegioId &&
                       a.Estado == EstadoAsignacion.Activa)
            .Select(a => a.ProfesorId);

        return await _dbSet
            .Include(p => p.Persona)
            .Where(p =>
                p.ColegioId == colegioId &&
                p.Estado == EstadoProfesor.Activo &&
                !p.IsDeleted &&
                !profesoresConAsignacion.Contains(p.Id))
            .OrderBy(p => p.Persona!.Apellidos)
            .ThenBy(p => p.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene directores de grupo activos
    /// </summary>
    public async Task<IEnumerable<Profesor>> GetDirectoresGrupoAsync(Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default)
    {
        var directoresGrupo = _context.Set<Grupo>()
            .Where(g => g.AnoAcademicoId == anoAcademicoId &&
                       g.ColegioId == colegioId &&
                       g.DirectorGrupoId != null &&
                       g.Activo)
            .Select(g => g.DirectorGrupoId!.Value);

        return await _dbSet
            .Include(p => p.Persona)
            .Where(p =>
                p.ColegioId == colegioId &&
                p.Estado == EstadoProfesor.Activo &&
                !p.IsDeleted &&
                directoresGrupo.Contains(p.Id))
            .OrderBy(p => p.Persona!.Apellidos)
            .ThenBy(p => p.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Consultas con Paginación

    /// <summary>
    /// Obtiene profesores paginados con filtros
    /// </summary>
    public async Task<(IEnumerable<Profesor> Items, int TotalCount)> GetPagedAsync(
        Guid colegioId,
        int pageNumber,
        int pageSize,
        EstadoProfesor? estado = null,
        CargoProfesor? cargo = null,
        string? searchTerm = null,
        bool soloActivos = true,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(p => p.Persona)
            .Where(p =>
                p.ColegioId == colegioId &&
                !p.IsDeleted);

        if (estado.HasValue)
            query = query.Where(p => p.Estado == estado);
        else if (soloActivos)
            query = query.Where(p => p.Estado == EstadoProfesor.Activo);

        if (cargo.HasValue)
            query = query.Where(p => p.Cargo == cargo);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(p =>
                p.CodigoProfesor.ToLower().Contains(searchTerm) ||
                p.Persona!.Nombres.ToLower().Contains(searchTerm) ||
                p.Persona!.Apellidos.ToLower().Contains(searchTerm) ||
                (p.Especialidades != null && p.Especialidades.ToLower().Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Persona!.Apellidos)
            .ThenBy(p => p.Persona!.Nombres)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    #endregion

    #region Validaciones de Negocio

    /// <summary>
    /// Verifica si existe un profesor con el mismo código
    /// </summary>
    public async Task<bool> ExisteDuplicadoAsync(string codigoProfesor, Guid colegioId, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(p =>
                p.CodigoProfesor == codigoProfesor &&
                p.ColegioId == colegioId &&
                !p.IsDeleted);

        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId);

        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Verifica si una persona ya es profesor en el colegio
    /// </summary>
    public async Task<bool> PersonaYaEsProfesorAsync(Guid personaId, Guid colegioId, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(p =>
                p.PersonaId == personaId &&
                p.ColegioId == colegioId &&
                !p.IsDeleted);

        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId);

        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Verifica si un profesor puede ser eliminado
    /// </summary>
    public async Task<bool> CanBeDeletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Verificar si tiene asignaciones activas
        var tieneAsignaciones = await _context.Set<Asignacion>()
            .AnyAsync(a => a.ProfesorId == id && a.Estado == EstadoAsignacion.Activa, cancellationToken);

        if (tieneAsignaciones)
            return false;

        // Verificar si es director de algún grupo activo
        var esDirectorGrupo = await _context.Set<Grupo>()
            .AnyAsync(g => g.DirectorGrupoId == id && g.Activo, cancellationToken);

        if (esDirectorGrupo)
            return false;

        // Verificar si tiene horarios programados
        var tieneHorarios = await _context.Set<Horario>()
            .AnyAsync(h => h.ProfesorId == id && h.Activo, cancellationToken);

        if (tieneHorarios)
            return false;

        // Verificar si ha registrado calificaciones
        var tieneCalificaciones = await _context.Set<Calificacion>()
            .AnyAsync(c => c.ProfesorId == id, cancellationToken);

        if (tieneCalificaciones)
            return false;

        // Verificar si ha registrado asistencias
        var tieneAsistencias = await _context.Set<Asistencia>()
            .AnyAsync(a => a.ProfesorId == id, cancellationToken);

        return !tieneAsistencias;
    }

    /// <summary>
    /// Obtiene el número de asignaciones de un profesor
    /// </summary>
    public async Task<int> GetConteoAsignacionesAsync(Guid profesorId, Guid? anoAcademicoId = null, bool soloActivas = true, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Asignacion>()
            .Where(a => a.ProfesorId == profesorId);

        if (anoAcademicoId.HasValue)
            query = query.Where(a => a.AnoAcademicoId == anoAcademicoId);

        if (soloActivas)
            query = query.Where(a => a.Estado == EstadoAsignacion.Activa);

        return await query.CountAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene el número de grupos dirigidos por un profesor
    /// </summary>
    public async Task<int> GetConteoGruposDirigidosAsync(Guid profesorId, Guid? anoAcademicoId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Grupo>()
            .Where(g => g.DirectorGrupoId == profesorId && g.Activo);

        if (anoAcademicoId.HasValue)
            query = query.Where(g => g.AnoAcademicoId == anoAcademicoId);

        return await query.CountAsync(cancellationToken);
    }

    #endregion

    #region Consultas de Reportes y Estadísticas

    /// <summary>
    /// Obtiene el conteo de profesores por estado
    /// </summary>
    public async Task<Dictionary<EstadoProfesor, int>> GetConteoByEstadoAsync(Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p =>
                p.ColegioId == colegioId &&
                !p.IsDeleted)
            .GroupBy(p => p.Estado)
            .Select(group => new { Estado = group.Key, Conteo = group.Count() })
            .ToDictionaryAsync(x => x.Estado, x => x.Conteo, cancellationToken);
    }

    /// <summary>
    /// Obtiene el conteo de profesores por cargo
    /// </summary>
    public async Task<Dictionary<CargoProfesor, int>> GetConteoByCargoAsync(Guid colegioId, bool soloActivos = true, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(p =>
                p.ColegioId == colegioId &&
                !p.IsDeleted);

        if (soloActivos)
            query = query.Where(p => p.Estado == EstadoProfesor.Activo);

        return await query
            .GroupBy(p => p.Cargo)
            .Select(group => new { Cargo = group.Key, Conteo = group.Count() })
            .ToDictionaryAsync(x => x.Cargo, x => x.Conteo, cancellationToken);
    }

    /// <summary>
    /// Obtiene el conteo de profesores por tipo de contrato
    /// </summary>
    public async Task<Dictionary<TipoContratoProfesor, int>> GetConteoByTipoContratoAsync(Guid colegioId, bool soloActivos = true, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(p =>
                p.ColegioId == colegioId &&
                !p.IsDeleted);

        if (soloActivos)
            query = query.Where(p => p.Estado == EstadoProfesor.Activo);

        return await query
            .GroupBy(p => p.TipoContrato)
            .Select(group => new { TipoContrato = group.Key, Conteo = group.Count() })
            .ToDictionaryAsync(x => x.TipoContrato, x => x.Conteo, cancellationToken);
    }

    /// <summary>
    /// Obtiene estadísticas de experiencia de profesores
    /// </summary>
    public async Task<(int TotalProfesores, decimal PromedioExperiencia, int MaximaExperiencia, int MinimaExperiencia)> GetEstadisticasExperienciaAsync(
        Guid colegioId,
        CancellationToken cancellationToken = default)
    {
        var profesores = await _dbSet
            .Where(p =>
                p.ColegioId == colegioId &&
                p.Estado == EstadoProfesor.Activo &&
                p.AnosExperiencia.HasValue &&
                !p.IsDeleted)
            .Select(p => p.AnosExperiencia!.Value)
            .ToListAsync(cancellationToken);

        if (!profesores.Any())
            return (0, 0, 0, 0);

        return (
            profesores.Count,
            Math.Round((decimal)profesores.Average(), 2),
            profesores.Max(),
            profesores.Min()
        );
    }

    /// <summary>
    /// Obtiene profesores próximos a jubilarse
    /// </summary>
    public async Task<IEnumerable<Profesor>> GetProximosJubilacionAsync(Guid colegioId, int edadJubilacion = 65, int anosAnticipacion = 5, CancellationToken cancellationToken = default)
    {
        var fechaLimite = DateTime.Today.AddYears(anosAnticipacion);
        var fechaNacimientoLimite = DateTime.Today.AddYears(-edadJubilacion + anosAnticipacion);

        return await _dbSet
            .Include(p => p.Persona)
            .Where(p =>
                p.ColegioId == colegioId &&
                p.Estado == EstadoProfesor.Activo &&
                p.Persona!.FechaNacimiento.HasValue &&
                p.Persona.FechaNacimiento <= fechaNacimientoLimite &&
                !p.IsDeleted)
            .OrderBy(p => p.Persona!.FechaNacimiento)
            .ToListAsync(cancellationToken);
    }

    #endregion
}