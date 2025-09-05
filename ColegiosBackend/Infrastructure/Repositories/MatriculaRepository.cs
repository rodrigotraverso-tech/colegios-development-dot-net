using ColegiosBackend.Core.Entities;
using ColegiosBackend.Core.Enums;
using ColegiosBackend.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ColegiosBackend.Infrastructure.Repositories;

/// <summary>
/// Implementaci�n del repositorio para la gesti�n de matr�culas
/// </summary>
public class MatriculaRepository : IMatriculaRepository
{
    private readonly DbContext _context;
    private readonly DbSet<Matricula> _dbSet;

    public MatriculaRepository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<Matricula>();
    }

    #region M�todos CRUD B�sicos

    /// <summary>
    /// Obtiene una matr�cula por su ID
    /// </summary>
    public async Task<Matricula?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Obtiene una matr�cula por su ID incluyendo entidades relacionadas
    /// </summary>
    public async Task<Matricula?> GetByIdWithIncludesAsync(Guid id, string[] includeEntities, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        // Agregar includes din�micamente
        foreach (var include in includeEntities)
        {
            query = query.Include(include);
        }

        return await query
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Agrega una nueva matr�cula
    /// </summary>
    public async Task AddAsync(Matricula matricula, CancellationToken cancellationToken = default)
    {
        if (matricula == null)
            throw new ArgumentNullException(nameof(matricula));

        await _dbSet.AddAsync(matricula, cancellationToken);
    }

    /// <summary>
    /// Actualiza una matr�cula existente
    /// </summary>
    public async Task UpdateAsync(Matricula matricula, CancellationToken cancellationToken = default)
    {
        if (matricula == null)
            throw new ArgumentNullException(nameof(matricula));

        matricula.MarkAsUpdated();
        _dbSet.Update(matricula);

        await Task.CompletedTask;
    }

    /// <summary>
    /// Elimina una matr�cula (soft delete)
    /// </summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var matricula = await GetByIdAsync(id, cancellationToken);
        if (matricula != null)
        {
            matricula.MarkAsDeleted();
            _dbSet.Update(matricula);
        }
    }

    #endregion

    #region Consultas Espec�ficas de Negocio

    /// <summary>
    /// Obtiene todas las matr�culas de un estudiante espec�fico
    /// </summary>
    public async Task<IEnumerable<Matricula>> GetByEstudianteAsync(Guid estudianteId, Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Grupo)
                .ThenInclude(g => g!.AnoAcademico)
            .Include(m => m.Estudiante)
                .ThenInclude(e => e!.Persona)
            .Where(m => m.EstudianteId == estudianteId &&
                       m.ColegioId == colegioId &&
                       !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene la matr�cula activa de un estudiante en un a�o acad�mico espec�fico
    /// </summary>
    public async Task<Matricula?> GetMatriculaActivaAsync(Guid estudianteId, Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Grupo)
                .ThenInclude(g => g!.AnoAcademico)
            .Include(m => m.Estudiante)
                .ThenInclude(e => e!.Persona)
            .FirstOrDefaultAsync(m => m.EstudianteId == estudianteId &&
                                    m.Grupo!.AnoAcademicoId == anoAcademicoId &&
                                    m.ColegioId == colegioId &&
                                    m.Estado == EstadoMatricula.Activa &&
                                    !m.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Obtiene todas las matr�culas de un grupo espec�fico
    /// </summary>
    public async Task<IEnumerable<Matricula>> GetByGrupoAsync(Guid grupoId, Guid colegioId, bool soloActivas = true, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(m => m.Estudiante)
                .ThenInclude(e => e!.Persona)
            .Include(m => m.Grupo)
                .ThenInclude(g => g!.AnoAcademico)
            .Where(m => m.GrupoId == grupoId &&
                       m.ColegioId == colegioId &&
                       !m.IsDeleted);

        if (soloActivas)
        {
            query = query.Where(m => m.Estado == EstadoMatricula.Activa);
        }

        return await query
            .OrderBy(m => m.Estudiante!.Persona!.Apellidos)
            .ThenBy(m => m.Estudiante!.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene todas las matr�culas de un a�o acad�mico
    /// </summary>
    public async Task<IEnumerable<Matricula>> GetByAnoAcademicoAsync(Guid anoAcademicoId, Guid colegioId, EstadoMatricula? estado = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(m => m.Estudiante)
                .ThenInclude(e => e!.Persona)
            .Include(m => m.Grupo)
                .ThenInclude(g => g!.AnoAcademico)
            .Where(m => m.Grupo!.AnoAcademicoId == anoAcademicoId &&
                       m.ColegioId == colegioId &&
                       !m.IsDeleted);

        if (estado.HasValue)
        {
            query = query.Where(m => m.Estado == estado.Value);
        }

        return await query
            .OrderBy(m => m.Grupo!.Nombre)
            .ThenBy(m => m.Estudiante!.Persona!.Apellidos)
            .ThenBy(m => m.Estudiante!.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Verifica si existe una matr�cula activa para un estudiante en un a�o acad�mico
    /// </summary>
    public async Task<bool> ExisteMatriculaActivaAsync(Guid estudianteId, Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(m => m.EstudianteId == estudianteId &&
                          m.Grupo!.AnoAcademicoId == anoAcademicoId &&
                          m.ColegioId == colegioId &&
                          m.Estado == EstadoMatricula.Activa &&
                          !m.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Obtiene el n�mero de matr�cula siguiente disponible para un a�o acad�mico
    /// </summary>
    public async Task<int> GetSiguienteNumeroMatriculaAsync(Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default)
    {
        // Buscar en matr�culas del a�o espec�fico a trav�s de la relaci�n Grupo->AnoAcademico
        var matriculasDelAno = await _dbSet
            .Include(m => m.Grupo)
            .Where(m => m.Grupo!.AnoAcademicoId == anoAcademicoId &&
                       m.ColegioId == colegioId &&
                       !m.IsDeleted)
            .Select(m => m.NumeroMatricula)
            .ToListAsync(cancellationToken);

        // Extraer la parte num�rica del formato "YYYY-NNNN"
        var a�o = DateTime.UtcNow.Year;
        var prefijo = $"{a�o}-";

        var numerosExistentes = matriculasDelAno
            .Where(n => n.StartsWith(prefijo))
            .Select(n => {
                var parteNumerica = n.Substring(prefijo.Length);
                return int.TryParse(parteNumerica, out var numero) ? numero : 0;
            })
            .ToList();

        var siguienteNumero = numerosExistentes.Any() ? numerosExistentes.Max() + 1 : 1;
        return siguienteNumero;
    }

    #endregion

    #region Consultas de Reportes y Estad�sticas

    /// <summary>
    /// Obtiene el conteo de matr�culas por estado en un a�o acad�mico
    /// </summary>
    public async Task<Dictionary<EstadoMatricula, int>> GetConteoByEstadoAsync(Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default)
    {
        var conteos = await _dbSet
            .Where(m => m.Grupo!.AnoAcademicoId == anoAcademicoId &&
                       m.ColegioId == colegioId &&
                       !m.IsDeleted)
            .GroupBy(m => m.Estado)
            .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
            .ToListAsync(cancellationToken);

        return conteos.ToDictionary(c => c.Estado, c => c.Cantidad);
    }

    /// <summary>
    /// Obtiene el conteo de matr�culas por tipo en un a�o acad�mico
    /// </summary>
    public async Task<Dictionary<TipoMatricula, int>> GetConteoByTipoAsync(Guid anoAcademicoId, Guid colegioId, CancellationToken cancellationToken = default)
    {
        var conteos = await _dbSet
            .Where(m => m.Grupo!.AnoAcademicoId == anoAcademicoId &&
                       m.ColegioId == colegioId &&
                       !m.IsDeleted)
            .GroupBy(m => m.TipoMatricula)
            .Select(g => new { Tipo = g.Key, Cantidad = g.Count() })
            .ToListAsync(cancellationToken);

        return conteos.ToDictionary(c => c.Tipo, c => c.Cantidad);
    }

    /// <summary>
    /// Obtiene las matr�culas con becas en un a�o acad�mico
    /// </summary>
    public async Task<IEnumerable<Matricula>> GetMatriculasConBecasAsync(Guid anoAcademicoId, Guid colegioId, TipoBecaMatricula? tipoBeca = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(m => m.Estudiante)
                .ThenInclude(e => e!.Persona)
            .Include(m => m.Grupo)
                .ThenInclude(g => g!.AnoAcademico)
            .Where(m => m.Grupo!.AnoAcademicoId == anoAcademicoId &&
                       m.ColegioId == colegioId &&
                       m.PorcentajeBeca > 0 &&
                       !m.IsDeleted);

        if (tipoBeca.HasValue)
        {
            query = query.Where(m => m.TipoBeca == tipoBeca.Value);
        }

        return await query
            .OrderBy(m => m.TipoBeca)
            .ThenBy(m => m.Estudiante!.Persona!.Apellidos)
            .ThenBy(m => m.Estudiante!.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene las matr�culas con pagos pendientes
    /// </summary>
    public async Task<IEnumerable<Matricula>> GetMatriculasConPagosPendientesAsync(Guid anoAcademicoId, Guid colegioId, int diasVencimiento = 30, CancellationToken cancellationToken = default)
    {
        var fechaLimite = DateTime.UtcNow.AddDays(-diasVencimiento);

        return await _dbSet
            .Include(m => m.Estudiante)
                .ThenInclude(e => e!.Persona)
            .Include(m => m.Grupo)
                .ThenInclude(g => g!.AnoAcademico)
            .Where(m => m.Grupo!.AnoAcademicoId == anoAcademicoId &&
                       m.ColegioId == colegioId &&
                       m.Estado == EstadoMatricula.Activa &&
                       m.MatriculaPagada == false &&
                       m.FechaMatricula <= fechaLimite &&
                       !m.IsDeleted)
            .OrderBy(m => m.FechaMatricula)
            .ThenBy(m => m.Estudiante!.Persona!.Apellidos)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Consultas de B�squeda y Filtrado

    /// <summary>
    /// Busca matr�culas por criterios m�ltiples
    /// </summary>
    public async Task<(IEnumerable<Matricula> Items, int TotalCount)> SearchAsync(
        Guid colegioId,
        Guid? anoAcademicoId = null,
        Guid? grupoId = null,
        EstadoMatricula? estado = null,
        TipoMatricula? tipo = null,
        string? buscarTexto = null,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(m => m.Estudiante)
                .ThenInclude(e => e!.Persona)
            .Include(m => m.Grupo)
                .ThenInclude(g => g!.AnoAcademico)
            .Where(m => m.ColegioId == colegioId && !m.IsDeleted);

        // Aplicar filtros
        if (anoAcademicoId.HasValue)
        {
            query = query.Where(m => m.Grupo!.AnoAcademicoId == anoAcademicoId.Value);
        }

        if (grupoId.HasValue)
        {
            query = query.Where(m => m.GrupoId == grupoId.Value);
        }

        if (estado.HasValue)
        {
            query = query.Where(m => m.Estado == estado.Value);
        }

        if (tipo.HasValue)
        {
            query = query.Where(m => m.TipoMatricula == tipo.Value);
        }

        if (!string.IsNullOrWhiteSpace(buscarTexto))
        {
            var searchText = buscarTexto.ToLower().Trim();
            query = query.Where(m =>
                m.Estudiante!.Persona!.Nombres.ToLower().Contains(searchText) ||
                m.Estudiante!.Persona!.Apellidos.ToLower().Contains(searchText) ||
                m.Estudiante!.Persona!.NumeroDocumento.Contains(searchText) ||
                m.Estudiante!.CodigoEstudiante.Contains(searchText) ||
                m.NumeroMatricula.Contains(searchText));
        }

        // Contar total antes de paginaci�n
        var totalCount = await query.CountAsync(cancellationToken);

        // Aplicar paginaci�n y ordenamiento
        var items = await query
            .OrderBy(m => m.Grupo!.AnoAcademico!.Nombre)
            .ThenBy(m => m.Grupo!.Nombre)
            .ThenBy(m => m.Estudiante!.Persona!.Apellidos)
            .ThenBy(m => m.Estudiante!.Persona!.Nombres)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <summary>
    /// Obtiene matr�culas que vencen en un rango de fechas
    /// </summary>
    public async Task<IEnumerable<Matricula>> GetMatriculasQueVencenAsync(DateTime fechaInicio, DateTime fechaFin, Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Estudiante)
                .ThenInclude(e => e!.Persona)
            .Include(m => m.Grupo)
                .ThenInclude(g => g!.AnoAcademico)
            .Where(m => m.ColegioId == colegioId &&
                       m.Estado == EstadoMatricula.Activa &&
                       m.MatriculaPagada == false &&
                       m.FechaMatricula >= fechaInicio &&
                       m.FechaMatricula <= fechaFin &&
                       !m.IsDeleted)
            .OrderBy(m => m.FechaMatricula)
            .ThenBy(m => m.Estudiante!.Persona!.Apellidos)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region M�todos de Validaci�n

    /// <summary>
    /// Verifica si un n�mero de matr�cula ya existe en un a�o acad�mico
    /// </summary>
    public async Task<bool> ExisteNumeroMatriculaAsync(int numeroMatricula, Guid anoAcademicoId, Guid colegioId, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var numeroString = numeroMatricula.ToString();
        var query = _dbSet
            .Where(m => m.NumeroMatricula.Contains(numeroString) &&
                       m.Grupo!.AnoAcademicoId == anoAcademicoId &&
                       m.ColegioId == colegioId &&
                       !m.IsDeleted);

        if (excludeId.HasValue)
        {
            query = query.Where(m => m.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Verifica si se puede eliminar una matr�cula
    /// </summary>
    public async Task<bool> CanDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var matricula = await GetByIdAsync(id, cancellationToken);
        if (matricula == null)
            return false;

        // Usar el m�todo de la entidad
        return matricula.CanBeDeleted();
    }

    #endregion
}