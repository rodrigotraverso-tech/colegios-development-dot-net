using ColegiosBackend.Core.Entities.Base;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Entities;

/// <summary>
/// Entidad Materia - Representa una asignatura o materia académica del colegio
/// Cada materia pertenece a un colegio específico y puede ser enseñada por múltiples profesores
/// </summary>
public class Materia : BaseEntity, ITenantEntity
{
    /// <summary>
    /// ID del colegio al que pertenece esta materia
    /// </summary>
    public Guid? ColegioId { get; private set; }

    /// <summary>
    /// Código único de la materia dentro del colegio
    /// Formato sugerido: MAT-XXX (ej: MAT-001, ESP-001, ING-001)
    /// </summary>
    public string CodigoMateria { get; private set; } = string.Empty;

    /// <summary>
    /// Nombre oficial de la materia
    /// Ejemplo: "Matemáticas", "Lengua Castellana", "Ciencias Naturales"
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Nombre corto o abreviatura de la materia
    /// Ejemplo: "Mat", "Español", "C.Nat"
    /// </summary>
    public string NombreCorto { get; private set; } = string.Empty;

    /// <summary>
    /// Descripción detallada de la materia
    /// </summary>
    public string? Descripcion { get; private set; }

    /// <summary>
    /// Área académica a la que pertenece la materia
    /// </summary>
    public AreaAcademica Area { get; private set; }

    /// <summary>
    /// Niveles educativos en los que se puede enseñar esta materia
    /// </summary>
    public NivelesEducativos NivelesPermitidos { get; private set; }

    /// <summary>
    /// Estado actual de la materia
    /// </summary>
    public EstadoMateria Estado { get; private set; }

    /// <summary>
    /// Intensidad horaria semanal en horas
    /// </summary>
    public int IntensidadHorariaSemanal { get; private set; }

    /// <summary>
    /// Número de créditos académicos (si aplica)
    /// </summary>
    public int? Creditos { get; private set; }

    /// <summary>
    /// Indica si la materia es obligatoria
    /// </summary>
    public bool EsObligatoria { get; private set; }

    /// <summary>
    /// Indica si la materia es práctica (laboratorio, talleres, etc.)
    /// </summary>
    public bool EsPractica { get; private set; }

    /// <summary>
    /// Indica si requiere materiales especiales
    /// </summary>
    public bool RequiereMateriales { get; private set; }

    /// <summary>
    /// Lista de materiales requeridos (JSON)
    /// Ejemplo: ["Calculadora científica", "Compás", "Regla"]
    /// </summary>
    public string? MaterialesRequeridos { get; private set; }

    /// <summary>
    /// Códigos de materias prerequisito (JSON array)
    /// Ejemplo: ["MAT-001", "MAT-002"] para materias avanzadas
    /// </summary>
    public string? MateriasPrerequisito { get; private set; }

    /// <summary>
    /// Competencias que desarrolla la materia
    /// </summary>
    public string? Competencias { get; private set; }

    /// <summary>
    /// Metodología de enseñanza recomendada
    /// </summary>
    public string? Metodologia { get; private set; }

    /// <summary>
    /// Criterios de evaluación de la materia
    /// </summary>
    public string? CriteriosEvaluacion { get; private set; }

    /// <summary>
    /// Porcentaje mínimo para aprobar (0-100)
    /// </summary>
    public int PorcentajeMinimoAprobacion { get; private set; }

    /// <summary>
    /// Color hex para identificación visual en horarios
    /// Ejemplo: "#FF5733" para rojo
    /// </summary>
    public string? ColorIdentificacion { get; private set; }

    /// <summary>
    /// Icono o emoji para identificación visual
    /// Ejemplo: "📐" para matemáticas, "📚" para literatura
    /// </summary>
    public string? Icono { get; private set; }

    /// <summary>
    /// Orden de presentación en listas
    /// </summary>
    public int OrdenPresentacion { get; private set; }

    /// <summary>
    /// Observaciones generales de la materia
    /// </summary>
    public string? Observaciones { get; private set; }

    // Propiedades de navegación
    /// <summary>
    /// Colegio al que pertenece
    /// </summary>
    public virtual Colegio? Colegio { get; set; }

    /// <summary>
    /// Constructor privado para EF Core
    /// </summary>
    private Materia() { }

    /// <summary>
    /// Constructor para crear nueva materia
    /// </summary>
    public Materia(
        Guid colegioId,
        string codigoMateria,
        string nombre,
        string nombreCorto,
        AreaAcademica area,
        NivelesEducativos nivelesPermitidos,
        int intensidadHorariaSemanal,
        bool esObligatoria = true,
        int porcentajeMinimoAprobacion = 60)
    {
        ColegioId = colegioId;
        CodigoMateria = codigoMateria;
        Nombre = nombre;
        NombreCorto = nombreCorto;
        Area = area;
        NivelesPermitidos = nivelesPermitidos;
        Estado = EstadoMateria.Activa;
        IntensidadHorariaSemanal = intensidadHorariaSemanal;
        EsObligatoria = esObligatoria;
        PorcentajeMinimoAprobacion = porcentajeMinimoAprobacion;
        EsPractica = false;
        RequiereMateriales = false;
        OrdenPresentacion = 999; // Al final por defecto

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
    /// Actualiza información básica de la materia
    /// </summary>
    public void ActualizarInformacionBasica(
        string nombre,
        string nombreCorto,
        string? descripcion,
        AreaAcademica area,
        Guid? updatedBy = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre es requerido", nameof(nombre));

        if (string.IsNullOrWhiteSpace(nombreCorto))
            throw new ArgumentException("El nombre corto es requerido", nameof(nombreCorto));

        Nombre = nombre;
        NombreCorto = nombreCorto;
        Descripcion = descripcion;
        Area = area;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Actualiza configuración académica
    /// </summary>
    public void ActualizarConfiguracionAcademica(
        NivelesEducativos nivelesPermitidos,
        int intensidadHorariaSemanal,
        int? creditos,
        bool esObligatoria,
        bool esPractica,
        int porcentajeMinimoAprobacion,
        Guid? updatedBy = null)
    {
        if (intensidadHorariaSemanal < 1 || intensidadHorariaSemanal > 40)
            throw new ArgumentException("La intensidad horaria debe estar entre 1 y 40 horas", nameof(intensidadHorariaSemanal));

        if (porcentajeMinimoAprobacion < 0 || porcentajeMinimoAprobacion > 100)
            throw new ArgumentException("El porcentaje mínimo debe estar entre 0 y 100", nameof(porcentajeMinimoAprobacion));

        NivelesPermitidos = nivelesPermitidos;
        IntensidadHorariaSemanal = intensidadHorariaSemanal;
        Creditos = creditos;
        EsObligatoria = esObligatoria;
        EsPractica = esPractica;
        PorcentajeMinimoAprobacion = porcentajeMinimoAprobacion;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Actualiza materiales requeridos
    /// </summary>
    public void ActualizarMateriales(
        bool requiereMateriales,
        string? materialesRequeridos = null,
        Guid? updatedBy = null)
    {
        RequiereMateriales = requiereMateriales;
        MaterialesRequeridos = requiereMateriales ? materialesRequeridos : null;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Establece materias prerequisito
    /// </summary>
    public void EstablecerPrerequisitos(string? materiasPrerequisito, Guid? updatedBy = null)
    {
        MateriasPrerequisito = materiasPrerequisito;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Actualiza información pedagógica
    /// </summary>
    public void ActualizarInformacionPedagogica(
        string? competencias,
        string? metodologia,
        string? criteriosEvaluacion,
        Guid? updatedBy = null)
    {
        Competencias = competencias;
        Metodologia = metodologia;
        CriteriosEvaluacion = criteriosEvaluacion;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Actualiza presentación visual
    /// </summary>
    public void ActualizarPresentacionVisual(
        string? colorIdentificacion,
        string? icono,
        int ordenPresentacion,
        Guid? updatedBy = null)
    {
        if (ordenPresentacion < 0)
            throw new ArgumentException("El orden de presentación debe ser mayor o igual a 0", nameof(ordenPresentacion));

        ColorIdentificacion = colorIdentificacion;
        Icono = icono;
        OrdenPresentacion = ordenPresentacion;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Desactiva la materia
    /// </summary>
    public void Desactivar(string? motivo = null, Guid? updatedBy = null)
    {
        Estado = EstadoMateria.Inactiva;

        if (!string.IsNullOrWhiteSpace(motivo))
        {
            Observaciones = string.IsNullOrWhiteSpace(Observaciones)
                ? $"Desactivada: {motivo}"
                : $"{Observaciones}\nDesactivada: {motivo}";
        }

        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Reactiva la materia
    /// </summary>
    public void Reactivar(string? motivo = null, Guid? updatedBy = null)
    {
        Estado = EstadoMateria.Activa;

        if (!string.IsNullOrWhiteSpace(motivo))
        {
            Observaciones = string.IsNullOrWhiteSpace(Observaciones)
                ? $"Reactivada: {motivo}"
                : $"{Observaciones}\nReactivada: {motivo}";
        }

        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Pone la materia en revisión
    /// </summary>
    public void PonerEnRevision(string? motivo = null, Guid? updatedBy = null)
    {
        Estado = EstadoMateria.EnRevision;

        if (!string.IsNullOrWhiteSpace(motivo))
        {
            Observaciones = string.IsNullOrWhiteSpace(Observaciones)
                ? $"En revisión: {motivo}"
                : $"{Observaciones}\nEn revisión: {motivo}";
        }

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
    /// Verifica si la materia está activa
    /// </summary>
    public bool EstaActiva() => Estado == EstadoMateria.Activa;

    /// <summary>
    /// Verifica si puede ser asignada a un nivel específico
    /// </summary>
    public bool PuedeAsignarseANivel(NivelesEducativos nivel)
    {
        return NivelesPermitidos.HasFlag(nivel);
    }

    /// <summary>
    /// Verifica si tiene prerequisitos
    /// </summary>
    public bool TienePrerequisitos()
    {
        return !string.IsNullOrWhiteSpace(MateriasPrerequisito);
    }

    /// <summary>
    /// Obtiene las horas totales por período académico (asumiendo 40 semanas)
    /// </summary>
    public int ObtenerHorasTotalesPorPeriodo(int semanasAcademicas = 40)
    {
        return IntensidadHorariaSemanal * semanasAcademicas;
    }

    /// <summary>
    /// Valida los datos de la materia
    /// </summary>
    private void ValidarDatos()
    {
        if (ColegioId == null || ColegioId == Guid.Empty)
            throw new ArgumentException("El ID del colegio es requerido", nameof(ColegioId));

        if (string.IsNullOrWhiteSpace(CodigoMateria))
            throw new ArgumentException("El código de la materia es requerido", nameof(CodigoMateria));

        if (string.IsNullOrWhiteSpace(Nombre))
            throw new ArgumentException("El nombre de la materia es requerido", nameof(Nombre));

        if (string.IsNullOrWhiteSpace(NombreCorto))
            throw new ArgumentException("El nombre corto es requerido", nameof(NombreCorto));

        if (IntensidadHorariaSemanal < 1 || IntensidadHorariaSemanal > 40)
            throw new ArgumentException("La intensidad horaria debe estar entre 1 y 40 horas", nameof(IntensidadHorariaSemanal));

        if (PorcentajeMinimoAprobacion < 0 || PorcentajeMinimoAprobacion > 100)
            throw new ArgumentException("El porcentaje mínimo debe estar entre 0 y 100", nameof(PorcentajeMinimoAprobacion));

        if (NivelesPermitidos == NivelesEducativos.Ninguno)
            throw new ArgumentException("Debe especificar al menos un nivel educativo", nameof(NivelesPermitidos));

        if (Creditos.HasValue && Creditos < 0)
            throw new ArgumentException("Los créditos no pueden ser negativos", nameof(Creditos));
    }

    /// <summary>
    /// Override ToString para debugging
    /// </summary>
    public override string ToString()
    {
        return $"Materia: {CodigoMateria} - {Nombre} ({NombreCorto}) - {Area}";
    }
}