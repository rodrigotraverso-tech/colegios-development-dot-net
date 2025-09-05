using System;
using System.Collections.Generic;
using ColegiosBackend.Core.Entities.Base;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.Entities;

/// <summary>
/// Entidad que representa una persona en el sistema
/// Tabla global sin multi-tenancy (una persona puede estar en múltiples colegios)
/// </summary>
public class Persona : BaseEntity, IAuditableEntity
{
    /// <summary>
    /// Tipo de documento de identidad (FK a tipos_documento)
    /// </summary>
    public int TipoDocumentoId { get; private set; }

    /// <summary>
    /// Número del documento de identidad
    /// </summary>
    public string NumeroDocumento { get; private set; } = string.Empty;

    /// <summary>
    /// Nombres de la persona (primer y segundo nombre combinados)
    /// </summary>
    public string Nombres { get; private set; } = string.Empty;

    /// <summary>
    /// Apellidos de la persona (primer y segundo apellido combinados)
    /// </summary>
    public string Apellidos { get; private set; } = string.Empty;

    /// <summary>
    /// Segundo nombre de la persona (opcional)
    /// </summary>
    public string? SegundoNombre { get; private set; }

    /// <summary>
    /// Segundo apellido de la persona (opcional)
    /// </summary>
    public string? SegundoApellido { get; private set; }

    /// <summary>
    /// Fecha de nacimiento
    /// </summary>
    public DateTime? FechaNacimiento { get; private set; }

    /// <summary>
    /// Género de la persona (M, F, O)
    /// </summary>
    public char? Genero { get; private set; }

    /// <summary>
    /// Dirección de residencia (campo texto)
    /// </summary>
    public string? Direccion { get; private set; }

    /// <summary>
    /// Número de teléfono fijo
    /// </summary>
    public string? Telefono { get; private set; }

    /// <summary>
    /// Número de teléfono celular
    /// </summary>
    public string? Celular { get; private set; }

    /// <summary>
    /// Email de contacto
    /// </summary>
    public string? Email { get; private set; }

    /// <summary>
    /// URL de la foto de perfil
    /// </summary>
    public string? FotoUrl { get; private set; }

    /// <summary>
    /// Indica si la persona está activa en el sistema
    /// </summary>
    public bool Activo { get; private set; }

    /// <summary>
    /// Fecha de creación (campo de BD, no de BaseEntity)
    /// </summary>
    public DateTime FechaCreacion { get; private set; }

    /// <summary>
    /// Fecha de última actualización (campo de BD, no de BaseEntity)
    /// </summary>
    public DateTime FechaActualizacion { get; private set; }

    // Navegación
    /// <summary>
    /// Usuario asociado a esta persona (relación 1:1 opcional)
    /// </summary>
    public Usuario? Usuario { get; private set; }

    /// <summary>
    /// Estudiantes que son esta persona
    /// </summary>
    public ICollection<Estudiante> Estudiantes { get; private set; } = new List<Estudiante>();

    /// <summary>
    /// Relaciones como acudiente de estudiantes
    /// </summary>
    public ICollection<EstudianteAcudiente> EstudiantesAcudidos { get; private set; } = new List<EstudianteAcudiente>();

    /// <summary>
    /// Relaciones como profesor en colegios
    /// </summary>
    public ICollection<ProfesorColegio> ProfesoresColegios { get; private set; } = new List<ProfesorColegio>();

    /// <summary>
    /// Nombre completo calculado
    /// </summary>
    public string NombreCompleto
    {
        get
        {
            var nombres = string.IsNullOrWhiteSpace(SegundoNombre)
                ? Nombres
                : $"{Nombres} {SegundoNombre}";

            var apellidos = string.IsNullOrWhiteSpace(SegundoApellido)
                ? Apellidos
                : $"{Apellidos} {SegundoApellido}";

            return $"{nombres} {apellidos}".Trim();
        }
    }

    /// <summary>
    /// Apellidos y nombres (formato oficial)
    /// </summary>
    public string ApellidosNombres
    {
        get
        {
            var apellidos = string.IsNullOrWhiteSpace(SegundoApellido)
                ? Apellidos
                : $"{Apellidos} {SegundoApellido}";

            var nombres = string.IsNullOrWhiteSpace(SegundoNombre)
                ? Nombres
                : $"{Nombres} {SegundoNombre}";

            return $"{apellidos}, {nombres}";
        }
    }

    /// <summary>
    /// Edad calculada basada en la fecha de nacimiento
    /// </summary>
    public int? Edad
    {
        get
        {
            if (!FechaNacimiento.HasValue) return null;

            var today = DateTime.Today;
            var age = today.Year - FechaNacimiento.Value.Year;

            if (FechaNacimiento.Value.Date > today.AddYears(-age))
                age--;

            return age;
        }
    }

    /// <summary>
    /// Género como enum para facilitar el uso
    /// </summary>
    public Genero? GeneroEnum
    {
        get
        {
            return Genero switch
            {
                'M' => ColegiosBackend.Core.Enums.Genero.Masculino,
                'F' => ColegiosBackend.Core.Enums.Genero.Femenino,
                'O' => ColegiosBackend.Core.Enums.Genero.Otro,
                _ => null
            };
        }
    }

    /// <summary>
    /// Constructor privado para Entity Framework
    /// </summary>
    private Persona() : base() { }

    /// <summary>
    /// Constructor para crear una nueva persona
    /// </summary>
    public Persona(
        int tipoDocumentoId,
        string numeroDocumento,
        string nombres,
        string apellidos,
        string? segundoNombre = null,
        string? segundoApellido = null,
        DateTime? fechaNacimiento = null,
        char? genero = null,
        string? direccion = null,
        string? telefono = null,
        string? celular = null,
        string? email = null) : base()
    {
        SetDocumentoIdentidad(tipoDocumentoId, numeroDocumento);
        SetNombres(nombres, segundoNombre);
        SetApellidos(apellidos, segundoApellido);
        SetFechaNacimiento(fechaNacimiento);
        SetGenero(genero);
        SetDireccion(direccion);
        SetTelefono(telefono);
        SetCelular(celular);
        SetEmail(email);

        Activo = true;
        FechaCreacion = DateTime.UtcNow;
        FechaActualizacion = DateTime.UtcNow;
    }

    /// <summary>
    /// Actualiza el documento de identidad
    /// </summary>
    public void SetDocumentoIdentidad(int tipoDocumentoId, string numeroDocumento)
    {
        if (tipoDocumentoId <= 0)
            throw new ArgumentException("El tipo de documento debe ser válido", nameof(tipoDocumentoId));

        if (string.IsNullOrWhiteSpace(numeroDocumento))
            throw new ArgumentException("El número de documento es obligatorio", nameof(numeroDocumento));

        if (numeroDocumento.Length > 20)
            throw new ArgumentException("El número de documento no puede exceder 20 caracteres", nameof(numeroDocumento));

        TipoDocumentoId = tipoDocumentoId;
        NumeroDocumento = numeroDocumento.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Actualiza los nombres de la persona
    /// </summary>
    public void SetNombres(string nombres, string? segundoNombre = null)
    {
        if (string.IsNullOrWhiteSpace(nombres))
            throw new ArgumentException("Los nombres son obligatorios", nameof(nombres));

        if (nombres.Length > 100)
            throw new ArgumentException("Los nombres no pueden exceder 100 caracteres", nameof(nombres));

        if (!string.IsNullOrWhiteSpace(segundoNombre) && segundoNombre.Length > 100)
            throw new ArgumentException("El segundo nombre no puede exceder 100 caracteres", nameof(segundoNombre));

        Nombres = nombres.Trim();
        SegundoNombre = string.IsNullOrWhiteSpace(segundoNombre) ? null : segundoNombre.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Actualiza los apellidos de la persona
    /// </summary>
    public void SetApellidos(string apellidos, string? segundoApellido = null)
    {
        if (string.IsNullOrWhiteSpace(apellidos))
            throw new ArgumentException("Los apellidos son obligatorios", nameof(apellidos));

        if (apellidos.Length > 100)
            throw new ArgumentException("Los apellidos no pueden exceder 100 caracteres", nameof(apellidos));

        if (!string.IsNullOrWhiteSpace(segundoApellido) && segundoApellido.Length > 100)
            throw new ArgumentException("El segundo apellido no puede exceder 100 caracteres", nameof(segundoApellido));

        Apellidos = apellidos.Trim();
        SegundoApellido = string.IsNullOrWhiteSpace(segundoApellido) ? null : segundoApellido.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Actualiza la fecha de nacimiento
    /// </summary>
    public void SetFechaNacimiento(DateTime? fechaNacimiento)
    {
        if (fechaNacimiento.HasValue)
        {
            if (fechaNacimiento.Value > DateTime.Today)
                throw new ArgumentException("La fecha de nacimiento no puede ser futura", nameof(fechaNacimiento));

            if (fechaNacimiento.Value < DateTime.Today.AddYears(-120))
                throw new ArgumentException("La fecha de nacimiento no puede ser mayor a 120 años", nameof(fechaNacimiento));
        }

        FechaNacimiento = fechaNacimiento?.Date;
        MarkAsUpdated();
    }

    /// <summary>
    /// Actualiza el género
    /// </summary>
    public void SetGenero(char? genero)
    {
        if (genero.HasValue && genero != 'M' && genero != 'F' && genero != 'O')
            throw new ArgumentException("El género debe ser M (Masculino), F (Femenino) u O (Otro)", nameof(genero));

        Genero = genero;
        MarkAsUpdated();
    }

    /// <summary>
    /// Actualiza la dirección
    /// </summary>
    public void SetDireccion(string? direccion)
    {
        Direccion = string.IsNullOrWhiteSpace(direccion) ? null : direccion.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Actualiza el teléfono fijo
    /// </summary>
    public void SetTelefono(string? telefono)
    {
        if (!string.IsNullOrWhiteSpace(telefono) && telefono.Length > 20)
            throw new ArgumentException("El teléfono no puede exceder 20 caracteres", nameof(telefono));

        Telefono = string.IsNullOrWhiteSpace(telefono) ? null : telefono.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Actualiza el teléfono celular
    /// </summary>
    public void SetCelular(string? celular)
    {
        if (!string.IsNullOrWhiteSpace(celular) && celular.Length > 20)
            throw new ArgumentException("El celular no puede exceder 20 caracteres", nameof(celular));

        Celular = string.IsNullOrWhiteSpace(celular) ? null : celular.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Actualiza el email
    /// </summary>
    public void SetEmail(string? email)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            if (email.Length > 100)
                throw new ArgumentException("El email no puede exceder 100 caracteres", nameof(email));

            // Validación básica de email
            if (!email.Contains('@') || !email.Contains('.'))
                throw new ArgumentException("El formato del email no es válido", nameof(email));
        }

        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim().ToLowerInvariant();
        MarkAsUpdated();
    }

    /// <summary>
    /// Actualiza la URL de la foto
    /// </summary>
    public void SetFotoUrl(string? fotoUrl)
    {
        if (!string.IsNullOrWhiteSpace(fotoUrl) && fotoUrl.Length > 500)
            throw new ArgumentException("La URL de la foto no puede exceder 500 caracteres", nameof(fotoUrl));

        FotoUrl = string.IsNullOrWhiteSpace(fotoUrl) ? null : fotoUrl.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Activa la persona
    /// </summary>
    public void Activar()
    {
        if (!Activo)
        {
            Activo = true;
            MarkAsUpdated();
        }
    }

    /// <summary>
    /// Desactiva la persona
    /// </summary>
    public void Desactivar()
    {
        if (Activo)
        {
            Activo = false;
            MarkAsUpdated();
        }
    }

    /// <summary>
    /// Verifica si la persona es mayor de edad
    /// </summary>
    public bool EsMayorDeEdad => Edad >= 18;

    /// <summary>
    /// Verifica si la persona tiene usuario asociado
    /// </summary>
    public bool TieneUsuario => Usuario != null;

    /// <summary>
    /// Verifica si la persona puede ser eliminada
    /// </summary>
    public override bool CanBeDeleted()
    {
        // Una persona no puede ser eliminada si:
        // - Tiene usuario asociado
        // - Es acudiente de estudiantes activos
        // - Es profesor en colegios
        // - Es estudiante activo
        return !TieneUsuario &&
               !EstudiantesAcudidos.Any(ea => ea.EstaActiva) &&
               !ProfesoresColegios.Any(pc => pc.Activo) &&
               !Estudiantes.Any(e => e.Activo);
    }

    /// <summary>
    /// Actualiza timestamp de modificación (override para usar campo de BD)
    /// </summary>
    public override void MarkAsUpdated(Guid? updatedBy = null)
    {
        base.MarkAsUpdated(updatedBy);
        FechaActualizacion = DateTime.UtcNow;
    }
}