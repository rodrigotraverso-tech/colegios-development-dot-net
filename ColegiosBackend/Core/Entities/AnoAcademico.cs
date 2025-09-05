using ColegiosBackend.Core.Entities.Base;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Entities;

/// <summary>
/// Entidad AnoAcademico - Representa un período académico en el colegio
/// Ejemplos: "2024", "2024-2025", "2024-I", "2024-II"
/// </summary>
public class AnoAcademico : BaseEntity, ITenantEntity
{
    /// <summary>
    /// ID del colegio al que pertenece este año académico
    /// </summary>
    public Guid? ColegioId { get; private set; }

    /// <summary>
    /// Código único del año académico dentro del colegio
    /// Ejemplos: "2024", "2024-I", "2024-2025"
    /// </summary>
    public string Codigo { get; private set; } = string.Empty;

    /// <summary>
    /// Nombre descriptivo del año académico
    /// Ejemplos: "Año Académico 2024", "Primer Semestre 2024"
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Fecha de inicio del año académico
    /// </summary>
    public DateTime FechaInicio { get; private set; }

    /// <summary>
    /// Fecha de finalización del año académico
    /// </summary>
    public DateTime FechaFin { get; private set; }

    /// <summary>
    /// Indica si este año académico está activo
    /// Solo debe haber uno activo por colegio
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Configuración específica del año académico (JSON)
    /// Puede incluir: períodos de evaluación, vacaciones, eventos importantes
    /// </summary>
    public string? Configuracion { get; private set; }

    // Propiedades de navegación
    /// <summary>
    /// Colegio al que pertenece
    /// </summary>
    public virtual Colegio? Colegio { get; set; }

    /// <summary>
    /// Grupos académicos del año
    /// </summary>
    public virtual ICollection<Grupo> Grupos { get; set; } = new List<Grupo>();

    /// <summary>
    /// Constructor privado para EF Core
    /// </summary>
    private AnoAcademico() { }

    /// <summary>
    /// Constructor para crear nuevo año académico
    /// </summary>
    public AnoAcademico(
        Guid colegioId,
        string codigo,
        string nombre,
        DateTime fechaInicio,
        DateTime fechaFin,
        bool activo = false)
    {
        ColegioId = colegioId;
        Codigo = codigo;
        Nombre = nombre;
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
        Activo = activo;

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
    /// Actualiza información básica del año académico
    /// </summary>
    public void ActualizarInformacionBasica(
        string codigo,
        string nombre,
        DateTime fechaInicio,
        DateTime fechaFin,
        Guid? updatedBy = null)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("El código es requerido", nameof(codigo));

        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre es requerido", nameof(nombre));

        if (fechaFin <= fechaInicio)
            throw new ArgumentException("La fecha de fin debe ser posterior a la fecha de inicio");

        Codigo = codigo;
        Nombre = nombre;
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Activa este año académico
    /// NOTA: La lógica de negocio debe asegurar que solo haya uno activo por colegio
    /// </summary>
    public void Activar(Guid? updatedBy = null)
    {
        Activo = true;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Desactiva este año académico
    /// </summary>
    public void Desactivar(Guid? updatedBy = null)
    {
        Activo = false;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Actualiza la configuración del año académico
    /// </summary>
    public void ActualizarConfiguracion(string? configuracion, Guid? updatedBy = null)
    {
        Configuracion = configuracion;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Extiende la duración del año académico
    /// </summary>
    public void ExtenderDuracion(DateTime nuevaFechaFin, Guid? updatedBy = null)
    {
        if (nuevaFechaFin <= FechaFin)
            throw new ArgumentException("La nueva fecha debe ser posterior a la fecha actual de fin");

        if (nuevaFechaFin <= DateTime.UtcNow.Date)
            throw new ArgumentException("La nueva fecha no puede ser en el pasado");

        FechaFin = nuevaFechaFin;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Adelanta la fecha de inicio (solo si aún no ha comenzado)
    /// </summary>
    public void AdelantarInicio(DateTime nuevaFechaInicio, Guid? updatedBy = null)
    {
        if (DateTime.UtcNow.Date >= FechaInicio)
            throw new InvalidOperationException("No se puede cambiar la fecha de inicio de un año que ya comenzó");

        if (nuevaFechaInicio >= FechaFin)
            throw new ArgumentException("La fecha de inicio no puede ser posterior o igual a la fecha de fin");

        FechaInicio = nuevaFechaInicio;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Verifica si el año académico está activo
    /// </summary>
    public bool EstaActivo() => Activo;

    /// <summary>
    /// Verifica si el año académico está en curso (basado en fechas)
    /// </summary>
    public bool EstaEnCurso()
    {
        var hoy = DateTime.UtcNow.Date;
        return hoy >= FechaInicio && hoy <= FechaFin;
    }

    /// <summary>
    /// Verifica si el año académico ha finalizado
    /// </summary>
    public bool HaFinalizado()
    {
        return DateTime.UtcNow.Date > FechaFin;
    }

    /// <summary>
    /// Verifica si el año académico aún no ha comenzado
    /// </summary>
    public bool NoHaComenzado()
    {
        return DateTime.UtcNow.Date < FechaInicio;
    }

    /// <summary>
    /// Calcula la duración del año académico en días
    /// </summary>
    public int DuracionEnDias()
    {
        return (FechaFin - FechaInicio).Days + 1;
    }

    /// <summary>
    /// Calcula cuántos días han pasado desde el inicio
    /// </summary>
    public int DiasTranscurridos()
    {
        if (NoHaComenzado()) return 0;

        var hoy = DateTime.UtcNow.Date;
        var diasPasados = (hoy - FechaInicio).Days;

        return Math.Max(0, Math.Min(diasPasados, DuracionEnDias()));
    }

    /// <summary>
    /// Calcula cuántos días faltan para finalizar
    /// </summary>
    public int DiasRestantes()
    {
        if (HaFinalizado()) return 0;

        var hoy = DateTime.UtcNow.Date;
        return Math.Max(0, (FechaFin - hoy).Days);
    }

    /// <summary>
    /// Calcula el porcentaje de avance del año académico
    /// </summary>
    public double PorcentajeAvance()
    {
        if (NoHaComenzado()) return 0.0;
        if (HaFinalizado()) return 100.0;

        var diasTranscurridos = DiasTranscurridos();
        var duracionTotal = DuracionEnDias();

        return duracionTotal > 0 ? (double)diasTranscurridos / duracionTotal * 100 : 0.0;
    }

    /// <summary>
    /// Obtiene el estado actual del año académico
    /// </summary>
    public string ObtenerEstadoActual()
    {
        if (NoHaComenzado()) return "Por Iniciar";
        if (EstaEnCurso()) return Activo ? "En Curso" : "En Curso (Inactivo)";
        if (HaFinalizado()) return "Finalizado";

        return "Indefinido";
    }

    /// <summary>
    /// Verifica si una fecha está dentro del período académico
    /// </summary>
    public bool FechaEnPeriodo(DateTime fecha)
    {
        var fechaSoloFecha = fecha.Date;
        return fechaSoloFecha >= FechaInicio && fechaSoloFecha <= FechaFin;
    }

    /// <summary>
    /// Genera un código automático basado en el año
    /// </summary>
    public static string GenerarCodigoAutomatico(int ano, int? semestre = null)
    {
        return semestre.HasValue ? $"{ano}-{semestre}" : ano.ToString();
    }

    /// <summary>
    /// Genera un nombre automático basado en el código
    /// </summary>
    public static string GenerarNombreAutomatico(string codigo)
    {
        if (codigo.Contains('-') && int.TryParse(codigo.Split('-')[1], out int semestre))
        {
            var numeroSemestre = semestre == 1 ? "Primer" : "Segundo";
            return $"{numeroSemestre} Semestre {codigo.Split('-')[0]}";
        }

        return $"Año Académico {codigo}";
    }

    /// <summary>
    /// Valida los datos del año académico
    /// </summary>
    private void ValidarDatos()
    {
        if (ColegioId == null || ColegioId == Guid.Empty)
            throw new ArgumentException("El ID del colegio es requerido", nameof(ColegioId));

        if (string.IsNullOrWhiteSpace(Codigo))
            throw new ArgumentException("El código es requerido", nameof(Codigo));

        if (string.IsNullOrWhiteSpace(Nombre))
            throw new ArgumentException("El nombre es requerido", nameof(Nombre));

        if (FechaFin <= FechaInicio)
            throw new ArgumentException("La fecha de fin debe ser posterior a la fecha de inicio");

        // Validar que el año académico no sea muy largo (más de 2 años)
        var duracionMaxima = TimeSpan.FromDays(730); // ~2 años
        if (FechaFin - FechaInicio > duracionMaxima)
            throw new ArgumentException("La duración del año académico no puede ser mayor a 2 años");

        // Validar que no sea muy corto (menos de 30 días)
        var duracionMinima = TimeSpan.FromDays(30);
        if (FechaFin - FechaInicio < duracionMinima)
            throw new ArgumentException("La duración del año académico debe ser de al menos 30 días");
    }

    /// <summary>
    /// Override ToString para debugging
    /// </summary>
    public override string ToString()
    {
        return $"AnoAcademico: {Codigo} - {Nombre} ({FechaInicio:dd/MM/yyyy} - {FechaFin:dd/MM/yyyy}) - Activo: {Activo}";
    }
}