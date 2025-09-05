using ColegiosBackend.Core.Entities.Base;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Entities;

/// <summary>
/// Representa un período evaluativo dentro de un año académico (bimestre, trimestre, etc.)
/// Los períodos evaluativos definen cuándo se realizan las evaluaciones y calificaciones
/// </summary>
public class PeriodoEvaluativo : BaseEntity, ITenantEntity, IAuditableEntity
{
    /// <summary>
    /// Constructor privado para Entity Framework
    /// </summary>
    private PeriodoEvaluativo() { }

    /// <summary>
    /// Constructor principal del período evaluativo
    /// </summary>
    public PeriodoEvaluativo(
        Guid anoAcademicoId,
        Guid? colegioId,
        int numero,
        string nombre,
        DateTime fechaInicio,
        DateTime fechaFin,
        TipoPeriodoEvaluativo tipo,
        decimal pesoCalificacion = 100m)
    {
        AnoAcademicoId = anoAcademicoId;
        ColegioId = colegioId;
        Numero = numero;
        Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        FechaInicio = fechaInicio.Date;
        FechaFin = fechaFin.Date;
        Tipo = tipo;
        PesoCalificacion = pesoCalificacion;
        Estado = EstadoPeriodoEvaluativo.Planificado;
        PermiteCalificaciones = true;
        Activo = true;

        ValidarFechas();
        ValidarPeso();
    }

    // Propiedades principales
    /// <summary>
    /// Referencia al año académico al que pertenece este período
    /// </summary>
    public Guid AnoAcademicoId { get; private set; }

    /// <summary>
    /// Navegación al año académico
    /// </summary>
    public virtual AnoAcademico? AnoAcademico { get; private set; }

    /// <summary>
    /// ID del colegio (tenant) - nullable para compatibilidad
    /// </summary>
    public Guid? ColegioId { get; private set; }

    /// <summary>
    /// Número del período dentro del año académico (1, 2, 3, 4)
    /// </summary>
    public int Numero { get; private set; }

    /// <summary>
    /// Nombre descriptivo del período
    /// Ejemplo: "Primer Bimestre", "Segundo Trimestre"
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Descripción opcional del período evaluativo
    /// </summary>
    public string? Descripcion { get; private set; }

    /// <summary>
    /// Fecha de inicio del período evaluativo
    /// </summary>
    public DateTime FechaInicio { get; private set; }

    /// <summary>
    /// Fecha de fin del período evaluativo
    /// </summary>
    public DateTime FechaFin { get; private set; }

    /// <summary>
    /// Fecha límite para ingreso de calificaciones
    /// </summary>
    public DateTime? FechaLimiteCalificaciones { get; private set; }

    /// <summary>
    /// Tipo de período evaluativo
    /// </summary>
    public TipoPeriodoEvaluativo Tipo { get; private set; }

    /// <summary>
    /// Estado actual del período evaluativo
    /// </summary>
    public EstadoPeriodoEvaluativo Estado { get; private set; }

    /// <summary>
    /// Peso de este período en la calificación final (porcentaje)
    /// Por defecto 100% si es el único período, o distribuido equitativamente
    /// </summary>
    public decimal PesoCalificacion { get; private set; }

    /// <summary>
    /// Indica si actualmente se pueden ingresar/modificar calificaciones
    /// </summary>
    public bool PermiteCalificaciones { get; private set; }

    /// <summary>
    /// Configuración adicional del período en formato JSON
    /// Puede incluir: criterios de evaluación, escalas de calificación, etc.
    /// </summary>
    public string? ConfiguracionEvaluacion { get; private set; }

    /// <summary>
    /// Observaciones o notas adicionales sobre el período
    /// </summary>
    public string? Observaciones { get; private set; }

    /// <summary>
    /// Indica si el período está activo
    /// </summary>
    public bool Activo { get; private set; }
    
    // Métodos de negocio

    /// <summary>
    /// Actualiza la información básica del período evaluativo
    /// </summary>
    public void ActualizarInformacion(string nombre, string? descripcion, DateTime fechaInicio, DateTime fechaFin)
    {
        if (Estado == EstadoPeriodoEvaluativo.Cerrado)
            throw new InvalidOperationException("No se puede modificar un período evaluativo cerrado");

        Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        Descripcion = descripcion;
        FechaInicio = fechaInicio.Date;
        FechaFin = fechaFin.Date;

        ValidarFechas();
    }

    /// <summary>
    /// Establece la fecha límite para ingreso de calificaciones
    /// </summary>
    public void EstablecerFechaLimiteCalificaciones(DateTime fechaLimite)
    {
        if (fechaLimite.Date < FechaFin.Date)
            throw new ArgumentException("La fecha límite no puede ser anterior a la fecha de fin del período");

        FechaLimiteCalificaciones = fechaLimite.Date;
    }

    /// <summary>
    /// Actualiza el peso de calificación del período
    /// </summary>
    public void ActualizarPesoCalificacion(decimal peso)
    {
        if (Estado == EstadoPeriodoEvaluativo.Cerrado)
            throw new InvalidOperationException("No se puede modificar el peso de un período cerrado");

        PesoCalificacion = peso;
        ValidarPeso();
    }

    /// <summary>
    /// Abre el período para permitir calificaciones
    /// </summary>
    public void AbrirPeriodo()
    {
        if (Estado == EstadoPeriodoEvaluativo.Cerrado)
            throw new InvalidOperationException("No se puede reabrir un período cerrado");

        Estado = EstadoPeriodoEvaluativo.Activo;
        PermiteCalificaciones = true;
    }

    /// <summary>
    /// Cierra el período y bloquea modificaciones de calificaciones
    /// </summary>
    public void CerrarPeriodo()
    {
        Estado = EstadoPeriodoEvaluativo.Cerrado;
        PermiteCalificaciones = false;
    }

    /// <summary>
    /// Suspende temporalmente las calificaciones
    /// </summary>
    public void SuspenderCalificaciones()
    {
        if (Estado != EstadoPeriodoEvaluativo.Cerrado)
        {
            PermiteCalificaciones = false;
        }
    }

    /// <summary>
    /// Reactiva las calificaciones (si el período no está cerrado)
    /// </summary>
    public void ReactivarCalificaciones()
    {
        if (Estado != EstadoPeriodoEvaluativo.Cerrado)
        {
            PermiteCalificaciones = true;
        }
    }

    /// <summary>
    /// Establece configuración de evaluación en formato JSON
    /// </summary>
    public void EstablecerConfiguracionEvaluacion(string? configuracion)
    {
        ConfiguracionEvaluacion = configuracion;
    }

    /// <summary>
    /// Actualiza observaciones del período
    /// </summary>
    public void ActualizarObservaciones(string? observaciones)
    {
        Observaciones = observaciones;
    }

    /// <summary>
    /// Activa el período evaluativo
    /// </summary>
    public void Activar()
    {
        Activo = true;
    }

    /// <summary>
    /// Desactiva el período evaluativo
    /// </summary>
    public void Desactivar()
    {
        if (Estado == EstadoPeriodoEvaluativo.Activo)
            throw new InvalidOperationException("No se puede desactivar un período activo. Ciérrelo primero.");

        Activo = false;
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

    /// <summary>
    /// Verifica si el período está vigente en la fecha actual
    /// </summary>
    public bool EstaVigente()
    {
        var hoy = DateTime.Today;
        return hoy >= FechaInicio && hoy <= FechaFin && Activo;
    }

    /// <summary>
    /// Verifica si el período contiene la fecha especificada
    /// </summary>
    public bool Contienefecha(DateTime fecha)
    {
        var fechaSolo = fecha.Date;
        return fechaSolo >= FechaInicio && fechaSolo <= FechaFin;
    }

    /// <summary>
    /// Calcula la duración del período en días
    /// </summary>
    public int DuracionEnDias()
    {
        return (FechaFin - FechaInicio).Days + 1;
    }

    // Métodos de validación privados

    private void ValidarFechas()
    {
        if (FechaInicio >= FechaFin)
            throw new ArgumentException("La fecha de inicio debe ser anterior a la fecha de fin");
    }

    private void ValidarPeso()
    {
        if (PesoCalificacion < 0 || PesoCalificacion > 100)
            throw new ArgumentException("El peso de calificación debe estar entre 0 y 100");
    }
}