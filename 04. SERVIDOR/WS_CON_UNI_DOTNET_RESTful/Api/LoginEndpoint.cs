using Microsoft.AspNetCore.Mvc;
using WS_CON_UNI_DOTNET_RESTful.Controllers;
using WS_CON_UNI_DOTNET_RESTful.Models.DTO;
using WS_CON_UNI_DOTNET_RESTful.Models.Enums;

namespace WS_CON_UNI_DOTNET_RESTful.Api
{
  [ApiController]
  [Route("login")]
  public class LoginEndpoint : ControllerBase
  {
    [HttpPost]
    public ActionResult<LoginResponseDTO> LoginUser([FromBody] LoginRequestDTO req)
    {
      string username = req.username ?? "";
      string password = req.password ?? "";

      LoginResponseDTO dto;

      try
      {
        string token = LoginController.Login(username, password);

        dto = new LoginResponseDTO
        {
          estado = EstadosEnum.OK,
          mensaje = "Ingreso de Sesión Exitoso!",
          token = token
        };

        return Ok(dto);
      }
      catch (Exception) 
      {
        dto = new LoginResponseDTO(
          EstadosEnum.ERROR,
          "Credenciales Incorrectas",
          ""
        );
      }

      return Unauthorized(dto);
    }
  }
}
