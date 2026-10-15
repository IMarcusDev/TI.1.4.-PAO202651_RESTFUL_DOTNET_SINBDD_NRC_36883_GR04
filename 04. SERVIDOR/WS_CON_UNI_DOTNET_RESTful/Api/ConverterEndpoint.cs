using Microsoft.AspNetCore.Mvc;
using WS_CON_UNI_DOTNET_RESTful.Controllers;
using WS_CON_UNI_DOTNET_RESTful.Models.DTO;
using WS_CON_UNI_DOTNET_RESTful.Models.Enums;

namespace WS_CON_UNI_DOTNET_RESTful.Api
{
  [ApiController]
  [Route("converter")]
  public class ConverterEndpoint : ControllerBase
  {
    [HttpPost]
    public ActionResult<ConverterResponseDTO> Convert(
      [FromHeader(Name = "Authorization")] string token,
      [FromBody] ConvertRequestDTO req
     )
    {
      string type = req.type ?? "";
      double value = req.value;
      string from = req.from ?? "";
      string to = req.to ?? "";

      ConverterResponseDTO dto;

      if (!LoginController.ValidateUser(token))
      {
        dto = new ConverterResponseDTO(EstadosEnum.ERROR, "Token Inválido", 0);

        return Unauthorized(dto);
      }

      try
      {
        dto = new ConverterResponseDTO(
          EstadosEnum.OK,
          "Conversión Exitosa",
          ConverterController.Convert(type, value, from, to)
        );

        return Ok(dto);
      }
      catch (Exception e)
      {
        dto = new ConverterResponseDTO(
          EstadosEnum.ERROR,
          "No se pudo hacer la conversión",
          0
        );
      }

      return Unauthorized(dto);
    }
  }
}
