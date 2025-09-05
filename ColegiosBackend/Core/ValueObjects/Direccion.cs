using System;
using System.Collections.Generic;
using System.Text;

namespace ColegiosBackend.Core.ValueObjects;

/// <summary>
/// Value Object que representa una dirección
/// Diseñado para manejar direcciones en formato boliviano/latinoamericano
/// </summary>
public sealed class Direccion : IEquatable<Direccion>
{
    /// <summary>
    /// Dirección principal (calle, carrera, etc.)
    /// </summary>
    public string DireccionPrincipal { get; }

    /// <summary>
    /// Complemento de la dirección (apartamento, oficina, etc.)
    /// </summary>
    public string? Complemento { get; }

    /// <summary>
    /// Barrio o sector
    /// </summary>
    public string? Barrio { get; }

    /// <summary>
    /// Ciudad
    /// </summary>
    public string Ciudad { get; }

    /// <summary>
    /// Departamento o estado
    /// </summary>
    public string? Departamento { get; }

    /// <summary>
    /// País
    /// </summary>
    public string Pais { get; }

    /// <summary>
    /// Código postal
    /// </summary>
    public string? CodigoPostal { get; }

    /// <summary>
    /// Coordenadas geográficas (opcional)
    /// </summary>
    public Coordenadas? Coordenadas { get; }

    /// <summary>
    /// Referencias adicionales para ubicar la dirección
    /// </summary>
    public string? Referencias { get; }

    /// <summary>
    /// Constructor privado para Entity Framework
    /// </summary>
    private Direccion()
    {
        DireccionPrincipal = string.Empty;
        Ciudad = string.Empty;
        Pais = string.Empty;
    }

    /// <summary>
    /// Constructor principal
    /// </summary>
    public Direccion(
        string direccionPrincipal,
        string ciudad,
        string pais,
        string? complemento = null,
        string? barrio = null,
        string? departamento = null,
        string? codigoPostal = null,
        Coordenadas? coordenadas = null,
        string? referencias = null)
    {
        if (string.IsNullOrWhiteSpace(direccionPrincipal))
            throw new ArgumentException("La dirección principal es obligatoria", nameof(direccionPrincipal));

        if (string.IsNullOrWhiteSpace(ciudad))
            throw new ArgumentException("La ciudad es obligatoria", nameof(ciudad));

        if (string.IsNullOrWhiteSpace(pais))
            throw new ArgumentException("El país es obligatorio", nameof(pais));

        DireccionPrincipal = ValidateAndTrim(direccionPrincipal, 100, "dirección principal");
        Ciudad = ValidateAndTrim(ciudad, 50, "ciudad");
        Pais = ValidateAndTrim(pais, 50, "país");
        Complemento = ValidateAndTrimOptional(complemento, 50, "complemento");
        Barrio = ValidateAndTrimOptional(barrio, 50, "barrio");
        Departamento = ValidateAndTrimOptional(departamento, 50, "departamento");
        CodigoPostal = ValidateAndTrimOptional(codigoPostal, 10, "código postal");
        Referencias = ValidateAndTrimOptional(referencias, 200, "referencias");
        Coordenadas = coordenadas;
    }

    /// <summary>
    /// Constructor específico para Bolivia
    /// </summary>
    public static Direccion Bolivia(
        string direccionPrincipal,
        string ciudad,
        string departamento,
        string? complemento = null,
        string? barrio = null,
        string? codigoPostal = null,
        string? referencias = null)
    {
        return new Direccion(
            direccionPrincipal,
            ciudad,
            "Bolivia",
            complemento,
            barrio,
            departamento,
            codigoPostal,
            null,
            referencias);
    }

    /// <summary>
    /// Dirección completa formateada
    /// </summary>
    public string DireccionCompleta
    {
        get
        {
            var sb = new StringBuilder();
            sb.Append(DireccionPrincipal);

            if (!string.IsNullOrWhiteSpace(Complemento))
                sb.Append($", {Complemento}");

            if (!string.IsNullOrWhiteSpace(Barrio))
                sb.Append($", {Barrio}");

            sb.Append($", {Ciudad}");

            if (!string.IsNullOrWhiteSpace(Departamento))
                sb.Append($", {Departamento}");

            if (!string.IsNullOrWhiteSpace(CodigoPostal))
                sb.Append($" {CodigoPostal}");

            sb.Append($", {Pais}");

            return sb.ToString();
        }
    }

    /// <summary>
    /// Dirección formateada para mostrar (sin país si es Colombia)
    /// </summary>
    public string DireccionDisplay
    {
        get
        {
            var sb = new StringBuilder();
            sb.Append(DireccionPrincipal);

            if (!string.IsNullOrWhiteSpace(Complemento))
                sb.Append($", {Complemento}");

            if (!string.IsNullOrWhiteSpace(Barrio))
                sb.Append($", {Barrio}");

            sb.Append($", {Ciudad}");

            if (!string.IsNullOrWhiteSpace(Departamento))
                sb.Append($", {Departamento}");

            // Solo incluir país si no es Bolivia
            if (!string.Equals(Pais, "Bolivia", StringComparison.OrdinalIgnoreCase))
                sb.Append($", {Pais}");

            return sb.ToString();
        }
    }

    /// <summary>
    /// Valida y recorta un campo obligatorio
    /// </summary>
    private static string ValidateAndTrim(string value, int maxLength, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"El campo {fieldName} es obligatorio");

        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
            throw new ArgumentException($"El campo {fieldName} no puede exceder {maxLength} caracteres");

        return trimmed;
    }

    /// <summary>
    /// Valida y recorta un campo opcional
    /// </summary>
    private static string? ValidateAndTrimOptional(string? value, int maxLength, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
            throw new ArgumentException($"El campo {fieldName} no puede exceder {maxLength} caracteres");

        return trimmed;
    }

    /// <summary>
    /// Crea una nueva dirección con referencias actualizadas
    /// </summary>
    public Direccion WithReferencias(string? referencias)
    {
        return new Direccion(
            DireccionPrincipal,
            Ciudad,
            Pais,
            Complemento,
            Barrio,
            Departamento,
            CodigoPostal,
            Coordenadas,
            referencias);
    }

    /// <summary>
    /// Crea una nueva dirección con coordenadas
    /// </summary>
    public Direccion WithCoordenadas(Coordenadas coordenadas)
    {
        return new Direccion(
            DireccionPrincipal,
            Ciudad,
            Pais,
            Complemento,
            Barrio,
            Departamento,
            CodigoPostal,
            coordenadas,
            Referencias);
    }

    public override string ToString() => DireccionDisplay;

    #region IEquatable Implementation

    public bool Equals(Direccion? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return string.Equals(DireccionPrincipal, other.DireccionPrincipal, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(Complemento, other.Complemento, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(Barrio, other.Barrio, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(Ciudad, other.Ciudad, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(Departamento, other.Departamento, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(Pais, other.Pais, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(CodigoPostal, other.CodigoPostal, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => Equals(obj as Direccion);

    public override int GetHashCode()
    {
        return HashCode.Combine(
            DireccionPrincipal?.ToLowerInvariant(),
            Complemento?.ToLowerInvariant(),
            Barrio?.ToLowerInvariant(),
            Ciudad?.ToLowerInvariant(),
            Departamento?.ToLowerInvariant(),
            Pais?.ToLowerInvariant(),
            CodigoPostal?.ToLowerInvariant());
    }

    public static bool operator ==(Direccion? left, Direccion? right)
    {
        return EqualityComparer<Direccion>.Default.Equals(left, right);
    }

    public static bool operator !=(Direccion? left, Direccion? right)
    {
        return !(left == right);
    }

    #endregion
}

/// <summary>
/// Representa coordenadas geográficas
/// </summary>
public sealed class Coordenadas : IEquatable<Coordenadas>
{
    public decimal Latitud { get; }
    public decimal Longitud { get; }

    private Coordenadas()
    {
    }

    public Coordenadas(decimal latitud, decimal longitud)
    {
        if (latitud < -90 || latitud > 90)
            throw new ArgumentException("La latitud debe estar entre -90 y 90 grados", nameof(latitud));

        if (longitud < -180 || longitud > 180)
            throw new ArgumentException("La longitud debe estar entre -180 y 180 grados", nameof(longitud));

        Latitud = latitud;
        Longitud = longitud;
    }

    public override string ToString() => $"{Latitud}, {Longitud}";

    public bool Equals(Coordenadas? other)
    {
        if (other is null) return false;
        return Latitud == other.Latitud && Longitud == other.Longitud;
    }

    public override bool Equals(object? obj) => Equals(obj as Coordenadas);
    public override int GetHashCode() => HashCode.Combine(Latitud, Longitud);
}