using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ColegiosBackend.Core.Enums;

namespace ColegiosBackend.Core.ValueObjects
{
    /// <summary>
    /// Value Object que representa un documento de identidad
    /// Incluye validaciones específicas según el tipo de documento
    /// </summary>
    public sealed class DocumentoIdentidad : IEquatable<DocumentoIdentidad>
    {
        /// <summary>
        /// Tipo de documento
        /// </summary>
        public TipoDocumento TipoDocumento { get; }

        /// <summary>
        /// Número del documento
        /// </summary>
        public string Numero { get; }

        /// <summary>
        /// Lugar de expedición del documento
        /// </summary>
        public string? LugarExpedicion { get; }

        /// <summary>
        /// Fecha de expedición del documento
        /// </summary>
        public DateTime? FechaExpedicion { get; }

        /// <summary>
        /// Fecha de vencimiento del documento (si aplica)
        /// </summary>
        public DateTime? FechaVencimiento { get; }

        /// <summary>
        /// Constructor privado para Entity Framework
        /// </summary>
        private DocumentoIdentidad()
        {
            Numero = string.Empty;
        }

        /// <summary>
        /// Constructor principal
        /// </summary>
        public DocumentoIdentidad(
            TipoDocumento tipoDocumento,
            string numero,
            string? lugarExpedicion = null,
            DateTime? fechaExpedicion = null,
            DateTime? fechaVencimiento = null)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new ArgumentException("El número de documento no puede estar vacío", nameof(numero));

            TipoDocumento = tipoDocumento;
            Numero = ValidateAndCleanNumber(numero, tipoDocumento);
            LugarExpedicion = string.IsNullOrWhiteSpace(lugarExpedicion) ? null : lugarExpedicion.Trim();
            FechaExpedicion = fechaExpedicion;
            FechaVencimiento = fechaVencimiento;

            ValidateDates();
        }

        /// <summary>
        /// Valida y limpia el número según el tipo de documento
        /// </summary>
        private static string ValidateAndCleanNumber(string numero, TipoDocumento tipoDocumento)
        {
            var cleanNumber = numero.Trim().Replace(".", "").Replace("-", "").Replace(" ", "");

            return tipoDocumento switch
            {
                TipoDocumento.CedulaCiudadania => ValidateCedulaCiudadania(cleanNumber),
                TipoDocumento.TarjetaIdentidad => ValidateTarjetaIdentidad(cleanNumber),
                TipoDocumento.RegistroCivil => ValidateRegistroCivil(cleanNumber),
                TipoDocumento.CedulaExtranjeria => ValidateCedulaExtranjeria(cleanNumber),
                TipoDocumento.Pasaporte => ValidatePasaporte(cleanNumber),
                TipoDocumento.DNI => ValidateDNI(cleanNumber),
                _ => ValidateGenericDocument(cleanNumber)
            };
        }

        /// <summary>
        /// Valida cédula de ciudadanía colombiana
        /// </summary>
        private static string ValidateCedulaCiudadania(string numero)
        {
            if (!Regex.IsMatch(numero, @"^\d{6,10}$"))
                throw new ArgumentException("La cédula de ciudadanía debe tener entre 6 y 10 dígitos");

            return numero;
        }

        /// <summary>
        /// Valida tarjeta de identidad
        /// </summary>
        private static string ValidateTarjetaIdentidad(string numero)
        {
            if (!Regex.IsMatch(numero, @"^\d{10,11}$"))
                throw new ArgumentException("La tarjeta de identidad debe tener entre 10 y 11 dígitos");

            return numero;
        }

        /// <summary>
        /// Valida registro civil
        /// </summary>
        private static string ValidateRegistroCivil(string numero)
        {
            if (!Regex.IsMatch(numero, @"^\d{10,15}$"))
                throw new ArgumentException("El registro civil debe tener entre 10 y 15 dígitos");

            return numero;
        }

        /// <summary>
        /// Valida cédula de extranjería
        /// </summary>
        private static string ValidateCedulaExtranjeria(string numero)
        {
            if (!Regex.IsMatch(numero, @"^\d{6,12}$"))
                throw new ArgumentException("La cédula de extranjería debe tener entre 6 y 12 dígitos");

            return numero;
        }

        /// <summary>
        /// Valida formato de pasaporte
        /// </summary>
        private static string ValidatePasaporte(string numero)
        {
            if (!Regex.IsMatch(numero, @"^[A-Z0-9]{6,15}$"))
                throw new ArgumentException("El pasaporte debe tener entre 6 y 15 caracteres alfanuméricos");

            return numero.ToUpperInvariant();
        }

        /// <summary>
        /// Valida DNI (formato general)
        /// </summary>
        private static string ValidateDNI(string numero)
        {
            if (!Regex.IsMatch(numero, @"^\d{7,12}$"))
                throw new ArgumentException("El DNI debe tener entre 7 y 12 dígitos");

            return numero;
        }

        /// <summary>
        /// Validación genérica para otros documentos
        /// </summary>
        private static string ValidateGenericDocument(string numero)
        {
            if (numero.Length < 3 || numero.Length > 20)
                throw new ArgumentException("El documento debe tener entre 3 y 20 caracteres");

            return numero.ToUpperInvariant();
        }

        /// <summary>
        /// Valida las fechas del documento
        /// </summary>
        private void ValidateDates()
        {
            if (FechaExpedicion.HasValue && FechaExpedicion.Value > DateTime.Today)
                throw new ArgumentException("La fecha de expedición no puede ser futura");

            if (FechaVencimiento.HasValue)
            {
                if (FechaVencimiento.Value <= DateTime.Today)
                    throw new ArgumentException("La fecha de vencimiento debe ser futura");

                if (FechaExpedicion.HasValue && FechaVencimiento.Value <= FechaExpedicion.Value)
                    throw new ArgumentException("La fecha de vencimiento debe ser posterior a la fecha de expedición");
            }
        }

        /// <summary>
        /// Verifica si el documento está vencido
        /// </summary>
        public bool EstaVencido => FechaVencimiento.HasValue && FechaVencimiento.Value <= DateTime.Today;

        /// <summary>
        /// Verifica si el documento vence pronto (en los próximos 30 días)
        /// </summary>
        public bool VencePronto => FechaVencimiento.HasValue &&
                                   FechaVencimiento.Value <= DateTime.Today.AddDays(30) &&
                                   FechaVencimiento.Value > DateTime.Today;

        /// <summary>
        /// Obtiene el número de documento formateado según su tipo
        /// </summary>
        public string NumeroFormateado
        {
            get
            {
                return TipoDocumento switch
                {
                    TipoDocumento.CedulaCiudadania => FormatCedula(Numero),
                    TipoDocumento.TarjetaIdentidad => FormatTarjetaIdentidad(Numero),
                    _ => Numero
                };
            }
        }

        /// <summary>
        /// Formatea cédula de ciudadanía con puntos
        /// </summary>
        private static string FormatCedula(string numero)
        {
            if (numero.Length <= 6) return numero;

            var reversed = numero.ToCharArray();
            Array.Reverse(reversed);
            var result = new List<char>();

            for (int i = 0; i < reversed.Length; i++)
            {
                if (i > 0 && i % 3 == 0)
                    result.Add('.');
                result.Add(reversed[i]);
            }

            result.Reverse();
            return new string(result.ToArray());
        }

        /// <summary>
        /// Formatea tarjeta de identidad
        /// </summary>
        private static string FormatTarjetaIdentidad(string numero)
        {
            if (numero.Length != 11) return numero;
            return $"{numero.Substring(0, 2)}.{numero.Substring(2, 3)}.{numero.Substring(5, 3)}.{numero.Substring(8, 3)}";
        }

        /// <summary>
        /// Representación como string
        /// </summary>
        public override string ToString()
        {
            return $"{TipoDocumento}: {NumeroFormateado}";
        }

        #region IEquatable Implementation

        public bool Equals(DocumentoIdentidad? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return TipoDocumento == other.TipoDocumento &&
                   string.Equals(Numero, other.Numero, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as DocumentoIdentidad);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TipoDocumento, Numero.ToUpperInvariant());
        }

        public static bool operator ==(DocumentoIdentidad? left, DocumentoIdentidad? right)
        {
            return EqualityComparer<DocumentoIdentidad>.Default.Equals(left, right);
        }

        public static bool operator !=(DocumentoIdentidad? left, DocumentoIdentidad? right)
        {
            return !(left == right);
        }

        #endregion
    }
}
