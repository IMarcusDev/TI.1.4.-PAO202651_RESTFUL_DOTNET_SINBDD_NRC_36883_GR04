namespace ec.edu.monster.modelo
{
  /// <summary>
  /// Nombres de magnitud tal como los espera el servidor RESTful, en el orden
  /// en que se muestran en las pantallas.
  /// </summary>
  public static class Magnitudes
  {
    public static readonly IReadOnlyList<string> Todas =
      ["Longitud", "Masa", "Volumen", "Tiempo", "Temperatura", "Area"];

    /// <summary>Texto que ve el usuario para un nombre de magnitud del servidor.</summary>
    public static string NombreVisible(string magnitud)
    {
      return magnitud == "Area" ? "Área" : magnitud;
    }

    public static bool Existe(string? magnitud)
    {
      return magnitud != null && Todas.Contains(magnitud);
    }
  }
}
