using ColegiosBackend.Core.Entities.Base;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Entities;

/// <summary>
/// Entidad que representa el registro de asistencia de un estudiante a una clase específica
/// </summary>
public class Asistencia : BaseEntity, ITenantEntity
{
    #region Propiedades Principales
    /// <summary>
    /// ID del estudiante al que pertenece la asistencia
    /// </summary>
    public Guid EstudianteId { get; private set; }

    /// <summary>
    /// ID de la materia de la clase
    /// </summary>
    public Guid MateriaId { get; private set; }

    /// <summary>
    /// ID del grupo/curso donde se tomó la asistencia
    /// </summary>
    public Guid GrupoId { get; private set; }

    /// <summary>
    /// ID del año académico
    /// </summary>
    public Guid AnoAcademicoId { get; private set; }

    /// <summary>
    /// ID del período evaluativo (bimestre/trimestre)
    /// </summary>
    public Guid PeriodoEvaluativoId { get; private set; }

    /// <summary>
    /// Fecha y hora específica de la clase
    /// </summary>
    public DateTime FechaClase { get; private set; }

    /// <summary>
    /// Estado de la asistencia (presente, ausente, tardanza, etc.)
    /// </summary>
    public EstadoAsistencia Estado { get; private set; }

    /// <summary>
    /// Observaciones adicionales sobre la asistencia
    /// </summary>
    public string? Observaciones { get; private set; }

    /// <summary>
    /// ID del profesor que registró la asistencia
    /// </summary>
    public Guid ProfesorId { get; private set; }

    /// <summary>
    /// Indica si la falta fue justificada posteriormente
    /// </summary>
    public bool EsJustificada { get; private set; }

    /// <summary>
    /// Fecha en que se justificó la ausencia (si aplica)
    /// </summary>
    public DateTime? FechaJustificacion { get; private set; }

    /// <summary>
    /// Motivo de la justificación
    /// </summary>
    public string? MotivoJustificacion { get; private set; }

    /// <summary>
    /// ID del usuario que justificó la ausencia
    /// </summary>
    public Guid? JustificadoPorId { get; private set; }

    /// <summary>
    /// Registro activo o inactivo
    /// </summary>
    public bool Activo { get; private set; } = true;

    #endregion

    #region Propiedades de Navegación

    /// <summary>
    /// Estudiante al que pertenece la asistencia
    /// </summary>
    public virtual Estudiante Estudiante { get; private set; } = null!;

    /// <summary>
    /// Materia de la clase
    /// </summary>
    public virtual Materia Materia { get; private set; } = null!;

    /// <summary>
    /// Grupo donde se tomó la asistencia
    /// </summary>
    public virtual Grupo Grupo { get; private set; } = null!;

    /// <summary>
    /// Año académico
    /// </summary>
    public virtual AnoAcademico AnoAcademico { get; private set; } = null!;

    /// <summary>
    /// Período evaluativo
    /// </summary>
    public virtual PeriodoEvaluativo PeriodoEvaluativo { get; private set; } = null!;

    /// <summary>
    /// Profesor que registró la asistencia
    /// </summary>
    public virtual Profesor Profesor { get; private set; } = null!;

    #endregion

    #region ITenantEntity Implementation

    /// <summary>
    /// ID del colegio (heredado del estudiante)
    /// </summary>
    public Guid? ColegioId { get; private set; }

    /// <summary>
    /// Establece el colegio para la asistencia
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    public void SetColegio(Guid colegioId)
    {
        ColegioId = colegioId;
    }

    /// <summary>
    /// Verifica si la asistencia pertenece al colegio especificado
    /// </summary>
    /// <param name="colegioId">ID del colegio a verificar</param>
    /// <returns>True si pertenece al colegio</returns>
    public bool BelongsToColegio(Guid colegioId)
    {
        return ColegioId == colegioId;
    }

    #endregion

    #region Constructores

    /// <summary>
    /// Constructor privado para EF Core
    /// </summary>
    private Asistencia() : base() { }

    /// <summary>
    /// Constructor para crear nueva asistencia
    /// </summary>
    public Asistencia(
        Guid estudianteId,
        Guid materiaId,
        Guid grupoId,
        Guid anoAcademicoId,
        Guid periodoEvaluativoId,
        DateTime fechaClase,
        EstadoAsistencia estado,
        Guid profesorId,
        Guid colegioId,
        string? observaciones = null) : base()
    {
        Id = Guid.NewGuid();
        EstudianteId = estudianteId;
        MateriaId = materiaId;
        GrupoId = grupoId;
        AnoAcademicoId = anoAcademicoId;
        PeriodoEvaluativoId = periodoEvaluativoId;
        FechaClase = fechaClase;
        Estado = estado;
        ProfesorId = profesorId;
        Observaciones = observaciones?.Trim();
        ColegioId = colegioId;
        EsJustificada = false;
        Activo = true;

        ValidarDatos();
    }

    #endregion

    #region Métodos de Negocio

    /// <summary>
    /// Actualiza el estado de asistencia
    /// </summary>
    public void ActualizarEstado(EstadoAsistencia nuevoEstado, string? observaciones = null)
    {
        if (!Activo)
            throw new InvalidOperationException("No se puede actualizar una asistencia inactiva");

        Estado = nuevoEstado;

        if (!string.IsNullOrWhiteSpace(observaciones))
            Observaciones = observaciones.Trim();

        // Si se marca como presente, quitar justificación previa
        if (nuevoEstado == EstadoAsistencia.Presente)
        {
            EsJustificada = false;
            FechaJustificacion = null;
            MotivoJustificacion = null;
            JustificadoPorId = null;
        }
    }

    /// <summary>
    /// Justifica una ausencia o tardanza
    /// </summary>
    public void Justificar(string motivoJustificacion, Guid justificadoPorId)
    {
        if (!Activo)
            throw new InvalidOperationException("No se puede justificar una asistencia inactiva");

        if (Estado == EstadoAsistencia.Presente)
            throw new InvalidOperationException("No se puede justificar una asistencia marcada como presente");

        if (string.IsNullOrWhiteSpace(motivoJustificacion))
            throw new ArgumentException("El motivo de justificación es requerido");

        EsJustificada = true;
        FechaJustificacion = DateTime.UtcNow;
        MotivoJustificacion = motivoJustificacion.Trim();
        JustificadoPorId = justificadoPorId;
    }

    /// <summary>
    /// Quita la justificación de una ausencia
    /// </summary>
    public void QuitarJustificacion()
    {
        if (!Activo)
            throw new InvalidOperationException("No se puede modificar una asistencia inactiva");

        EsJustificada = false;
        FechaJustificacion = null;
        MotivoJustificacion = null;
        JustificadoPorId = null;
    }

    /// <summary>
    /// Verifica si es una ausencia injustificada
    /// </summary>
    public bool EsAusenciaInjustificada()
    {
        return Estado == EstadoAsistencia.Ausente && !EsJustificada;
    }

    /// <summary>
    /// Verifica si es una tardanza injustificada
    /// </summary>
    public bool EsTardanzaInjustificada()
    {
        return Estado == EstadoAsistencia.Tardanza && !EsJustificada;
    }

    /// <summary>
    /// Obtiene descripción completa del estado de asistencia
    /// </summary>
    public string GetDescripcionCompleta()
    {
        var descripcion = Estado switch
        {
            EstadoAsistencia.Presente => "Presente",
            EstadoAsistencia.Ausente => EsJustificada ? "Ausente (Justificada)" : "Ausente",
            EstadoAsistencia.Tardanza => EsJustificada ? "Tardanza (Justificada)" : "Tardanza",
            EstadoAsistencia.AusentePorSuspension => "Ausente por Suspensión",
            _ => "Estado Desconocido"
        };

        if (!string.IsNullOrWhiteSpace(Observaciones))
            descripcion += $" - {Observaciones}";

        return descripcion;
    }

    /// <summary>
    /// Desactiva el registro de asistencia
    /// </summary>
    public void Desactivar()
    {
        Activo = false;
    }

    /// <summary>
    /// Reactiva el registro de asistencia
    /// </summary>
    public void Reactivar()
    {
        Activo = true;
    }

    #endregion

    #region Validaciones

    /// <summary>
    /// Valida los datos de la entidad
    /// </summary>
    private void ValidarDatos()
    {
        if (EstudianteId == Guid.Empty)
            throw new ArgumentException("El ID del estudiante es requerido");

        if (MateriaId == Guid.Empty)
            throw new ArgumentException("El ID de la materia es requerido");

        if (GrupoId == Guid.Empty)
            throw new ArgumentException("El ID del grupo es requerido");

        if (AnoAcademicoId == Guid.Empty)
            throw new ArgumentException("El ID del año académico es requerido");

        if (PeriodoEvaluativoId == Guid.Empty)
            throw new ArgumentException("El ID del período evaluativo es requerido");

        if (ProfesorId == Guid.Empty)
            throw new ArgumentException("El ID del profesor es requerido");

        if (ColegioId == null || ColegioId == Guid.Empty)
            throw new ArgumentException("El ID del colegio es requerido");

        if (FechaClase == default)
            throw new ArgumentException("La fecha de clase es requerida");

        if (FechaClase > DateTime.Now.AddDays(1))
            throw new ArgumentException("La fecha de clase no puede ser más de un día en el futuro");
    }

    /// <summary>
    /// Verifica si se puede eliminar la asistencia
    /// </summary>
    public override bool CanBeDeleted()
    {
        // Solo se puede eliminar si es del día actual o es de prueba
        var fechaLimite = DateTime.Today.AddDays(-1);
        return FechaClase >= fechaLimite;
    }

    #endregion
}