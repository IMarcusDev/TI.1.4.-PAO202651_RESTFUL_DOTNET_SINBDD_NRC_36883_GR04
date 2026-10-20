using CONSOLE_CON_UNI_DOTNET_RESTful.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CONSOLE_CON_UNI_DOTNET_RESTful.Models.DTO
{
  internal class CatalogResponseDTO : ResponseDTO
  {
    private HashSet<string> resultado;

    public CatalogResponseDTO(
        EstadosEnum estado,
        string mensaje,
        HashSet<string> resultado)
        : base(estado, mensaje)
    {
      this.resultado = resultado;
    }

    public HashSet<string> GetResultado()
    {
      return resultado;
    }

    public void SetResultado(HashSet<string> resultado)
    {
      this.resultado = resultado;
    }

    public static new CatalogResponseDTO FromJSON(string json)
    {
      ResponseDTO response = ResponseDTO.FromJSON(json);

      using JsonDocument doc = JsonDocument.Parse(json);
      JsonElement obj = doc.RootElement;

      HashSet<string> resultado = new HashSet<string>();

      if (obj.TryGetProperty("resultado", out JsonElement array)
          && array.ValueKind == JsonValueKind.Array)
      {
        foreach (JsonElement item in array.EnumerateArray())
        {
          resultado.Add(item.GetString()!);
        }
      }

      return new CatalogResponseDTO(
          response.GetEstado(),
          response.GetMensaje(),
          resultado
      );
    }
  }
}
