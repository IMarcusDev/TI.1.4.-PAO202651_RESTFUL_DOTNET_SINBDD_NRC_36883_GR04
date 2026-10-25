using System.Globalization;

namespace ec.edu.monster.modelo
{
  /// <summary>
  /// Texto del resultado de una conversión, con coma decimal: "2,5 m = 8,2021 ft".
  /// </summary>
  public static class FormatoResultado
  {
    private static readonly NumberFormatInfo Simbolos = new()
    {
      NumberDecimalSeparator = ",",
      NegativeSign = "-",
    };

    public static string Formatear(double valor, string desde, double resultado, string hacia)
    {
      return valor.ToString("0.####", Simbolos) + " " + desde + " = "
        + resultado.ToString("0.0000", Simbolos) + " " + hacia;
    }
  }
}
