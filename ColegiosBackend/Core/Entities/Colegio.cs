using System;
using System.Collections.Generic;
using ColegiosBackend.Core.Entities.Base;
using ColegiosBackend.Core.ValueObjects;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Entities;

/// <summary>
/// Entidad que representa un colegio en el sistema
/// Es la entidad principal del multi-tenancy
/// </summary>
public class Colegio : BaseEntity, IAuditableEntity
{
    /// <summary>
    /// Nombre oficial del colegio
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Nombre comercial o abreviado del colegio
    /// </summary>
    public string? NombreComercial { get; private set; }

    /// <summary>
    /// Código único del colegio (usado para identificación rápida)
    /// </summary>
    public string Codigo { get; private set; } = string.Empty;

    /// <summary>
    /// Número de identificación tributaria del colegio
    /// </summary>
    public string? Nit { get; private set; }

    /// <summary>
    /// Dirección física del colegio
    /// </summary>
    public Direccion? Direccion { get; private set; }

    /// <summary>
    /// Información de contacto del colegio
    /// </summary>
    public ContactInfo? ContactInfo { get; private set; }

    /// <summary>
    /// Indica si el colegio está activo en el sistema
    /// </summary>
    public bool EstaActivo { get; private set; }

    /// <summary>
    /// Fecha de fundación del colegio
    /// </summary>
    public DateTime? FechaFundacion { get; private set; }

    /// <summary>
    /// Logo del colegio (URL o ruta del archivo)
    /// </summary>
    public string? LogoUrl { get; private set; }

    /// <summary>
    /// Configuración específica del colegio en formato JSON
    /// </summary>
    public string? ConfiguracionJson { get; private set; }

    /// <summary>
    /// Tipo de institución educativa
    /// </summary>
    public TipoInstitucion TipoInstitucion { get; private set; }

    /// <summary>
    /// Niveles educativos que ofrece el colegio
    /// </summary>
    public NivelesEducativos NivelesEducativos { get; private set; }

    // Navegación - Colecciones relacionadas
    private readonly List<Usuario> _usuarios = new();
    private readonly List<Estudiante> _estudiantes = new();
    private readonly List<Profesor> _profesores = new();
    private readonly List<AnoAcademico> _aniosEscolares = new();

    /// <summary>
    /// Usuarios asociados al colegio
    /// </summary>
    public IReadOnlyCollection<Usuario> Usuarios => _usuarios.AsReadOnly();

    /// <summary>
    /// Estudiantes del colegio
    /// </summary>
    public IReadOnlyCollection<Estudiante> Estudiantes => _estudiantes.AsReadOnly();

    /// <summary>
    /// Profesores del colegio
    /// </summary>
    public IReadOnlyCollection<Profesor> Profesores => _profesores.AsReadOnly();

    /// <summary>
    /// Años escolares del colegio
    /// </summary>
    public IReadOnlyCollection<AnoAcademico> AniosEscolares => _aniosEscolares.AsReadOnly();

    /// <summary>
    /// Constructor privado para Entity Framework
    /// </summary>
    private Colegio() : base() { }

    /// <summary>
    /// Constructor para crear un nuevo colegio
    /// </summary>
    public Colegio(
        string nombre,
        string codigo,
        TipoInstitucion tipoInstitucion,
        NivelesEducativos nivelesEducativos,
        string? nombreComercial = null,
        string? nit = null,
        Direccion? direccion = null,
        ContactInfo? contactInfo = null,
        DateTime? fechaFundacion = null) : base()
    {
        SetNombre(nombre);
        SetCodigo(codigo);
        TipoInstitucion = tipoInstitucion;
        NivelesEducativos = nivelesEducativos;
        NombreComercial = nombreComercial?.Trim();
        Nit = nit?.Trim();
        Direccion = direccion;
        ContactInfo = contactInfo;
        FechaFundacion = fechaFundacion;
        EstaActivo = true;
    }

    /// <summary>
    /// Actualiza el nombre del colegio
    /// </summary>
    public void SetNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del colegio no puede estar vacío", nameof(nombre));

        if (nombre.Length > 200)
            throw new ArgumentException("El nombre del colegio no puede exceder 200 caracteres", nameof(nombre));

        Nombre = nombre.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Actualiza el código del colegio
    /// </summary>
    public void SetCodigo(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("El código del colegio no puede estar vacío", nameof(codigo));

        if (codigo.Length > 20)
            throw new ArgumentException("El código del colegio no puede exceder 20 caracteres", nameof(codigo));

        Codigo = codigo.Trim().ToUpperInvariant();
        MarkAsUpdated();
    }

    /// <summary>
    /// Actualiza la información de contacto
    /// </summary>
    public void UpdateContactInfo(ContactInfo? contactInfo)
    {
        ContactInfo = contactInfo;
        MarkAsUpdated();
    }

    /// <summary>
    /// Actualiza la dirección del colegio
    /// </summary>
    public void UpdateDireccion(Direccion? direccion)
    {
        Direccion = direccion;
        MarkAsUpdated();
    }

    /// <summary>
    /// Activa el colegio
    /// </summary>
    public void Activar()
    {
        if (!EstaActivo)
        {
            EstaActivo = true;
            MarkAsUpdated();
        }
    }

    /// <summary>
    /// Desactiva el colegio
    /// </summary>
    public void Desactivar()
    {
        if (EstaActivo)
        {
            EstaActivo = false;
            MarkAsUpdated();
        }
    }

    /// <summary>
    /// Actualiza el logo del colegio
    /// </summary>
    public void UpdateLogo(string? logoUrl)
    {
        LogoUrl = logoUrl?.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Actualiza la configuración del colegio
    /// </summary>
    public void UpdateConfiguracion(string? configuracionJson)
    {
        ConfiguracionJson = configuracionJson;
        MarkAsUpdated();
    }

    /// <summary>
    /// Verifica si el colegio puede ser eliminado
    /// </summary>
    public override bool CanBeDeleted()
    {
        return _usuarios.Count == 0 && _estudiantes.Count == 0 && _profesores.Count == 0;
    }
}
