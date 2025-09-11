using Microsoft.AspNetCore.Mvc;
using ColegiosBackend.Application.DTOs.Estudiante;
using ColegiosBackend.Application.Interfaces;
using ColegiosBackend.Core.Enums;
using Microsoft.AspNetCore.Authorization;

namespace ColegiosBackend.WebApi.Controllers;

/// <summary>
/// Controlador para la gestión de estudiantes
/// Proporciona endpoints para CRUD y operaciones específicas de estudiantes
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Requiere autenticación para todos los endpoints
public class EstudiantesController : ControllerBase
{
    private readonly IEstudianteService _estudianteService;
    private readonly ILogger<EstudiantesController> _logger;

    public EstudiantesController(
        IEstudianteService estudianteService,
        ILogger<EstudiantesController> logger)
    {
        _estudianteService = estudianteService ?? throw new ArgumentNullException(nameof(estudianteService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // ==============================================
    // ENDPOINTS DE CONSULTA
    // ==============================================

    /// <summary>
    /// Obtiene un estudiante por su ID
    /// </summary>
    /// <param name="id">ID del estudiante</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Información básica del estudiante</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EstudianteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EstudianteDto>> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest("El ID del estudiante no puede estar vacío");

            var estudiante = await _estudianteService.GetByIdAsync(id, cancellationToken);

            if (estudiante == null)
                return NotFound($"No se encontró el estudiante con ID: {id}");

            return Ok(estudiante);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estudiante por ID: {EstudianteId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error interno del servidor al obtener el estudiante");
        }
    }

    /// <summary>
    /// Obtiene información detallada de un estudiante por su ID
    /// </summary>
    /// <param name="id">ID del estudiante</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Información detallada del estudiante</returns>
    [HttpGet("{id:guid}/detalle")]
    [ProducesResponseType(typeof(EstudianteDetalleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EstudianteDetalleDto>> GetDetalle(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest("El ID del estudiante no puede estar vacío");

            var estudiante = await _estudianteService.GetDetalleByIdAsync(id, cancellationToken);

            if (estudiante == null)
                return NotFound($"No se encontró el estudiante con ID: {id}");

            return Ok(estudiante);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener detalle del estudiante: {EstudianteId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error interno del servidor al obtener el detalle del estudiante");
        }
    }

    /// <summary>
    /// Obtiene un estudiante por su código en un colegio específico
    /// </summary>
    /// <param name="codigoEstudiante">Código del estudiante</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Información del estudiante</returns>
    [HttpGet("codigo/{codigoEstudiante}")]
    [ProducesResponseType(typeof(EstudianteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EstudianteDto>> GetByCodigo(
        string codigoEstudiante,
        [FromQuery] Guid colegioId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(codigoEstudiante))
                return BadRequest("El código del estudiante no puede estar vacío");

            if (colegioId == Guid.Empty)
                return BadRequest("El ID del colegio no puede estar vacío");

            var estudiante = await _estudianteService.GetByCodigoAsync(codigoEstudiante, colegioId, cancellationToken);

            if (estudiante == null)
                return NotFound($"No se encontró el estudiante con código: {codigoEstudiante}");

            return Ok(estudiante);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estudiante por código: {CodigoEstudiante}", codigoEstudiante);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error interno del servidor al obtener el estudiante");
        }
    }

    /// <summary>
    /// Obtiene todos los estudiantes de un colegio con paginación
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="pageNumber">Número de página (1-based)</param>
    /// <param name="pageSize">Tamaño de página</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista paginada de estudiantes</returns>
    [HttpGet("colegio/{colegioId:guid}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<object>> GetByColegioId(
        Guid colegioId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (colegioId == Guid.Empty)
                return BadRequest("El ID del colegio no puede estar vacío");

            if (pageNumber < 1)
                return BadRequest("El número de página debe ser mayor a 0");

            if (pageSize < 1 || pageSize > 100)
                return BadRequest("El tamaño de página debe estar entre 1 y 100");

            var (estudiantes, totalCount) = await _estudianteService.GetAllByColegioAsync(
                colegioId, pageNumber, pageSize, cancellationToken);

            var response = new
            {
                Data = estudiantes,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estudiantes del colegio: {ColegioId}", colegioId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error interno del servidor al obtener los estudiantes");
        }
    }

    /// <summary>
    /// Busca estudiantes por múltiples criterios
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="filtroTexto">Texto para buscar en nombre, apellido o documento</param>
    /// <param name="gradoId">ID del grado (opcional)</param>
    /// <param name="grupoId">ID del grupo (opcional)</param>
    /// <param name="estadoMatricula">Estado de matrícula (opcional)</param>
    /// <param name="fechaMatriculaDesde">Fecha de matrícula desde (opcional)</param>
    /// <param name="fechaMatriculaHasta">Fecha de matrícula hasta (opcional)</param>
    /// <param name="pageNumber">Número de página</param>
    /// <param name="pageSize">Tamaño de página</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista paginada de estudiantes que coinciden con los criterios</returns>
    [HttpGet("buscar")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<object>> Buscar(
        [FromQuery] Guid colegioId,
        [FromQuery] string? filtroTexto = null,
        [FromQuery] Guid? gradoId = null,
        [FromQuery] Guid? grupoId = null,
        [FromQuery] EstadoMatricula? estadoMatricula = null,
        [FromQuery] DateTime? fechaMatriculaDesde = null,
        [FromQuery] DateTime? fechaMatriculaHasta = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (colegioId == Guid.Empty)
                return BadRequest("El ID del colegio no puede estar vacío");

            if (pageNumber < 1)
                return BadRequest("El número de página debe ser mayor a 0");

            if (pageSize < 1 || pageSize > 100)
                return BadRequest("El tamaño de página debe estar entre 1 y 100");

            var (estudiantes, totalCount) = await _estudianteService.BuscarEstudiantesAsync(
                colegioId, filtroTexto, gradoId, grupoId, estadoMatricula,
                fechaMatriculaDesde, fechaMatriculaHasta, pageNumber, pageSize, cancellationToken);

            var response = new
            {
                Data = estudiantes,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Filtros = new
                {
                    FiltroTexto = filtroTexto,
                    GradoId = gradoId,
                    GrupoId = grupoId,
                    EstadoMatricula = estadoMatricula,
                    FechaMatriculaDesde = fechaMatriculaDesde,
                    FechaMatriculaHasta = fechaMatriculaHasta
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar estudiantes en el colegio: {ColegioId}", colegioId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error interno del servidor al buscar estudiantes");
        }
    }

    // ==============================================
    // ENDPOINTS DE CREACIÓN Y MODIFICACIÓN
    // ==============================================

    /// <summary>
    /// Crea un nuevo estudiante
    /// </summary>
    /// <param name="crearEstudianteDto">Datos del nuevo estudiante</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Estudiante creado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(EstudianteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EstudianteDto>> Crear(
        [FromBody] CrearEstudianteDto crearEstudianteDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (crearEstudianteDto == null)
                return BadRequest("Los datos del estudiante son requeridos");

            // Validar que el código no esté en uso
            var codigoDisponible = await _estudianteService.CodigoDisponibleAsync(
                crearEstudianteDto.CodigoEstudiante,
                crearEstudianteDto.ColegioId,
                null,
                cancellationToken);

            if (!codigoDisponible)
                return Conflict($"El código de estudiante '{crearEstudianteDto.CodigoEstudiante}' ya está en uso");

            var estudianteCreado = await _estudianteService.CrearAsync(crearEstudianteDto, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = estudianteCreado.Id },
                estudianteCreado);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Datos inválidos al crear estudiante");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear estudiante");
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error interno del servidor al crear el estudiante");
        }
    }

    /// <summary>
    /// Actualiza un estudiante existente
    /// </summary>
    /// <param name="id">ID del estudiante</param>
    /// <param name="actualizarEstudianteDto">Datos a actualizar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Estudiante actualizado</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EstudianteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EstudianteDto>> Actualizar(
        Guid id,
        [FromBody] ActualizarEstudianteDto actualizarEstudianteDto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest("El ID del estudiante no puede estar vacío");

            if (actualizarEstudianteDto == null)
                return BadRequest("Los datos de actualización son requeridos");

            var estudianteActualizado = await _estudianteService.ActualizarAsync(id, actualizarEstudianteDto, cancellationToken);

            if (estudianteActualizado == null)
                return NotFound($"No se encontró el estudiante con ID: {id}");

            return Ok(estudianteActualizado);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Datos inválidos al actualizar estudiante: {EstudianteId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estudiante: {EstudianteId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error interno del servidor al actualizar el estudiante");
        }
    }

    /// <summary>
    /// Elimina un estudiante (soft delete)
    /// </summary>
    /// <param name="id">ID del estudiante</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Resultado de la eliminación</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Eliminar(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest("El ID del estudiante no puede estar vacío");

            // Verificar que el estudiante existe
            var estudiante = await _estudianteService.GetByIdAsync(id, cancellationToken);
            if (estudiante == null)
                return NotFound($"No se encontró el estudiante con ID: {id}");

            // Verificar si puede ser eliminado
            var puedeSerEliminado = await _estudianteService.PuedeSerEliminadoAsync(id, cancellationToken);
            if (!puedeSerEliminado)
            {
                var dependencias = await _estudianteService.GetDependenciasEliminacionAsync(id, cancellationToken);
                var mensaje = $"No se puede eliminar el estudiante. Dependencias: {string.Join(", ", dependencias)}";
                return Conflict(mensaje);
            }

            await _estudianteService.EliminarAsync(id, cancellationToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar estudiante: {EstudianteId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error interno del servidor al eliminar el estudiante");
        }
    }

    // ==============================================
    // ENDPOINTS DE OPERACIONES ESPECÍFICAS
    // ==============================================

    /// <summary>
    /// Transfiere un estudiante a otro grupo
    /// </summary>
    /// <param name="estudianteId">ID del estudiante</param>
    /// <param name="nuevoGrupoId">ID del nuevo grupo</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("{estudianteId:guid}/transferir-grupo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TransferirGrupo(
        Guid estudianteId,
        [FromBody] TransferirGrupoRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (estudianteId == Guid.Empty)
                return BadRequest("El ID del estudiante no puede estar vacío");

            if (request?.NuevoGrupoId == Guid.Empty)
                return BadRequest("El ID del nuevo grupo no puede estar vacío");

            var resultado = await _estudianteService.TransferirGrupoAsync(
                estudianteId, request.NuevoGrupoId, cancellationToken);

            if (!resultado)
                return NotFound($"No se pudo realizar la transferencia del estudiante: {estudianteId}");

            return Ok(new { Mensaje = "Estudiante transferido exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al transferir estudiante: {EstudianteId}", estudianteId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error interno del servidor al transferir el estudiante");
        }
    }

    /// <summary>
    /// Cambia el estado de matrícula de un estudiante
    /// </summary>
    /// <param name="estudianteId">ID del estudiante</param>
    /// <param name="request">Datos del cambio de estado</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("{estudianteId:guid}/cambiar-estado-matricula")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CambiarEstadoMatricula(
        Guid estudianteId,
        [FromBody] CambiarEstadoMatriculaRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (estudianteId == Guid.Empty)
                return BadRequest("El ID del estudiante no puede estar vacío");

            if (request == null)
                return BadRequest("Los datos del cambio de estado son requeridos");

            var resultado = await _estudianteService.CambiarEstadoMatriculaAsync(
                estudianteId, request.NuevoEstado, request.Observaciones, cancellationToken);

            if (!resultado)
                return NotFound($"No se pudo cambiar el estado de matrícula del estudiante: {estudianteId}");

            return Ok(new { Mensaje = "Estado de matrícula cambiado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado de matrícula: {EstudianteId}", estudianteId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error interno del servidor al cambiar el estado de matrícula");
        }
    }

    // ==============================================
    // ENDPOINTS DE REPORTES Y ESTADÍSTICAS
    // ==============================================

    /// <summary>
    /// Obtiene estadísticas generales de estudiantes del colegio
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Estadísticas de estudiantes</returns>
    [HttpGet("estadisticas/{colegioId:guid}")]
    [ProducesResponseType(typeof(EstadisticasEstudiantesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EstadisticasEstudiantesDto>> GetEstadisticas(
        Guid colegioId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (colegioId == Guid.Empty)
                return BadRequest("El ID del colegio no puede estar vacío");

            var estadisticas = await _estudianteService.GetEstadisticasAsync(colegioId, cancellationToken);

            return Ok(estadisticas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas de estudiantes: {ColegioId}", colegioId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error interno del servidor al obtener las estadísticas");
        }
    }

    /// <summary>
    /// Obtiene reporte de estudiantes por grado
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Reporte de estudiantes por grado</returns>
    [HttpGet("reporte-por-grado/{colegioId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<ReporteEstudiantesPorGradoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ReporteEstudiantesPorGradoDto>>> GetReportePorGrado(
        Guid colegioId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (colegioId == Guid.Empty)
                return BadRequest("El ID del colegio no puede estar vacío");

            var reporte = await _estudianteService.GetReporteEstudiantesPorGradoAsync(colegioId, cancellationToken);

            return Ok(reporte);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener reporte por grado: {ColegioId}", colegioId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error interno del servidor al obtener el reporte");
        }
    }

    /// <summary>
    /// Obtiene reporte de estudiantes con información familiar
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="gradoId">ID del grado (opcional)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Reporte de estudiantes con información familiar</returns>
    [HttpGet("reporte-familia/{colegioId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<ReporteEstudianteFamiliaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ReporteEstudianteFamiliaDto>>> GetReporteFamilia(
        Guid colegioId,
        [FromQuery] Guid? gradoId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (colegioId == Guid.Empty)
                return BadRequest("El ID del colegio no puede estar vacío");

            var reporte = await _estudianteService.GetReporteEstudiantesConFamiliaAsync(
                colegioId, gradoId, cancellationToken);

            return Ok(reporte);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener reporte de familia: {ColegioId}", colegioId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error interno del servidor al obtener el reporte");
        }
    }

    // ==============================================
    // ENDPOINTS DE VALIDACIÓN
    // ==============================================

    /// <summary>
    /// Verifica si un código de estudiante está disponible
    /// </summary>
    /// <param name="codigoEstudiante">Código a verificar</param>
    /// <param name="colegioId">ID del colegio</param>
    /// <param name="estudianteId">ID del estudiante (para excluir en actualizaciones)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Disponibilidad del código</returns>
    [HttpGet("codigo-disponible")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<object>> VerificarCodigoDisponible(
        [FromQuery] string codigoEstudiante,
        [FromQuery] Guid colegioId,
        [FromQuery] Guid? estudianteId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(codigoEstudiante))
                return BadRequest("El código del estudiante es requerido");

            if (colegioId == Guid.Empty)
                return BadRequest("El ID del colegio no puede estar vacío");

            var disponible = await _estudianteService.CodigoDisponibleAsync(
                codigoEstudiante, colegioId, estudianteId, cancellationToken);

            return Ok(new { CodigoDisponible = disponible });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar disponibilidad del código: {CodigoEstudiante}", codigoEstudiante);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error interno del servidor al verificar el código");
        }
    }

    /// <summary>
    /// Verifica si un estudiante puede ser eliminado
    /// </summary>
    /// <param name="id">ID del estudiante</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Información sobre la posibilidad de eliminación</returns>
    [HttpGet("{id:guid}/puede-eliminar")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<object>> VerificarPuedeEliminar(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest("El ID del estudiante no puede estar vacío");

            var puedeEliminar = await _estudianteService.PuedeSerEliminadoAsync(id, cancellationToken);
            var dependencias = puedeEliminar
                ? new List<string>()
                : await _estudianteService.GetDependenciasEliminacionAsync(id, cancellationToken);

            return Ok(new
            {
                PuedeEliminar = puedeEliminar,
                Dependencias = dependencias
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar si puede eliminar estudiante: {EstudianteId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error interno del servidor al verificar la eliminación");
        }
    }
}

// ==============================================
// MODELOS DE REQUEST PARA ENDPOINTS ESPECÍFICOS
// ==============================================

/// <summary>
/// Modelo para la transferencia de grupo
/// </summary>
public class TransferirGrupoRequest
{
    /// <summary>
    /// ID del nuevo grupo
    /// </summary>
    public Guid NuevoGrupoId { get; set; }

    /// <summary>
    /// Observaciones sobre la transferencia
    /// </summary>
    public string? Observaciones { get; set; }
}

/// <summary>
/// Modelo para el cambio de estado de matrícula
/// </summary>
public class CambiarEstadoMatriculaRequest
{
    /// <summary>
    /// Nuevo estado de la matrícula
    /// </summary>
    public EstadoMatricula NuevoEstado { get; set; }

    /// <summary>
    /// Observaciones sobre el cambio de estado
    /// </summary>
    public string? Observaciones { get; set; }
}