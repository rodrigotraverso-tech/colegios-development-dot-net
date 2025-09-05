namespace ColegiosBackend.Core.Entities.Base;

/// <summary>
/// Interface que define las propiedades necesarias para el multi-tenancy
/// Las entidades que implementen esta interface estarán aisladas por colegio
/// </summary>
public interface ITenantEntity
{
    /// <summary>
    /// ID del colegio al que pertenece esta entidad
    /// Null solo para entidades globales (roles ADMIN_GLOBAL, SUPER_ADMIN)
    /// </summary>
    Guid? ColegioId { get; }

    /// <summary>
    /// Establece el colegio para esta entidad
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    void SetColegio(Guid colegioId);

    /// <summary>
    /// Verifica si la entidad pertenece al colegio especificado
    /// </summary>
    /// <param name="colegioId">ID del colegio a verificar</param>
    /// <returns>True si pertenece al colegio o es una entidad global</returns>
    bool BelongsToColegio(Guid colegioId);

    /// <summary>
    /// Indica si es una entidad global (no pertenece a ningún colegio específico)
    /// </summary>
    bool IsGlobalEntity => ColegioId == null;
}
