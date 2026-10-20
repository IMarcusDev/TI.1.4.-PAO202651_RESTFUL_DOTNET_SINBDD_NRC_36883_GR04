using CONSOLE_CON_UNI_DOTNET_RESTful.Models.DTO;
using CONSOLE_CON_UNI_DOTNET_RESTful.Models.Enums;
using CONSOLE_CON_UNI_DOTNET_RESTful.Models.HTTP;
using CONSOLE_CON_UNI_DOTNET_SOAP.Models.Enums;
using CONSOLE_CON_UNI_DOTNET_SOAP.Models.HTTP;

namespace CONSOLE_CON_UNI_DOTNET_SOAP.Controllers
{
  internal class CatalogController
  {
    private readonly TypesEnum type;
    private readonly RESTfulClient client;

    public CatalogController(TypesEnum type)
    {
      this.type = type;
      this.client = new RESTfulClient();
    }


    public async Task<CatalogResponseDTO> GetCatalogAsync()
    {
      try
      {
        string json = await this.client.GetAsync(
          EndpointsEnum.catalog.ToString() + "/" + this.type,
          ConfigHTTP.token
        );

        return CatalogResponseDTO.FromJSON(json);
      }
      catch (Exception e)
      {
        return new CatalogResponseDTO(
          EstadosEnum.ERROR,
          "Error al obtener catálogo: " + e.Message,
          new HashSet<string>()
        );
      }
    }
  }
}
