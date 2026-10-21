using System.Globalization;
using System.Text.RegularExpressions;
using ec.edu.monster.modelo;

namespace ec.edu.monster.controlador
{
  /// <summary>
  /// Validación del formulario de conversión. Mismas reglas y mensajes que los
  /// demás clientes.
  /// </summary>
  public static partial class ValidadorConversion
  {
    public const string ValorInvalido = "Ingrese un valor numérico válido";
    public const string SinMagnitud = "Seleccione una magnitud";
    public const string SinUnidades = "Seleccione la unidad de origen y de destino";

    // Signo opcional, dígitos y como máximo un separador decimal ("." o ","). Sin exponentes.
    [GeneratedRegex(@"^[+-]?(\d+([.,]\d+)?|[.,]\d+)$")]
    private static partial Regex Numero();

    /// <summary>Acepta decimales simples como "2.5" o "2,5"; devuelve null en otro caso.</summary>
    public static double? LeerValor(string? texto)
    {
      if (texto == null || !Numero().IsMatch(texto.Trim()))
        return null;

      double valor = double.Parse(texto.Trim().Replace(',', '.'), CultureInfo.InvariantCulture);
      return double.IsFinite(valor) ? valor : null;
    }

    /// <summary>Mensaje del primer campo inválido, o null si la petición está completa.</summary>
    public static string? Validar(string? valor, string? magnitud, string? desde, string? hacia)
    {
      if (LeerValor(valor) == null)
        return ValorInvalido;
      if (!Magnitudes.Existe(magnitud))
        return SinMagnitud;
      if (string.IsNullOrWhiteSpace(desde) || string.IsNullOrWhiteSpace(hacia))
        return SinUnidades;

      return null;
    }
  }
}
