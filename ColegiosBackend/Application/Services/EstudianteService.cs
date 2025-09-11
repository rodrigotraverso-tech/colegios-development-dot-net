using AutoMapper;
using ColegiosBackend.Application.DTOs.Estudiante;
using ColegiosBackend.Application.Interfaces;
using ColegiosBackend.Core.Entities;
using ColegiosBackend.Core.Enums;
using ColegiosBackend.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace ColegiosBackend.Application.Services;

/// <summary>
/// Implementación del servicio de gestión de estudiantes
/// </summary>
public class EstudianteService : IEstudianteService
{
    private readonly IEstudianteRepository _estudianteRepository;
    private readonly IMatriculaRepository _matriculaRepository;
    private readonly IAsistenciaRepository _asistenciaRepository;
    private readonly ICalificacionRepository _calificacionRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<EstudianteService> _logger;

    public EstudianteService(
        IEstudianteRepository estudianteRepository,
        IMatriculaRepository matriculaRepository,
        IAsistenciaRepository asistenciaRepository,
        ICalificacionRepository calificacionRepository,
        IMapper mapper,
        ILogger<EstudianteService> logger)
    {
        _estudianteRepository = estudianteRepository ?? throw new ArgumentNullException(nameof(estudianteRepository));
        _matriculaRepository = matriculaRepository ?? throw new ArgumentNullException(nameof(matriculaRepository));
        _asistenciaRepository = asistenciaRepository ?? throw new ArgumentNullException(nameof(asistenciaRepository));
        _calificacionRepository = calificacionRepository ?? throw new ArgumentNullException(nameof(calificacionRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Consultas

    public async Task<EstudianteDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var includeEntities = new[] { "Persona", "Colegio" };
            var estudiante = await _estudianteRepository.GetByIdWithIncludesAsync(id, includeEntities, cancellationToken);

            return estudiante != null ? _mapper.Map<EstudianteDto>(estudiante) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estudiante por ID: {EstudianteId}", id);
            throw;
        }
    }

    public async Task<EstudianteDetalleDto?> GetDetalleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var includeEntities = new[] {
                "Persona",
                "Colegio",
                "Acudientes",
                "Acudientes.Persona"
            };

            var estudiante = await _estudianteRepository.GetByIdWithIncludesAsync(id, includeEntities, cancellationToken);

            if (estudiante == null)
                return null;

            var estudianteDetalle = _mapper.Map<EstudianteDetalleDto>(estudiante);

            // Obtener historial de matrículas
            var matriculas = await _matriculaRepository.GetByEstudianteAsync(id, estudiante.ColegioId!.Value, cancellationToken);
            estudianteDetalle.HistorialMatriculas = _mapper.Map<List<MatriculaEstudianteDto>>(matriculas);

            // Obtener estadísticas académicas actuales
            estudianteDetalle.EstadisticasActuales = await ObtenerEstadisticasAcademicasAsync(id, cancellationToken);

            return estudianteDetalle;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener detalle del estudiante: {EstudianteId}", id);
            throw;
        }
    }

    public async Task<EstudianteDto?> GetByCodigoAsync(string codigoEstudiante, Guid colegioId, CancellationToken cancellationToken = default)
    {
        try
        {
            var estudiante = await _estudianteRepository.GetByCodigoAsync(codigoEstudiante, colegioId, cancellationToken);
            return estudiante != null ? _mapper.Map<EstudianteDto>(estudiante) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estudiante por código: {CodigoEstudiante} en colegio: {ColegioId}",
                codigoEstudiante, colegioId);
            throw;
        }
    }

    public async Task<(IEnumerable<EstudianteDto> Items, int TotalCount)> GetAllByColegioAsync(
        Guid colegioId,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Usar método correcto con parámetros adecuados
            var estudiantes = await _estudianteRepository.GetByColegioAsync(colegioId, null, true, cancellationToken);
            var totalCount = estudiantes.Count();

            var estudiantesPaginados = estudiantes
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var estudiantesDto = _mapper.Map<IEnumerable<EstudianteDto>>(estudiantesPaginados);

            return (estudiantesDto, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estudiantes del colegio: {ColegioId}", colegioId);
            throw;
        }
    }

    public async Task<(IEnumerable<EstudianteDto> Items, int TotalCount)> BuscarEstudiantesAsync(
        Guid colegioId,
        string? filtroTexto = null,
        Guid? gradoId = null,
        Guid? grupoId = null,
        EstadoMatricula? estadoMatricula = null,
        DateTime? fechaMatriculaDesde = null,
        DateTime? fechaMatriculaHasta = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Implementación simplificada usando métodos existentes
            var estudiantes = await _estudianteRepository.GetByColegioAsync(colegioId, null, true, cancellationToken);

            // Aplicar filtros básicos en memoria (se puede optimizar posteriormente)
            var estudiantesFiltrados = estudiantes.AsQueryable();

            if (!string.IsNullOrEmpty(filtroTexto))
            {
                estudiantesFiltrados = estudiantesFiltrados.Where(e =>
                    e.CodigoEstudiante.Contains(filtroTexto) ||
                    (e.Persona != null &&
                     (e.Persona.Nombres.Contains(filtroTexto) ||
                      e.Persona.Apellidos.Contains(filtroTexto) ||
                      e.Persona.NumeroDocumento.Contains(filtroTexto))));
            }

            var totalCount = estudiantesFiltrados.Count();
            var estudiantesPaginados = estudiantesFiltrados
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var estudiantesDto = _mapper.Map<IEnumerable<EstudianteDto>>(estudiantesPaginados);

            return (estudiantesDto, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar estudiantes con filtros en colegio: {ColegioId}", colegioId);
            throw;
        }
    }

    public async Task<IEnumerable<EstudianteDto>> GetByGrupoAsync(Guid grupoId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Obtener el colegioId desde el primer estudiante encontrado
            var todasMatriculas = await _matriculaRepository.GetByGrupoAsync(grupoId, Guid.Empty, true, cancellationToken);
            var estudiantesIds = todasMatriculas.Where(m => m.Estado == EstadoMatricula.Activa)
                                              .Select(m => m.EstudianteId)
                                              .Distinct();

            var estudiantes = new List<Estudiante>();
            foreach (var estudianteId in estudiantesIds)
            {
                var estudiante = await _estudianteRepository.GetByIdAsync(estudianteId, cancellationToken);
                if (estudiante != null)
                    estudiantes.Add(estudiante);
            }

            return _mapper.Map<IEnumerable<EstudianteDto>>(estudiantes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estudiantes del grupo: {GrupoId}", grupoId);
            throw;
        }
    }

    public async Task<IEnumerable<EstudianteDto>> GetHermanosAsync(Guid estudianteId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Implementación simplificada - obtener hermanos a través de relaciones familiares
            var estudiante = await _estudianteRepository.GetByIdAsync(estudianteId, cancellationToken);
            if (estudiante == null)
                return new List<EstudianteDto>();

            // Por ahora retornamos lista vacía - se puede implementar la lógica completa después
            return new List<EstudianteDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener hermanos del estudiante: {EstudianteId}", estudianteId);
            throw;
        }
    }

    #endregion

    #region Operaciones

    public async Task<EstudianteDto> CrearAsync(CrearEstudianteDto crearEstudianteDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creando nuevo estudiante con código: {CodigoEstudiante}", crearEstudianteDto.CodigoEstudiante);

            // Validar que el código no exista
            var codigoDisponible = await CodigoDisponibleAsync(
                crearEstudianteDto.CodigoEstudiante,
                crearEstudianteDto.ColegioId,
                null,
                cancellationToken);

            if (!codigoDisponible)
            {
                throw new InvalidOperationException($"El código de estudiante '{crearEstudianteDto.CodigoEstudiante}' ya existe en el colegio.");
            }

            var estudiante = new Estudiante(
                crearEstudianteDto.ColegioId,
                crearEstudianteDto.PersonaId,
                crearEstudianteDto.CodigoEstudiante,
                crearEstudianteDto.FechaIngreso,
                crearEstudianteDto.AnoIngreso,
                crearEstudianteDto.NumeroMatricula);

            // Agregar información adicional si se proporciona
            if (!string.IsNullOrEmpty(crearEstudianteDto.InformacionMedica))
            {
                estudiante.ActualizarInformacionMedica(crearEstudianteDto.InformacionMedica);
            }

            if (!string.IsNullOrEmpty(crearEstudianteDto.ContactoEmergenciaNombre))
            {
                estudiante.ActualizarContactoEmergencia(
                    crearEstudianteDto.ContactoEmergenciaNombre,
                    crearEstudianteDto.ContactoEmergenciaTelefono,
                    crearEstudianteDto.ContactoEmergenciaRelacion);
            }

            if (!string.IsNullOrEmpty(crearEstudianteDto.Observaciones))
            {
                estudiante.ActualizarObservaciones(crearEstudianteDto.Observaciones);
            }

            await _estudianteRepository.AddAsync(estudiante, cancellationToken);

            _logger.LogInformation("Estudiante creado exitosamente: {EstudianteId}", estudiante.Id);

            return _mapper.Map<EstudianteDto>(estudiante);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear estudiante con código: {CodigoEstudiante}", crearEstudianteDto.CodigoEstudiante);
            throw;
        }
    }

    public async Task<EstudianteDto> ActualizarAsync(Guid id, ActualizarEstudianteDto actualizarEstudianteDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var estudiante = await _estudianteRepository.GetByIdAsync(id, cancellationToken);

            if (estudiante == null)
            {
                throw new InvalidOperationException($"No se encontró el estudiante con ID: {id}");
            }

            // Actualizar campos modificables
            if (!string.IsNullOrEmpty(actualizarEstudianteDto.NumeroMatricula))
            {
                estudiante.ActualizarMatricula(actualizarEstudianteDto.NumeroMatricula);
            }

            estudiante.ActualizarInformacionMedica(actualizarEstudianteDto.InformacionMedica);

            estudiante.ActualizarContactoEmergencia(
                actualizarEstudianteDto.ContactoEmergenciaNombre,
                actualizarEstudianteDto.ContactoEmergenciaTelefono,
                actualizarEstudianteDto.ContactoEmergenciaRelacion);

            estudiante.ActualizarObservaciones(actualizarEstudianteDto.Observaciones);

            if (!actualizarEstudianteDto.Activo)
            {
                estudiante.Desactivar();
            }
            else
            {
                estudiante.Activar();
            }

            await _estudianteRepository.UpdateAsync(estudiante, cancellationToken);

            _logger.LogInformation("Estudiante actualizado exitosamente: {EstudianteId}", id);

            return _mapper.Map<EstudianteDto>(estudiante);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estudiante: {EstudianteId}", id);
            throw;
        }
    }

    public async Task<bool> EliminarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var puedeSerEliminado = await PuedeSerEliminadoAsync(id, cancellationToken);

            if (!puedeSerEliminado)
            {
                var dependencias = await GetDependenciasEliminacionAsync(id, cancellationToken);
                var mensajeDependencias = string.Join(", ", dependencias);
                throw new InvalidOperationException($"No se puede eliminar el estudiante debido a las siguientes dependencias: {mensajeDependencias}");
            }

            await _estudianteRepository.DeleteAsync(id, cancellationToken);

            _logger.LogInformation("Estudiante eliminado exitosamente: {EstudianteId}", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar estudiante: {EstudianteId}", id);
            throw;
        }
    }

    public async Task<bool> TransferirGrupoAsync(Guid estudianteId, Guid nuevoGrupoId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Implementación simplificada - actualizar la matrícula activa del estudiante
            var estudiante = await _estudianteRepository.GetByIdAsync(estudianteId, cancellationToken);
            if (estudiante == null)
                return false;

            var matriculas = await _matriculaRepository.GetByEstudianteAsync(estudianteId, estudiante.ColegioId!.Value, cancellationToken);
            var matriculaActiva = matriculas.FirstOrDefault(m => m.Estado == EstadoMatricula.Activa);

            if (matriculaActiva != null)
            {
                // Como no existe CambiarGrupo, usamos RegistrarTraslado y creamos nueva matrícula
                matriculaActiva.RegistrarTraslado(DateTime.UtcNow, "Transferencia de grupo");
                await _matriculaRepository.UpdateAsync(matriculaActiva, cancellationToken);

                // Crear nueva matrícula en el nuevo grupo - usar TipoMatricula.Regular que existe
                var nuevaMatricula = new Matricula(
                    estudiante.ColegioId.Value,
                    estudianteId,
                    nuevoGrupoId,
                    matriculaActiva.NumeroMatricula,
                    DateTime.UtcNow,
                    TipoMatricula.Regular);

                await _matriculaRepository.AddAsync(nuevaMatricula, cancellationToken);

                _logger.LogInformation("Estudiante transferido exitosamente: {EstudianteId} al grupo: {GrupoId}",
                    estudianteId, nuevoGrupoId);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al transferir estudiante: {EstudianteId} al grupo: {GrupoId}",
                estudianteId, nuevoGrupoId);
            throw;
        }
    }

    public async Task<bool> CambiarEstadoMatriculaAsync(Guid estudianteId, EstadoMatricula nuevoEstado, string? observaciones = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var estudiante = await _estudianteRepository.GetByIdAsync(estudianteId, cancellationToken);
            if (estudiante == null)
                return false;

            var matriculas = await _matriculaRepository.GetByEstudianteAsync(estudianteId, estudiante.ColegioId!.Value, cancellationToken);
            var matriculaActiva = matriculas.FirstOrDefault(m => m.Estado == EstadoMatricula.Activa);

            if (matriculaActiva != null)
            {
                // Como no existe CambiarEstado, usamos los métodos existentes según el estado
                switch (nuevoEstado)
                {
                    case EstadoMatricula.Retirada:
                        matriculaActiva.RegistrarRetiro(DateTime.UtcNow, observaciones ?? "Retiro del estudiante");
                        break;
                    case EstadoMatricula.Graduada:
                        matriculaActiva.RegistrarGraduacion(DateTime.UtcNow);
                        break;
                    case EstadoMatricula.Trasladada:
                        matriculaActiva.RegistrarTraslado(DateTime.UtcNow, observaciones ?? "Traslado del estudiante");
                        break;
                        // Para otros estados, actualizar directamente si es necesario
                }

                await _matriculaRepository.UpdateAsync(matriculaActiva, cancellationToken);

                _logger.LogInformation("Estado de matrícula cambiado exitosamente para estudiante: {EstudianteId} a: {NuevoEstado}",
                    estudianteId, nuevoEstado);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado de matrícula del estudiante: {EstudianteId}", estudianteId);
            throw;
        }
    }

    #endregion

    #region Reportes y Estadísticas

    public async Task<EstadisticasEstudiantesDto> GetEstadisticasAsync(Guid colegioId, CancellationToken cancellationToken = default)
    {
        try
        {
            var estudiantes = await _estudianteRepository.GetByColegioAsync(colegioId, null, true, cancellationToken);

            var estadisticas = new EstadisticasEstudiantesDto
            {
                TotalEstudiantes = estudiantes.Count(),
                EstudiantesActivos = estudiantes.Count(e => e.Estado == EstadoEstudiante.Activo),
                EstudiantesInactivos = estudiantes.Count(e => !e.Activo),
                EstudiantesRetirados = estudiantes.Count(e => e.Estado == EstadoEstudiante.Retirado),
                EstudiantesNuevos = estudiantes.Count(e => e.AnoIngreso == DateTime.Now.Year)
            };

            return estadisticas;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas de estudiantes del colegio: {ColegioId}", colegioId);
            throw;
        }
    }

    public async Task<IEnumerable<ReporteEstudiantesPorGradoDto>> GetReporteEstudiantesPorGradoAsync(Guid colegioId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Implementación simplificada - retorna lista vacía por ahora
            return new List<ReporteEstudiantesPorGradoDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener reporte de estudiantes por grado del colegio: {ColegioId}", colegioId);
            throw;
        }
    }

    public async Task<IEnumerable<ReporteEstudianteFamiliaDto>> GetReporteEstudiantesConFamiliaAsync(
        Guid colegioId,
        Guid? gradoId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Implementación simplificada - retorna lista vacía por ahora
            return new List<ReporteEstudianteFamiliaDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener reporte de estudiantes con familia del colegio: {ColegioId}", colegioId);
            throw;
        }
    }

    #endregion

    #region Validaciones

    public async Task<bool> CodigoDisponibleAsync(string codigoEstudiante, Guid colegioId, Guid? estudianteId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var estudianteExistente = await _estudianteRepository.GetByCodigoAsync(codigoEstudiante, colegioId, cancellationToken);

            if (estudianteExistente == null)
                return true;

            // Si existe pero es el mismo estudiante que estamos editando, está disponible
            if (estudianteId.HasValue && estudianteExistente.Id == estudianteId.Value)
                return true;

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar disponibilidad del código: {CodigoEstudiante}", codigoEstudiante);
            throw;
        }
    }

    public async Task<bool> PuedeSerEliminadoAsync(Guid estudianteId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Verificar si tiene matrículas
            var estudiante = await _estudianteRepository.GetByIdAsync(estudianteId, cancellationToken);
            if (estudiante == null)
                return false;

            var matriculas = await _matriculaRepository.GetByEstudianteAsync(estudianteId, estudiante.ColegioId!.Value, cancellationToken);
            if (matriculas.Any(m => m.Estado == EstadoMatricula.Activa))
                return false;

            // Verificar si tiene calificaciones - usando método correcto con parámetros adecuados
            var calificaciones = await _calificacionRepository.GetByEstudianteYPeriodoAsync(estudianteId, Guid.Empty, estudiante.ColegioId.Value, cancellationToken);
            if (calificaciones.Any())
                return false;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar si el estudiante puede ser eliminado: {EstudianteId}", estudianteId);
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetDependenciasEliminacionAsync(Guid estudianteId, CancellationToken cancellationToken = default)
    {
        try
        {
            var dependencias = new List<string>();

            var estudiante = await _estudianteRepository.GetByIdAsync(estudianteId, cancellationToken);
            if (estudiante == null)
                return dependencias;

            // Verificar matrículas activas
            var matriculas = await _matriculaRepository.GetByEstudianteAsync(estudianteId, estudiante.ColegioId!.Value, cancellationToken);
            if (matriculas.Any(m => m.Estado == EstadoMatricula.Activa))
                dependencias.Add("Matrículas activas");

            // Verificar calificaciones - usando método correcto
            var calificaciones = await _calificacionRepository.GetByEstudianteYPeriodoAsync(estudianteId, Guid.Empty, estudiante.ColegioId.Value, cancellationToken);
            if (calificaciones.Any())
                dependencias.Add("Calificaciones registradas");

            // Verificar asistencias - usando método disponible en IAsistenciaRepository
            var asistencias = await _asistenciaRepository.GetAllAsync(estudiante.ColegioId.Value, false, null, cancellationToken);
            var asistenciasEstudiante = asistencias.Where(a => a.EstudianteId == estudianteId);
            if (asistenciasEstudiante.Any())
                dependencias.Add("Registros de asistencia");

            return dependencias;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener dependencias de eliminación del estudiante: {EstudianteId}", estudianteId);
            throw;
        }
    }

    #endregion

    #region Métodos Privados

    private async Task<EstadisticasAcademicasEstudianteDto?> ObtenerEstadisticasAcademicasAsync(Guid estudianteId, CancellationToken cancellationToken)
    {
        try
        {
            // Obtener matrícula activa actual
            var estudiante = await _estudianteRepository.GetByIdAsync(estudianteId, cancellationToken);
            if (estudiante == null)
                return null;

            var matriculasEstudiante = await _matriculaRepository.GetByEstudianteAsync(estudianteId, estudiante.ColegioId!.Value, cancellationToken);
            var matriculaActual = matriculasEstudiante.FirstOrDefault(m => m.Estado == EstadoMatricula.Activa);

            if (matriculaActual == null)
                return null;

            // Obtener calificaciones del estudiante - usando método correcto
            var calificacionesActuales = await _calificacionRepository.GetByEstudianteYPeriodoAsync(estudianteId, Guid.Empty, estudiante.ColegioId.Value, cancellationToken);

            // Obtener asistencias del estudiante - usando método disponible
            var todasAsistencias = await _asistenciaRepository.GetAllAsync(estudiante.ColegioId.Value, false, null, cancellationToken);
            var asistenciasActuales = todasAsistencias.Where(a => a.EstudianteId == estudianteId);

            // Calcular estadísticas
            var estadisticas = new EstadisticasAcademicasEstudianteDto
            {
                // Usar propiedades correctas de la entidad Calificacion
                NumeroMaterias = calificacionesActuales.GroupBy(c => c.Asignacion?.MateriaId ?? Guid.Empty).Count(),
                PromedioGeneral = calificacionesActuales.Any() ? calificacionesActuales.Average(c => c.CalificacionValor) : 0,
                PorcentajeAsistencia = CalcularPorcentajeAsistencia(asistenciasActuales),
                FaltasInjustificadas = asistenciasActuales.Count(a => a.Estado == EstadoAsistencia.Ausente && !a.EsJustificada),
                FaltasJustificadas = asistenciasActuales.Count(a => a.Estado == EstadoAsistencia.Ausente && a.EsJustificada),
                Tardanzas = asistenciasActuales.Count(a => a.Estado == EstadoAsistencia.Tardanza),
                PeriodoEvaluativo = "Período Actual"
            };

            // Calcular materias aprobadas/reprobadas (nota mínima 60)
            var materiasPorEstado = calificacionesActuales
                .GroupBy(c => c.Asignacion?.MateriaId ?? Guid.Empty)
                .Select(g => new { MateriaId = g.Key, Promedio = g.Average(c => c.CalificacionValor) })
                .ToList();

            estadisticas.MateriasAprobadas = materiasPorEstado.Count(m => m.Promedio >= 60);
            estadisticas.MateriasReprobadas = materiasPorEstado.Count(m => m.Promedio < 60);
            estadisticas.MateriasEnRiesgo = materiasPorEstado.Count(m => m.Promedio >= 60 && m.Promedio < 70);

            return estadisticas;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error al calcular estadísticas académicas para estudiante: {EstudianteId}", estudianteId);
            return null;
        }
    }

    private static decimal CalcularPorcentajeAsistencia(IEnumerable<Asistencia> asistencias)
    {
        if (!asistencias.Any())
            return 100;

        var totalClases = asistencias.Count();
        var clasesPresente = asistencias.Count(a => a.Estado == EstadoAsistencia.Presente || a.Estado == EstadoAsistencia.Tardanza);

        return Math.Round((decimal)clasesPresente / totalClases * 100, 2);
    }

    #endregion
}