using ColegiosBackend.Core.Entities;
using ColegiosBackend.Core.Enums;
using ColegiosBackend.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ColegiosBackend.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio para la gestión de estudiantes
/// </summary>
public class EstudianteRepository : IEstudianteRepository
{
    private readonly DbContext _context;
    private readonly DbSet<Estudiante> _dbSet;

    public EstudianteRepository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<Estudiante>();
    }

    #region Métodos CRUD Básicos

    /// <summary>
    /// Obtiene un estudiante por su ID
    /// </summary>
    public async Task<Estudiante?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Obtiene un estudiante por su ID incluyendo entidades relacionadas
    /// </summary>
    public async Task<Estudiante?> GetByIdWithIncludesAsync(Guid id, string[] includeEntities, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();

        // Agregar includes dinámicamente
        foreach (var include in includeEntities)
        {
            query = query.Include(include);
        }

        return await query
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Agrega un nuevo estudiante
    /// </summary>
    public async Task AddAsync(Estudiante estudiante, CancellationToken cancellationToken = default)
    {
        if (estudiante == null)
            throw new ArgumentNullException(nameof(estudiante));

        await _dbSet.AddAsync(estudiante, cancellationToken);
    }

    /// <summary>
    /// Actualiza un estudiante existente
    /// </summary>
    public async Task UpdateAsync(Estudiante estudiante, CancellationToken cancellationToken = default)
    {
        if (estudiante == null)
            throw new ArgumentNullException(nameof(estudiante));

        estudiante.MarkAsUpdated();
        _dbSet.Update(estudiante);

        await Task.CompletedTask;
    }

    /// <summary>
    /// Elimina un estudiante (soft delete)
    /// </summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var estudiante = await GetByIdAsync(id, cancellationToken);
        if (estudiante != null)
        {
            estudiante.MarkAsDeleted();
            _dbSet.Update(estudiante);
        }
    }

    #endregion

    #region Consultas Específicas de Negocio

    /// <summary>
    /// Obtiene un estudiante por su PersonaId
    /// </summary>
    public async Task<Estudiante?> GetByPersonaIdAsync(Guid personaId, Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.Persona)
            .FirstOrDefaultAsync(e => e.PersonaId == personaId &&
                                    e.ColegioId == colegioId &&
                                    !e.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Obtiene un estudiante por su código único
    /// </summary>
    public async Task<Estudiante?> GetByCodigoAsync(string codigoEstudiante, Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.Persona)
            .FirstOrDefaultAsync(e => e.CodigoEstudiante == codigoEstudiante &&
                                    e.ColegioId == colegioId &&
                                    !e.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Obtiene un estudiante por su número de matrícula actual
    /// </summary>
    public async Task<Estudiante?> GetByNumeroMatriculaAsync(string numeroMatricula, Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.Persona)
            .FirstOrDefaultAsync(e => e.NumeroMatricula == numeroMatricula &&
                                    e.ColegioId == colegioId &&
                                    !e.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Obtiene todos los estudiantes de un colegio
    /// </summary>
    public async Task<IEnumerable<Estudiante>> GetByColegioAsync(Guid colegioId, EstadoEstudiante? estado = null, bool soloActivos = true, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(e => e.Persona)
            .Where(e => e.ColegioId == colegioId && !e.IsDeleted);

        if (estado.HasValue)
        {
            query = query.Where(e => e.Estado == estado.Value);
        }

        if (soloActivos)
        {
            query = query.Where(e => e.Activo);
        }

        return await query
            .OrderBy(e => e.Persona!.Apellidos)
            .ThenBy(e => e.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene estudiantes por año de ingreso
    /// </summary>
    public async Task<IEnumerable<Estudiante>> GetByAnoIngresoAsync(int anoIngreso, Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.Persona)
            .Where(e => e.AnoIngreso == anoIngreso &&
                       e.ColegioId == colegioId &&
                       !e.IsDeleted)
            .OrderBy(e => e.Persona!.Apellidos)
            .ThenBy(e => e.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene estudiantes por rango de edades
    /// </summary>
    public async Task<IEnumerable<Estudiante>> GetByRangoEdadAsync(int edadMinima, int edadMaxima, Guid colegioId, CancellationToken cancellationToken = default)
    {
        var fechaMaximaNacimiento = DateTime.Today.AddYears(-edadMinima);
        var fechaMinimaNacimiento = DateTime.Today.AddYears(-edadMaxima - 1);

        return await _dbSet
            .Include(e => e.Persona)
            .Where(e => e.ColegioId == colegioId &&
                       !e.IsDeleted &&
                       e.Persona!.FechaNacimiento.HasValue &&
                       e.Persona.FechaNacimiento.Value >= fechaMinimaNacimiento &&
                       e.Persona.FechaNacimiento.Value <= fechaMaximaNacimiento)
            .OrderBy(e => e.Persona!.FechaNacimiento)
            .ThenBy(e => e.Persona!.Apellidos)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Consultas de Acudientes

    /// <summary>
    /// Obtiene todos los acudientes de un estudiante
    /// </summary>
    public async Task<IEnumerable<EstudianteAcudiente>> GetAcudientesAsync(Guid estudianteId, bool soloActivos = true, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<EstudianteAcudiente>()
            .Include(ea => ea.Acudiente)
            .Where(ea => ea.EstudianteId == estudianteId && !ea.IsDeleted);

        if (soloActivos)
        {
            query = query.Where(ea => ea.EstaActiva);
        }

        return await query
            .OrderBy(ea => ea.OrdenPrioridad)
            .ThenByDescending(ea => ea.EsPrincipal)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene el acudiente principal de un estudiante
    /// </summary>
    public async Task<EstudianteAcudiente?> GetAcudientePrincipalAsync(Guid estudianteId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<EstudianteAcudiente>()
            .Include(ea => ea.Acudiente)
            .FirstOrDefaultAsync(ea => ea.EstudianteId == estudianteId &&
                                     ea.EsPrincipal &&
                                     ea.EstaActiva &&
                                     !ea.IsDeleted, cancellationToken);
    }

    /// <summary>
    /// Obtiene estudiantes que tienen a una persona como acudiente
    /// </summary>
    public async Task<IEnumerable<Estudiante>> GetEstudiantesByAcudienteAsync(Guid acudienteId, Guid colegioId, bool soloActivos = true, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(e => e.Persona)
            .Include(e => e.Acudientes.Where(a => a.AcudienteId == acudienteId))
            .Where(e => e.ColegioId == colegioId &&
                       !e.IsDeleted &&
                       e.Acudientes.Any(a => a.AcudienteId == acudienteId && !a.IsDeleted));

        if (soloActivos)
        {
            query = query.Where(e => e.Activo &&
                                   e.Acudientes.Any(a => a.AcudienteId == acudienteId && a.EstaActiva));
        }

        return await query
            .OrderBy(e => e.Persona!.Apellidos)
            .ThenBy(e => e.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Consultas de Reportes y Estadísticas

    /// <summary>
    /// Obtiene el conteo de estudiantes por estado
    /// </summary>
    public async Task<Dictionary<EstadoEstudiante, int>> GetConteoByEstadoAsync(Guid colegioId, CancellationToken cancellationToken = default)
    {
        var conteos = await _dbSet
            .Where(e => e.ColegioId == colegioId && !e.IsDeleted)
            .GroupBy(e => e.Estado)
            .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
            .ToListAsync(cancellationToken);

        return conteos.ToDictionary(c => c.Estado, c => c.Cantidad);
    }

    /// <summary>
    /// Obtiene el conteo de estudiantes por año de ingreso
    /// </summary>
    public async Task<Dictionary<int, int>> GetConteoByAnoIngresoAsync(Guid colegioId, CancellationToken cancellationToken = default)
    {
        var conteos = await _dbSet
            .Where(e => e.ColegioId == colegioId && !e.IsDeleted)
            .GroupBy(e => e.AnoIngreso)
            .Select(g => new { Ano = g.Key, Cantidad = g.Count() })
            .ToListAsync(cancellationToken);

        return conteos.ToDictionary(c => c.Ano, c => c.Cantidad);
    }

    /// <summary>
    /// Obtiene el conteo de estudiantes por género
    /// </summary>
    public async Task<Dictionary<string, int>> GetConteoByGeneroAsync(Guid colegioId, CancellationToken cancellationToken = default)
    {
        var conteos = await _dbSet
            .Include(e => e.Persona)
            .Where(e => e.ColegioId == colegioId && !e.IsDeleted)
            .GroupBy(e => e.Persona!.Genero.HasValue ? e.Persona.Genero.ToString() : "No especificado")
            .Select(g => new { Genero = g.Key, Cantidad = g.Count() })
            .ToListAsync(cancellationToken);

        return conteos.ToDictionary(c => c.Genero!, c => c.Cantidad);
    }

    /// <summary>
    /// Obtiene estudiantes con información médica relevante
    /// </summary>
    public async Task<IEnumerable<Estudiante>> GetConInformacionMedicaAsync(Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.Persona)
            .Where(e => e.ColegioId == colegioId &&
                       !e.IsDeleted &&
                       !string.IsNullOrWhiteSpace(e.InformacionMedica))
            .OrderBy(e => e.Persona!.Apellidos)
            .ThenBy(e => e.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene estudiantes sin acudientes activos
    /// </summary>
    public async Task<IEnumerable<Estudiante>> GetSinAcudientesActivosAsync(Guid colegioId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.Persona)
            .Where(e => e.ColegioId == colegioId &&
                       !e.IsDeleted &&
                       e.Activo &&
                       !e.Acudientes.Any(a => a.EstaActiva && !a.IsDeleted))
            .OrderBy(e => e.Persona!.Apellidos)
            .ThenBy(e => e.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Consultas de Búsqueda y Filtrado

    /// <summary>
    /// Busca estudiantes por criterios múltiples
    /// </summary>
    public async Task<(IEnumerable<Estudiante> Items, int TotalCount)> SearchAsync(
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
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(e => e.Persona)
            .Where(e => e.ColegioId == colegioId && !e.IsDeleted);

        // Aplicar filtros
        if (!string.IsNullOrWhiteSpace(buscarTexto))
        {
            var searchText = buscarTexto.ToLower().Trim();
            query = query.Where(e =>
                e.Persona!.Nombres.ToLower().Contains(searchText) ||
                e.Persona!.Apellidos.ToLower().Contains(searchText) ||
                e.Persona!.NumeroDocumento.Contains(searchText) ||
                e.CodigoEstudiante.Contains(searchText) ||
                (e.NumeroMatricula != null && e.NumeroMatricula.Contains(searchText)));
        }

        if (estado.HasValue)
        {
            query = query.Where(e => e.Estado == estado.Value);
        }

        if (anoIngresoDesde.HasValue)
        {
            query = query.Where(e => e.AnoIngreso >= anoIngresoDesde.Value);
        }

        if (anoIngresoHasta.HasValue)
        {
            query = query.Where(e => e.AnoIngreso <= anoIngresoHasta.Value);
        }

        if (edadDesde.HasValue || edadHasta.HasValue)
        {
            var fechaMaxima = edadDesde.HasValue ? DateTime.Today.AddYears(-edadDesde.Value) : DateTime.MaxValue;
            var fechaMinima = edadHasta.HasValue ? DateTime.Today.AddYears(-edadHasta.Value - 1) : DateTime.MinValue;

            query = query.Where(e => e.Persona!.FechaNacimiento.HasValue &&
                                   e.Persona.FechaNacimiento.Value >= fechaMinima &&
                                   e.Persona.FechaNacimiento.Value <= fechaMaxima);
        }

        if (genero.HasValue)
        {
            query = query.Where(e => e.Persona!.Genero == genero.Value);
        }

        if (soloActivos)
        {
            query = query.Where(e => e.Activo);
        }

        // Contar total antes de paginación
        var totalCount = await query.CountAsync(cancellationToken);

        // Aplicar paginación y ordenamiento
        var items = await query
            .OrderBy(e => e.Persona!.Apellidos)
            .ThenBy(e => e.Persona!.Nombres)
            .ThenBy(e => e.CodigoEstudiante)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <summary>
    /// Busca estudiantes por número de documento de identidad
    /// </summary>
    public async Task<IEnumerable<Estudiante>> GetByNumeroDocumentoAsync(string numeroDocumento, Guid? colegioId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(e => e.Persona)
            .Where(e => e.Persona!.NumeroDocumento == numeroDocumento && !e.IsDeleted);

        if (colegioId.HasValue)
        {
            query = query.Where(e => e.ColegioId == colegioId.Value);
        }

        return await query
            .OrderBy(e => e.Persona!.Apellidos)
            .ThenBy(e => e.Persona!.Nombres)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Obtiene estudiantes que cumplen años en un rango de fechas
    /// </summary>
    public async Task<IEnumerable<Estudiante>> GetCumpleanosAsync(DateTime fechaInicio, DateTime fechaFin, Guid colegioId, CancellationToken cancellationToken = default)
    {
        // Convertir fechas a formato MM-dd para comparación de cumpleaños
        var mesInicio = fechaInicio.Month;
        var diaInicio = fechaInicio.Day;
        var mesFin = fechaFin.Month;
        var diaFin = fechaFin.Day;

        var query = _dbSet
            .Include(e => e.Persona)
            .Where(e => e.ColegioId == colegioId &&
                       !e.IsDeleted &&
                       e.Activo &&
                       e.Persona!.FechaNacimiento.HasValue);

        // Si el rango no cruza el año (ej: enero a marzo)
        if (mesInicio <= mesFin)
        {
            query = query.Where(e =>
                (e.Persona!.FechaNacimiento!.Value.Month > mesInicio ||
                 (e.Persona.FechaNacimiento.Value.Month == mesInicio && e.Persona.FechaNacimiento.Value.Day >= diaInicio)) &&
                (e.Persona.FechaNacimiento.Value.Month < mesFin ||
                 (e.Persona.FechaNacimiento.Value.Month == mesFin && e.Persona.FechaNacimiento.Value.Day <= diaFin)));
        }
        // Si el rango cruza el año (ej: diciembre a febrero)
        else
        {
            query = query.Where(e =>
                (e.Persona!.FechaNacimiento!.Value.Month > mesInicio ||
                 (e.Persona.FechaNacimiento.Value.Month == mesInicio && e.Persona.FechaNacimiento.Value.Day >= diaInicio)) ||
                (e.Persona.FechaNacimiento.Value.Month < mesFin ||
                 (e.Persona.FechaNacimiento.Value.Month == mesFin && e.Persona.FechaNacimiento.Value.Day <= diaFin)));
        }

        return await query
            .OrderBy(e => e.Persona!.FechaNacimiento!.Value.Month)
            .ThenBy(e => e.Persona!.FechaNacimiento!.Value.Day)
            .ThenBy(e => e.Persona!.Apellidos)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Métodos de Validación

    /// <summary>
    /// Verifica si un código de estudiante ya existe
    /// </summary>
    public async Task<bool> ExisteCodigoAsync(string codigoEstudiante, Guid colegioId, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(e => e.CodigoEstudiante == codigoEstudiante &&
                       e.ColegioId == colegioId &&
                       !e.IsDeleted);

        if (excludeId.HasValue)
        {
            query = query.Where(e => e.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Verifica si una persona ya es estudiante en el colegio
    /// </summary>
    public async Task<bool> ExistePersonaComoEstudianteAsync(Guid personaId, Guid colegioId, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(e => e.PersonaId == personaId &&
                       e.ColegioId == colegioId &&
                       !e.IsDeleted);

        if (excludeId.HasValue)
        {
            query = query.Where(e => e.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Verifica si se puede eliminar un estudiante
    /// </summary>
    public async Task<bool> CanDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var estudiante = await GetByIdAsync(id, cancellationToken);
        if (estudiante == null)
            return false;

        // Usar el método de la entidad
        return estudiante.CanBeDeleted();
    }

    /// <summary>
    /// Obtiene el siguiente código de estudiante disponible
    /// </summary>
    public async Task<string> GetSiguienteCodigoAsync(Guid colegioId, int anoIngreso, CancellationToken cancellationToken = default)
    {
        var prefijo = $"EST-{anoIngreso}-";

        var ultimoCodigo = await _dbSet
            .Where(e => e.ColegioId == colegioId &&
                       e.AnoIngreso == anoIngreso &&
                       e.CodigoEstudiante.StartsWith(prefijo) &&
                       !e.IsDeleted)
            .Select(e => e.CodigoEstudiante)
            .OrderByDescending(c => c)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrEmpty(ultimoCodigo))
        {
            return $"{prefijo}0001";
        }

        // Extraer la parte numérica
        var parteNumerica = ultimoCodigo.Substring(prefijo.Length);
        if (int.TryParse(parteNumerica, out var numero))
        {
            return $"{prefijo}{(numero + 1):D4}";
        }

        return $"{prefijo}0001";
    }

    #endregion
}