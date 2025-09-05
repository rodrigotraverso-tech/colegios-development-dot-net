using ColegiosBackend.Core.Entities.Base;

namespace ColegiosBackend.Core.Entities;

/// <summary>
/// Representa un tipo de evaluación (examen, tarea, proyecto, etc.)
/// Define cómo se categorizan las calificaciones
/// </summary>
public class TipoEvaluacion : BaseEntity, ITenantEntity, IAuditableEntity
{
    /// <summary>
    /// Constructor privado para Entity Framework
    /// </summary>
    private TipoEvaluacion() { }

    /// <summary>
    /// Constructor principal
    /// </summary>
    public TipoEvaluacion(
        Guid colegioId,
        string codigo,
        string nombre,
        string? descripcion = null,
        decimal? porcentaje = null)
    {
        ColegioId = colegioId;
        Codigo = codigo ?? throw new ArgumentNullException(nameof(codigo));
        Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        Descripcion = descripcion;
        Porcentaje = porcentaje;
        Activo = true;

        ValidarDatos();
    }

    /// <summary>
    /// ID del colegio (tenant)
    /// </summary>
    public Guid? ColegioId { get; private set; }

    /// <summary>
    /// Código único del tipo de evaluación
    /// </summary>
    public string Codigo { get; private set; } = string.Empty;

    /// <summary>
    /// Nombre descriptivo del tipo de evaluación
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del tipo de evaluación
    /// </summary>
    public string? Descripcion { get; private set; }

    /// <summary>
    /// Porcentaje que representa este tipo en la nota final
    /// </summary>
    public decimal? Porcentaje { get; private set; }

    /// <summary>
    /// Indica si el tipo de evaluación está activo
    /// </summary>
    public bool Activo { get; private set; }
        
    // Métodos de negocio

    /// <summary>
    /// Actualiza la información básica del tipo de evaluación
    /// </summary>
    public void ActualizarInformacion(string nombre, string? descripcion = null)
    {
        Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        Descripcion = descripcion;
    }

    /// <summary>
    /// Establece el porcentaje de peso en la nota final
    /// </summary>
    public void EstablecerPorcentaje(decimal? porcentaje)
    {
        if (porcentaje.HasValue && (porcentaje < 0 || porcentaje > 100))
            throw new ArgumentException("El porcentaje debe estar entre 0 y 100");

        Porcentaje = porcentaje;
    }

    /// <summary>
    /// Activa el tipo de evaluación
    /// </summary>
    public void Activar()
    {
        Activo = true;
    }

    /// <summary>
    /// Desactiva el tipo de evaluación
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

    // Validaciones privadas

    private void ValidarDatos()
    {
        if (string.IsNullOrWhiteSpace(Codigo))
            throw new ArgumentException("El código es requerido");

        if (string.IsNullOrWhiteSpace(Nombre))
            throw new ArgumentException("El nombre es requerido");

        if (Porcentaje.HasValue && (Porcentaje < 0 || Porcentaje > 100))
            throw new ArgumentException("El porcentaje debe estar entre 0 y 100");
    }
}