using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ColegiosBackend.Core.Entities.Base;

namespace ColegiosBackend.Core.Entities;

/// <summary>
/// Entidad que representa un usuario del sistema
/// Maneja autenticación SSO y puede tener acceso a múltiples colegios
/// </summary>
public class Usuario : BaseEntity, IAuditableEntity
{
    /// <summary>
    /// Nombre de usuario único en el sistema
    /// </summary>
    public string Username { get; private set; } = string.Empty;

    /// <summary>
    /// Email único del usuario (usado para login y recuperación)
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// Hash de la contraseña
    /// </summary>
    public string PasswordHash { get; private set; } = string.Empty;

    /// <summary>
    /// Salt usado para el hash de la contraseña
    /// </summary>
    public string Salt { get; private set; } = string.Empty;

    /// <summary>
    /// Indica si el usuario está activo en el sistema
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Indica si el usuario debe cambiar su contraseña en el próximo login
    /// </summary>
    public bool RequiereCambioPassword { get; private set; }

    /// <summary>
    /// Fecha y hora del último acceso al sistema
    /// </summary>
    public DateTime? UltimoAcceso { get; private set; }

    /// <summary>
    /// Número de intentos fallidos de login consecutivos
    /// </summary>
    public int IntentosFallidos { get; private set; }

    /// <summary>
    /// Fecha hasta la cual el usuario está bloqueado (null si no está bloqueado)
    /// </summary>
    public DateTime? BloqueadoHasta { get; private set; }

    /// <summary>
    /// ID de la persona asociada a este usuario
    /// </summary>
    public Guid? PersonaId { get; private set; }

    /// <summary>
    /// Fecha de creación del usuario (campo de BD)
    /// </summary>
    public DateTime FechaCreacion { get; private set; }

    /// <summary>
    /// Fecha de última actualización (campo de BD)
    /// </summary>
    public DateTime FechaActualizacion { get; private set; }

    // Navegación
    /// <summary>
    /// Persona asociada al usuario
    /// </summary>
    public Persona? Persona { get; private set; }

    /// <summary>
    /// Roles asignados al usuario en diferentes colegios
    /// </summary>
    public ICollection<UsuarioRol> UsuarioRoles { get; private set; } = new List<UsuarioRol>();

    /// <summary>
    /// Verifica si el usuario está bloqueado actualmente
    /// </summary>
    public bool EstaBloqueado => BloqueadoHasta.HasValue && BloqueadoHasta.Value > DateTime.UtcNow;

    /// <summary>
    /// Verifica si el usuario puede iniciar sesión
    /// </summary>
    public bool PuedeIniciarSesion => Activo && !EstaBloqueado;

    /// <summary>
    /// Obtiene los colegios a los que tiene acceso el usuario
    /// </summary>
    public IEnumerable<Guid> ColegiosAcceso => UsuarioRoles
        .Where(ur => ur.Activo && ur.Colegio != null)
        .Select(ur => ur.ColegioId!.Value)
        .Distinct();

    /// <summary>
    /// Verifica si tiene roles globales (ADMIN_GLOBAL, SUPER_ADMIN)
    /// </summary>
    public bool TieneRolesGlobales => UsuarioRoles
        .Any(ur => ur.Activo && ur.ColegioId == null);

    /// <summary>
    /// Constructor privado para Entity Framework
    /// </summary>
    private Usuario() : base() { }

    /// <summary>
    /// Constructor para crear un nuevo usuario
    /// </summary>
    public Usuario(
        string username,
        string email,
        string passwordHash,
        string salt,
        Guid? personaId = null) : base()
    {
        SetUsername(username);
        SetEmail(email);
        SetPassword(passwordHash, salt);
        PersonaId = personaId;

        Activo = true;
        RequiereCambioPassword = true; // Por defecto debe cambiar contraseña
        IntentosFallidos = 0;
        FechaCreacion = DateTime.UtcNow;
        FechaActualizacion = DateTime.UtcNow;
    }

    /// <summary>
    /// Actualiza el nombre de usuario
    /// </summary>
    public void SetUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("El nombre de usuario es obligatorio", nameof(username));

        if (username.Length < 3 || username.Length > 50)
            throw new ArgumentException("El nombre de usuario debe tener entre 3 y 50 caracteres", nameof(username));

        // Validar que solo contenga caracteres alfanuméricos, puntos y guiones bajos
        if (!Regex.IsMatch(username, @"^[a-zA-Z0-9._]+$"))
            throw new ArgumentException("El nombre de usuario solo puede contener letras, números, puntos y guiones bajos", nameof(username));

        Username = username.ToLowerInvariant();
        MarkAsUpdated();
    }

    /// <summary>
    /// Actualiza el email del usuario
    /// </summary>
    public void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email es obligatorio", nameof(email));

        if (email.Length > 100)
            throw new ArgumentException("El email no puede exceder 100 caracteres", nameof(email));

        // Validación básica de email
        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new ArgumentException("El formato del email no es válido", nameof(email));

        Email = email.ToLowerInvariant();
        MarkAsUpdated();
    }

    /// <summary>
    /// Actualiza la contraseña del usuario
    /// </summary>
    public void SetPassword(string passwordHash, string salt)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("El hash de la contraseña es obligatorio", nameof(passwordHash));

        if (string.IsNullOrWhiteSpace(salt))
            throw new ArgumentException("El salt es obligatorio", nameof(salt));

        if (passwordHash.Length > 255)
            throw new ArgumentException("El hash de la contraseña no puede exceder 255 caracteres", nameof(passwordHash));

        if (salt.Length > 100)
            throw new ArgumentException("El salt no puede exceder 100 caracteres", nameof(salt));

        PasswordHash = passwordHash;
        Salt = salt;
        RequiereCambioPassword = false; // Al cambiar contraseña, ya no requiere cambio
        MarkAsUpdated();
    }

    /// <summary>
    /// Asocia el usuario con una persona
    /// </summary>
    public void SetPersona(Guid personaId)
    {
        if (personaId == Guid.Empty)
            throw new ArgumentException("El ID de la persona no puede estar vacío", nameof(personaId));

        PersonaId = personaId;
        MarkAsUpdated();
    }

    /// <summary>
    /// Remueve la asociación con la persona
    /// </summary>
    public void RemovePersona()
    {
        PersonaId = null;
        MarkAsUpdated();
    }

    /// <summary>
    /// Activa el usuario
    /// </summary>
    public void Activar()
    {
        if (!Activo)
        {
            Activo = true;
            MarkAsUpdated();
        }
    }

    /// <summary>
    /// Desactiva el usuario
    /// </summary>
    public void Desactivar()
    {
        if (Activo)
        {
            Activo = false;
            MarkAsUpdated();
        }
    }

    /// <summary>
    /// Marca que el usuario requiere cambio de contraseña
    /// </summary>
    public void RequirePasswordChange()
    {
        if (!RequiereCambioPassword)
        {
            RequiereCambioPassword = true;
            MarkAsUpdated();
        }
    }

    /// <summary>
    /// Registra un acceso exitoso
    /// </summary>
    public void RegistrarAccesoExitoso()
    {
        UltimoAcceso = DateTime.UtcNow;
        IntentosFallidos = 0;
        BloqueadoHasta = null;
        MarkAsUpdated();
    }

    /// <summary>
    /// Registra un intento fallido de login
    /// </summary>
    public void RegistrarIntentoFallido(int maxIntentos = 5, int minutosBloqueo = 30)
    {
        IntentosFallidos++;

        if (IntentosFallidos >= maxIntentos)
        {
            BloqueadoHasta = DateTime.UtcNow.AddMinutes(minutosBloqueo);
        }

        MarkAsUpdated();
    }

    /// <summary>
    /// Desbloquea el usuario manualmente
    /// </summary>
    public void Desbloquear()
    {
        if (EstaBloqueado)
        {
            BloqueadoHasta = null;
            IntentosFallidos = 0;
            MarkAsUpdated();
        }
    }

    /// <summary>
    /// Verifica si el usuario tiene acceso a un colegio específico
    /// </summary>
    public bool TieneAccesoColegio(Guid colegioId)
    {
        // Si tiene roles globales, tiene acceso a todos los colegios
        if (TieneRolesGlobales)
            return true;

        // Verificar si tiene roles específicos para este colegio
        return UsuarioRoles.Any(ur => ur.Activo && ur.ColegioId == colegioId);
    }

    /// <summary>
    /// Obtiene los roles activos del usuario para un colegio específico
    /// </summary>
    public IEnumerable<UsuarioRol> GetRolesPorColegio(Guid colegioId)
    {
        return UsuarioRoles.Where(ur => ur.Activo && ur.ColegioId == colegioId);
    }

    /// <summary>
    /// Obtiene los roles globales del usuario
    /// </summary>
    public IEnumerable<UsuarioRol> GetRolesGlobales()
    {
        return UsuarioRoles.Where(ur => ur.Activo && ur.ColegioId == null);
    }

    /// <summary>
    /// Verifica si el usuario puede ser eliminado
    /// </summary>
    public override bool CanBeDeleted()
    {
        // Un usuario no puede ser eliminado si tiene roles activos
        return !UsuarioRoles.Any(ur => ur.Activo);
    }

    /// <summary>
    /// Actualiza el timestamp de modificación
    /// </summary>
    public override void MarkAsUpdated(Guid? updatedBy = null)
    {
        base.MarkAsUpdated(updatedBy);
        FechaActualizacion = DateTime.UtcNow;
    }

    /// <summary>
    /// Información resumida del usuario para logs
    /// </summary>
    public string InfoResumida => $"{Username} ({Email}) - Activo: {Activo}";
}
