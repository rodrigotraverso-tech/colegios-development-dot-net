using ColegiosBackend.Core.Entities.Base;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Entities;

/// <summary>
/// Representa una calificación de un estudiante en una materia específica
/// </summary>
public class Calificacion : BaseEntity, ITenantEntity, IAuditableEntity
{
    /// <summary>
    /// Constructor privado para Entity Framework
    /// </summary>
    private Calificacion() { }

    /// <summary>
    /// Constructor principal
    /// </summary>
    public Calificacion(
        Guid estudianteId,
        Guid asignacionId,
        Guid periodoAcademicoId,
        Guid tipoEvaluacionId,
        decimal calificacion,
        Guid profesorId,
        Guid colegioId,
        DateTime? fechaCalificacion = null)
    {
        EstudianteId = estudianteId;
        AsignacionId = asignacionId;
        PeriodoAcademicoId = periodoAcademicoId;
        TipoEvaluacionId = tipoEvaluacionId;
        CalificacionValor = calificacion;
        ProfesorId = profesorId;
        ColegioId = colegioId;
        FechaCalificacion = fechaCalificacion ?? DateTime.Today;
        Estado = EstadoCalificacion.Borrador;
        Activo = true;

        ValidarCalificacion();
    }

    // Propiedades principales
    /// <summary>
    /// Referencia al estudiante calificado
    /// </summary>
    public Guid EstudianteId { get; private set; }

    /// <summary>
    /// Navegación al estudiante
    /// </summary>
    public virtual Estudiante? Estudiante { get; private set; }

    /// <summary>
    /// Referencia a la asignación (profesor-materia-grupo)
    /// </summary>
    public Guid AsignacionId { get; private set; }

    /// <summary>
    /// Navegación a la asignación
    /// </summary>
    public virtual Asignacion? Asignacion { get; private set; }

    /// <summary>
    /// Referencia al período académico
    /// </summary>
    public Guid PeriodoAcademicoId { get; private set; }

    /// <summary>
    /// Navegación al período académico
    /// </summary>
    public virtual PeriodoEvaluativo? PeriodoAcademico { get; private set; }

    /// <summary>
    /// Referencia al tipo de evaluación
    /// </summary>
    public Guid TipoEvaluacionId { get; private set; }

    /// <summary>
    /// Navegación al tipo de evaluación
    /// </summary>
    public virtual TipoEvaluacion? TipoEvaluacion { get; private set; }

    /// <summary>
    /// Valor numérico de la calificación
    /// </summary>
    public decimal CalificacionValor { get; private set; }

    /// <summary>
    /// Calificación cualitativa opcional (A, B, C, etc.)
    /// </summary>
    public string? CalificacionCualitativa { get; private set; }

    /// <summary>
    /// Observaciones sobre la calificación
    /// </summary>
    public string? Observaciones { get; private set; }

    /// <summary>
    /// Fecha en que se registró la calificación
    /// </summary>
    public DateTime FechaCalificacion { get; private set; }

    /// <summary>
    /// Referencia al profesor que registró la calificación
    /// </summary>
    public Guid ProfesorId { get; private set; }

    /// <summary>
    /// Navegación al profesor
    /// </summary>
    public virtual Profesor? Profesor { get; private set; }

    /// <summary>
    /// ID del colegio (tenant)
    /// </summary>
    public Guid? ColegioId { get; private set; }

    /// <summary>
    /// Estado de la calificación
    /// </summary>
    public EstadoCalificacion Estado { get; private set; }

    /// <summary>
    /// Indica si la calificación fue recuperada/corregida
    /// </summary>
    public bool EsRecuperacion { get; private set; }

    /// <summary>
    /// Referencia a la calificación original (si es recuperación)
    /// </summary>
    public Guid? CalificacionOriginalId { get; private set; }

    /// <summary>
    /// Navegación a la calificación original
    /// </summary>
    public virtual Calificacion? CalificacionOriginal { get; private set; }

    /// <summary>
    /// Peso de esta calificación en el cálculo final
    /// </summary>
    public decimal Peso { get; private set; } = 1.0m;

    /// <summary>
    /// Configuración adicional en formato JSON
    /// </summary>
    public string? ConfiguracionCalificacion { get; private set; }

    /// <summary>
    /// Indica si la calificación está activa
    /// </summary>
    public bool Activo { get; private set; }
    
    // Métodos de negocio

    /// <summary>
    /// Actualiza el valor de la calificación
    /// </summary>
    public void ActualizarCalificacion(decimal nuevaCalificacion, string? observaciones = null)
    {
        if (Estado == EstadoCalificacion.Publicada)
            throw new InvalidOperationException("No se puede modificar una calificación publicada");

        ValidarValorCalificacion(nuevaCalificacion);

        CalificacionValor = nuevaCalificacion;
        if (observaciones != null)
            Observaciones = observaciones;
    }

    /// <summary>
    /// Establece la calificación cualitativa
    /// </summary>
    public void EstablecerCalificacionCualitativa(string? calificacionCualitativa)
    {
        if (Estado == EstadoCalificacion.Publicada)
            throw new InvalidOperationException("No se puede modificar una calificación publicada");

        CalificacionCualitativa = calificacionCualitativa;
    }

    /// <summary>
    /// Actualiza las observaciones
    /// </summary>
    public void ActualizarObservaciones(string? observaciones)
    {
        if (Estado == EstadoCalificacion.Publicada)
            throw new InvalidOperationException("No se puede modificar una calificación publicada");

        Observaciones = observaciones;
    }

    /// <summary>
    /// Establece el peso de la calificación
    /// </summary>
    public void EstablecerPeso(decimal peso)
    {
        if (peso < 0 || peso > 10)
            throw new ArgumentException("El peso debe estar entre 0 y 10");

        Peso = peso;
    }

    /// <summary>
    /// Marca la calificación como borrador
    /// </summary>
    public void MarcarComoBorrador()
    {
        if (Estado == EstadoCalificacion.Publicada)
            throw new InvalidOperationException("No se puede cambiar el estado de una calificación publicada");

        Estado = EstadoCalificacion.Borrador;
    }

    /// <summary>
    /// Marca la calificación como pendiente de revisión
    /// </summary>
    public void MarcarPendienteRevision()
    {
        Estado = EstadoCalificacion.PendienteRevision;
    }

    /// <summary>
    /// Publica la calificación (la hace oficial)
    /// </summary>
    public void Publicar()
    {
        if (CalificacionValor < 0)
            throw new InvalidOperationException("No se puede publicar una calificación con valor negativo");

        Estado = EstadoCalificacion.Publicada;
    }

    /// <summary>
    /// Anula la calificación
    /// </summary>
    public void Anular(string motivo)
    {
        if (string.IsNullOrWhiteSpace(motivo))
            throw new ArgumentException("Debe especificar el motivo de anulación");

        Estado = EstadoCalificacion.Anulada;
        Observaciones = $"{Observaciones} [ANULADA: {motivo}]".Trim();
        Activo = false;
    }

    /// <summary>
    /// Marca esta calificación como recuperación de otra
    /// </summary>
    public void MarcarComoRecuperacion(Guid calificacionOriginalId)
    {
        EsRecuperacion = true;
        CalificacionOriginalId = calificacionOriginalId;
    }

    /// <summary>
    /// Establece configuración adicional
    /// </summary>
    public void EstablecerConfiguracion(string? configuracion)
    {
        ConfiguracionCalificacion = configuracion;
    }

    /// <summary>
    /// Verifica si la calificación puede ser modificada
    /// </summary>
    public bool PuedeSerModificada()
    {
        return Estado != EstadoCalificacion.Publicada &&
               Estado != EstadoCalificacion.Anulada &&
               Activo;
    }

    /// <summary>
    /// Verifica si es una calificación aprobatoria
    /// </summary>
    public bool EsAprobatoria(decimal notaMinima = 6.0m)
    {
        return CalificacionValor >= notaMinima;
    }

    /// <summary>
    /// Obtiene el equivalente en letra según la escala tradicional
    /// </summary>
    public string ObtenerEquivalenteEnLetra()
    {
        return CalificacionValor switch
        {
            >= 9.0m => "A",
            >= 8.0m => "B",
            >= 7.0m => "C",
            >= 6.0m => "D",
            _ => "F"
        };
    }

    /// <summary>
    /// Activa la calificación
    /// </summary>
    public void Activar()
    {
        Activo = true;
    }

    /// <summary>
    /// Desactiva la calificación
    /// </summary>
    public void Desactivar()
    {
        Activo = false;
    }

    // Implementación de ITenantEntity

    /// <summary>
    /// Establece el colegio para esta entidad
    /// </summary>
    public void SetColegio(Guid colegioId)
    {
        ColegioId = colegioId;
    }

    /// <summary>
    /// Verifica si la entidad pertenece al colegio especificado
    /// </summary>
    public bool BelongsToColegio(Guid colegioId)
    {
        return ColegioId == null || ColegioId == colegioId;
    }

    // Métodos de validación privados

    private void ValidarCalificacion()
    {
        ValidarValorCalificacion(CalificacionValor);
    }

    private static void ValidarValorCalificacion(decimal valor)
    {
        if (valor < 0 || valor > 10)
            throw new ArgumentException("La calificación debe estar entre 0 y 10");
    }
}
