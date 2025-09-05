using ColegiosBackend.Core.Entities.Base;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Entities;

/// <summary>
/// Entidad Estudiante - Representa un estudiante matriculado en un colegio específico
/// Un estudiante está vinculado a una Persona y puede estar matriculado en múltiples colegios
/// </summary>
public class Estudiante : BaseEntity, ITenantEntity
{
    /// <summary>
    /// ID del colegio al que pertenece este registro de estudiante
    /// </summary>
    public Guid? ColegioId { get; private set; }

    /// <summary>
    /// ID de la persona asociada (tabla personas)
    /// </summary>
    public Guid PersonaId { get; private set; }

    /// <summary>
    /// Código único del estudiante dentro del colegio
    /// Formato sugerido: EST-YYYY-NNNN (ej: EST-2024-0001)
    /// </summary>
    public string CodigoEstudiante { get; private set; } = string.Empty;

    /// <summary>
    /// Estado actual del estudiante en el colegio
    /// </summary>
    public EstadoEstudiante Estado { get; private set; }

    /// <summary>
    /// Fecha de primera matrícula en este colegio
    /// </summary>
    public DateTime FechaIngreso { get; private set; }

    /// <summary>
    /// Fecha de egreso/retiro (null si sigue activo)
    /// </summary>
    public DateTime? FechaEgreso { get; private set; }

    /// <summary>
    /// Motivo del egreso/retiro
    /// </summary>
    public string? MotivoEgreso { get; private set; }

    /// <summary>
    /// Número de matrícula actual (puede cambiar cada año académico)
    /// </summary>
    public string? NumeroMatricula { get; private set; }

    /// <summary>
    /// Año académico de ingreso
    /// </summary>
    public int AnoIngreso { get; private set; }

    /// <summary>
    /// Información médica relevante (alergias, medicamentos, etc.)
    /// </summary>
    public string? InformacionMedica { get; private set; }

    /// <summary>
    /// Contacto de emergencia - Nombre
    /// </summary>
    public string? ContactoEmergenciaNombre { get; private set; }

    /// <summary>
    /// Contacto de emergencia - Teléfono
    /// </summary>
    public string? ContactoEmergenciaTelefono { get; private set; }

    /// <summary>
    /// Contacto de emergencia - Relación con el estudiante
    /// </summary>
    public string? ContactoEmergenciaRelacion { get; private set; }

    /// <summary>
    /// Observaciones generales del estudiante
    /// </summary>
    public string? Observaciones { get; private set; }

    /// <summary>
    /// Indica si el registro del estudiante está activo
    /// </summary>
    public bool Activo { get; private set; } = true;

    // Propiedades de navegación
    /// <summary>
    /// Colegio al que pertenece
    /// </summary>
    public virtual Colegio? Colegio { get; set; }

    /// <summary>
    /// Datos personales del estudiante
    /// </summary>
    public virtual Persona? Persona { get; set; }

    /// <summary>
    /// Relaciones con acudientes (padres/tutores)
    /// </summary>
    public virtual ICollection<EstudianteAcudiente> Acudientes { get; set; } = new List<EstudianteAcudiente>();

    /// <summary>
    /// Constructor privado para EF Core
    /// </summary>
    private Estudiante() { }

    /// <summary>
    /// Constructor para crear nuevo estudiante
    /// </summary>
    public Estudiante(
        Guid colegioId,
        Guid personaId,
        string codigoEstudiante,
        DateTime fechaIngreso,
        int anoIngreso,
        string? numeroMatricula = null)
    {
        ColegioId = colegioId;
        PersonaId = personaId;
        CodigoEstudiante = codigoEstudiante;
        Estado = EstadoEstudiante.Activo;
        FechaIngreso = fechaIngreso;
        AnoIngreso = anoIngreso;
        NumeroMatricula = numeroMatricula;

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
        return ColegioId == colegioId || ColegioId == null; // null = entidad global
    }

    /// <summary>
    /// Actualiza el número de matrícula
    /// </summary>
    public void ActualizarMatricula(string numeroMatricula, Guid? updatedBy = null)
    {
        if (string.IsNullOrWhiteSpace(numeroMatricula))
            throw new ArgumentException("El número de matrícula no puede estar vacío", nameof(numeroMatricula));

        NumeroMatricula = numeroMatricula;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Registra el egreso del estudiante
    /// </summary>
    public void RegistrarEgreso(DateTime fechaEgreso, string motivoEgreso, Guid? updatedBy = null)
    {
        if (fechaEgreso < FechaIngreso)
            throw new ArgumentException("La fecha de egreso no puede ser anterior a la fecha de ingreso");

        if (string.IsNullOrWhiteSpace(motivoEgreso))
            throw new ArgumentException("Debe especificar el motivo del egreso", nameof(motivoEgreso));

        Estado = EstadoEstudiante.Retirado;
        FechaEgreso = fechaEgreso;
        MotivoEgreso = motivoEgreso;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Reactiva un estudiante retirado
    /// </summary>
    public void Reactivar(string? motivoReactivacion = null, Guid? updatedBy = null)
    {
        if (Estado != EstadoEstudiante.Retirado)
            throw new InvalidOperationException("Solo se pueden reactivar estudiantes retirados");

        Estado = EstadoEstudiante.Activo;
        FechaEgreso = null;
        MotivoEgreso = null;

        if (!string.IsNullOrWhiteSpace(motivoReactivacion))
        {
            Observaciones = string.IsNullOrWhiteSpace(Observaciones)
                ? $"Reactivado: {motivoReactivacion}"
                : $"{Observaciones}\nReactivado: {motivoReactivacion}";
        }

        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Suspende temporalmente al estudiante
    /// </summary>
    public void Suspender(string motivo, Guid? updatedBy = null)
    {
        if (string.IsNullOrWhiteSpace(motivo))
            throw new ArgumentException("Debe especificar el motivo de suspensión", nameof(motivo));

        Estado = EstadoEstudiante.Suspendido;
        Observaciones = string.IsNullOrWhiteSpace(Observaciones)
            ? $"Suspendido: {motivo}"
            : $"{Observaciones}\nSuspendido: {motivo}";

        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Gradúa al estudiante
    /// </summary>
    public void Graduar(DateTime fechaGraduacion, Guid? updatedBy = null)
    {
        if (fechaGraduacion < FechaIngreso)
            throw new ArgumentException("La fecha de graduación no puede ser anterior a la fecha de ingreso");

        Estado = EstadoEstudiante.Graduado;
        FechaEgreso = fechaGraduacion;
        MotivoEgreso = "Graduación";
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Activa el estudiante
    /// </summary>
    public void Activar()
    {
        Activo = true;
    }

    /// <summary>
    /// Desactiva el estudiante
    /// </summary>
    public void Desactivar()
    {
        Activo = false;
    }

    /// <summary>
    /// Actualiza información médica
    /// </summary>
    public void ActualizarInformacionMedica(string? informacionMedica, Guid? updatedBy = null)
    {
        InformacionMedica = informacionMedica;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Actualiza contacto de emergencia
    /// </summary>
    public void ActualizarContactoEmergencia(
        string? nombre,
        string? telefono,
        string? relacion,
        Guid? updatedBy = null)
    {
        ContactoEmergenciaNombre = nombre;
        ContactoEmergenciaTelefono = telefono;
        ContactoEmergenciaRelacion = relacion;
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
    /// Verifica si el estudiante está activo
    /// </summary>
    public bool EstaActivo() => Estado == EstadoEstudiante.Activo;

    /// <summary>
    /// Verifica si el estudiante puede ser matriculado
    /// </summary>
    public bool PuedeSerMatriculado() => Estado is EstadoEstudiante.Activo or EstadoEstudiante.PreMatricula;

    /// <summary>
    /// Obtiene la edad del estudiante basada en la fecha de nacimiento de la persona
    /// Nota: Requiere que la propiedad Persona esté cargada
    /// </summary>
    public int? ObtenerEdad()
    {
        if (Persona?.FechaNacimiento == null) return null;

        var hoy = DateTime.Today;
        var edad = hoy.Year - Persona.FechaNacimiento.Value.Year;

        // Ajustar si aún no ha cumplido años este año
        if (Persona.FechaNacimiento.Value.Date > hoy.AddYears(-edad))
            edad--;

        return edad;
    }

    /// <summary>
    /// Valida los datos del estudiante
    /// </summary>
    private void ValidarDatos()
    {
        if (ColegioId == null || ColegioId == Guid.Empty)
            throw new ArgumentException("El ID del colegio es requerido", nameof(ColegioId));

        if (PersonaId == Guid.Empty)
            throw new ArgumentException("El ID de la persona es requerido", nameof(PersonaId));

        if (string.IsNullOrWhiteSpace(CodigoEstudiante))
            throw new ArgumentException("El código del estudiante es requerido", nameof(CodigoEstudiante));

        if (FechaIngreso > DateTime.UtcNow)
            throw new ArgumentException("La fecha de ingreso no puede ser futura", nameof(FechaIngreso));

        if (AnoIngreso < 1900 || AnoIngreso > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("El año de ingreso no es válido", nameof(AnoIngreso));
    }

    /// <summary>
    /// Override ToString para debugging
    /// </summary>
    public override string ToString()
    {
        return $"Estudiante: {CodigoEstudiante} - Estado: {Estado}";
    }
}