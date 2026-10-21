using ec.edu.monster.modelo;

namespace ec.edu.monster.controlador
{
  /// <summary>
  /// Un valor que se envía al servidor junto con el texto que ve el usuario.
  /// </summary>
  public record Opcion(string Valor, string Texto)
  {
    /// <summary>Magnitudes del servidor con su texto visible.</summary>
    public static IReadOnlyList<Opcion> Magnitudes()
    {
      return modelo.Magnitudes.Todas
        .Select(m => new Opcion(m, modelo.Magnitudes.NombreVisible(m)))
        .ToList();
    }

    /// <summary>Símbolos de unidad con su nombre en español.</summary>
    public static IReadOnlyList<Opcion> Unidades(IEnumerable<string> simbolos)
    {
      return simbolos
        .Select(s => new Opcion(s, NombresUnidades.Nombre(s)))
        .ToList();
    }
  }
}
