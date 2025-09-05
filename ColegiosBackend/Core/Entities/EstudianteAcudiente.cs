using ColegiosBackend.Core.Entities.Base;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Entities;

/// <summary>
/// Entidad que representa la relación entre un estudiante y sus acudientes (padres, tutores, etc.)
/// Permite que un estudiante tenga múltiples acudientes y que un acudiente tenga múltiples estudiantes
/// </summary>
public class EstudianteAcudiente : BaseEntity, ITenantEntity
{
    /// <summary>
    /// ID del colegio (mismo del estudiante para consistencia)
    /// </summary>
    public Guid? ColegioId { get; private set; }

    /// <summary>
    /// ID del estudiante
    /// </summary>
    public Guid EstudianteId { get; private set; }

    /// <summary>
    /// ID de la persona que actúa como acudiente
    /// </summary>
    public Guid AcudienteId { get; private set; }

    /// <summary>
    /// Tipo de relación entre el acudiente y el estudiante
    /// </summary>
    public TipoRelacionAcudiente TipoRelacion { get; private set; }

    /// <summary>
    /// Indica si este acudiente es el principal/primario
    /// Solo puede haber uno principal por estudiante
    /// </summary>
    public bool EsPrincipal { get; private set; }

    /// <summary>
    /// Indica si el acudiente puede retirar al estudiante del colegio
    /// </summary>
    public bool PuedeRetirar { get; private set; }

    /// <summary>
    /// Indica si el acudiente puede autorizar salidas/permisos
    /// </summary>
    public bool PuedeAutorizar { get; private set; }

    /// <summary>
    /// Indica si el acudiente debe recibir notificaciones académicas
    /// </summary>
    public bool RecibirNotificacionesAcademicas { get; private set; }

    /// <summary>
    /// Indica si el acudiente debe recibir notificaciones disciplinarias
    /// </summary>
    public bool RecibirNotificacionesDisciplinarias { get; private set; }

    /// <summary>
    /// Indica si el acudiente debe recibir notificaciones financieras
    /// </summary>
    public bool RecibirNotificacionesFinancieras { get; private set; }

    /// <summary>
    /// Orden de prioridad para contacto (1 = más prioritario)
    /// </summary>
    public int OrdenPrioridad { get; private set; }

    /// <summary>
    /// Fecha desde cuando es válida esta relación
    /// </summary>
    public DateTime FechaInicioRelacion { get; private set; }

    /// <summary>
    /// Fecha hasta cuando es válida esta relación (null = indefinida)
    /// </summary>
    public DateTime? FechaFinRelacion { get; private set; }

    /// <summary>
    /// Indica si la relación está activa
    /// </summary>
    public bool EstaActiva { get; private set; }

    /// <summary>
    /// Observaciones sobre la relación acudiente-estudiante
    /// </summary>
    public string? Observaciones { get; private set; }

    /// <summary>
    /// Restricciones específicas para este acudiente
    /// Ejemplo: "No puede retirar los viernes", "Solo emergencias"
    /// </summary>
    public string? Restricciones { get; private set; }

    // Propiedades de navegación
    /// <summary>
    /// Estudiante asociado
    /// </summary>
    public virtual Estudiante? Estudiante { get; set; }

    /// <summary>
    /// Persona que actúa como acudiente
    /// </summary>
    public virtual Persona? Acudiente { get; set; }

    /// <summary>
    /// Colegio donde se establece la relación
    /// </summary>
    public virtual Colegio? Colegio { get; set; }

    /// <summary>
    /// Constructor privado para EF Core
    /// </summary>
    private EstudianteAcudiente() { }

    /// <summary>
    /// Constructor para crear nueva relación estudiante-acudiente
    /// </summary>
    public EstudianteAcudiente(
        Guid colegioId,
        Guid estudianteId,
        Guid acudienteId,
        TipoRelacionAcudiente tipoRelacion,
        bool esPrincipal = false,
        int ordenPrioridad = 1)
    {
        ColegioId = colegioId;
        EstudianteId = estudianteId;
        AcudienteId = acudienteId;
        TipoRelacion = tipoRelacion;
        EsPrincipal = esPrincipal;
        OrdenPrioridad = ordenPrioridad;
        FechaInicioRelacion = DateTime.UtcNow;
        EstaActiva = true;

        // Permisos por defecto basados en tipo de relación
        ConfigurarPermisosDefecto();
        ValidarDatos();
    }

    /// <summary>
    /// Implementación de ITenantEntity - Establece el colegio
    /// </summary>
    public void SetColegio(Guid colegioId)
    {
        ColegioId = colegioId;
        MarkAsUpdated();
    }

    /// <summary>
    /// Implementación de ITenantEntity - Verifica pertenencia al colegio
    /// </summary>
    public bool BelongsToColegio(Guid colegioId)
    {
        return ColegioId == colegioId || ColegioId == null;
    }

    /// <summary>
    /// Establece como acudiente principal
    /// </summary>
    public void EstablecerComoPrincipal(Guid? updatedBy = null)
    {
        EsPrincipal = true;
        OrdenPrioridad = 1;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Remueve la designación de principal
    /// </summary>
    public void RemoverComoPrincipal(Guid? updatedBy = null)
    {
        EsPrincipal = false;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Actualiza los permisos del acudiente
    /// </summary>
    public void ActualizarPermisos(
        bool puedeRetirar,
        bool puedeAutorizar,
        bool notificacionesAcademicas,
        bool notificacionesDisciplinarias,
        bool notificacionesFinancieras,
        Guid? updatedBy = null)
    {
        PuedeRetirar = puedeRetirar;
        PuedeAutorizar = puedeAutorizar;
        RecibirNotificacionesAcademicas = notificacionesAcademicas;
        RecibirNotificacionesDisciplinarias = notificacionesDisciplinarias;
        RecibirNotificacionesFinancieras = notificacionesFinancieras;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Actualiza el orden de prioridad
    /// </summary>
    public void ActualizarOrdenPrioridad(int nuevoOrden, Guid? updatedBy = null)
    {
        if (nuevoOrden < 1)
            throw new ArgumentException("El orden de prioridad debe ser mayor a 0", nameof(nuevoOrden));

        OrdenPrioridad = nuevoOrden;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Desactiva la relación
    /// </summary>
    public void DesactivarRelacion(DateTime? fechaFin = null, string? motivo = null, Guid? updatedBy = null)
    {
        EstaActiva = false;
        FechaFinRelacion = fechaFin ?? DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(motivo))
        {
            Observaciones = string.IsNullOrWhiteSpace(Observaciones)
                ? $"Desactivada: {motivo}"
                : $"{Observaciones}\nDesactivada: {motivo}";
        }

        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Reactiva la relación
    /// </summary>
    public void ReactivarRelacion(string? motivo = null, Guid? updatedBy = null)
    {
        EstaActiva = true;
        FechaFinRelacion = null;

        if (!string.IsNullOrWhiteSpace(motivo))
        {
            Observaciones = string.IsNullOrWhiteSpace(Observaciones)
                ? $"Reactivada: {motivo}"
                : $"{Observaciones}\nReactivada: {motivo}";
        }

        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Actualiza restricciones
    /// </summary>
    public void ActualizarRestricciones(string? restricciones, Guid? updatedBy = null)
    {
        Restricciones = restricciones;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Actualiza observaciones
    /// </summary>
    public void ActualizarObservaciones(string? observaciones, Guid? updatedBy = null)
    {
        Observaciones = observaciones;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Verifica si el acudiente tiene algún permiso activo
    /// </summary>
    public bool TienePermisosActivos()
    {
        return EstaActiva && (PuedeRetirar || PuedeAutorizar ||
               RecibirNotificacionesAcademicas || RecibirNotificacionesDisciplinarias ||
               RecibirNotificacionesFinancieras);
    }

    /// <summary>
    /// Verifica si puede recibir algún tipo de notificación
    /// </summary>
    public bool PuedeRecibirNotificaciones()
    {
        return EstaActiva && (RecibirNotificacionesAcademicas ||
               RecibirNotificacionesDisciplinarias || RecibirNotificacionesFinancieras);
    }

    /// <summary>
    /// Configura permisos por defecto según el tipo de relación
    /// </summary>
    private void ConfigurarPermisosDefecto()
    {
        switch (TipoRelacion)
        {
            case TipoRelacionAcudiente.Padre:
            case TipoRelacionAcudiente.Madre:
                PuedeRetirar = true;
                PuedeAutorizar = true;
                RecibirNotificacionesAcademicas = true;
                RecibirNotificacionesDisciplinarias = true;
                RecibirNotificacionesFinancieras = true;
                break;

            case TipoRelacionAcudiente.TutorLegal:
                PuedeRetirar = true;
                PuedeAutorizar = true;
                RecibirNotificacionesAcademicas = true;
                RecibirNotificacionesDisciplinarias = true;
                RecibirNotificacionesFinancieras = true;
                break;

            case TipoRelacionAcudiente.Abuelo:
            case TipoRelacionAcudiente.Padrastro:
            case TipoRelacionAcudiente.Madrastra:
                PuedeRetirar = true;
                PuedeAutorizar = false;
                RecibirNotificacionesAcademicas = true;
                RecibirNotificacionesDisciplinarias = true;
                RecibirNotificacionesFinancieras = false;
                break;

            default:
                // Para hermanos, tíos, otros
                PuedeRetirar = false;
                PuedeAutorizar = false;
                RecibirNotificacionesAcademicas = false;
                RecibirNotificacionesDisciplinarias = false;
                RecibirNotificacionesFinancieras = false;
                break;
        }
    }

    /// <summary>
    /// Valida los datos de la relación
    /// </summary>
    private void ValidarDatos()
    {
        if (ColegioId == null || ColegioId == Guid.Empty)
            throw new ArgumentException("El ID del colegio es requerido", nameof(ColegioId));

        if (EstudianteId == Guid.Empty)
            throw new ArgumentException("El ID del estudiante es requerido", nameof(EstudianteId));

        if (AcudienteId == Guid.Empty)
            throw new ArgumentException("El ID del acudiente es requerido", nameof(AcudienteId));

        if (EstudianteId == AcudienteId)
            throw new ArgumentException("El estudiante no puede ser su propio acudiente");

        if (OrdenPrioridad < 1)
            throw new ArgumentException("El orden de prioridad debe ser mayor a 0", nameof(OrdenPrioridad));

        if (FechaInicioRelacion > DateTime.UtcNow.AddDays(1))
            throw new ArgumentException("La fecha de inicio no puede ser muy futura", nameof(FechaInicioRelacion));

        if (FechaFinRelacion.HasValue && FechaFinRelacion < FechaInicioRelacion)
            throw new ArgumentException("La fecha de fin no puede ser anterior a la fecha de inicio");
    }

    /// <summary>
    /// Override ToString para debugging
    /// </summary>
    public override string ToString()
    {
        return $"EstudianteAcudiente: {TipoRelacion} - Principal: {EsPrincipal} - Activa: {EstaActiva}";
    }
}