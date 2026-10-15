using Microsoft.AspNetCore.Mvc;
using WS_CON_UNI_DOTNET_RESTful.Controllers;
using WS_CON_UNI_DOTNET_RESTful.Models.DTO;
using WS_CON_UNI_DOTNET_RESTful.Models.Enums;

namespace WS_CON_UNI_DOTNET_RESTful.Api
{
  [ApiController]
  [Route("catalog")]
  public class CatalogEndpoint : ControllerBase
  {
    private static string FormatType(string? text)
    {
      if (string.IsNullOrEmpty(text))
      {
        return text ?? "";
      }

      return char.ToUpper(text[0]) +
             text.Substring(1).ToLower();
    }

    [HttpGet("{type}")]
    public ActionResult<CatalogResponseDTO> GetCatalog(
      [FromRoute] string type,
      [FromHeader(Name = "Authorization")] string? token)
    {
      type = FormatType(type);

      CatalogResponseDTO dto;

      // Validar el token
      if (!LoginController.ValidateUser(token ?? ""))
      {
        dto = new CatalogResponseDTO(
          EstadosEnum.ERROR,
          "Token Inválido",
          new HashSet<string>()
        );

        return Unauthorized(dto);
      }

      try
      {
        var units = ConverterController
          .GetConverter(type)
          .GetUnits();

        dto = new CatalogResponseDTO(
          EstadosEnum.OK,
          "Obtención de Catálogo Exitosa",
          units.ToHashSet()
        );

        return Ok(dto);
      }
      catch (Exception)
      {
        dto = new CatalogResponseDTO(
          EstadosEnum.ERROR,
          "Tipo de Catálogo Inexistente",
          new HashSet<string>()
        );

        return BadRequest(dto);
      }
    }
  }
}
