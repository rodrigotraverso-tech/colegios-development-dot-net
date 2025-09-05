using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.ValueObjects;

/// <summary>
/// Value Object que representa información de contacto
/// Incluye emails y teléfonos con sus respectivos tipos
/// </summary>
public sealed class ContactInfo : IEquatable<ContactInfo>
{
    /// <summary>
    /// Emails de contacto
    /// </summary>
    public IReadOnlyList<EmailContacto> Emails { get; }

    /// <summary>
    /// Teléfonos de contacto
    /// </summary>
    public IReadOnlyList<TelefonoContacto> Telefonos { get; }

    /// <summary>
    /// Constructor privado para Entity Framework
    /// </summary>
    private ContactInfo()
    {
        Emails = new List<EmailContacto>();
        Telefonos = new List<TelefonoContacto>();
    }

    /// <summary>
    /// Constructor principal
    /// </summary>
    public ContactInfo(
        IEnumerable<EmailContacto>? emails = null,
        IEnumerable<TelefonoContacto>? telefonos = null)
    {
        var emailList = emails?.ToList() ?? new List<EmailContacto>();
        var telefonoList = telefonos?.ToList() ?? new List<TelefonoContacto>();

        ValidateEmails(emailList);
        ValidateTelefonos(telefonoList);

        Emails = emailList.AsReadOnly();
        Telefonos = telefonoList.AsReadOnly();
    }

    /// <summary>
    /// Crea ContactInfo con un solo email
    /// </summary>
    public static ContactInfo WithEmail(string email, TipoEmail tipo = TipoEmail.Personal)
    {
        return new ContactInfo(emails: new[] { new EmailContacto(email, tipo) });
    }

    /// <summary>
    /// Crea ContactInfo con un solo teléfono
    /// </summary>
    public static ContactInfo WithTelefono(string telefono, TipoTelefono tipo = TipoTelefono.Movil)
    {
        return new ContactInfo(telefonos: new[] { new TelefonoContacto(telefono, tipo) });
    }

    /// <summary>
    /// Crea ContactInfo con email y teléfono
    /// </summary>
    public static ContactInfo WithEmailAndTelefono(
        string email,
        string telefono,
        TipoEmail tipoEmail = TipoEmail.Personal,
        TipoTelefono tipoTelefono = TipoTelefono.Movil)
    {
        return new ContactInfo(
            emails: new[] { new EmailContacto(email, tipoEmail) },
            telefonos: new[] { new TelefonoContacto(telefono, tipoTelefono) }
        );
    }

    /// <summary>
    /// Obtiene el email principal (primero en la lista)
    /// </summary>
    public EmailContacto? EmailPrincipal => Emails.FirstOrDefault();

    /// <summary>
    /// Obtiene el teléfono principal (primero en la lista)
    /// </summary>
    public TelefonoContacto? TelefonoPrincipal => Telefonos.FirstOrDefault();

    /// <summary>
    /// Obtiene el primer email del tipo especificado
    /// </summary>
    public EmailContacto? GetEmailByTipo(TipoEmail tipo)
    {
        return Emails.FirstOrDefault(e => e.Tipo == tipo);
    }

    /// <summary>
    /// Obtiene el primer teléfono del tipo especificado
    /// </summary>
    public TelefonoContacto? GetTelefonoByTipo(TipoTelefono tipo)
    {
        return Telefonos.FirstOrDefault(t => t.Tipo == tipo);
    }

    /// <summary>
    /// Agrega un nuevo email
    /// </summary>
    public ContactInfo AddEmail(string email, TipoEmail tipo)
    {
        var newEmail = new EmailContacto(email, tipo);
        var newEmails = new List<EmailContacto>(Emails) { newEmail };

        return new ContactInfo(newEmails, Telefonos);
    }

    /// <summary>
    /// Agrega un nuevo teléfono
    /// </summary>
    public ContactInfo AddTelefono(string telefono, TipoTelefono tipo)
    {
        var newTelefono = new TelefonoContacto(telefono, tipo);
        var newTelefonos = new List<TelefonoContacto>(Telefonos) { newTelefono };

        return new ContactInfo(Emails, newTelefonos);
    }

    /// <summary>
    /// Valida la lista de emails
    /// </summary>
    private static void ValidateEmails(List<EmailContacto> emails)
    {
        if (emails.Count > 5)
            throw new ArgumentException("No se pueden agregar más de 5 emails");

        var emailValues = emails.Select(e => e.Email.ToLowerInvariant()).ToList();
        if (emailValues.Distinct().Count() != emailValues.Count)
            throw new ArgumentException("No se pueden tener emails duplicados");
    }

    /// <summary>
    /// Valida la lista de teléfonos
    /// </summary>
    private static void ValidateTelefonos(List<TelefonoContacto> telefonos)
    {
        if (telefonos.Count > 5)
            throw new ArgumentException("No se pueden agregar más de 5 teléfonos");

        var telefonoValues = telefonos.Select(t => t.Numero).ToList();
        if (telefonoValues.Distinct().Count() != telefonoValues.Count)
            throw new ArgumentException("No se pueden tener teléfonos duplicados");
    }

    /// <summary>
    /// Representación como string
    /// </summary>
    public override string ToString()
    {
        var parts = new List<string>();

        if (EmailPrincipal != null)
            parts.Add($"Email: {EmailPrincipal.Email}");

        if (TelefonoPrincipal != null)
            parts.Add($"Tel: {TelefonoPrincipal.NumeroFormateado}");

        return string.Join(" | ", parts);
    }

    #region IEquatable Implementation

    public bool Equals(ContactInfo? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Emails.SequenceEqual(other.Emails) &&
               Telefonos.SequenceEqual(other.Telefonos);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ContactInfo);
    }

    public override int GetHashCode()
    {
        var emailsHash = Emails.Aggregate(0, (hash, email) => HashCode.Combine(hash, email.GetHashCode()));
        var telefonosHash = Telefonos.Aggregate(0, (hash, tel) => HashCode.Combine(hash, tel.GetHashCode()));

        return HashCode.Combine(emailsHash, telefonosHash);
    }

    public static bool operator ==(ContactInfo? left, ContactInfo? right)
    {
        return EqualityComparer<ContactInfo>.Default.Equals(left, right);
    }

    public static bool operator !=(ContactInfo? left, ContactInfo? right)
    {
        return !(left == right);
    }

    #endregion
}

/// <summary>
/// Representa un email con su tipo
/// </summary>
public sealed class EmailContacto : IEquatable<EmailContacto>
{
    public string Email { get; }
    public TipoEmail Tipo { get; }

    private EmailContacto()
    {
        Email = string.Empty;
    }

    public EmailContacto(string email, TipoEmail tipo)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email no puede estar vacío", nameof(email));

        var emailTrimmed = email.Trim().ToLowerInvariant();

        if (!IsValidEmail(emailTrimmed))
            throw new ArgumentException("El formato del email no es válido", nameof(email));

        Email = emailTrimmed;
        Tipo = tipo;
    }

    private static bool IsValidEmail(string email)
    {
        const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
    }

    public override string ToString() => $"{Email} ({Tipo})";

    public bool Equals(EmailContacto? other)
    {
        if (other is null) return false;
        return string.Equals(Email, other.Email, StringComparison.OrdinalIgnoreCase) && Tipo == other.Tipo;
    }

    public override bool Equals(object? obj) => Equals(obj as EmailContacto);
    public override int GetHashCode() => HashCode.Combine(Email.ToLowerInvariant(), Tipo);
}

/// <summary>
/// Representa un teléfono con su tipo
/// </summary>
public sealed class TelefonoContacto : IEquatable<TelefonoContacto>
{
    public string Numero { get; }
    public TipoTelefono Tipo { get; }

    private TelefonoContacto()
    {
        Numero = string.Empty;
    }

    public TelefonoContacto(string numero, TipoTelefono tipo)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("El número de teléfono no puede estar vacío", nameof(numero));

        var cleanNumber = CleanPhoneNumber(numero);

        if (!IsValidPhoneNumber(cleanNumber))
            throw new ArgumentException("El formato del teléfono no es válido", nameof(numero));

        Numero = cleanNumber;
        Tipo = tipo;
    }

    private static string CleanPhoneNumber(string numero)
    {
        // Remueve espacios, guiones, paréntesis y otros caracteres
        var cleaned = Regex.Replace(numero.Trim(), @"[\s\-\(\)\+]", "");

        // Si empieza con 591 (código de Bolivia), lo mantiene
        if (cleaned.StartsWith("591") && cleaned.Length > 8)
            return cleaned;

        return cleaned;
    }

    private static bool IsValidPhoneNumber(string numero)
    {
        // Acepta números de 7 a 15 dígitos (formatos internacionales)
        return Regex.IsMatch(numero, @"^\d{7,15}$");
    }

    /// <summary>
    /// Obtiene el número formateado para mostrar
    /// </summary>
    public string NumeroFormateado
    {
        get
        {
            if (Numero.Length == 8) // Formato colombiano móvil
                return $"{Numero.Substring(0, 3)} {Numero.Substring(3, 4)}";

            if (Numero.Length == 7) // Formato boliviano fijo
                return $"{Numero.Substring(0, 1)} {Numero.Substring(1, 3)} {Numero.Substring(4, 3)}";

            return Numero; // Otros formatos se muestran sin formatear
        }
    }

    public override string ToString() => $"{NumeroFormateado} ({Tipo})";

    public bool Equals(TelefonoContacto? other)
    {
        if (other is null) return false;
        return string.Equals(Numero, other.Numero) && Tipo == other.Tipo;
    }

    public override bool Equals(object? obj) => Equals(obj as TelefonoContacto);
    public override int GetHashCode() => HashCode.Combine(Numero, Tipo);
}
