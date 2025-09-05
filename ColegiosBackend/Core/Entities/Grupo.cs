using ColegiosBackend.Core.Entities.Base;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Entities;

/// <summary>
/// Entidad Grupo - Representa una sección o grupo específico dentro de un grado y año académico
/// Ejemplos: "3° A", "Quinto B", "Jardín Única"
/// Es la división organizacional donde se asignan los estudiantes
/// </summary>
public class Grupo : BaseEntity, ITenantEntity
{
    /// <summary>
    /// ID del colegio al que pertenece este grupo
    /// </summary>
    public Guid? ColegioId { get; private set; }

    /// <summary>
    /// ID del grado al que pertenece este grupo
    /// </summary>
    public Guid GradoId { get; private set; }

    /// <summary>
    /// ID del año académico en el que existe este grupo
    /// </summary>
    public Guid AnoAcademicoId { get; private set; }

    /// <summary>
    /// Código del grupo (generalmente una letra)
    /// Ejemplos: "A", "B", "C", "UNICA", "1", "2"
    /// </summary>
    public string Codigo { get; private set; } = string.Empty;

    /// <summary>
    /// Nombre completo del grupo
    /// Ejemplos: "Tercero A", "3° A", "Jardín Única", "Quinto B"
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Capacidad máxima de estudiantes para este grupo
    /// </summary>
    public int CapacidadMaxima { get; private set; }

    /// <summary>
    /// ID del profesor director/titular del grupo
    /// </summary>
    public Guid? DirectorGrupoId { get; private set; }

    /// <summary>
    /// Indica si el grupo está activo
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Aula o salón asignado al grupo
    /// Ejemplo: "Salón 201", "Aula A-15", "Laboratorio 1"
    /// </summary>
    public string? Aula { get; private set; }

    /// <summary>
    /// Horario de inicio de clases para este grupo
    /// Formato: "07:30", "08:00"
    /// </summary>
    public string? HorarioInicio { get; private set; }

    /// <summary>
    /// Horario de finalización de clases para este grupo
    /// Formato: "13:30", "15:00"
    /// </summary>
    public string? HorarioFin { get; private set; }

    /// <summary>
    /// Jornada del grupo (Mañana, Tarde, Nocturna, Única)
    /// </summary>
    public JornadaAcademica? Jornada { get; private set; }

    /// <summary>
    /// Observaciones específicas del grupo
    /// </summary>
    public string? Observaciones { get; private set; }

    /// <summary>
    /// Configuración específica del grupo (JSON)
    /// Puede incluir: reglas especiales, configuración de evaluación, etc.
    /// </summary>
    public string? Configuracion { get; private set; }

    /// <summary>
    /// Color de identificación del grupo (para UI)
    /// </summary>
    public string? ColorIdentificacion { get; private set; }

    /// <summary>
    /// Orden de presentación en listas
    /// </summary>
    public int OrdenPresentacion { get; private set; }

    // Propiedades de navegación
    /// <summary>
    /// Colegio al que pertenece
    /// </summary>
    public virtual Colegio? Colegio { get; set; }

    /// <summary>
    /// Grado al que pertenece
    /// </summary>
    public virtual Grado? Grado { get; set; }

    /// <summary>
    /// Año académico en el que existe
    /// </summary>
    public virtual AnoAcademico? AnoAcademico { get; set; }

    /// <summary>
    /// Profesor director del grupo
    /// </summary>
    public virtual Profesor? DirectorGrupo { get; set; }

    /// <summary>
    /// Constructor privado para EF Core
    /// </summary>
    private Grupo() { }

    /// <summary>
    /// Constructor para crear nuevo grupo
    /// </summary>
    public Grupo(
        Guid colegioId,
        Guid gradoId,
        Guid anoAcademicoId,
        string codigo,
        string nombre,
        int capacidadMaxima = 30)
    {
        ColegioId = colegioId;
        GradoId = gradoId;
        AnoAcademicoId = anoAcademicoId;
        Codigo = codigo;
        Nombre = nombre;
        CapacidadMaxima = capacidadMaxima;
        Activo = true;
        OrdenPresentacion = ObtenerOrdenPorCodigo(codigo);

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
    /// Actualiza información básica del grupo
    /// </summary>
    public void ActualizarInformacionBasica(
        string codigo,
        string nombre,
        int capacidadMaxima,
        Guid? updatedBy = null)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("El código es requerido", nameof(codigo));

        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre es requerido", nameof(nombre));

        if (capacidadMaxima < 1 || capacidadMaxima > 50)
            throw new ArgumentException("La capacidad debe estar entre 1 y 50 estudiantes", nameof(capacidadMaxima));

        Codigo = codigo;
        Nombre = nombre;
        CapacidadMaxima = capacidadMaxima;
        OrdenPresentacion = ObtenerOrdenPorCodigo(codigo);
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Asigna director de grupo
    /// </summary>
    public void AsignarDirectorGrupo(Guid? directorGrupoId, Guid? updatedBy = null)
    {
        DirectorGrupoId = directorGrupoId;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Configura ubicación física del grupo
    /// </summary>
    public void ConfigurarUbicacion(
        string? aula,
        string? observaciones = null,
        Guid? updatedBy = null)
    {
        Aula = aula;
        if (!string.IsNullOrWhiteSpace(observaciones))
        {
            Observaciones = observaciones;
        }
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Configura horarios del grupo
    /// </summary>
    public void ConfigurarHorarios(
        string? horarioInicio,
        string? horarioFin,
        JornadaAcademica? jornada,
        Guid? updatedBy = null)
    {
        // Validar formato de hora si se proporciona
        if (!string.IsNullOrWhiteSpace(horarioInicio) && !EsFormatoHoraValido(horarioInicio))
            throw new ArgumentException("Formato de hora inválido para inicio (usar HH:mm)", nameof(horarioInicio));

        if (!string.IsNullOrWhiteSpace(horarioFin) && !EsFormatoHoraValido(horarioFin))
            throw new ArgumentException("Formato de hora inválido para fin (usar HH:mm)", nameof(horarioFin));

        // Validar que hora de fin sea posterior a hora de inicio
        if (!string.IsNullOrWhiteSpace(horarioInicio) && !string.IsNullOrWhiteSpace(horarioFin))
        {
            if (TimeSpan.Parse(horarioFin) <= TimeSpan.Parse(horarioInicio))
                throw new ArgumentException("La hora de fin debe ser posterior a la hora de inicio");
        }

        HorarioInicio = horarioInicio;
        HorarioFin = horarioFin;
        Jornada = jornada;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Actualiza configuración específica del grupo
    /// </summary>
    public void ActualizarConfiguracion(string? configuracion, Guid? updatedBy = null)
    {
        Configuracion = configuracion;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Actualiza presentación visual
    /// </summary>
    public void ActualizarPresentacionVisual(
        string? colorIdentificacion,
        int? ordenPresentacion = null,
        Guid? updatedBy = null)
    {
        ColorIdentificacion = colorIdentificacion;

        if (ordenPresentacion.HasValue)
        {
            if (ordenPresentacion < 0)
                throw new ArgumentException("El orden debe ser mayor o igual a 0", nameof(ordenPresentacion));

            OrdenPresentacion = ordenPresentacion.Value;
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
    /// Activa el grupo
    /// </summary>
    public void Activar(Guid? updatedBy = null)
    {
        Activo = true;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Desactiva el grupo
    /// </summary>
    public void Desactivar(string? motivo = null, Guid? updatedBy = null)
    {
        Activo = false;

        if (!string.IsNullOrWhiteSpace(motivo))
        {
            Observaciones = string.IsNullOrWhiteSpace(Observaciones)
                ? $"Desactivado: {motivo}"
                : $"{Observaciones}\nDesactivado: {motivo}";
        }

        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Verifica si el grupo está activo
    /// </summary>
    public bool EstaActivo() => Activo;

    /// <summary>
    /// Verifica si tiene director de grupo asignado
    /// </summary>
    public bool TieneDirectorAsignado() => DirectorGrupoId.HasValue;

    /// <summary>
    /// Verifica si tiene aula asignada
    /// </summary>
    public bool TieneAulaAsignada() => !string.IsNullOrWhiteSpace(Aula);

    /// <summary>
    /// Verifica si tiene horarios configurados
    /// </summary>
    public bool TieneHorariosConfigurados() =>
        !string.IsNullOrWhiteSpace(HorarioInicio) && !string.IsNullOrWhiteSpace(HorarioFin);

    /// <summary>
    /// Calcula la duración de la jornada en horas
    /// </summary>
    public double? CalcularDuracionJornada()
    {
        if (!TieneHorariosConfigurados()) return null;

        var inicio = TimeSpan.Parse(HorarioInicio!);
        var fin = TimeSpan.Parse(HorarioFin!);

        return (fin - inicio).TotalHours;
    }

    /// <summary>
    /// Obtiene el nombre completo del grupo incluyendo grado
    /// Requiere que la navegación Grado esté cargada
    /// </summary>
    public string ObtenerNombreCompleto()
    {
        if (Grado != null)
        {
            return $"{Grado.NombreCorto} {Codigo}";
        }
        return Nombre;
    }

    /// <summary>
    /// Genera código automático para el grupo
    /// </summary>
    public static string GenerarCodigoAutomatico(int numeroSecuencia)
    {
        if (numeroSecuencia <= 0)
            throw new ArgumentException("El número debe ser mayor a 0");

        // Convertir número a letra: 1=A, 2=B, etc.
        if (numeroSecuencia <= 26)
        {
            return ((char)('A' + numeroSecuencia - 1)).ToString();
        }

        // Para más de 26 grupos: AA, AB, etc.
        var primera = (char)('A' + (numeroSecuencia - 1) / 26 - 1);
        var segunda = (char)('A' + (numeroSecuencia - 1) % 26);
        return $"{primera}{segunda}";
    }

    /// <summary>
    /// Genera nombre automático basado en grado y código
    /// </summary>
    public static string GenerarNombreAutomatico(string nombreGrado, string codigoGrupo)
    {
        return $"{nombreGrado} {codigoGrupo}";
    }

    /// <summary>
    /// Valida formato de hora (HH:mm)
    /// </summary>
    private bool EsFormatoHoraValido(string hora)
    {
        return TimeSpan.TryParse(hora, out _);
    }

    /// <summary>
    /// Obtiene orden de presentación basado en el código
    /// </summary>
    private int ObtenerOrdenPorCodigo(string codigo)
    {
        // Si es una letra simple (A=1, B=2, etc.)
        if (codigo.Length == 1 && char.IsLetter(codigo[0]))
        {
            return char.ToUpper(codigo[0]) - 'A' + 1;
        }

        // Si es un número
        if (int.TryParse(codigo, out int numero))
        {
            return numero;
        }

        // Casos especiales
        return codigo.ToUpper() switch
        {
            "UNICA" => 1,
            "UNICO" => 1,
            _ => 999 // Al final por defecto
        };
    }

    /// <summary>
    /// Valida los datos del grupo
    /// </summary>
    private void ValidarDatos()
    {
        if (ColegioId == null || ColegioId == Guid.Empty)
            throw new ArgumentException("El ID del colegio es requerido", nameof(ColegioId));

        if (GradoId == Guid.Empty)
            throw new ArgumentException("El ID del grado es requerido", nameof(GradoId));

        if (AnoAcademicoId == Guid.Empty)
            throw new ArgumentException("El ID del año académico es requerido", nameof(AnoAcademicoId));

        if (string.IsNullOrWhiteSpace(Codigo))
            throw new ArgumentException("El código es requerido", nameof(Codigo));

        if (string.IsNullOrWhiteSpace(Nombre))
            throw new ArgumentException("El nombre es requerido", nameof(Nombre));

        if (CapacidadMaxima < 1 || CapacidadMaxima > 50)
            throw new ArgumentException("La capacidad debe estar entre 1 y 50 estudiantes", nameof(CapacidadMaxima));
    }

    /// <summary>
    /// Override ToString para debugging
    /// </summary>
    public override string ToString()
    {
        return $"Grupo: {Nombre} ({Codigo}) - Capacidad: {CapacidadMaxima} - Activo: {Activo}";
    }
}