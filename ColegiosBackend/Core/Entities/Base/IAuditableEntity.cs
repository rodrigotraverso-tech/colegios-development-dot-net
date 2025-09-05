namespace ColegiosBackend.Core.Entities.Base;

/// <summary>
/// Interface que define las propiedades de auditoría para entidades
/// Permite identificar qué entidades requieren tracking de cambios
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// Fecha y hora de creación del registro
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Fecha y hora de la última actualización
    /// </summary>
    DateTime? UpdatedAt { get; }

    /// <summary>
    /// ID del usuario que creó el registro
    /// </summary>
    Guid? CreatedBy { get; }

    /// <summary>
    /// ID del usuario que realizó la última actualización
    /// </summary>
    Guid? UpdatedBy { get; }

    /// <summary>
    /// Marca la entidad como actualizada
    /// </summary>
    /// <param name="updatedBy">ID del usuario que realiza la actualización</param>
    void MarkAsUpdated(Guid? updatedBy = null);

    /// <summary>
    /// Establece información de auditoría para creación
    /// </summary>
    /// <param name="createdBy">ID del usuario que crea el registro</param>
    void SetCreationInfo(Guid? createdBy = null);
}
