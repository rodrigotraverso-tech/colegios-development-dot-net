using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ColegiosBackend.Core.ValueObjects.Colegio;

/// <summary>
/// Configuración de colores y tema visual del colegio
/// Almacenada en campo JSONB colores_tema de la tabla colegios
/// </summary>
public class ColoresTema
{
    #region Colores Principales

    /// <summary>
    /// Color primario principal del colegio (usado en headers, botones principales)
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "El color debe estar en formato hexadecimal #RRGGBB")]
    public string Primario { get; set; } = "#1565C0";

    /// <summary>
    /// Color secundario complementario (usado en acentos y elementos secundarios)
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "El color debe estar en formato hexadecimal #RRGGBB")]
    public string Secundario { get; set; } = "#FFA726";

    /// <summary>
    /// Color de acento para destacar elementos importantes
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "El color debe estar en formato hexadecimal #RRGGBB")]
    public string Acento { get; set; } = "#43A047";

    /// <summary>
    /// Color de fondo principal de la aplicación
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "El color debe estar en formato hexadecimal #RRGGBB")]
    public string Fondo { get; set; } = "#F5F5F5";

    #endregion

    #region Colores de Texto

    /// <summary>
    /// Color principal para textos (títulos, texto importante)
    /// </summary>
    [JsonPropertyName("texto_primario")]
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "El color debe estar en formato hexadecimal #RRGGBB")]
    public string TextoPrimario { get; set; } = "#212121";

    /// <summary>
    /// Color secundario para textos (subtítulos, texto secundario)
    /// </summary>
    [JsonPropertyName("texto_secundario")]
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "El color debe estar en formato hexadecimal #RRGGBB")]
    public string TextoSecundario { get; set; } = "#757575";

    /// <summary>
    /// Color para texto sobre fondos oscuros
    /// </summary>
    [JsonPropertyName("texto_invertido")]
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "El color debe estar en formato hexadecimal #RRGGBB")]
    public string TextoInvertido { get; set; } = "#FFFFFF";

    #endregion

    #region Colores de Estado

    /// <summary>
    /// Color para indicar errores y estados negativos
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "El color debe estar en formato hexadecimal #RRGGBB")]
    public string Error { get; set; } = "#D32F2F";

    /// <summary>
    /// Color para advertencias y alertas
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "El color debe estar en formato hexadecimal #RRGGBB")]
    public string Advertencia { get; set; } = "#F57C00";

    /// <summary>
    /// Color para indicar éxito y estados positivos
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "El color debe estar en formato hexadecimal #RRGGBB")]
    public string Exito { get; set; } = "#388E3C";

    /// <summary>
    /// Color para información neutral
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "El color debe estar en formato hexadecimal #RRGGBB")]
    public string Informacion { get; set; } = "#1976D2";

    #endregion

    #region Colores de Superficie

    /// <summary>
    /// Color de superficie principal (cards, modals, etc.)
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "El color debe estar en formato hexadecimal #RRGGBB")]
    public string Superficie { get; set; } = "#FFFFFF";

    /// <summary>
    /// Color de superficie variante (backgrounds alternativos)
    /// </summary>
    [JsonPropertyName("superficie_variante")]
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "El color debe estar en formato hexadecimal #RRGGBB")]
    public string SuperficieVariante { get; set; } = "#F8F9FA";

    /// <summary>
    /// Color de superficie elevada (dropdowns, tooltips)
    /// </summary>
    [JsonPropertyName("superficie_elevada")]
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "El color debe estar en formato hexadecimal #RRGGBB")]
    public string SuperficieElevada { get; set; } = "#FFFFFF";

    #endregion

    #region Colores de Interacción

    /// <summary>
    /// Color para bordes y divisores
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "El color debe estar en formato hexadecimal #RRGGBB")]
    public string Borde { get; set; } = "#E0E0E0";

    /// <summary>
    /// Color de sombra (puede incluir transparencia)
    /// </summary>
    [MaxLength(30)]
    public string Sombra { get; set; } = "rgba(0, 0, 0, 0.12)";

    /// <summary>
    /// Color de estado hover
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "El color debe estar en formato hexadecimal #RRGGBB")]
    public string Hover { get; set; } = "#E3F2FD";

    /// <summary>
    /// Color de estado activo/seleccionado
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "El color debe estar en formato hexadecimal #RRGGBB")]
    public string Activo { get; set; } = "#BBDEFB";

    /// <summary>
    /// Color para elementos deshabilitados
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "El color debe estar en formato hexadecimal #RRGGBB")]
    public string Deshabilitado { get; set; } = "#BDBDBD";

    /// <summary>
    /// Color para enfoque/focus
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "El color debe estar en formato hexadecimal #RRGGBB")]
    public string Enfoque { get; set; } = "#2196F3";

    #endregion

    #region Colores de Enlaces

    /// <summary>
    /// Color para enlaces normales
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "El color debe estar en formato hexadecimal #RRGGBB")]
    public string Link { get; set; } = "#1976D2";

    /// <summary>
    /// Color para enlaces visitados
    /// </summary>
    [JsonPropertyName("link_visitado")]
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "El color debe estar en formato hexadecimal #RRGGBB")]
    public string LinkVisitado { get; set; } = "#7B1FA2";

    /// <summary>
    /// Color para enlaces en estado hover
    /// </summary>
    [JsonPropertyName("link_hover")]
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$", ErrorMessage = "El color debe estar en formato hexadecimal #RRGGBB")]
    public string LinkHover { get; set; } = "#1565C0";

    #endregion

    #region Colores Académicos Específicos

    /// <summary>
    /// Configuración de colores para diferentes rangos de calificaciones
    /// </summary>
    [JsonPropertyName("calificaciones")]
    public ColoresCalificaciones Calificaciones { get; set; } = new();

    /// <summary>
    /// Configuración de colores para estados de asistencia
    /// </summary>
    [JsonPropertyName("asistencia")]
    public ColoresAsistencia Asistencia { get; set; } = new();

    /// <summary>
    /// Configuración de colores para estados financieros
    /// </summary>
    [JsonPropertyName("financiero")]
    public ColoresFinanciero Financiero { get; set; } = new();

    #endregion

    #region Configuración Adicional

    /// <summary>
    /// Configuración de opacidades para diferentes elementos
    /// </summary>
    public OpacidadesConfig Opacidades { get; set; } = new();

    /// <summary>
    /// Configuración de gradientes
    /// </summary>
    public GradientesConfig Gradientes { get; set; } = new();

    #endregion
}

#region Clases de Configuración Específica

/// <summary>
/// Colores específicos para rangos de calificaciones
/// </summary>
public class ColoresCalificaciones
{
    /// <summary>
    /// Color para calificaciones superiores (4.6-5.0)
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$")]
    public string Superior { get; set; } = "#4CAF50"; // Verde

    /// <summary>
    /// Color para calificaciones altas (4.0-4.5)
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$")]
    public string Alto { get; set; } = "#8BC34A"; // Verde claro

    /// <summary>
    /// Color para calificaciones básicas (3.0-3.9)
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$")]
    public string Basico { get; set; } = "#FF9800"; // Naranja

    /// <summary>
    /// Color para calificaciones bajas (1.0-2.9)
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$")]
    public string Bajo { get; set; } = "#F44336"; // Rojo

    /// <summary>
    /// Color para calificaciones pendientes
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$")]
    public string Pendiente { get; set; } = "#9E9E9E"; // Gris
}

/// <summary>
/// Colores específicos para estados de asistencia
/// </summary>
public class ColoresAsistencia
{
    /// <summary>
    /// Color para presente
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$")]
    public string Presente { get; set; } = "#4CAF50"; // Verde

    /// <summary>
    /// Color para ausente
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$")]
    public string Ausente { get; set; } = "#F44336"; // Rojo

    /// <summary>
    /// Color para llegada tarde
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$")]
    public string Tarde { get; set; } = "#FF9800"; // Naranja

    /// <summary>
    /// Color para excusado
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$")]
    public string Excusado { get; set; } = "#2196F3"; // Azul

    /// <summary>
    /// Color para permiso
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$")]
    public string Permiso { get; set; } = "#9C27B0"; // Púrpura
}

/// <summary>
/// Colores específicos para estados financieros
/// </summary>
public class ColoresFinanciero
{
    /// <summary>
    /// Color para facturas pagadas
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$")]
    public string Pagado { get; set; } = "#4CAF50"; // Verde

    /// <summary>
    /// Color para facturas pendientes
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$")]
    public string Pendiente { get; set; } = "#FF9800"; // Naranja

    /// <summary>
    /// Color para facturas vencidas
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$")]
    public string Vencido { get; set; } = "#F44336"; // Rojo

    /// <summary>
    /// Color para facturas anuladas
    /// </summary>
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$")]
    public string Anulado { get; set; } = "#9E9E9E"; // Gris

    /// <summary>
    /// Color para saldo a favor
    /// </summary>
    [JsonPropertyName("saldo_favor")]
    [MaxLength(7)]
    [RegularExpression(@"^#([A-Fa-f0-9]{6})$")]
    public string SaldoFavor { get; set; } = "#3F51B5"; // Azul índigo
}

/// <summary>
/// Configuración de opacidades para diferentes estados
/// </summary>
public class OpacidadesConfig
{
    /// <summary>
    /// Opacidad para elementos deshabilitados (0.0 - 1.0)
    /// </summary>
    [Range(0.0, 1.0)]
    public decimal Deshabilitado { get; set; } = 0.6m;

    /// <summary>
    /// Opacidad para hover (0.0 - 1.0)
    /// </summary>
    [Range(0.0, 1.0)]
    public decimal Hover { get; set; } = 0.8m;

    /// <summary>
    /// Opacidad para modal backdrop (0.0 - 1.0)
    /// </summary>
    [Range(0.0, 1.0)]
    public decimal Backdrop { get; set; } = 0.5m;

    /// <summary>
    /// Opacidad para tooltips (0.0 - 1.0)
    /// </summary>
    [Range(0.0, 1.0)]
    public decimal Tooltip { get; set; } = 0.9m;
}

/// <summary>
/// Configuración de gradientes
/// </summary>
public class GradientesConfig
{
    /// <summary>
    /// Gradiente principal del tema
    /// </summary>
    [JsonPropertyName("principal")]
    [MaxLength(200)]
    public string Principal { get; set; } = "linear-gradient(135deg, #1565C0 0%, #42A5F5 100%)";

    /// <summary>
    /// Gradiente secundario
    /// </summary>
    [JsonPropertyName("secundario")]
    [MaxLength(200)]
    public string Secundario { get; set; } = "linear-gradient(135deg, #FFA726 0%, #FFB74D 100%)";

    /// <summary>
    /// Gradiente para headers
    /// </summary>
    [JsonPropertyName("header")]
    [MaxLength(200)]
    public string Header { get; set; } = "linear-gradient(90deg, #1565C0 0%, #1976D2 100%)";

    /// <summary>
    /// Gradiente para botones de acción
    /// </summary>
    [JsonPropertyName("boton_accion")]
    [MaxLength(200)]
    public string BotonAccion { get; set; } = "linear-gradient(135deg, #43A047 0%, #66BB6A 100%)";
}

#endregion

#region Métodos de Extensión y Utilidades

/// <summary>
/// Métodos de extensión para trabajar con ColoresTema
/// </summary>
public static class ColoresTemaExtensions
{
    /// <summary>
    /// Valida que todos los colores estén en formato hexadecimal válido
    /// </summary>
    public static bool EsValido(this ColoresTema colores)
    {
        var propiedadesColor = typeof(ColoresTema).GetProperties()
            .Where(p => p.PropertyType == typeof(string) &&
                       p.GetCustomAttributes(typeof(RegularExpressionAttribute), false).Any())
            .ToList();

        foreach (var propiedad in propiedadesColor)
        {
            var valor = propiedad.GetValue(colores) as string;
            if (!string.IsNullOrEmpty(valor) && !EsColorHexValido(valor))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Genera variables CSS personalizadas basadas en los colores del tema
    /// </summary>
    public static Dictionary<string, string> GenerarVariablesCSS(this ColoresTema colores)
    {
        return new Dictionary<string, string>
        {
            ["--color-primario"] = colores.Primario,
            ["--color-secundario"] = colores.Secundario,
            ["--color-acento"] = colores.Acento,
            ["--color-fondo"] = colores.Fondo,
            ["--color-texto-primario"] = colores.TextoPrimario,
            ["--color-texto-secundario"] = colores.TextoSecundario,
            ["--color-texto-invertido"] = colores.TextoInvertido,
            ["--color-error"] = colores.Error,
            ["--color-advertencia"] = colores.Advertencia,
            ["--color-exito"] = colores.Exito,
            ["--color-informacion"] = colores.Informacion,
            ["--color-superficie"] = colores.Superficie,
            ["--color-superficie-variante"] = colores.SuperficieVariante,
            ["--color-superficie-elevada"] = colores.SuperficieElevada,
            ["--color-borde"] = colores.Borde,
            ["--color-sombra"] = colores.Sombra,
            ["--color-hover"] = colores.Hover,
            ["--color-activo"] = colores.Activo,
            ["--color-deshabilitado"] = colores.Deshabilitado,
            ["--color-enfoque"] = colores.Enfoque,
            ["--color-link"] = colores.Link,
            ["--color-link-visitado"] = colores.LinkVisitado,
            ["--color-link-hover"] = colores.LinkHover,

            // Calificaciones
            ["--color-calif-superior"] = colores.Calificaciones.Superior,
            ["--color-calif-alto"] = colores.Calificaciones.Alto,
            ["--color-calif-basico"] = colores.Calificaciones.Basico,
            ["--color-calif-bajo"] = colores.Calificaciones.Bajo,
            ["--color-calif-pendiente"] = colores.Calificaciones.Pendiente,

            // Asistencia
            ["--color-asist-presente"] = colores.Asistencia.Presente,
            ["--color-asist-ausente"] = colores.Asistencia.Ausente,
            ["--color-asist-tarde"] = colores.Asistencia.Tarde,
            ["--color-asist-excusado"] = colores.Asistencia.Excusado,
            ["--color-asist-permiso"] = colores.Asistencia.Permiso,

            // Financiero
            ["--color-fin-pagado"] = colores.Financiero.Pagado,
            ["--color-fin-pendiente"] = colores.Financiero.Pendiente,
            ["--color-fin-vencido"] = colores.Financiero.Vencido,
            ["--color-fin-anulado"] = colores.Financiero.Anulado,
            ["--color-fin-saldo-favor"] = colores.Financiero.SaldoFavor,

            // Gradientes
            ["--gradiente-principal"] = colores.Gradientes.Principal,
            ["--gradiente-secundario"] = colores.Gradientes.Secundario,
            ["--gradiente-header"] = colores.Gradientes.Header,
            ["--gradiente-boton-accion"] = colores.Gradientes.BotonAccion,

            // Opacidades
            ["--opacidad-deshabilitado"] = colores.Opacidades.Deshabilitado.ToString(),
            ["--opacidad-hover"] = colores.Opacidades.Hover.ToString(),
            ["--opacidad-backdrop"] = colores.Opacidades.Backdrop.ToString(),
            ["--opacidad-tooltip"] = colores.Opacidades.Tooltip.ToString()
        };
    }

    /// <summary>
    /// Obtiene el color apropiado para un rango de calificación
    /// </summary>
    public static string ObtenerColorCalificacion(this ColoresTema colores, decimal calificacion,
        decimal escalaMinima = 1.0m, decimal escalaMaxima = 5.0m)
    {
        if (calificacion < 0) return colores.Calificaciones.Pendiente;

        // Convertir a porcentaje basado en la escala
        var porcentaje = (calificacion - escalaMinima) / (escalaMaxima - escalaMinima) * 100;

        return porcentaje switch
        {
            >= 92 => colores.Calificaciones.Superior, // 4.6-5.0 en escala 1-5
            >= 80 => colores.Calificaciones.Alto,      // 4.0-4.5 en escala 1-5
            >= 60 => colores.Calificaciones.Basico,    // 3.0-3.9 en escala 1-5
            _ => colores.Calificaciones.Bajo           // 1.0-2.9 en escala 1-5
        };
    }

    /// <summary>
    /// Obtiene el color apropiado para un estado de asistencia
    /// </summary>
    public static string ObtenerColorAsistencia(this ColoresTema colores, string estado)
    {
        return estado.ToUpperInvariant() switch
        {
            "PRESENTE" => colores.Asistencia.Presente,
            "AUSENTE" => colores.Asistencia.Ausente,
            "TARDE" => colores.Asistencia.Tarde,
            "EXCUSADO" => colores.Asistencia.Excusado,
            "PERMISO" => colores.Asistencia.Permiso,
            _ => colores.TextoSecundario
        };
    }

    /// <summary>
    /// Obtiene el color apropiado para un estado financiero
    /// </summary>
    public static string ObtenerColorFinanciero(this ColoresTema colores, string estado)
    {
        return estado.ToUpperInvariant() switch
        {
            "PAGADO" or "PAGADA" => colores.Financiero.Pagado,
            "PENDIENTE" => colores.Financiero.Pendiente,
            "VENCIDO" or "VENCIDA" => colores.Financiero.Vencido,
            "ANULADO" or "ANULADA" => colores.Financiero.Anulado,
            "SALDO_FAVOR" => colores.Financiero.SaldoFavor,
            _ => colores.TextoSecundario
        };
    }

    /// <summary>
    /// Genera una paleta de colores complementarios basada en el color primario
    /// </summary>
    public static List<string> GenerarPaletaComplementaria(this ColoresTema colores, int cantidad = 5)
    {
        var paleta = new List<string> { colores.Primario };

        // Aquí se implementaría lógica para generar colores complementarios
        // Por simplicidad, devolvemos colores predefinidos
        paleta.AddRange(new[] {
            colores.Secundario,
            colores.Acento,
            colores.Informacion,
            colores.Exito
        }.Take(cantidad - 1));

        return paleta;
    }

    /// <summary>
    /// Convierte un color hexadecimal a RGB
    /// </summary>
    public static (int R, int G, int B) HexAtoRgb(string hex)
    {
        if (!EsColorHexValido(hex))
            throw new ArgumentException("Color hexadecimal inválido", nameof(hex));

        var color = hex.TrimStart('#');
        return (
            Convert.ToInt32(color[..2], 16),
            Convert.ToInt32(color.Substring(2, 2), 16),
            Convert.ToInt32(color.Substring(4, 2), 16)
        );
    }

    /// <summary>
    /// Calcula el contraste entre dos colores
    /// </summary>
    public static double CalcularContraste(string color1, string color2)
    {
        try
        {
            var (r1, g1, b1) = HexAtoRgb(color1);
            var (r2, g2, b2) = HexAtoRgb(color2);

            var l1 = CalcularLuminancia(r1, g1, b1);
            var l2 = CalcularLuminancia(r2, g2, b2);

            var lighter = Math.Max(l1, l2);
            var darker = Math.Min(l1, l2);

            return (lighter + 0.05) / (darker + 0.05);
        }
        catch
        {
            return 1.0; // Contraste neutral si hay error
        }
    }

    private static bool EsColorHexValido(string color)
    {
        if (string.IsNullOrWhiteSpace(color))
            return false;

        return Regex.IsMatch(color, @"^#([A-Fa-f0-9]{6})$");
    }

    private static double CalcularLuminancia(int r, int g, int b)
    {
        var rs = r / 255.0;
        var gs = g / 255.0;
        var bs = b / 255.0;

        rs = rs <= 0.03928 ? rs / 12.92 : Math.Pow((rs + 0.055) / 1.055, 2.4);
        gs = gs <= 0.03928 ? gs / 12.92 : Math.Pow((gs + 0.055) / 1.055, 2.4);
        bs = bs <= 0.03928 ? bs / 12.92 : Math.Pow((bs + 0.055) / 1.055, 2.4);

        return 0.2126 * rs + 0.7152 * gs + 0.0722 * bs;
    }
}

#endregion