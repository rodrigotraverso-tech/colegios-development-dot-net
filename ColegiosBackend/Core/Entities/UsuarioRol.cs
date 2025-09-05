using System;
using ColegiosBackend.Core.Entities.Base;

namespace ColegiosBackend.Core.Entities;

/// <summary>
/// Entidad que representa la asignación de un rol a un usuario en un colegio específico
/// Maneja tanto roles por colegio como roles globales (colegio_id = null)
/// </summary>
public class UsuarioRol : BaseEntity, IAuditableEntity
{
    /// <summary>
    /// ID del usuario al que se asigna el rol
    /// </summary>
    public Guid UsuarioId { get; private set; }

    /// <summary>
    /// ID del rol asignado
    /// </summary>
    public Guid RolId { get; private set; }

    /// <summary>
    /// ID del colegio para el cual se asigna el rol
    /// NULL para roles globales (ADMIN_GLOBAL, SUPER_ADMIN)
    /// </summary>
    public Guid? ColegioId { get; private set; }

    /// <summary>
    /// Indica si la asignación está activa
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Fecha cuando se asignó el rol
    /// </summary>
    public DateTime FechaAsignacion { get; private set; }

    /// <summary>
    /// Fecha cuando se revocó el rol (opcional)
    /// </summary>
    public DateTime? FechaRevocacion { get; private set; }

    /// <summary>
    /// Observaciones sobre la asignación del rol
    /// </summary>
    public string? Observaciones { get; private set; }

    // Navegación
    /// <summary>
    /// Usuario al que se asigna el rol
    /// </summary>
    public Usuario Usuario { get; private set; } = null!;

    /// <summary>
    /// Rol asignado
    /// </summary>
    public Rol Rol { get; private set; } = null!;

    /// <summary>
    /// Colegio para el cual se asigna el rol (null para roles globales)
    /// </summary>
    public Colegio? Colegio { get; private set; }

    /// <summary>
    /// Verifica si es una asignación de rol global
    /// </summary>
    public bool EsRolGlobal => ColegioId == null;

    /// <summary>
    /// Verifica si el rol está activo y no ha sido revocado
    /// </summary>
    public bool EstaVigente => Activo && !FechaRevocacion.HasValue;

    /// <summary>
    /// Constructor privado para Entity Framework
    /// </summary>
    private UsuarioRol() : base() { }

    /// <summary>
    /// Constructor para crear una nueva asignación de rol
    /// </summary>
    public UsuarioRol(
        Guid usuarioId,
        Guid rolId,
        Guid? colegioId = null,
        string? observaciones = null) : base()
    {
        if (usuarioId == Guid.Empty)
            throw new ArgumentException("El ID del usuario no puede estar vacío", nameof(usuarioId));

        if (rolId == Guid.Empty)
            throw new ArgumentException("El ID del rol no puede estar vacío", nameof(rolId));

        UsuarioId = usuarioId;
        RolId = rolId;
        ColegioId = colegioId;
        SetObservaciones(observaciones);

        Activo = true;
        FechaAsignacion = DateTime.UtcNow;
    }

    /// <summary>
    /// Constructor para roles globales (sin colegio)
    /// </summary>
    public static UsuarioRol CrearRolGlobal(
        Guid usuarioId,
        Guid rolId,
        string? observaciones = null)
    {
        return new UsuarioRol(usuarioId, rolId, null, observaciones);
    }

    /// <summary>
    /// Constructor para roles específicos de colegio
    /// </summary>
    public static UsuarioRol CrearRolColegio(
        Guid usuarioId,
        Guid rolId,
        Guid colegioId,
        string? observaciones = null)
    {
        if (colegioId == Guid.Empty)
            throw new ArgumentException("El ID del colegio no puede estar vacío para roles específicos", nameof(colegioId));

        return new UsuarioRol(usuarioId, rolId, colegioId, observaciones);
    }

    /// <summary>
    /// Actualiza las observaciones de la asignación
    /// </summary>
    public void SetObservaciones(string? observaciones)
    {
        if (!string.IsNullOrWhiteSpace(observaciones) && observaciones.Length > 500)
            throw new ArgumentException("Las observaciones no pueden exceder 500 caracteres", nameof(observaciones));

        Observaciones = string.IsNullOrWhiteSpace(observaciones) ? null : observaciones.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Cambia el colegio asociado al rol (solo para roles no globales)
    /// </summary>
    public void CambiarColegio(Guid colegioId)
    {
        if (EsRolGlobal)
            throw new InvalidOperationException("No se puede cambiar el colegio de un rol global");

        if (colegioId == Guid.Empty)
            throw new ArgumentException("El ID del colegio no puede estar vacío", nameof(colegioId));

        ColegioId = colegioId;
        MarkAsUpdated();
    }

    /// <summary>
    /// Activa la asignación del rol
    /// </summary>
    public void Activar()
    {
        if (!Activo)
        {
            Activo = true;
            FechaRevocacion = null;
            MarkAsUpdated();
        }
    }

    /// <summary>
    /// Desactiva la asignación del rol
    /// </summary>
    public void Desactivar(string? motivoRevocacion = null)
    {
        if (Activo)
        {
            Activo = false;
            FechaRevocacion = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(motivoRevocacion))
            {
                var observacionesActuales = Observaciones ?? "";
                var nuevaObservacion = $"[{DateTime.UtcNow:yyyy-MM-dd}] Revocado: {motivoRevocacion}";

                SetObservaciones(string.IsNullOrWhiteSpace(observacionesActuales)
                    ? nuevaObservacion
                    : $"{observacionesActuales}\n{nuevaObservacion}");
            }

            MarkAsUpdated();
        }
    }

    /// <summary>
    /// Revoca permanentemente el rol (soft delete)
    /// </summary>
    public void Revocar(string? motivo = null)
    {
        Desactivar(motivo);
        MarkAsDeleted();
    }

    /// <summary>
    /// Verifica si esta asignación es compatible con el rol y colegio especificados
    /// </summary>
    public bool EsCompatibleCon(Guid rolId, Guid? colegioId)
    {
        return RolId == rolId && ColegioId == colegioId;
    }

    /// <summary>
    /// Verifica si el usuario puede acceder al colegio con este rol
    /// </summary>
    public bool PermiteAccesoAColegio(Guid colegioId)
    {
        if (!EstaVigente) return false;

        // Si es rol global, permite acceso a cualquier colegio
        if (EsRolGlobal) return true;

        // Si es rol específico, debe coincidir el colegio
        return ColegioId == colegioId;
    }

    /// <summary>
    /// Obtiene una descripción textual de la asignación
    /// </summary>
    public string DescripcionAsignacion
    {
        get
        {
            var tipo = EsRolGlobal ? "Global" : "Colegio";
            var estado = EstaVigente ? "Vigente" : "No vigente";
            return $"Rol {tipo} - {estado} desde {FechaAsignacion:yyyy-MM-dd}";
        }
    }

    /// <summary>
    /// Verifica si la asignación puede ser eliminada
    /// </summary>
    public override bool CanBeDeleted()
    {
        // Las asignaciones pueden ser eliminadas si están inactivas
        // o si han sido revocadas hace más de 30 días (para auditoría)
        if (!Activo) return true;

        return FechaRevocacion.HasValue &&
               FechaRevocacion.Value.AddDays(30) < DateTime.UtcNow;
    }

    /// <summary>
    /// Validaciones de negocio para la entidad
    /// </summary>
    public void ValidarConsistencia()
    {
        if (UsuarioId == Guid.Empty)
            throw new InvalidOperationException("La asignación debe tener un usuario válido");

        if (RolId == Guid.Empty)
            throw new InvalidOperationException("La asignación debe tener un rol válido");

        // Si es rol global, no debe tener colegio
        // Si es rol específico, debe tener colegio
        // Esta validación se complementa con los triggers de BD
    }

    public override string ToString()
    {
        var colegio = EsRolGlobal ? "GLOBAL" : $"Colegio {ColegioId}";
        return $"Usuario {UsuarioId} - Rol {RolId} - {colegio} - {(Activo ? "Activo" : "Inactivo")}";
    }
}