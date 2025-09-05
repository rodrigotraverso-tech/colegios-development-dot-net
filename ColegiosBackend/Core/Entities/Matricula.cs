using ColegiosBackend.Core.Entities.Base;
using ColegiosBackend.Core.Enums;
using ColegiosBackend.Core.ValueObjects.Financiero;

namespace ColegiosBackend.Core.Entities;

/// <summary>
/// Entidad Matricula - Representa la inscripción de un estudiante en un grupo específico durante un año académico
/// Esta es la entidad que conecta estudiantes con su ubicación académica específica
/// </summary>
public class Matricula : BaseEntity, ITenantEntity
{
    /// <summary>
    /// ID del colegio (heredado del grupo para consistencia)
    /// </summary>
    public Guid? ColegioId { get; private set; }

    /// <summary>
    /// ID del estudiante matriculado
    /// </summary>
    public Guid EstudianteId { get; private set; }

    /// <summary>
    /// ID del grupo en el que está matriculado
    /// </summary>
    public Guid GrupoId { get; private set; }

    /// <summary>
    /// Número único de matrícula dentro del colegio y año académico
    /// Formato sugerido: 2024-0001, 2024-0002, etc.
    /// </summary>
    public string NumeroMatricula { get; private set; } = string.Empty;

    /// <summary>
    /// Estado actual de la matrícula
    /// </summary>
    public EstadoMatricula Estado { get; private set; }

    /// <summary>
    /// Fecha en que se realizó la matrícula
    /// </summary>
    public DateTime FechaMatricula { get; private set; }

    /// <summary>
    /// Fecha de inicio de clases para esta matrícula
    /// </summary>
    public DateTime? FechaInicioClases { get; private set; }

    /// <summary>
    /// Fecha de finalización de la matrícula (por retiro, graduación, etc.)
    /// </summary>
    public DateTime? FechaFinalizacion { get; private set; }

    /// <summary>
    /// Motivo de finalización de la matrícula
    /// </summary>
    public string? MotivoFinalizacion { get; private set; }

    /// <summary>
    /// Tipo de matrícula
    /// </summary>
    public TipoMatricula TipoMatricula { get; private set; }

    /// <summary>
    /// Indica si el estudiante es nuevo en el colegio
    /// </summary>
    public bool EsEstudianteNuevo { get; private set; }

    /// <summary>
    /// Indica si el estudiante es repitente del grado
    /// </summary>
    public bool EsRepitente { get; private set; }

    /// <summary>
    /// Colegio de procedencia (si es estudiante nuevo)
    /// </summary>
    public string? ColegioProcedencia { get; private set; }

    /// <summary>
    /// Porcentaje de beca otorgada (0-100)
    /// </summary>
    public decimal PorcentajeBeca { get; private set; }

    /// <summary>
    /// Tipo de beca otorgada
    /// </summary>
    public TipoBecaMatricula? TipoBeca { get; private set; }

    /// <summary>
    /// Motivo de la beca
    /// </summary>
    public string? MotivoBeca { get; private set; }

    /// <summary>
    /// Valor de la matrícula antes de descuentos
    /// </summary>
    public decimal? ValorMatricula { get; private set; }

    /// <summary>
    /// Valor de descuentos aplicados
    /// </summary>
    public decimal? ValorDescuentos { get; private set; }

    /// <summary>
    /// Valor final a pagar por la matrícula
    /// </summary>
    public decimal? ValorFinal { get; private set; }

    /// <summary>
    /// Indica si la matrícula ha sido pagada
    /// </summary>
    public bool MatriculaPagada { get; private set; }

    /// <summary>
    /// Fecha de pago de la matrícula
    /// </summary>
    public DateTime? FechaPagoMatricula { get; private set; }

    /// <summary>
    /// Documentos requeridos presentados (JSON array)
    /// Ejemplo: ["Registro Civil", "Carnet Vacunas", "Fotos"]
    /// </summary>
    public string? DocumentosPresentados { get; private set; }

    /// <summary>
    /// Documentos pendientes por entregar (JSON array)
    /// </summary>
    public string? DocumentosPendientes { get; private set; }

    /// <summary>
    /// Observaciones sobre la matrícula
    /// </summary>
    public string? Observaciones { get; private set; }

    /// <summary>
    /// Condiciones especiales de la matrícula
    /// Ejemplo: "Período de prueba", "Compromiso académico"
    /// </summary>
    public string? CondicionesEspeciales { get; private set; }

    /// <summary>
    /// ID del usuario que procesó la matrícula
    /// </summary>
    public Guid? ProcesadoPorId { get; private set; }

    // Propiedades de navegación
    /// <summary>
    /// Estudiante matriculado
    /// </summary>
    public virtual Estudiante? Estudiante { get; set; }

    /// <summary>
    /// Grupo en el que está matriculado
    /// </summary>
    public virtual Grupo? Grupo { get; set; }

    /// <summary>
    /// Colegio donde se realizó la matrícula
    /// </summary>
    public virtual Colegio? Colegio { get; set; }

    /// <summary>
    /// Usuario que procesó la matrícula
    /// </summary>
    public virtual Usuario? ProcesadoPor { get; set; }

    /// <summary>
    /// Código del tipo de beca configurado en el sistema (referencia al ValueObject)
    /// Permite vincular con las becas administrativas definidas en ConfiguracionBecas
    /// </summary>
    public string? CodigoTipoBecaConfigurado { get; private set; }

    /// <summary>
    /// Constructor privado para EF Core
    /// </summary>
    private Matricula() { }

    /// <summary>
    /// Constructor para crear nueva matrícula
    /// </summary>
    public Matricula(
        Guid colegioId,
        Guid estudianteId,
        Guid grupoId,
        string numeroMatricula,
        DateTime fechaMatricula,
        TipoMatricula tipoMatricula,
        bool esEstudianteNuevo = false,
        Guid? procesadoPorId = null)
    {
        ColegioId = colegioId;
        EstudianteId = estudianteId;
        GrupoId = grupoId;
        NumeroMatricula = numeroMatricula;
        Estado = EstadoMatricula.Activa;
        FechaMatricula = fechaMatricula;
        TipoMatricula = tipoMatricula;
        EsEstudianteNuevo = esEstudianteNuevo;
        EsRepitente = false;
        PorcentajeBeca = 0;
        MatriculaPagada = false;
        ProcesadoPorId = procesadoPorId;

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
    /// Actualiza información básica de la matrícula
    /// </summary>
    public void ActualizarInformacionBasica(
        DateTime? fechaInicioClases,
        bool esRepitente,
        string? colegioProcedencia = null,
        Guid? updatedBy = null)
    {
        FechaInicioClases = fechaInicioClases;
        EsRepitente = esRepitente;
        ColegioProcedencia = colegioProcedencia;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Establece información de beca
    /// </summary>
    public void EstablecerBeca(
        decimal porcentajeBeca,
        TipoBecaMatricula? tipoBeca = null,
        string? motivoBeca = null,
        string? codigoTipoBecaConfigurado = null,
        Guid? updatedBy = null)
    {
        if (porcentajeBeca < 0 || porcentajeBeca > 100)
            throw new ArgumentException("El porcentaje de beca debe estar entre 0 y 100", nameof(porcentajeBeca));

        PorcentajeBeca = porcentajeBeca;
        TipoBeca = tipoBeca;
        MotivoBeca = motivoBeca;

        // Recalcular valor final si hay valor de matrícula
        if (ValorMatricula.HasValue)
        {
            CalcularValorFinal();
        }
        CodigoTipoBecaConfigurado = codigoTipoBecaConfigurado;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Establece información financiera de la matrícula
    /// </summary>
    public void EstablecerInformacionFinanciera(
        decimal valorMatricula,
        decimal valorDescuentos = 0,
        Guid? updatedBy = null)
    {
        if (valorMatricula < 0)
            throw new ArgumentException("El valor de matrícula no puede ser negativo", nameof(valorMatricula));

        if (valorDescuentos < 0)
            throw new ArgumentException("El valor de descuentos no puede ser negativo", nameof(valorDescuentos));

        ValorMatricula = valorMatricula;
        ValorDescuentos = valorDescuentos;
        CalcularValorFinal();
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Registra el pago de la matrícula
    /// </summary>
    public void RegistrarPagoMatricula(DateTime fechaPago, Guid? updatedBy = null)
    {
        if (fechaPago > DateTime.UtcNow)
            throw new ArgumentException("La fecha de pago no puede ser futura", nameof(fechaPago));

        MatriculaPagada = true;
        FechaPagoMatricula = fechaPago;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Anula el pago de la matrícula
    /// </summary>
    public void AnularPagoMatricula(string motivo, Guid? updatedBy = null)
    {
        if (string.IsNullOrWhiteSpace(motivo))
            throw new ArgumentException("Debe especificar el motivo de anulación", nameof(motivo));

        MatriculaPagada = false;
        FechaPagoMatricula = null;

        Observaciones = string.IsNullOrWhiteSpace(Observaciones)
            ? $"Pago anulado: {motivo}"
            : $"{Observaciones}\nPago anulado: {motivo}";

        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Actualiza documentos presentados
    /// </summary>
    public void ActualizarDocumentosPresentados(
        string? documentosPresentados,
        string? documentosPendientes = null,
        Guid? updatedBy = null)
    {
        DocumentosPresentados = documentosPresentados;
        DocumentosPendientes = documentosPendientes;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Suspende la matrícula temporalmente
    /// </summary>
    public void Suspender(string motivo, Guid? updatedBy = null)
    {
        if (string.IsNullOrWhiteSpace(motivo))
            throw new ArgumentException("Debe especificar el motivo de suspensión", nameof(motivo));

        Estado = EstadoMatricula.Suspendida;
        Observaciones = string.IsNullOrWhiteSpace(Observaciones)
            ? $"Suspendida: {motivo}"
            : $"{Observaciones}\nSuspendida: {motivo}";

        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Reactiva una matrícula suspendida
    /// </summary>
    public void Reactivar(string? motivo = null, Guid? updatedBy = null)
    {
        if (Estado != EstadoMatricula.Suspendida)
            throw new InvalidOperationException("Solo se pueden reactivar matrículas suspendidas");

        Estado = EstadoMatricula.Activa;

        if (!string.IsNullOrWhiteSpace(motivo))
        {
            Observaciones = string.IsNullOrWhiteSpace(Observaciones)
                ? $"Reactivada: {motivo}"
                : $"{Observaciones}\nReactivada: {motivo}";
        }

        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Registra el retiro del estudiante
    /// </summary>
    public void RegistrarRetiro(DateTime fechaRetiro, string motivoRetiro, Guid? updatedBy = null)
    {
        if (fechaRetiro < FechaMatricula)
            throw new ArgumentException("La fecha de retiro no puede ser anterior a la fecha de matrícula");

        if (string.IsNullOrWhiteSpace(motivoRetiro))
            throw new ArgumentException("Debe especificar el motivo del retiro", nameof(motivoRetiro));

        Estado = EstadoMatricula.Retirada;
        FechaFinalizacion = fechaRetiro;
        MotivoFinalizacion = motivoRetiro;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Registra la graduación del estudiante
    /// </summary>
    public void RegistrarGraduacion(DateTime fechaGraduacion, Guid? updatedBy = null)
    {
        if (fechaGraduacion < FechaMatricula)
            throw new ArgumentException("La fecha de graduación no puede ser anterior a la fecha de matrícula");

        Estado = EstadoMatricula.Graduada;
        FechaFinalizacion = fechaGraduacion;
        MotivoFinalizacion = "Graduación";
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Registra el traslado a otro grupo
    /// </summary>
    public void RegistrarTraslado(DateTime fechaTraslado, string motivoTraslado, Guid? updatedBy = null)
    {
        if (fechaTraslado < FechaMatricula)
            throw new ArgumentException("La fecha de traslado no puede ser anterior a la fecha de matrícula");

        if (string.IsNullOrWhiteSpace(motivoTraslado))
            throw new ArgumentException("Debe especificar el motivo del traslado", nameof(motivoTraslado));

        Estado = EstadoMatricula.Trasladada;
        FechaFinalizacion = fechaTraslado;
        MotivoFinalizacion = motivoTraslado;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Actualiza condiciones especiales
    /// </summary>
    public void ActualizarCondicionesEspeciales(string? condicionesEspeciales, Guid? updatedBy = null)
    {
        CondicionesEspeciales = condicionesEspeciales;
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
    /// Verifica si la matrícula está activa
    /// </summary>
    public bool EstaActiva() => Estado == EstadoMatricula.Activa;

    /// <summary>
    /// Verifica si el estudiante puede asistir a clases
    /// </summary>
    public bool PuedeAsistirAClases() => Estado is EstadoMatricula.Activa or EstadoMatricula.CondicionalAcademica;

    /// <summary>
    /// Verifica si tiene beca
    /// </summary>
    public bool TieneBeca() => PorcentajeBeca > 0;

    /// <summary>
    /// Verifica si la documentación está completa
    /// </summary>
    public bool DocumentacionCompleta() => string.IsNullOrWhiteSpace(DocumentosPendientes);

    /// <summary>
    /// Calcula los días desde la matrícula
    /// </summary>
    public int DiasDesdeMatricula()
    {
        return (DateTime.UtcNow.Date - FechaMatricula.Date).Days;
    }

    /// <summary>
    /// Obtiene el año académico de la matrícula (requiere navegación Grupo cargada)
    /// </summary>
    public AnoAcademico? ObtenerAnoAcademico()
    {
        return Grupo?.AnoAcademico;
    }

    /// <summary>
    /// Obtiene el grado de la matrícula (requiere navegación Grupo cargada)
    /// </summary>
    public Grado? ObtenerGrado()
    {
        return Grupo?.Grado;
    }

    /// <summary>
    /// Calcula el valor final considerando descuentos y becas
    /// </summary>
    private void CalcularValorFinal()
    {
        if (!ValorMatricula.HasValue) return;

        var valorConDescuentos = ValorMatricula.Value - (ValorDescuentos ?? 0);
        var descuentoPorBeca = valorConDescuentos * (PorcentajeBeca / 100);
        ValorFinal = valorConDescuentos - descuentoPorBeca;
    }

    /// <summary>
    /// Genera número de matrícula automático
    /// </summary>
    public static string GenerarNumeroMatriculaAutomatico(int ano, int secuencia)
    {
        return $"{ano}-{secuencia:D4}";
    }

    /// <summary>
    /// Establece la referencia al tipo de beca configurado
    /// </summary>
    public void EstablecerTipoBecaConfigurado(string? codigoTipoBeca, Guid? updatedBy = null)
    {
        CodigoTipoBecaConfigurado = codigoTipoBeca;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Valida los datos de la matrícula
    /// </summary>
    private void ValidarDatos()
    {
        if (ColegioId == null || ColegioId == Guid.Empty)
            throw new ArgumentException("El ID del colegio es requerido", nameof(ColegioId));

        if (EstudianteId == Guid.Empty)
            throw new ArgumentException("El ID del estudiante es requerido", nameof(EstudianteId));

        if (GrupoId == Guid.Empty)
            throw new ArgumentException("El ID del grupo es requerido", nameof(GrupoId));

        if (string.IsNullOrWhiteSpace(NumeroMatricula))
            throw new ArgumentException("El número de matrícula es requerido", nameof(NumeroMatricula));

        if (FechaMatricula > DateTime.UtcNow.AddDays(30))
            throw new ArgumentException("La fecha de matrícula no puede ser muy futura", nameof(FechaMatricula));

        if (PorcentajeBeca < 0 || PorcentajeBeca > 100)
            throw new ArgumentException("El porcentaje de beca debe estar entre 0 y 100", nameof(PorcentajeBeca));

        if (ValorMatricula.HasValue && ValorMatricula < 0)
            throw new ArgumentException("El valor de matrícula no puede ser negativo", nameof(ValorMatricula));
    }

    /// <summary>
    /// Override ToString para debugging
    /// </summary>
    public override string ToString()
    {
        return $"Matricula: {NumeroMatricula} - Estado: {Estado} - Beca: {PorcentajeBeca}%";
    }
}