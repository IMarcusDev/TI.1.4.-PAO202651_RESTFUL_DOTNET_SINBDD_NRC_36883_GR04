using System.Text.Json;
using ec.edu.monster.modelo.dto;
using ec.edu.monster.modelo.enums;

namespace ec.edu.monster.modelo.http
{
  /// <summary>
  /// <see cref="IConversorApi"/> sobre HTTP: POST /converter con cuerpo JSON y
  /// GET /catalog/{type}; el token va en la cabecera Authorization.
  /// </summary>
  public class ConversorRestApi(RESTfulClient cliente) : IConversorApi
  {
    public async Task<ConversorResponseDTO> ConvertAsync(string token, string type, double value, string from, string to)
    {
      string cuerpo = JsonSerializer.Serialize(new { type, value, from, to });
      return ConversorResponseDTO.FromJSON(await cliente.PostAsync(EndpointsEnum.converter, cuerpo, token));
    }

    public async Task<CatalogResponseDTO> CatalogAsync(string token, string type)
    {
      string recurso = EndpointsEnum.catalog + "/" + Uri.EscapeDataString(type);
      return CatalogResponseDTO.FromJSON(await cliente.GetAsync(recurso, token));
    }
  }
}
