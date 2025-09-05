using ColegiosBackend.Core.Entities.Base;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Entities;

/// <summary>
/// Representa la asignación de un profesor a una materia específica en un grupo durante un año académico
/// Esta entidad controla qué profesor puede calificar qué materia en qué grupo
/// </summary>
public class Asignacion : BaseEntity, ITenantEntity, IAuditableEntity
{
    /// <summary>
    /// Constructor privado para Entity Framework
    /// </summary>
    private Asignacion() { }

    /// <summary>
    /// Constructor principal de la asignación
    /// </summary>
    public Asignacion(
        Guid profesorId,
        Guid grupoId,
        Guid materiaId,
        Guid colegioId,
        Guid anoAcademicoId,
        TipoAsignacion tipo = TipoAsignacion.Titular)
    {
        ProfesorId = profesorId;
        GrupoId = grupoId;
        MateriaId = materiaId;
        ColegioId = colegioId;
        AnoAcademicoId = anoAcademicoId;
        Tipo = tipo;
        Estado = EstadoAsignacion.Activa;
        PermiteCalificar = true;
        Activo = true;
        FechaAsignacion = DateTime.UtcNow;
    }

    // Propiedades principales
    /// <summary>
    /// Referencia al profesor asignado
    /// </summary>
    public Guid ProfesorId { get; private set; }

    /// <summary>
    /// Navegación al profesor
    /// </summary>
    public virtual Profesor? Profesor { get; private set; }

    /// <summary>
    /// Referencia al grupo al que se enseña
    /// </summary>
    public Guid GrupoId { get; private set; }

    /// <summary>
    /// Navegación al grupo
    /// </summary>
    public virtual Grupo? Grupo { get; private set; }

    /// <summary>
    /// Referencia a la materia que se enseña
    /// </summary>
    public Guid MateriaId { get; private set; }

    /// <summary>
    /// Navegación a la materia
    /// </summary>
    public virtual Materia? Materia { get; private set; }

    /// <summary>
    /// ID del colegio (tenant)
    /// </summary>
    public Guid? ColegioId { get; private set; }

    /// <summary>
    /// Referencia al año académico
    /// </summary>
    public Guid AnoAcademicoId { get; private set; }

    /// <summary>
    /// Navegación al año académico
    /// </summary>
    public virtual AnoAcademico? AnoAcademico { get; private set; }

    /// <summary>
    /// Tipo de asignación (Titular, Suplente, Apoyo)
    /// </summary>
    public TipoAsignacion Tipo { get; private set; }

    /// <summary>
    /// Estado actual de la asignación
    /// </summary>
    public EstadoAsignacion Estado { get; private set; }

    /// <summary>
    /// Fecha en que se realizó la asignación
    /// </summary>
    public DateTime FechaAsignacion { get; private set; }

    /// <summary>
    /// Fecha de inicio de la asignación (puede ser diferente a la fecha de asignación)
    /// </summary>
    public DateTime? FechaInicio { get; private set; }

    /// <summary>
    /// Fecha de fin de la asignación
    /// </summary>
    public DateTime? FechaFin { get; private set; }

    /// <summary>
    /// Intensidad horaria semanal para esta asignación
    /// </summary>
    public int IntensidadHorariaSemanal { get; private set; } = 1;

    /// <summary>
    /// Indica si el profesor puede calificar en esta asignación
    /// </summary>
    public bool PermiteCalificar { get; private set; }

    /// <summary>
    /// Indica si el profesor puede registrar asistencia
    /// </summary>
    public bool PermiteRegistrarAsistencia { get; private set; } = true;

    /// <summary>
    /// Observaciones sobre la asignación
    /// </summary>
    public string? Observaciones { get; private set; }

    /// <summary>
    /// Configuración específica de la asignación en formato JSON
    /// Puede incluir: metodología, recursos, evaluación personalizada, etc.
    /// </summary>
    public string? ConfiguracionAsignacion { get; private set; }

    /// <summary>
    /// Indica si la asignación está activa
    /// </summary>
    public bool Activo { get; private set; }
        
    // Métodos de negocio

    /// <summary>
    /// Actualiza las fechas de vigencia de la asignación
    /// </summary>
    public void EstablecerPeriodoVigencia(DateTime? fechaInicio, DateTime? fechaFin)
    {
        if (fechaInicio.HasValue && fechaFin.HasValue && fechaInicio >= fechaFin)
            throw new ArgumentException("La fecha de inicio debe ser anterior a la fecha de fin");

        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
    }

    /// <summary>
    /// Actualiza la intensidad horaria semanal
    /// </summary>
    public void ActualizarIntensidadHoraria(int horas)
    {
        if (horas < 1 || horas > 40)
            throw new ArgumentException("La intensidad horaria debe estar entre 1 y 40 horas");

        IntensidadHorariaSemanal = horas;
    }

    /// <summary>
    /// Cambia el tipo de asignación
    /// </summary>
    public void CambiarTipo(TipoAsignacion nuevoTipo)
    {
        if (Estado == EstadoAsignacion.Finalizada)
            throw new InvalidOperationException("No se puede cambiar el tipo de una asignación finalizada");

        Tipo = nuevoTipo;
    }

    /// <summary>
    /// Suspende temporalmente la asignación
    /// </summary>
    public void Suspender(string? motivo = null)
    {
        if (Estado == EstadoAsignacion.Finalizada)
            throw new InvalidOperationException("No se puede suspender una asignación finalizada");

        Estado = EstadoAsignacion.Suspendida;
        PermiteCalificar = false;
        PermiteRegistrarAsistencia = false;

        if (!string.IsNullOrEmpty(motivo))
        {
            Observaciones = $"{Observaciones} [SUSPENDIDA: {motivo}]".Trim();
        }
    }

    /// <summary>
    /// Reactiva una asignación suspendida
    /// </summary>
    public void Reactivar()
    {
        if (Estado == EstadoAsignacion.Finalizada)
            throw new InvalidOperationException("No se puede reactivar una asignación finalizada");

        Estado = EstadoAsignacion.Activa;
        PermiteCalificar = true;
        PermiteRegistrarAsistencia = true;
    }

    /// <summary>
    /// Finaliza la asignación permanentemente
    /// </summary>
    public void Finalizar(string? motivo = null)
    {
        Estado = EstadoAsignacion.Finalizada;
        PermiteCalificar = false;
        PermiteRegistrarAsistencia = false;
        FechaFin = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(motivo))
        {
            Observaciones = $"{Observaciones} [FINALIZADA: {motivo}]".Trim();
        }
    }

    /// <summary>
    /// Habilita o deshabilita permisos de calificación
    /// </summary>
    public void ConfigurarPermisoCalificar(bool permite)
    {
        if (Estado == EstadoAsignacion.Activa)
        {
            PermiteCalificar = permite;
        }
    }

    /// <summary>
    /// Habilita o deshabilita permisos de registro de asistencia
    /// </summary>
    public void ConfigurarPermisoAsistencia(bool permite)
    {
        if (Estado == EstadoAsignacion.Activa)
        {
            PermiteRegistrarAsistencia = permite;
        }
    }

    /// <summary>
    /// Actualiza observaciones de la asignación
    /// </summary>
    public void ActualizarObservaciones(string? observaciones)
    {
        Observaciones = observaciones;
    }

    /// <summary>
    /// Establece configuración específica de la asignación
    /// </summary>
    public void EstablecerConfiguracion(string? configuracion)
    {
        ConfiguracionAsignacion = configuracion;
    }

    /// <summary>
    /// Verifica si la asignación está vigente en una fecha específica
    /// </summary>
    public bool EstaVigenteEn(DateTime fecha)
    {
        var fechaSolo = fecha.Date;

        if (!Activo || Estado != EstadoAsignacion.Activa)
            return false;

        if (FechaInicio.HasValue && fechaSolo < FechaInicio.Value.Date)
            return false;

        if (FechaFin.HasValue && fechaSolo > FechaFin.Value.Date)
            return false;

        return true;
    }

    /// <summary>
    /// Verifica si el profesor puede calificar actualmente
    /// </summary>
    public bool PuedeCalificar()
    {
        return Activo && Estado == EstadoAsignacion.Activa && PermiteCalificar;
    }

    /// <summary>
    /// Verifica si el profesor puede registrar asistencia
    /// </summary>
    public bool PuedeRegistrarAsistencia()
    {
        return Activo && Estado == EstadoAsignacion.Activa && PermiteRegistrarAsistencia;
    }

    /// <summary>
    /// Activa la asignación
    /// </summary>
    public void Activar()
    {
        Activo = true;
    }

    /// <summary>
    /// Desactiva la asignación
    /// </summary>
    public void Desactivar()
    {
        Activo = false;
        PermiteCalificar = false;
        PermiteRegistrarAsistencia = false;
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
}
