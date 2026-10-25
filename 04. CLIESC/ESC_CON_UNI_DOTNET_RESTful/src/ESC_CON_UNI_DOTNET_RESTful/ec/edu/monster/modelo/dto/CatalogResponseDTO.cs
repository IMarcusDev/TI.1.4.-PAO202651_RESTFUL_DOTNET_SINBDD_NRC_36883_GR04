using System.Text.Json;
using ec.edu.monster.modelo.enums;

namespace ec.edu.monster.modelo.dto
{
  /// <summary>
  /// Respuesta de GET /catalog/{type}: los símbolos de unidad en el orden en que el servidor los envió.
  /// </summary>
  public class CatalogResponseDTO : ResponseDTO
  {
    /// <summary>Unidades, o null cuando el servidor no envió ninguna.</summary>
    public IReadOnlyList<string>? Resultado { get; set; }

    public CatalogResponseDTO() { }

    public CatalogResponseDTO(EstadosEnum estado, string? mensaje, IReadOnlyList<string>? resultado)
      : base(estado, mensaje)
    {
      Resultado = resultado;
    }

    /// <exception cref="JsonException">Si el texto no es un objeto JSON.</exception>
    public static CatalogResponseDTO FromJSON(string texto)
    {
      using var documento = Leer(texto);
      var json = documento.RootElement;

      List<string>? unidades = null;
      if (json.TryGetProperty("resultado", out JsonElement arreglo) && arreglo.ValueKind == JsonValueKind.Array)
      {
        unidades = arreglo.EnumerateArray()
          .Where(e => e.ValueKind == JsonValueKind.String)
          .Select(e => e.GetString()!)
          .ToList();
      }

      return new CatalogResponseDTO(LeerEstado(json), LeerTexto(json, "mensaje"), unidades);
    }
  }
}
