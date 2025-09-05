using ColegiosBackend.Core.Entities.Base;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Entities;

/// <summary>
/// Entidad Grado - Representa un grado o curso académico dentro de un colegio
/// Ejemplos: Jardín, Transición, 1°, 2°, 3°, etc.
/// </summary>
public class Grado : BaseEntity, ITenantEntity
{
    /// <summary>
    /// ID del colegio al que pertenece este grado
    /// </summary>
    public Guid? ColegioId { get; private set; }

    /// <summary>
    /// Código único del grado dentro del colegio
    /// Formato sugerido: NIVEL-GRADO (ej: PRE-JAR, PRI-01, SEC-06)
    /// </summary>
    public string CodigoGrado { get; private set; } = string.Empty;

    /// <summary>
    /// Nombre oficial del grado
    /// Ejemplo: "Jardín", "Transición", "Primero", "Sexto"
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Nombre corto o abreviatura
    /// Ejemplo: "Jardín", "T°", "1°", "6°"
    /// </summary>
    public string NombreCorto { get; private set; } = string.Empty;

    /// <summary>
    /// Nivel educativo al que pertenece
    /// </summary>
    public NivelesEducativos Nivel { get; private set; }

    /// <summary>
    /// Número ordinal del grado dentro de su nivel
    /// Ejemplo: 1 para 1°, 6 para 6°, 0 para Jardín
    /// </summary>
    public int NumeroGrado { get; private set; }

    /// <summary>
    /// Estado actual del grado
    /// </summary>
    public EstadoGrado Estado { get; private set; }

    /// <summary>
    /// Edad mínima recomendada para ingresar al grado
    /// </summary>
    public int EdadMinimaRecomendada { get; private set; }

    /// <summary>
    /// Edad máxima recomendada para el grado
    /// </summary>
    public int EdadMaximaRecomendada { get; private set; }

    /// <summary>
    /// Capacidad máxima de estudiantes por grado (total)
    /// </summary>
    public int CapacidadMaxima { get; private set; }

    /// <summary>
    /// Capacidad máxima por sección/grupo
    /// </summary>
    public int CapacidadMaximaPorSeccion { get; private set; }

    /// <summary>
    /// Número mínimo de estudiantes para abrir el grado
    /// </summary>
    public int MinimoEstudiantesParaAbrir { get; private set; }

    /// <summary>
    /// ID del profesor coordinador del grado (opcional)
    /// </summary>
    public Guid? CoordinadorId { get; private set; }

    /// <summary>
    /// Descripción del grado y sus características
    /// </summary>
    public string? Descripcion { get; private set; }

    /// <summary>
    /// Objetivos académicos del grado
    /// </summary>
    public string? ObjetivosAcademicos { get; private set; }

    /// <summary>
    /// Competencias que se desarrollan en este grado
    /// </summary>
    public string? Competencias { get; private set; }

    /// <summary>
    /// Perfil del estudiante al finalizar el grado
    /// </summary>
    public string? PerfilEgreso { get; private set; }

    /// <summary>
    /// Requisitos de promoción al siguiente grado
    /// </summary>
    public string? RequisitosPromocion { get; private set; }

    /// <summary>
    /// Duración del grado en periodos académicos
    /// Normalmente 1 para anual, 2 para semestral
    /// </summary>
    public int DuracionPeriodos { get; private set; }

    /// <summary>
    /// Indica si el grado requiere uniforme específico
    /// </summary>
    public bool RequiereUniformeEspecifico { get; private set; }

    /// <summary>
    /// Descripción del uniforme (si aplica)
    /// </summary>
    public string? DescripcionUniforme { get; private set; }

    /// <summary>
    /// Horario de inicio de clases (formato 24h)
    /// Ejemplo: "07:00"
    /// </summary>
    public string? HorarioInicio { get; private set; }

    /// <summary>
    /// Horario de finalización de clases (formato 24h)
    /// Ejemplo: "14:00"
    /// </summary>
    public string? HorarioFin { get; private set; }

    /// <summary>
    /// Días de la semana en que funciona
    /// Formato JSON: ["Lunes", "Martes", "Miércoles", "Jueves", "Viernes"]
    /// </summary>
    public string? DiasFuncionamiento { get; private set; }

    /// <summary>
    /// Color de identificación para el grado (hex)
    /// </summary>
    public string? ColorIdentificacion { get; private set; }

    /// <summary>
    /// Orden de presentación en listas
    /// </summary>
    public int OrdenPresentacion { get; private set; }

    /// <summary>
    /// Observaciones generales del grado
    /// </summary>
    public string? Observaciones { get; private set; }

    // Propiedades de navegación
    /// <summary>
    /// Colegio al que pertenece
    /// </summary>
    public virtual Colegio? Colegio { get; set; }

    /// <summary>
    /// Profesor coordinador del grado
    /// </summary>
    public virtual Profesor? Coordinador { get; set; }

    /// <summary>
    /// Constructor privado para EF Core
    /// </summary>
    private Grado() { }

    /// <summary>
    /// Constructor para crear nuevo grado
    /// </summary>
    public Grado(
        Guid colegioId,
        string codigoGrado,
        string nombre,
        string nombreCorto,
        NivelesEducativos nivel,
        int numeroGrado,
        int edadMinimaRecomendada,
        int edadMaximaRecomendada,
        int capacidadMaxima,
        int capacidadMaximaPorSeccion)
    {
        ColegioId = colegioId;
        CodigoGrado = codigoGrado;
        Nombre = nombre;
        NombreCorto = nombreCorto;
        Nivel = nivel;
        NumeroGrado = numeroGrado;
        Estado = EstadoGrado.Activo;
        EdadMinimaRecomendada = edadMinimaRecomendada;
        EdadMaximaRecomendada = edadMaximaRecomendada;
        CapacidadMaxima = capacidadMaxima;
        CapacidadMaximaPorSeccion = capacidadMaximaPorSeccion;
        MinimoEstudiantesParaAbrir = 1;
        DuracionPeriodos = 1; // Anual por defecto
        RequiereUniformeEspecifico = false;
        OrdenPresentacion = numeroGrado;

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
    /// Actualiza información básica del grado
    /// </summary>
    public void ActualizarInformacionBasica(
        string nombre,
        string nombreCorto,
        string? descripcion,
        Guid? updatedBy = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre es requerido", nameof(nombre));

        if (string.IsNullOrWhiteSpace(nombreCorto))
            throw new ArgumentException("El nombre corto es requerido", nameof(nombreCorto));

        Nombre = nombre;
        NombreCorto = nombreCorto;
        Descripcion = descripcion;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Actualiza configuración académica
    /// </summary>
    public void ActualizarConfiguracionAcademica(
        string? objetivosAcademicos,
        string? competencias,
        string? perfilEgreso,
        string? requisitosPromocion,
        int duracionPeriodos,
        Guid? updatedBy = null)
    {
        if (duracionPeriodos < 1 || duracionPeriodos > 4)
            throw new ArgumentException("La duración debe estar entre 1 y 4 periodos", nameof(duracionPeriodos));

        ObjetivosAcademicos = objetivosAcademicos;
        Competencias = competencias;
        PerfilEgreso = perfilEgreso;
        RequisitosPromocion = requisitosPromocion;
        DuracionPeriodos = duracionPeriodos;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Actualiza capacidades y restricciones
    /// </summary>
    public void ActualizarCapacidades(
        int capacidadMaxima,
        int capacidadMaximaPorSeccion,
        int minimoEstudiantesParaAbrir,
        Guid? updatedBy = null)
    {
        if (capacidadMaxima < 1)
            throw new ArgumentException("La capacidad máxima debe ser mayor a 0", nameof(capacidadMaxima));

        if (capacidadMaximaPorSeccion < 1)
            throw new ArgumentException("La capacidad por sección debe ser mayor a 0", nameof(capacidadMaximaPorSeccion));

        if (minimoEstudiantesParaAbrir < 1)
            throw new ArgumentException("El mínimo para abrir debe ser mayor a 0", nameof(minimoEstudiantesParaAbrir));

        if (capacidadMaximaPorSeccion > capacidadMaxima)
            throw new ArgumentException("La capacidad por sección no puede ser mayor a la capacidad total");

        CapacidadMaxima = capacidadMaxima;
        CapacidadMaximaPorSeccion = capacidadMaximaPorSeccion;
        MinimoEstudiantesParaAbrir = minimoEstudiantesParaAbrir;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Actualiza edades recomendadas
    /// </summary>
    public void ActualizarEdadesRecomendadas(
        int edadMinima,
        int edadMaxima,
        Guid? updatedBy = null)
    {
        if (edadMinima < 3 || edadMinima > 25)
            throw new ArgumentException("La edad mínima debe estar entre 3 y 25 años", nameof(edadMinima));

        if (edadMaxima < edadMinima || edadMaxima > 30)
            throw new ArgumentException("La edad máxima debe ser mayor a la mínima y menor a 30", nameof(edadMaxima));

        EdadMinimaRecomendada = edadMinima;
        EdadMaximaRecomendada = edadMaxima;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Asigna coordinador al grado
    /// </summary>
    public void AsignarCoordinador(Guid? coordinadorId, Guid? updatedBy = null)
    {
        CoordinadorId = coordinadorId;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Configura información de uniforme
    /// </summary>
    public void ConfigurarUniforme(
        bool requiereUniformeEspecifico,
        string? descripcionUniforme = null,
        Guid? updatedBy = null)
    {
        RequiereUniformeEspecifico = requiereUniformeEspecifico;
        DescripcionUniforme = requiereUniformeEspecifico ? descripcionUniforme : null;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Configura horarios del grado
    /// </summary>
    public void ConfigurarHorarios(
        string? horarioInicio,
        string? horarioFin,
        string? diasFuncionamiento,
        Guid? updatedBy = null)
    {
        HorarioInicio = horarioInicio;
        HorarioFin = horarioFin;
        DiasFuncionamiento = diasFuncionamiento;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Actualiza presentación visual
    /// </summary>
    public void ActualizarPresentacionVisual(
        string? colorIdentificacion,
        int ordenPresentacion,
        Guid? updatedBy = null)
    {
        if (ordenPresentacion < 0)
            throw new ArgumentException("El orden debe ser mayor o igual a 0", nameof(ordenPresentacion));

        ColorIdentificacion = colorIdentificacion;
        OrdenPresentacion = ordenPresentacion;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Desactiva el grado
    /// </summary>
    public void Desactivar(string? motivo = null, Guid? updatedBy = null)
    {
        Estado = EstadoGrado.Inactivo;

        if (!string.IsNullOrWhiteSpace(motivo))
        {
            Observaciones = string.IsNullOrWhiteSpace(Observaciones)
                ? $"Desactivado: {motivo}"
                : $"{Observaciones}\nDesactivado: {motivo}";
        }

        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Reactiva el grado
    /// </summary>
    public void Reactivar(string? motivo = null, Guid? updatedBy = null)
    {
        Estado = EstadoGrado.Activo;

        if (!string.IsNullOrWhiteSpace(motivo))
        {
            Observaciones = string.IsNullOrWhiteSpace(Observaciones)
                ? $"Reactivado: {motivo}"
                : $"{Observaciones}\nReactivado: {motivo}";
        }

        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Suspende el grado temporalmente
    /// </summary>
    public void Suspender(string motivo, Guid? updatedBy = null)
    {
        if (string.IsNullOrWhiteSpace(motivo))
            throw new ArgumentException("Debe especificar el motivo de suspensión", nameof(motivo));

        Estado = EstadoGrado.Suspendido;
        Observaciones = string.IsNullOrWhiteSpace(Observaciones)
            ? $"Suspendido: {motivo}"
            : $"{Observaciones}\nSuspendido: {motivo}";

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
    /// Verifica si el grado está activo
    /// </summary>
    public bool EstaActivo() => Estado == EstadoGrado.Activo;

    /// <summary>
    /// Verifica si puede recibir nuevos estudiantes
    /// </summary>
    public bool PuedeRecibirEstudiantes() => Estado == EstadoGrado.Activo;

    /// <summary>
    /// Verifica si una edad está dentro del rango recomendado
    /// </summary>
    public bool EdadEnRangoRecomendado(int edad)
    {
        return edad >= EdadMinimaRecomendada && edad <= EdadMaximaRecomendada;
    }

    /// <summary>
    /// Calcula el número máximo de secciones posibles
    /// </summary>
    public int CalcularMaximoSecciones()
    {
        return (int)Math.Ceiling((double)CapacidadMaxima / CapacidadMaximaPorSeccion);
    }

    /// <summary>
    /// Obtiene el siguiente grado en la secuencia
    /// </summary>
    public int ObtenerSiguienteNumeroGrado()
    {
        return NumeroGrado + 1;
    }

    /// <summary>
    /// Verifica si es el último grado del nivel
    /// </summary>
    public bool EsUltimoGradoDelNivel()
    {
        return Nivel switch
        {
            NivelesEducativos.Preescolar => NumeroGrado >= 2, // Jardín(0), Transición(1)
            NivelesEducativos.Primaria => NumeroGrado >= 5,   // 1° a 5°
            NivelesEducativos.SecundariaBasica => NumeroGrado >= 9, // 6° a 9°
            NivelesEducativos.SecundariaMedia => NumeroGrado >= 11, // 10° y 11°
            _ => false
        };
    }

    /// <summary>
    /// Valida los datos del grado
    /// </summary>
    private void ValidarDatos()
    {
        if (ColegioId == null || ColegioId == Guid.Empty)
            throw new ArgumentException("El ID del colegio es requerido", nameof(ColegioId));

        if (string.IsNullOrWhiteSpace(CodigoGrado))
            throw new ArgumentException("El código del grado es requerido", nameof(CodigoGrado));

        if (string.IsNullOrWhiteSpace(Nombre))
            throw new ArgumentException("El nombre del grado es requerido", nameof(Nombre));

        if (string.IsNullOrWhiteSpace(NombreCorto))
            throw new ArgumentException("El nombre corto es requerido", nameof(NombreCorto));

        if (NumeroGrado < 0 || NumeroGrado > 15)
            throw new ArgumentException("El número de grado debe estar entre 0 y 15", nameof(NumeroGrado));

        if (EdadMinimaRecomendada < 3 || EdadMinimaRecomendada > 25)
            throw new ArgumentException("La edad mínima debe estar entre 3 y 25 años", nameof(EdadMinimaRecomendada));

        if (EdadMaximaRecomendada < EdadMinimaRecomendada || EdadMaximaRecomendada > 30)
            throw new ArgumentException("La edad máxima debe ser mayor a la mínima y menor a 30", nameof(EdadMaximaRecomendada));

        if (CapacidadMaxima < 1)
            throw new ArgumentException("La capacidad máxima debe ser mayor a 0", nameof(CapacidadMaxima));

        if (CapacidadMaximaPorSeccion < 1 || CapacidadMaximaPorSeccion > CapacidadMaxima)
            throw new ArgumentException("La capacidad por sección debe ser válida", nameof(CapacidadMaximaPorSeccion));

        if (Nivel == NivelesEducativos.Ninguno)
            throw new ArgumentException("Debe especificar un nivel educativo válido", nameof(Nivel));
    }

    /// <summary>
    /// Override ToString para debugging
    /// </summary>
    public override string ToString()
    {
        return $"Grado: {CodigoGrado} - {Nombre} ({NombreCorto}) - {Nivel}";
    }
}