using ColegiosBackend.Core.Entities.Base;
using ColegiosBackend.Core.Enums;
using System.Text.Json;

namespace ColegiosBackend.Core.Entities;

/// <summary>
/// Entidad que representa la programación horaria de clases para un grupo específico
/// </summary>
public class Horario : BaseEntity, ITenantEntity
{
    #region Propiedades Principales
    /// <summary>
    /// ID del grupo al que pertenece el horario
    /// </summary>
    public Guid GrupoId { get; private set; }

    /// <summary>
    /// ID del año académico
    /// </summary>
    public Guid AnoAcademicoId { get; private set; }

    /// <summary>
    /// Nombre descriptivo del horario
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Descripción adicional del horario
    /// </summary>
    public string? Descripcion { get; private set; }

    /// <summary>
    /// Día de la semana (1=Lunes, 2=Martes, etc.)
    /// </summary>
    public DiaSemana DiaSemana { get; private set; }

    /// <summary>
    /// Hora de inicio de la clase
    /// </summary>
    public TimeOnly HoraInicio { get; private set; }

    /// <summary>
    /// Hora de fin de la clase
    /// </summary>
    public TimeOnly HoraFin { get; private set; }

    /// <summary>
    /// ID de la materia que se imparte
    /// </summary>
    public Guid MateriaId { get; private set; }

    /// <summary>
    /// ID del profesor asignado a esta hora
    /// </summary>
    public Guid ProfesorId { get; private set; }

    /// <summary>
    /// Aula o salón donde se imparte la clase
    /// </summary>
    public string? Aula { get; private set; }

    /// <summary>
    /// Tipo de clase (Regular, Laboratorio, Educación Física, etc.)
    /// </summary>
    public TipoClase TipoClase { get; private set; }

    /// <summary>
    /// Estado del horario
    /// </summary>
    public EstadoHorario Estado { get; private set; }

    /// <summary>
    /// Fecha de inicio de vigencia del horario
    /// </summary>
    public DateOnly FechaInicioVigencia { get; private set; }

    /// <summary>
    /// Fecha de fin de vigencia del horario
    /// </summary>
    public DateOnly? FechaFinVigencia { get; private set; }

    /// <summary>
    /// Configuración adicional en formato JSON (recursos, notas especiales, etc.)
    /// </summary>
    public string? ConfiguracionJson { get; private set; }

    /// <summary>
    /// Prioridad del horario (para resolver conflictos)
    /// </summary>
    public int Prioridad { get; private set; } = 1;

    /// <summary>
    /// Registro activo o inactivo
    /// </summary>
    public bool Activo { get; private set; } = true;

    #endregion

    #region Propiedades de Navegación

    /// <summary>
    /// Grupo al que pertenece el horario
    /// </summary>
    public virtual Grupo Grupo { get; private set; } = null!;

    /// <summary>
    /// Año académico
    /// </summary>
    public virtual AnoAcademico AnoAcademico { get; private set; } = null!;

    /// <summary>
    /// Materia que se imparte
    /// </summary>
    public virtual Materia Materia { get; private set; } = null!;

    /// <summary>
    /// Profesor asignado
    /// </summary>
    public virtual Profesor Profesor { get; private set; } = null!;

    #endregion

    #region ITenantEntity Implementation

    /// <summary>
    /// ID del colegio (heredado del grupo)
    /// </summary>
    public Guid? ColegioId { get; private set; }

    /// <summary>
    /// Establece el colegio para el horario
    /// </summary>
    /// <param name="colegioId">ID del colegio</param>
    public void SetColegio(Guid colegioId)
    {
        ColegioId = colegioId;
    }

    /// <summary>
    /// Verifica si el horario pertenece al colegio especificado
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
    private Horario() : base() { }

    /// <summary>
    /// Constructor para crear nuevo horario
    /// </summary>
    public Horario(
        Guid grupoId,
        Guid anoAcademicoId,
        string nombre,
        DiaSemana diaSemana,
        TimeOnly horaInicio,
        TimeOnly horaFin,
        Guid materiaId,
        Guid profesorId,
        Guid colegioId,
        TipoClase tipoClase = TipoClase.Regular,
        DateOnly? fechaInicioVigencia = null,
        string? aula = null,
        string? descripcion = null,
        int prioridad = 1) : base()
    {
        Id = Guid.NewGuid();
        GrupoId = grupoId;
        AnoAcademicoId = anoAcademicoId;
        Nombre = nombre?.Trim() ?? string.Empty;
        Descripcion = descripcion?.Trim();
        DiaSemana = diaSemana;
        HoraInicio = horaInicio;
        HoraFin = horaFin;
        MateriaId = materiaId;
        ProfesorId = profesorId;
        Aula = aula?.Trim();
        TipoClase = tipoClase;
        Estado = EstadoHorario.Activo;
        FechaInicioVigencia = fechaInicioVigencia ?? DateOnly.FromDateTime(DateTime.Today);
        ColegioId = colegioId;
        Prioridad = prioridad;
        Activo = true;

        ValidarDatos();
    }

    #endregion

    #region Métodos de Negocio

    /// <summary>
    /// Actualiza la información básica del horario
    /// </summary>
    public void ActualizarInformacion(
        string nombre,
        DiaSemana diaSemana,
        TimeOnly horaInicio,
        TimeOnly horaFin,
        string? aula = null,
        string? descripcion = null)
    {
        if (!Activo)
            throw new InvalidOperationException("No se puede actualizar un horario inactivo");

        if (Estado == EstadoHorario.Suspendido)
            throw new InvalidOperationException("No se puede actualizar un horario suspendido");

        Nombre = nombre?.Trim() ?? string.Empty;
        DiaSemana = diaSemana;
        HoraInicio = horaInicio;
        HoraFin = horaFin;
        Aula = aula?.Trim();
        Descripcion = descripcion?.Trim();

        ValidarHorarios();
    }

    /// <summary>
    /// Cambia el profesor asignado al horario
    /// </summary>
    public void CambiarProfesor(Guid nuevoProfesorId)
    {
        if (!Activo)
            throw new InvalidOperationException("No se puede cambiar profesor en un horario inactivo");

        if (nuevoProfesorId == Guid.Empty)
            throw new ArgumentException("El ID del profesor es requerido");

        ProfesorId = nuevoProfesorId;
    }

    /// <summary>
    /// Cambia la materia del horario
    /// </summary>
    public void CambiarMateria(Guid nuevaMateriaId)
    {
        if (!Activo)
            throw new InvalidOperationException("No se puede cambiar materia en un horario inactivo");

        if (nuevaMateriaId == Guid.Empty)
            throw new ArgumentException("El ID de la materia es requerido");

        MateriaId = nuevaMateriaId;
    }

    /// <summary>
    /// Suspende temporalmente el horario
    /// </summary>
    public void Suspender(DateOnly? fechaFinSuspension = null)
    {
        if (!Activo)
            throw new InvalidOperationException("No se puede suspender un horario inactivo");

        Estado = EstadoHorario.Suspendido;

        if (fechaFinSuspension.HasValue)
        {
            FechaFinVigencia = fechaFinSuspension.Value;
        }
    }

    /// <summary>
    /// Reactiva un horario suspendido
    /// </summary>
    public void Reactivar()
    {
        if (!Activo)
            throw new InvalidOperationException("No se puede reactivar un horario inactivo");

        Estado = EstadoHorario.Activo;
        FechaFinVigencia = null;
    }

    /// <summary>
    /// Establece la vigencia del horario
    /// </summary>
    public void EstablecerVigencia(DateOnly fechaInicio, DateOnly? fechaFin = null)
    {
        if (fechaFin.HasValue && fechaFin.Value <= fechaInicio)
            throw new ArgumentException("La fecha de fin debe ser posterior a la fecha de inicio");

        FechaInicioVigencia = fechaInicio;
        FechaFinVigencia = fechaFin;
    }

    /// <summary>
    /// Establece configuración adicional en JSON
    /// </summary>
    public void EstablecerConfiguracion<T>(T configuracion) where T : class
    {
        if (configuracion == null)
        {
            ConfiguracionJson = null;
            return;
        }

        try
        {
            ConfiguracionJson = JsonSerializer.Serialize(configuracion, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Error al serializar la configuración: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtiene la configuración deserializada
    /// </summary>
    public T? GetConfiguracion<T>() where T : class
    {
        if (string.IsNullOrWhiteSpace(ConfiguracionJson))
            return null;

        try
        {
            return JsonSerializer.Deserialize<T>(ConfiguracionJson, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Verifica si el horario está vigente en una fecha específica
    /// </summary>
    public bool EstaVigenteEn(DateOnly fecha)
    {
        if (!Activo || Estado != EstadoHorario.Activo)
            return false;

        if (fecha < FechaInicioVigencia)
            return false;

        if (FechaFinVigencia.HasValue && fecha > FechaFinVigencia.Value)
            return false;

        return true;
    }

    /// <summary>
    /// Verifica si hay conflicto de horario con otro período
    /// </summary>
    public bool TieneConflictoCon(DiaSemana dia, TimeOnly horaInicio, TimeOnly horaFin)
    {
        if (DiaSemana != dia)
            return false;

        // Verificar solapamiento de horarios
        return HoraInicio < horaFin && horaInicio < HoraFin;
    }

    /// <summary>
    /// Calcula la duración de la clase en minutos
    /// </summary>
    public int GetDuracionEnMinutos()
    {
        return (int)(HoraFin - HoraInicio).TotalMinutes;
    }

    /// <summary>
    /// Obtiene una descripción completa del horario
    /// </summary>
    public string GetDescripcionCompleta()
    {
        var dia = DiaSemana switch
        {
            DiaSemana.Lunes => "Lunes",
            DiaSemana.Martes => "Martes",
            DiaSemana.Miercoles => "Miércoles",
            DiaSemana.Jueves => "Jueves",
            DiaSemana.Viernes => "Viernes",
            DiaSemana.Sabado => "Sábado",
            DiaSemana.Domingo => "Domingo",
            _ => "Día Desconocido"
        };

        var descripcion = $"{dia} {HoraInicio:HH:mm} - {HoraFin:HH:mm}";

        if (!string.IsNullOrWhiteSpace(Aula))
            descripcion += $" (Aula: {Aula})";

        return descripcion;
    }

    /// <summary>
    /// Desactiva el horario
    /// </summary>
    public void Desactivar()
    {
        Activo = false;
        Estado = EstadoHorario.Inactivo;
    }

    #endregion

    #region Validaciones

    /// <summary>
    /// Valida los datos de la entidad
    /// </summary>
    private void ValidarDatos()
    {
        if (GrupoId == Guid.Empty)
            throw new ArgumentException("El ID del grupo es requerido");

        if (AnoAcademicoId == Guid.Empty)
            throw new ArgumentException("El ID del año académico es requerido");

        if (MateriaId == Guid.Empty)
            throw new ArgumentException("El ID de la materia es requerido");

        if (ProfesorId == Guid.Empty)
            throw new ArgumentException("El ID del profesor es requerido");

        if (ColegioId == null || ColegioId == Guid.Empty)
            throw new ArgumentException("El ID del colegio es requerido");

        if (string.IsNullOrWhiteSpace(Nombre))
            throw new ArgumentException("El nombre del horario es requerido");

        ValidarHorarios();
    }

    /// <summary>
    /// Valida las horas del horario
    /// </summary>
    private void ValidarHorarios()
    {
        if (HoraFin <= HoraInicio)
            throw new ArgumentException("La hora de fin debe ser posterior a la hora de inicio");

        var duracionMinutos = GetDuracionEnMinutos();
        if (duracionMinutos < 15)
            throw new ArgumentException("La duración mínima de una clase es 15 minutos");

        if (duracionMinutos > 480) // 8 horas
            throw new ArgumentException("La duración máxima de una clase es 8 horas");
    }

    /// <summary>
    /// Verifica si se puede eliminar el horario
    /// </summary>
    public override bool CanBeDeleted()
    {
        // Solo se puede eliminar si no ha iniciado su vigencia o si es futuro
        return FechaInicioVigencia > DateOnly.FromDateTime(DateTime.Today);
    }

    #endregion
}