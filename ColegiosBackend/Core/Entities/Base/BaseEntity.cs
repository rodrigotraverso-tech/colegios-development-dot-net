namespace ColegiosBackend.Core.Entities.Base;

/// <summary>
/// Clase base para todas las entidades del dominio
/// Proporciona propiedades comunes como ID y campos de auditoría
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identificador único de la entidad
    /// </summary>
    public Guid Id { get; protected set; } = Guid.NewGuid();

    /// <summary>
    /// Fecha y hora de creación del registro
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// Fecha y hora de la última actualización
    /// </summary>
    public DateTime? UpdatedAt { get; protected set; }

    /// <summary>
    /// ID del usuario que creó el registro
    /// </summary>
    public Guid? CreatedBy { get; protected set; }

    /// <summary>
    /// ID del usuario que realizó la última actualización
    /// </summary>
    public Guid? UpdatedBy { get; protected set; }

    /// <summary>
    /// Indica si el registro está marcado como eliminado (soft delete)
    /// </summary>
    public bool IsDeleted { get; protected set; }

    /// <summary>
    /// Fecha y hora del soft delete
    /// </summary>
    public DateTime? DeletedAt { get; protected set; }

    /// <summary>
    /// ID del usuario que eliminó el registro
    /// </summary>
    public Guid? DeletedBy { get; protected set; }

    /// <summary>
    /// Constructor protegido para inicializar campos de auditoría
    /// </summary>
    protected BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    /// <summary>
    /// Constructor protegido que permite especificar el ID
    /// (útil para casos específicos como importaciones)
    /// </summary>
    protected BaseEntity(Guid id)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    /// <summary>
    /// Marca la entidad como actualizada
    /// </summary>
    /// <param name="updatedBy">ID del usuario que realiza la actualización</param>
    public virtual void MarkAsUpdated(Guid? updatedBy = null)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Realiza un soft delete de la entidad
    /// </summary>
    /// <param name="deletedBy">ID del usuario que realiza la eliminación</param>
    public virtual void MarkAsDeleted(Guid? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Restaura una entidad eliminada
    /// </summary>
    /// <param name="restoredBy">ID del usuario que realiza la restauración</param>
    public virtual void Restore(Guid? restoredBy = null)
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        MarkAsUpdated(restoredBy);
    }

    /// <summary>
    /// Establece información de auditoría para creación
    /// (usado típicamente por el repositorio)
    /// </summary>
    public virtual void SetCreationInfo(Guid? createdBy = null)
    {
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Verifica si la entidad puede ser eliminada
    /// Las clases hijas pueden sobrescribir este método para implementar validaciones específicas
    /// </summary>
    /// <returns>True si puede ser eliminada, False si tiene dependencias</returns>
    public virtual bool CanBeDeleted()
    {
        return true; // Por defecto permite eliminación
    }
}
