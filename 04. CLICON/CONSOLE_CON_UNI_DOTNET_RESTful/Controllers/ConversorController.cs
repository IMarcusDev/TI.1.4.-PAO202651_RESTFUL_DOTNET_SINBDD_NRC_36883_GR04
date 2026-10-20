using CONSOLE_CON_UNI_DOTNET_RESTful.Models.DTO;
using CONSOLE_CON_UNI_DOTNET_RESTful.Models.Enums;
using CONSOLE_CON_UNI_DOTNET_RESTful.Models.HTTP;
using CONSOLE_CON_UNI_DOTNET_SOAP.Models.Enums;
using CONSOLE_CON_UNI_DOTNET_SOAP.Models.HTTP;
using System.Text.Json;

namespace CONSOLE_CON_UNI_DOTNET_SOAP.Controllers
{
  internal class ConversorController
  {
    private readonly TypesEnum type;
    private readonly RESTfulClient client;

    public ConversorController(TypesEnum type)
    {
      this.type = type;
      this.client = new RESTfulClient();
    }

    public async Task<ConversorResponseDTO> MakeConversionAsync(
        double value,
        string from,
        string to)
    {
      var body = new
      {
        type = this.type.ToString(),
        value = value,
        from = from,
        to = to
      };

      try
      {
        string json = await this.client.PostAsync(
            EndpointsEnum.converter,
            JsonSerializer.Serialize(body),
            ConfigHTTP.token
        );

        return ConversorResponseDTO.FromJSON(json);
      }
      catch (Exception e)
      {
        return new ConversorResponseDTO(
            EstadosEnum.ERROR,
            "Error al realizar conversión: " + e.Message,
            0
        );
      }
    }
  }
}
