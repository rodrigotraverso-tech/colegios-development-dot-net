using ColegiosBackend.Core.Entities.Base;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Entities;

/// <summary>
/// Representa la relación entre un profesor y un colegio específico
/// Un profesor puede trabajar en múltiples colegios
/// </summary>
public class ProfesorColegio : BaseEntity, ITenantEntity, IAuditableEntity
{
    /// <summary>
    /// Constructor privado para Entity Framework
    /// </summary>
    private ProfesorColegio() { }

    /// <summary>
    /// Constructor principal
    /// </summary>
    public ProfesorColegio(Guid profesorId, Guid colegioId, DateTime? fechaIngreso = null)
    {
        ProfesorId = profesorId;
        ColegioId = colegioId;
        FechaIngreso = fechaIngreso ?? DateTime.Today;
        Activo = true;
    }

    /// <summary>
    /// Referencia al profesor
    /// </summary>
    public Guid ProfesorId { get; private set; }

    /// <summary>
    /// Navegación al profesor
    /// </summary>
    public virtual Profesor? Profesor { get; private set; }

    /// <summary>
    /// ID del colegio
    /// </summary>
    public Guid? ColegioId { get; private set; }

    /// <summary>
    /// Navegación al colegio
    /// </summary>
    public virtual Colegio? Colegio { get; private set; }

    /// <summary>
    /// Fecha de ingreso del profesor al colegio
    /// </summary>
    public DateTime FechaIngreso { get; private set; }

    /// <summary>
    /// Fecha de salida del profesor del colegio (si aplica)
    /// </summary>
    public DateTime? FechaSalida { get; private set; }

    /// <summary>
    /// Indica si la relación está activa
    /// </summary>
    public bool Activo { get; private set; }

    // Métodos de negocio

    /// <summary>
    /// Establece la fecha de salida y desactiva la relación
    /// </summary>
    public void RegistrarSalida(DateTime fechaSalida)
    {
        FechaSalida = fechaSalida.Date;
        Activo = false;
    }

    /// <summary>
    /// Reactiva la relación profesor-colegio
    /// </summary>
    public void Reactivar()
    {
        FechaSalida = null;
        Activo = true;
    }

    /// <summary>
    /// Actualiza la fecha de ingreso
    /// </summary>
    public void ActualizarFechaIngreso(DateTime nuevaFecha)
    {
        if (FechaSalida.HasValue && nuevaFecha >= FechaSalida.Value)
            throw new ArgumentException("La fecha de ingreso debe ser anterior a la fecha de salida");

        FechaIngreso = nuevaFecha.Date;
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
        return ColegioId == colegioId;
    }
}
