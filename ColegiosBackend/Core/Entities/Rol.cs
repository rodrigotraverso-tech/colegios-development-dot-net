using ColegiosBackend.Core.Entities.Base;
using ColegiosBackend.Core.ValueObjects.Permisos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ColegiosBackend.Core.Entities;

/// <summary>
/// Entidad que representa un rol del sistema con permisos granulares
/// Puede ser global (colegio_id = null) o específico por colegio
/// </summary>
public class Rol : BaseEntity, IAuditableEntity, ITenantEntity
{
    /// <summary>
    /// ID del colegio al que pertenece el rol (NULL para roles globales)
    /// </summary>
    public Guid? ColegioId { get; private set; }

    /// <summary>
    /// Código único del rol (ej: ADMIN_GLOBAL, PROFESOR, COORDINADOR)
    /// </summary>
    public string Codigo { get; private set; } = string.Empty;

    /// <summary>
    /// Nombre descriptivo del rol
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del rol
    /// </summary>
    public string? Descripcion { get; private set; }

    /// <summary>
    /// Permisos del rol en formato JSON (campo JSONB en BD)
    /// </summary>
    public string PermisosJson { get; private set; } = "{}";

    /// <summary>
    /// Indica si el rol está activo
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Fecha de creación del rol (campo de BD)
    /// </summary>
    public DateTime FechaCreacion { get; private set; }

    /// <summary>
    /// Fecha de última actualización (campo de BD)
    /// </summary>
    public DateTime FechaActualizacion { get; private set; }

    // Navegación
    /// <summary>
    /// Colegio al que pertenece el rol (null para roles globales)
    /// </summary>
    public Colegio? Colegio { get; private set; }

    /// <summary>
    /// Asignaciones de usuarios con este rol
    /// </summary>
    public ICollection<UsuarioRol> UsuarioRoles { get; private set; } = new List<UsuarioRol>();

    /// <summary>
    /// Permisos del rol deserializados desde JSON
    /// </summary>
    public PermisosRol Permisos
    {
        get
        {
            try
            {
                return JsonSerializer.Deserialize<PermisosRol>(PermisosJson) ?? new PermisosRol();
            }
            catch
            {
                return new PermisosRol();
            }
        }
        private set
        {
            PermisosJson = JsonSerializer.Serialize(value, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = true
            });
        }
    }

    /// <summary>
    /// Verifica si es un rol global
    /// </summary>
    public bool EsRolGlobal => ColegioId == null;

    /// <summary>
    /// Verifica si es uno de los roles globales predefinidos
    /// </summary>
    public bool EsRolGlobalPredefinido => EsRolGlobal &&
        (Codigo == "ADMIN_GLOBAL" || Codigo == "SUPER_ADMIN");

    /// <summary>
    /// Constructor privado para Entity Framework
    /// </summary>
    private Rol() : base() { }

    /// <summary>
    /// Constructor para crear un nuevo rol
    /// </summary>
    public Rol(
        string codigo,
        string nombre,
        PermisosRol permisos,
        Guid? colegioId = null,
        string? descripcion = null) : base()
    {
        SetCodigo(codigo);
        SetNombre(nombre);
        SetPermisos(permisos);
        ColegioId = colegioId;
        SetDescripcion(descripcion);

        Activo = true;
        FechaCreacion = DateTime.UtcNow;
        FechaActualizacion = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method para crear rol global
    /// </summary>
    public static Rol CrearRolGlobal(
        string codigo,
        string nombre,
        PermisosRol permisos,
        string? descripcion = null)
    {
        if (codigo != "ADMIN_GLOBAL" && codigo != "SUPER_ADMIN")
            throw new ArgumentException($"Solo se permiten códigos ADMIN_GLOBAL y SUPER_ADMIN para roles globales. Código recibido: {codigo}");

        return new Rol(codigo, nombre, permisos, null, descripcion);
    }

    /// <summary>
    /// Factory method para crear rol específico de colegio
    /// </summary>
    public static Rol CrearRolColegio(
        string codigo,
        string nombre,
        PermisosRol permisos,
        Guid colegioId,
        string? descripcion = null)
    {
        if (colegioId == Guid.Empty)
            throw new ArgumentException("El ID del colegio no puede estar vacío para roles específicos");

        if (codigo == "ADMIN_GLOBAL" || codigo == "SUPER_ADMIN")
            throw new ArgumentException("Los códigos ADMIN_GLOBAL y SUPER_ADMIN están reservados para roles globales");

        return new Rol(codigo, nombre, permisos, colegioId, descripcion);
    }

    /// <summary>
    /// Actualiza el código del rol
    /// </summary>
    public void SetCodigo(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("El código del rol es obligatorio", nameof(codigo));

        if (codigo.Length > 50)
            throw new ArgumentException("El código del rol no puede exceder 50 caracteres", nameof(codigo));

        // Validar consistencia con tipo de rol
        if (EsRolGlobal && codigo != "ADMIN_GLOBAL" && codigo != "SUPER_ADMIN")
            throw new ArgumentException("Solo se permiten códigos ADMIN_GLOBAL y SUPER_ADMIN para roles globales");

        if (!EsRolGlobal && (codigo == "ADMIN_GLOBAL" || codigo == "SUPER_ADMIN"))
            throw new ArgumentException("Los códigos ADMIN_GLOBAL y SUPER_ADMIN están reservados para roles globales");

        Codigo = codigo.ToUpperInvariant();
        MarkAsUpdated();
    }

    /// <summary>
    /// Actualiza el nombre del rol
    /// </summary>
    public void SetNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del rol es obligatorio", nameof(nombre));

        if (nombre.Length > 100)
            throw new ArgumentException("El nombre del rol no puede exceder 100 caracteres", nameof(nombre));

        Nombre = nombre.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Actualiza la descripción del rol
    /// </summary>
    public void SetDescripcion(string? descripcion)
    {
        if (!string.IsNullOrWhiteSpace(descripcion) && descripcion.Length > 500)
            throw new ArgumentException("La descripción no puede exceder 500 caracteres", nameof(descripcion));

        Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Actualiza los permisos del rol
    /// </summary>
    public void SetPermisos(PermisosRol permisos)
    {
        if (permisos == null)
            throw new ArgumentNullException(nameof(permisos));

        Permisos = permisos;
        MarkAsUpdated();
    }

    /// <summary>
    /// Actualiza un permiso específico
    /// </summary>
    public void UpdatePermiso(string modulo, string accion, bool valor)
    {
        var permisosActuales = Permisos;

        // Esta implementación requeriría reflexión o un switch extenso
        // Por simplicidad, se recomienda usar SetPermisos con el objeto completo
        throw new NotImplementedException("Use SetPermisos() con el objeto PermisosRol completo actualizado");
    }

    /// <summary>
    /// Activa el rol
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
    /// Desactiva el rol
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
    /// Verifica si el rol tiene un permiso específico
    /// </summary>
    public bool TienePermiso(string modulo, string accion)
    {
        return Permisos.TienePermiso(modulo, accion);
    }

    /// <summary>
    /// Verifica si el usuario puede acceder en el horario especificado
    /// </summary>
    public bool PermiteAccesoEnHorario(DateTime fechaHora)
    {
        return Permisos.Restricciones.EstaEnHorarioPermitido(fechaHora);
    }

    /// <summary>
    /// Verifica si el usuario puede acceder desde la IP especificada
    /// </summary>
    public bool PermiteAccesoDesdeIp(string ipAddress)
    {
        return Permisos.Restricciones.EsIpPermitida(ipAddress);
    }

    /// <summary>
    /// Obtiene un resumen de los permisos principales
    /// </summary>
    public string ResumenPermisos
    {
        get
        {
            var permisos = Permisos;
            var resumen = new List<string>();

            if (permisos.Modulos.Estudiantes.Ver) resumen.Add("Ver Estudiantes");
            if (permisos.Modulos.Calificaciones.Crear) resumen.Add("Crear Calificaciones");
            if (permisos.Modulos.Administracion.GestionarUsuarios) resumen.Add("Gestionar Usuarios");
            if (permisos.ConfiguracionEspecial.AccesoTodosColegios) resumen.Add("Acceso Global");

            return resumen.Any() ? string.Join(", ", resumen) : "Sin permisos principales";
        }
    }

    /// <summary>
    /// Clona el rol para crear uno nuevo basado en este
    /// </summary>
    public Rol ClonarPara(string nuevoCodigo, string nuevoNombre, Guid? nuevoColegioId = null)
    {
        var permisosClonados = JsonSerializer.Deserialize<PermisosRol>(PermisosJson) ?? new PermisosRol();

        return nuevoColegioId == null
            ? CrearRolGlobal(nuevoCodigo, nuevoNombre, permisosClonados, Descripcion)
            : CrearRolColegio(nuevoCodigo, nuevoNombre, permisosClonados, nuevoColegioId.Value, Descripcion);
    }

    /// <summary>
    /// Implementación de ITenantEntity
    /// </summary>
    public void SetColegio(Guid colegioId)
    {
        if (EsRolGlobalPredefinido)
            throw new InvalidOperationException("No se puede cambiar el colegio de un rol global predefinido");

        if (colegioId == Guid.Empty)
            throw new ArgumentException("El ID del colegio no puede estar vacío", nameof(colegioId));

        ColegioId = colegioId;
        MarkAsUpdated();
    }

    /// <summary>
    /// Verifica si el rol pertenece al colegio especificado
    /// </summary>
    public bool BelongsToColegio(Guid colegioId)
    {
        return ColegioId == colegioId || EsRolGlobal; // Roles globales pertenecen a todos los colegios
    }

    /// <summary>
    /// Verifica si el rol puede ser eliminado
    /// </summary>
    public override bool CanBeDeleted()
    {
        // Los roles globales predefinidos no pueden ser eliminados
        if (EsRolGlobalPredefinido) return false;

        // Los roles con asignaciones activas no pueden ser eliminados
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
    /// Información resumida del rol
    /// </summary>
    public override string ToString()
    {
        var ambito = EsRolGlobal ? "GLOBAL" : $"Colegio {ColegioId}";
        return $"{Codigo} ({Nombre}) - {ambito} - {(Activo ? "Activo" : "Inactivo")}";
    }
}
