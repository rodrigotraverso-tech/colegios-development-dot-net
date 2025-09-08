using ColegiosBackend.Core.Entities;
using ColegiosBackend.Core.Enums;
using ColegiosBackend.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ColegiosBackend.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio para la gestión de horarios académicos
/// </summary>
public class HorarioRepository : IHorarioRepository
{
    private readonly DbContext _context;
    private readonly DbSet<Horario> _dbSet;

    public HorarioRepository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<Horario>();
    }

    #region Métodos CRUD Básicos

    /// <summary>
    /// Obtiene un horario por su ID
    /// </summary>
    public async Task<Horario?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Obtiene un horario por su ID incluyendo entidades relacionadas
    /// </summary>
    public async Task<Horario?> GetByIdWithIncludesAsync(Guid id, string[] includeEntities, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        // Agregar includes dinámicamente
        foreach (var include in includeEntities)
        {
            query = query.Include(include);
        }

        return await query
            .FirstOrDefaultAsync(h => h.Id == id && !h.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Agrega un nuevo horario
    /// </summary>
    public async Task AddAsync(Horario horario, CancellationToken cancellationToken = default)
    {
        if (horario == null)
            throw new ArgumentNullException(nameof(horario));

        // Validar conflictos de profesor
        var tieneConflictoProfesor = await TieneConflictoProfesorAsync(
            horario.ProfesorId,
            horario.DiaSemana,
            horario.HoraInicio,
            horario.HoraFin,
            horario.AnoAcademicoId,
            null,
            cancellationToken);

        if (tieneConflictoProfesor)
            throw new InvalidOperationException("El profesor ya tiene una clase asignada en este horario.");

        // Validar conflictos de grupo
        var tieneConflictoGrupo = await TieneConflictoGrupoAsync(
            horario.GrupoId,
            horario.DiaSemana,
            horario.HoraInicio,
            horario.HoraFin,
            horario.AnoAcademicoId,
            null,
            cancellationToken);

        if (tieneConflictoGrupo)
            throw new InvalidOperationException("El grupo ya tiene una clase asignada en este horario.");

        // Validar conflictos de aula (si se especifica)
        if (!string.IsNullOrWhiteSpace(horario.Aula))
        {
            var tieneConflictoAula = await TieneConflictoAulaAsync(
                horario.Aula,
                horario.DiaSemana,
                horario.HoraInicio,
                horario.HoraFin,
                horario.ColegioId!.Value,
                horario.AnoAcademicoId,
                null,
                cancellationToken);

            if (tieneConflictoAula)
                throw new InvalidOperationException($"El aula '{horario.Aula}' ya está ocupada en este horario.");
        }

        await _dbSet.AddAsync(horario, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Actualiza un horario existente
    /// </summary>
    public async Task UpdateAsync(Horario horario, CancellationToken cancellationToken = default)
    {
        if (horario == null)
            throw new ArgumentNullException(nameof(horario));

        // Validar conflictos excluyendo el horario actual
        var tieneConflictoProfesor = await TieneConflictoProfesorAsync(
            horario.ProfesorId,
            horario.DiaSemana,
            horario.HoraInicio,
            horario.HoraFin,
            horario.AnoAcademicoId,
            horario.Id,
            cancellationToken);

        if (tieneConflictoProfesor)
            throw new InvalidOperationException("El profesor ya tiene una clase asignada en este horario.");

        var tieneConflictoGrupo = await TieneConflictoGrupoAsync(
            horario.GrupoId,
            horario.DiaSemana,
            horario.HoraInicio,
            horario.HoraFin,
            horario.AnoAcademicoId,
            horario.Id,
            cancellationToken);

        if (tieneConflictoGrupo)
            throw new InvalidOperationException("El grupo ya tiene una clase asignada en este horario.");

        if (!string.IsNullOrWhiteSpace(horario.Aula))
        {
            var tieneConflictoAula = await TieneConflictoAulaAsync(
                horario.Aula,
                horario.DiaSemana,
                horario.HoraInicio,
                horario.HoraFin,
                horario.ColegioId!.Value,
                horario.AnoAcademicoId,
                horario.Id,
                cancellationToken);

            if (tieneConflictoAula)
                throw new InvalidOperationException($"El aula '{horario.Aula}' ya está ocupada en este horario.");
        }

        _dbSet.Update(horario);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Elimina un horario (soft delete)
    /// </summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var horario = await GetByIdAsync(id, cancellationToken);
        if (horario == null)
            throw new InvalidOperationException("El horario no existe.");

        if (!await CanBeDeletedAsync(id, cancellationToken))
            throw new InvalidOperationException("El horario no puede ser eliminado porque tiene asistencias registradas.");

        horario.MarkAsDeleted();
        await _context.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region Consultas por Entidades Relacionadas

    /// <summary>
    /// Obtiene todos los horarios de un grupo específico
    /// </summary>
    public async Task<IEnumerable<Horario>> GetByGrupoAsync(Guid grupoId, Guid anoAcademicoId, bool soloActivos = true, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(h => h.Materia)
            .Include(h => h.Profesor)
            .ThenInclude(p => p!.Persona)
            .Where(h =>
                h.GrupoId == grupoId &&
                h.AnoAcademicoId == anoAcademicoId &&
                !h.IsDeleted);

        if (soloActivos)
            query = query.Where(h => h.Activo && h.Estado == EstadoHorario.Activo);

        return await query
            .OrderBy(h => h.DiaSemana)
            .ThenBy(h => h.HoraInicio)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene todos los horarios de un profesor específico
    /// </summary>
    public async Task<IEnumerable<Horario>> GetByProfesorAsync(Guid profesorId, Guid anoAcademicoId, DiaSemana? diaSemana = null, bool soloActivos = true, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(h => h.Grupo)
            .ThenInclude(g => g!.Grado)
            .Include(h => h.Materia)
            .Where(h =>
                h.ProfesorId == profesorId &&
                h.AnoAcademicoId == anoAcademicoId &&
                !h.IsDeleted);

        if (diaSemana.HasValue)
            query = query.Where(h => h.DiaSemana == diaSemana);

        if (soloActivos)
            query = query.Where(h => h.Activo && h.Estado == EstadoHorario.Activo);

        return await query
            .OrderBy(h => h.DiaSemana)
            .ThenBy(h => h.HoraInicio)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene todos los horarios de una materia específica
    /// </summary>
    public async Task<IEnumerable<Horario>> GetByMateriaAsync(Guid materiaId, Guid anoAcademicoId, bool soloActivos = true, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(h => h.Grupo)
            .ThenInclude(g => g!.Grado)
            .Include(h => h.Profesor)
            .ThenInclude(p => p!.Persona)
            .Where(h =>
                h.MateriaId == materiaId &&
                h.AnoAcademicoId == anoAcademicoId &&
                !h.IsDeleted);

        if (soloActivos)
            query = query.Where(h => h.Activo && h.Estado == EstadoHorario.Activo);

        return await query
            .OrderBy(h => h.DiaSemana)
            .ThenBy(h => h.HoraInicio)
            .ThenBy(h => h.Grupo!.Nombre)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene horarios por día de la semana
    /// </summary>
    public async Task<IEnumerable<Horario>> GetByDiaSemanaAsync(DiaSemana diaSemana, Guid colegioId, Guid anoAcademicoId, bool soloActivos = true, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(h => h.Grupo)
            .ThenInclude(g => g!.Grado)
            .Include(h => h.Materia)
            .Include(h => h.Profesor)
            .ThenInclude(p => p!.Persona)
            .Where(h =>
                h.DiaSemana == diaSemana &&
                h.ColegioId == colegioId &&
                h.AnoAcademicoId == anoAcademicoId &&
                !h.IsDeleted);

        if (soloActivos)
            query = query.Where(h => h.Activo && h.Estado == EstadoHorario.Activo);

        return await query
            .OrderBy(h => h.HoraInicio)
            .ThenBy(h => h.Grupo!.Nombre)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene horarios por aula o salón
    /// </summary>
    public async Task<IEnumerable<Horario>> GetByAulaAsync(string aula, Guid colegioId, Guid anoAcademicoId, DiaSemana? diaSemana = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(h => h.Grupo)
            .ThenInclude(g => g!.Grado)
            .Include(h => h.Materia)
            .Include(h => h.Profesor)
            .ThenInclude(p => p!.Persona)
            .Where(h =>
                h.Aula == aula &&
                h.ColegioId == colegioId &&
                h.AnoAcademicoId == anoAcademicoId &&
                h.Activo &&
                h.Estado == EstadoHorario.Activo &&
                !h.IsDeleted);

        if (diaSemana.HasValue)
            query = query.Where(h => h.DiaSemana == diaSemana);

        return await query
            .OrderBy(h => h.DiaSemana)
            .ThenBy(h => h.HoraInicio)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Consultas por Filtros Específicos

    /// <summary>
    /// Obtiene horarios por tipo de clase
    /// </summary>
    public async Task<IEnumerable<Horario>> GetByTipoClaseAsync(TipoClase tipoClase, Guid colegioId, Guid anoAcademicoId, bool soloActivos = true, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(h => h.Grupo)
            .ThenInclude(g => g!.Grado)
            .Include(h => h.Materia)
            .Include(h => h.Profesor)
            .ThenInclude(p => p!.Persona)
            .Where(h =>
                h.TipoClase == tipoClase &&
                h.ColegioId == colegioId &&
                h.AnoAcademicoId == anoAcademicoId &&
                !h.IsDeleted);

        if (soloActivos)
            query = query.Where(h => h.Activo && h.Estado == EstadoHorario.Activo);

        return await query
            .OrderBy(h => h.DiaSemana)
            .ThenBy(h => h.HoraInicio)
            .ThenBy(h => h.Grupo!.Nombre)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene horarios por estado
    /// </summary>
    public async Task<IEnumerable<Horario>> GetByEstadoAsync(EstadoHorario estado, Guid colegioId, Guid anoAcademicoId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(h => h.Grupo)
            .ThenInclude(g => g!.Grado)
            .Include(h => h.Materia)
            .Include(h => h.Profesor)
            .ThenInclude(p => p!.Persona)
            .Where(h =>
                h.Estado == estado &&
                h.ColegioId == colegioId &&
                h.AnoAcademicoId == anoAcademicoId &&
                !h.IsDeleted)
            .OrderBy(h => h.DiaSemana)
            .ThenBy(h => h.HoraInicio)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene horarios en un rango de tiempo específico
    /// </summary>
    public async Task<IEnumerable<Horario>> GetByRangoHorarioAsync(TimeOnly horaInicio, TimeOnly horaFin, Guid colegioId, Guid anoAcademicoId, DiaSemana? diaSemana = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(h => h.Grupo)
            .ThenInclude(g => g!.Grado)
            .Include(h => h.Materia)
            .Include(h => h.Profesor)
            .ThenInclude(p => p!.Persona)
            .Where(h =>
                h.ColegioId == colegioId &&
                h.AnoAcademicoId == anoAcademicoId &&
                h.HoraInicio >= horaInicio &&
                h.HoraFin <= horaFin &&
                h.Activo &&
                h.Estado == EstadoHorario.Activo &&
                !h.IsDeleted);

        if (diaSemana.HasValue)
            query = query.Where(h => h.DiaSemana == diaSemana);

        return await query
            .OrderBy(h => h.DiaSemana)
            .ThenBy(h => h.HoraInicio)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene horarios vigentes en una fecha específica
    /// </summary>
    public async Task<IEnumerable<Horario>> GetVigentesEnFechaAsync(DateOnly fecha, Guid colegioId, Guid anoAcademicoId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(h => h.Grupo)
            .ThenInclude(g => g!.Grado)
            .Include(h => h.Materia)
            .Include(h => h.Profesor)
            .ThenInclude(p => p!.Persona)
            .Where(h =>
                h.ColegioId == colegioId &&
                h.AnoAcademicoId == anoAcademicoId &&
                h.FechaInicioVigencia <= fecha &&
                (h.FechaFinVigencia == null || h.FechaFinVigencia >= fecha) &&
                h.Activo &&
                h.Estado == EstadoHorario.Activo &&
                !h.IsDeleted)
            .OrderBy(h => h.DiaSemana)
            .ThenBy(h => h.HoraInicio)
            .ToListAsync(cancellationToken);
    }

    #endregion
    #region Consultas de Disponibilidad y Conflictos

    /// <summary>
    /// Verifica si hay conflictos de horario para un profesor
    /// </summary>
    public async Task<bool> TieneConflictoProfesorAsync(Guid profesorId, DiaSemana diaSemana, TimeOnly horaInicio, TimeOnly horaFin, Guid anoAcademicoId, Guid? excludeHorarioId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(h =>
                h.ProfesorId == profesorId &&
                h.DiaSemana == diaSemana &&
                h.AnoAcademicoId == anoAcademicoId &&
                h.Activo &&
                h.Estado == EstadoHorario.Activo &&
                !h.IsDeleted &&
                // Verificar solapamiento de horarios
                h.HoraInicio < horaFin && horaInicio < h.HoraFin);

        if (excludeHorarioId.HasValue)
            query = query.Where(h => h.Id != excludeHorarioId);

        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Verifica si hay conflictos de horario para un grupo
    /// </summary>
    public async Task<bool> TieneConflictoGrupoAsync(Guid grupoId, DiaSemana diaSemana, TimeOnly horaInicio, TimeOnly horaFin, Guid anoAcademicoId, Guid? excludeHorarioId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(h =>
                h.GrupoId == grupoId &&
                h.DiaSemana == diaSemana &&
                h.AnoAcademicoId == anoAcademicoId &&
                h.Activo &&
                h.Estado == EstadoHorario.Activo &&
                !h.IsDeleted &&
                // Verificar solapamiento de horarios
                h.HoraInicio < horaFin && horaInicio < h.HoraFin);

        if (excludeHorarioId.HasValue)
            query = query.Where(h => h.Id != excludeHorarioId);

        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Verifica si hay conflictos de aula
    /// </summary>
    public async Task<bool> TieneConflictoAulaAsync(string aula, DiaSemana diaSemana, TimeOnly horaInicio, TimeOnly horaFin, Guid colegioId, Guid anoAcademicoId, Guid? excludeHorarioId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(h =>
                h.Aula == aula &&
                h.DiaSemana == diaSemana &&
                h.ColegioId == colegioId &&
                h.AnoAcademicoId == anoAcademicoId &&
                h.Activo &&
                h.Estado == EstadoHorario.Activo &&
                !h.IsDeleted &&
                // Verificar solapamiento de horarios
                h.HoraInicio < horaFin && horaInicio < h.HoraFin);

        if (excludeHorarioId.HasValue)
            query = query.Where(h => h.Id != excludeHorarioId);

        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene horarios con conflictos en el sistema
    /// </summary>
    public async Task<IEnumerable<Horario>> GetConflictosAsync(Guid colegioId, Guid anoAcademicoId, CancellationToken cancellationToken = default)
    {
        var horarios = await _dbSet
            .Include(h => h.Grupo)
            .ThenInclude(g => g!.Grado)
            .Include(h => h.Materia)
            .Include(h => h.Profesor)
            .ThenInclude(p => p!.Persona)
            .Where(h =>
                h.ColegioId == colegioId &&
                h.AnoAcademicoId == anoAcademicoId &&
                h.Activo &&
                h.Estado == EstadoHorario.Activo &&
                !h.IsDeleted)
            .ToListAsync(cancellationToken);

        var conflictos = new List<Horario>();

        // Buscar conflictos de profesor
        var conflictosProfesor = horarios
            .GroupBy(h => new { h.ProfesorId, h.DiaSemana })
            .Where(group => group.Count() > 1)
            .SelectMany(group => group.Where(h1 =>
                group.Any(h2 => h2.Id != h1.Id &&
                               h1.HoraInicio < h2.HoraFin &&
                               h2.HoraInicio < h1.HoraFin)));

        conflictos.AddRange(conflictosProfesor);

        // Buscar conflictos de grupo
        var conflictosGrupo = horarios
            .GroupBy(h => new { h.GrupoId, h.DiaSemana })
            .Where(group => group.Count() > 1)
            .SelectMany(group => group.Where(h1 =>
                group.Any(h2 => h2.Id != h1.Id &&
                               h1.HoraInicio < h2.HoraFin &&
                               h2.HoraInicio < h1.HoraFin)));

        conflictos.AddRange(conflictosGrupo);

        // Buscar conflictos de aula
        var conflictosAula = horarios
            .Where(h => !string.IsNullOrWhiteSpace(h.Aula))
            .GroupBy(h => new { h.Aula, h.DiaSemana })
            .Where(group => group.Count() > 1)
            .SelectMany(group => group.Where(h1 =>
                group.Any(h2 => h2.Id != h1.Id &&
                               h1.HoraInicio < h2.HoraFin &&
                               h2.HoraInicio < h1.HoraFin)));

        conflictos.AddRange(conflictosAula);

        return conflictos.Distinct().OrderBy(h => h.DiaSemana).ThenBy(h => h.HoraInicio);
    }

    #endregion

    #region Consultas con Paginación

    /// <summary>
    /// Obtiene horarios paginados con filtros
    /// </summary>
    public async Task<(IEnumerable<Horario> Items, int TotalCount)> GetPagedAsync(
        Guid colegioId,
        Guid anoAcademicoId,
        int pageNumber,
        int pageSize,
        Guid? grupoId = null,
        Guid? profesorId = null,
        DiaSemana? diaSemana = null,
        bool soloActivos = true,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(h => h.Grupo)
            .ThenInclude(g => g!.Grado)
            .Include(h => h.Materia)
            .Include(h => h.Profesor)
            .ThenInclude(p => p!.Persona)
            .Where(h =>
                h.ColegioId == colegioId &&
                h.AnoAcademicoId == anoAcademicoId &&
                !h.IsDeleted);

        if (grupoId.HasValue)
            query = query.Where(h => h.GrupoId == grupoId);

        if (profesorId.HasValue)
            query = query.Where(h => h.ProfesorId == profesorId);

        if (diaSemana.HasValue)
            query = query.Where(h => h.DiaSemana == diaSemana);

        if (soloActivos)
            query = query.Where(h => h.Activo && h.Estado == EstadoHorario.Activo);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(h => h.DiaSemana)
            .ThenBy(h => h.HoraInicio)
            .ThenBy(h => h.Grupo!.Nombre)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    #endregion

    #region Validaciones de Negocio

    /// <summary>
    /// Verifica si un horario puede ser eliminado
    /// </summary>
    public async Task<bool> CanBeDeletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Verificar si tiene asistencias registradas
        // Primero obtener el horario
        var horario = await _dbSet.FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
        if (horario == null)
            return true;

        // Verificar si tiene asistencias registradas
        var tieneAsistencias = await _context.Set<Asistencia>()
            .AnyAsync(a => a.ProfesorId == horario.ProfesorId &&
                           a.MateriaId == horario.MateriaId &&
                           a.GrupoId == horario.GrupoId,
                    cancellationToken);

        return !tieneAsistencias;
    }

    /// <summary>
    /// Obtiene el número total de horarios de un grupo
    /// </summary>
    public async Task<int> GetConteoHorariosGrupoAsync(Guid grupoId, Guid anoAcademicoId, bool soloActivos = true, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(h =>
                h.GrupoId == grupoId &&
                h.AnoAcademicoId == anoAcademicoId &&
                !h.IsDeleted);

        if (soloActivos)
            query = query.Where(h => h.Activo && h.Estado == EstadoHorario.Activo);

        return await query.CountAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene el número total de horas semanales de un profesor
    /// </summary>
    public async Task<int> GetHorasSemanalesProfesorAsync(Guid profesorId, Guid anoAcademicoId, CancellationToken cancellationToken = default)
    {
        var horarios = await _dbSet
            .Where(h =>
                h.ProfesorId == profesorId &&
                h.AnoAcademicoId == anoAcademicoId &&
                h.Activo &&
                h.Estado == EstadoHorario.Activo &&
                !h.IsDeleted)
            .ToListAsync(cancellationToken);

        return horarios.Sum(h => h.GetDuracionEnMinutos()) / 60; // Convertir a horas
    }

    #endregion

    #region Consultas de Reportes y Estadísticas

    /// <summary>
    /// Obtiene estadísticas de uso por día de la semana
    /// </summary>
    public async Task<Dictionary<DiaSemana, int>> GetEstadisticasPorDiaAsync(Guid colegioId, Guid anoAcademicoId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(h =>
                h.ColegioId == colegioId &&
                h.AnoAcademicoId == anoAcademicoId &&
                h.Activo &&
                h.Estado == EstadoHorario.Activo &&
                !h.IsDeleted)
            .GroupBy(h => h.DiaSemana)
            .Select(group => new { Dia = group.Key, Conteo = group.Count() })
            .ToDictionaryAsync(x => x.Dia, x => x.Conteo, cancellationToken);
    }

    /// <summary>
    /// Obtiene estadísticas de uso por tipo de clase
    /// </summary>
    public async Task<Dictionary<TipoClase, int>> GetEstadisticasPorTipoClaseAsync(Guid colegioId, Guid anoAcademicoId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(h =>
                h.ColegioId == colegioId &&
                h.AnoAcademicoId == anoAcademicoId &&
                h.Activo &&
                h.Estado == EstadoHorario.Activo &&
                !h.IsDeleted)
            .GroupBy(h => h.TipoClase)
            .Select(group => new { Tipo = group.Key, Conteo = group.Count() })
            .ToDictionaryAsync(x => x.Tipo, x => x.Conteo, cancellationToken);
    }

    /// <summary>
    /// Obtiene estadísticas de ocupación de aulas
    /// </summary>
    public async Task<IEnumerable<(string Aula, int TotalHorarios, int HorasSemanales)>> GetEstadisticasOcupacionAulasAsync(Guid colegioId, Guid anoAcademicoId, CancellationToken cancellationToken = default)
    {
        var horariosPorAula = await _dbSet
            .Where(h =>
                h.ColegioId == colegioId &&
                h.AnoAcademicoId == anoAcademicoId &&
                h.Aula != null &&
                h.Activo &&
                h.Estado == EstadoHorario.Activo &&
                !h.IsDeleted)
            .GroupBy(h => h.Aula)
            .Select(group => new {
                Aula = group.Key!,
                TotalHorarios = group.Count(),
                Horarios = group.ToList()
            })
            .ToListAsync(cancellationToken);

        return horariosPorAula.Select(x => (
            x.Aula,
            x.TotalHorarios,
            x.Horarios.Sum(h => h.GetDuracionEnMinutos()) / 60 // Convertir a horas
        ));
    }

    /// <summary>
    /// Obtiene profesores con mayor carga horaria
    /// </summary>
    public async Task<IEnumerable<(Guid ProfesorId, string NombreProfesor, int HorasSemanales, int TotalClases)>> GetProfesoresMayorCargaAsync(Guid colegioId, Guid anoAcademicoId, int limite = 10, CancellationToken cancellationToken = default)
    {
        var cargaPorProfesor = await _dbSet
            .Include(h => h.Profesor)
            .ThenInclude(p => p!.Persona)
            .Where(h =>
                h.ColegioId == colegioId &&
                h.AnoAcademicoId == anoAcademicoId &&
                h.Activo &&
                h.Estado == EstadoHorario.Activo &&
                !h.IsDeleted)
            .GroupBy(h => new { h.ProfesorId, NombreCompleto = h.Profesor!.Persona!.Nombres + " " + h.Profesor.Persona.Apellidos })
            .Select(group => new {
                ProfesorId = group.Key.ProfesorId,
                NombreProfesor = group.Key.NombreCompleto,
                TotalClases = group.Count(),
                Horarios = group.ToList()
            })
            .ToListAsync(cancellationToken);

        return cargaPorProfesor
            .Select(x => (
                x.ProfesorId,
                x.NombreProfesor,
                x.Horarios.Sum(h => h.GetDuracionEnMinutos()) / 60, // Horas semanales
                x.TotalClases
            ))
            .OrderByDescending(x => x.Item3) // Ordenar por horas semanales
            .Take(limite);
    }

    #endregion
}