using ColegiosBackend.Core.Entities.Base;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Entities;

/// <summary>
/// Entidad Profesor - Representa un docente vinculado a un colegio específico
/// Un profesor está vinculado a una Persona y puede trabajar en múltiples colegios
/// </summary>
public class Profesor : BaseEntity, ITenantEntity
{
    /// <summary>
    /// ID del colegio al que pertenece este registro de profesor
    /// </summary>
    public Guid? ColegioId { get; private set; }

    /// <summary>
    /// ID de la persona asociada (tabla personas)
    /// </summary>
    public Guid PersonaId { get; private set; }

    /// <summary>
    /// Código único del profesor dentro del colegio
    /// Formato sugerido: PROF-YYYY-NNNN (ej: PROF-2024-0001)
    /// </summary>
    public string CodigoProfesor { get; private set; } = string.Empty;

    /// <summary>
    /// Estado actual del profesor en el colegio
    /// </summary>
    public EstadoProfesor Estado { get; private set; }

    /// <summary>
    /// Fecha de ingreso como profesor al colegio
    /// </summary>
    public DateTime FechaIngreso { get; private set; }

    /// <summary>
    /// Fecha de retiro/terminación (null si está activo)
    /// </summary>
    public DateTime? FechaRetiro { get; private set; }

    /// <summary>
    /// Motivo del retiro/terminación
    /// </summary>
    public string? MotivoRetiro { get; private set; }

    /// <summary>
    /// Tipo de contrato del profesor
    /// </summary>
    public TipoContratoProfesor TipoContrato { get; private set; }

    /// <summary>
    /// Cargo principal del profesor
    /// </summary>
    public CargoProfesor Cargo { get; private set; }

    /// <summary>
    /// Especialidades académicas del profesor (JSON array)
    /// Ejemplo: ["Matemáticas", "Física", "Química"]
    /// </summary>
    public string? Especialidades { get; private set; }

    /// <summary>
    /// Títulos académicos del profesor
    /// Ejemplo: "Licenciatura en Matemáticas, Maestría en Educación"
    /// </summary>
    public string? TitulosAcademicos { get; private set; }

    /// <summary>
    /// Años de experiencia docente
    /// </summary>
    public int? AnosExperiencia { get; private set; }

    /// <summary>
    /// Número de registro profesional (si aplica)
    /// </summary>
    public string? RegistroProfesional { get; private set; }

    /// <summary>
    /// Salario base mensual
    /// </summary>
    public decimal? SalarioBase { get; private set; }

    /// <summary>
    /// Jornada laboral (horas por semana)
    /// </summary>
    public int? HorasSemanales { get; private set; }

    /// <summary>
    /// Indica si puede ser coordinador de área
    /// </summary>
    public bool PuedeSerCoordinador { get; private set; }

    /// <summary>
    /// Indica si puede ser director de grupo
    /// </summary>
    public bool PuedeSerDirectorGrupo { get; private set; }

    /// <summary>
    /// Indica si está disponible para reemplazos
    /// </summary>
    public bool DisponibleReemplazos { get; private set; }

    /// <summary>
    /// Observaciones generales del profesor
    /// </summary>
    public string? Observaciones { get; private set; }

    /// <summary>
    /// Información de contacto de emergencia - Nombre
    /// </summary>
    public string? ContactoEmergenciaNombre { get; private set; }

    /// <summary>
    /// Información de contacto de emergencia - Teléfono
    /// </summary>
    public string? ContactoEmergenciaTelefono { get; private set; }

    /// <summary>
    /// Información de contacto de emergencia - Relación
    /// </summary>
    public string? ContactoEmergenciaRelacion { get; private set; }

    // Propiedades de navegación
    /// <summary>
    /// Colegio al que pertenece
    /// </summary>
    public virtual Colegio? Colegio { get; set; }

    /// <summary>
    /// Datos personales del profesor
    /// </summary>
    public virtual Persona? Persona { get; set; }

    /// <summary>
    /// Constructor privado para EF Core
    /// </summary>
    private Profesor() { }

    /// <summary>
    /// Constructor para crear nuevo profesor
    /// </summary>
    public Profesor(
        Guid colegioId,
        Guid personaId,
        string codigoProfesor,
        DateTime fechaIngreso,
        TipoContratoProfesor tipoContrato,
        CargoProfesor cargo)
    {
        ColegioId = colegioId;
        PersonaId = personaId;
        CodigoProfesor = codigoProfesor;
        Estado = EstadoProfesor.Activo;
        FechaIngreso = fechaIngreso;
        TipoContrato = tipoContrato;
        Cargo = cargo;
        PuedeSerCoordinador = false;
        PuedeSerDirectorGrupo = true;
        DisponibleReemplazos = true;

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
    /// Actualiza información académica del profesor
    /// </summary>
    public void ActualizarInformacionAcademica(
        string? especialidades,
        string? titulosAcademicos,
        int? anosExperiencia,
        string? registroProfesional,
        Guid? updatedBy = null)
    {
        Especialidades = especialidades;
        TitulosAcademicos = titulosAcademicos;
        AnosExperiencia = anosExperiencia;
        RegistroProfesional = registroProfesional;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Actualiza información laboral
    /// </summary>
    public void ActualizarInformacionLaboral(
        TipoContratoProfesor tipoContrato,
        CargoProfesor cargo,
        decimal? salarioBase,
        int? horasSemanales,
        Guid? updatedBy = null)
    {
        TipoContrato = tipoContrato;
        Cargo = cargo;
        SalarioBase = salarioBase;
        HorasSemanales = horasSemanales;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Actualiza permisos y capacidades
    /// </summary>
    public void ActualizarPermisos(
        bool puedeSerCoordinador,
        bool puedeSerDirectorGrupo,
        bool disponibleReemplazos,
        Guid? updatedBy = null)
    {
        PuedeSerCoordinador = puedeSerCoordinador;
        PuedeSerDirectorGrupo = puedeSerDirectorGrupo;
        DisponibleReemplazos = disponibleReemplazos;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Suspende temporalmente al profesor
    /// </summary>
    public void Suspender(string motivo, Guid? updatedBy = null)
    {
        if (string.IsNullOrWhiteSpace(motivo))
            throw new ArgumentException("Debe especificar el motivo de suspensión", nameof(motivo));

        Estado = EstadoProfesor.Suspendido;
        Observaciones = string.IsNullOrWhiteSpace(Observaciones)
            ? $"Suspendido: {motivo}"
            : $"{Observaciones}\nSuspendido: {motivo}";

        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Reactiva un profesor suspendido
    /// </summary>
    public void Reactivar(string? motivoReactivacion = null, Guid? updatedBy = null)
    {
        if (Estado != EstadoProfesor.Suspendido)
            throw new InvalidOperationException("Solo se pueden reactivar profesores suspendidos");

        Estado = EstadoProfesor.Activo;

        if (!string.IsNullOrWhiteSpace(motivoReactivacion))
        {
            Observaciones = string.IsNullOrWhiteSpace(Observaciones)
                ? $"Reactivado: {motivoReactivacion}"
                : $"{Observaciones}\nReactivado: {motivoReactivacion}";
        }

        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Registra el retiro del profesor
    /// </summary>
    public void RegistrarRetiro(DateTime fechaRetiro, string motivoRetiro, Guid? updatedBy = null)
    {
        if (fechaRetiro < FechaIngreso)
            throw new ArgumentException("La fecha de retiro no puede ser anterior a la fecha de ingreso");

        if (string.IsNullOrWhiteSpace(motivoRetiro))
            throw new ArgumentException("Debe especificar el motivo del retiro", nameof(motivoRetiro));

        Estado = EstadoProfesor.Retirado;
        FechaRetiro = fechaRetiro;
        MotivoRetiro = motivoRetiro;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Pone al profesor en licencia
    /// </summary>
    public void PonerEnLicencia(string motivo, Guid? updatedBy = null)
    {
        if (string.IsNullOrWhiteSpace(motivo))
            throw new ArgumentException("Debe especificar el motivo de la licencia", nameof(motivo));

        Estado = EstadoProfesor.EnLicencia;
        Observaciones = string.IsNullOrWhiteSpace(Observaciones)
            ? $"En licencia: {motivo}"
            : $"{Observaciones}\nEn licencia: {motivo}";

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
    /// Verifica si el profesor está activo
    /// </summary>
    public bool EstaActivo() => Estado == EstadoProfesor.Activo;

    /// <summary>
    /// Verifica si puede enseñar (activo o en período de prueba)
    /// </summary>
    public bool PuedeEnsenar() => Estado is EstadoProfesor.Activo or EstadoProfesor.PeriodoPrueba;

    /// <summary>
    /// Obtiene la edad del profesor basada en la fecha de nacimiento de la persona
    /// </summary>
    public int? ObtenerEdad()
    {
        if (Persona?.FechaNacimiento == null) return null;

        var hoy = DateTime.Today;
        var edad = hoy.Year - Persona.FechaNacimiento.Value.Year;

        if (Persona.FechaNacimiento.Value.Date > hoy.AddYears(-edad))
            edad--;

        return edad;
    }

    /// <summary>
    /// Calcula años de antigüedad en el colegio
    /// </summary>
    public int AnosAntiguedad()
    {
        var fechaReferencia = FechaRetiro ?? DateTime.Today;
        var anos = fechaReferencia.Year - FechaIngreso.Year;

        if (FechaIngreso.Date > fechaReferencia.AddYears(-anos))
            anos--;

        return Math.Max(0, anos);
    }

    /// <summary>
    /// Verifica si tiene una especialidad específica
    /// </summary>
    public bool TieneEspecialidad(string especialidad)
    {
        if (string.IsNullOrWhiteSpace(Especialidades) || string.IsNullOrWhiteSpace(especialidad))
            return false;

        return Especialidades.Contains(especialidad, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Valida los datos del profesor
    /// </summary>
    private void ValidarDatos()
    {
        if (ColegioId == null || ColegioId == Guid.Empty)
            throw new ArgumentException("El ID del colegio es requerido", nameof(ColegioId));

        if (PersonaId == Guid.Empty)
            throw new ArgumentException("El ID de la persona es requerido", nameof(PersonaId));

        if (string.IsNullOrWhiteSpace(CodigoProfesor))
            throw new ArgumentException("El código del profesor es requerido", nameof(CodigoProfesor));

        if (FechaIngreso > DateTime.UtcNow)
            throw new ArgumentException("La fecha de ingreso no puede ser futura", nameof(FechaIngreso));

        if (SalarioBase.HasValue && SalarioBase < 0)
            throw new ArgumentException("El salario no puede ser negativo", nameof(SalarioBase));

        if (HorasSemanales.HasValue && (HorasSemanales < 1 || HorasSemanales > 60))
            throw new ArgumentException("Las horas semanales deben estar entre 1 y 60", nameof(HorasSemanales));

        if (AnosExperiencia.HasValue && AnosExperiencia < 0)
            throw new ArgumentException("Los años de experiencia no pueden ser negativos", nameof(AnosExperiencia));
    }

    /// <summary>
    /// Override ToString para debugging
    /// </summary>
    public override string ToString()
    {
        return $"Profesor: {CodigoProfesor} - {Cargo} - Estado: {Estado}";
    }
}