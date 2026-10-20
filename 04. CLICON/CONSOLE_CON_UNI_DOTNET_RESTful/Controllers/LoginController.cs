using CONSOLE_CON_UNI_DOTNET_SOAP.Models.HTTP;
using LoginServiceRef;
using System;
using System.Collections.Generic;
using System.Text;

namespace CONSOLE_CON_UNI_DOTNET_SOAP.Controllers
{
  internal class LoginController
  {
    public LoginResponseDTO? Login(string username, string password)
    {
      LoginHTTP http = new LoginHTTP();

      LoginResponseDTO? dto = http.LoginAsync(username, password).Result;

      // Save Token
      if (dto?.estado == EstadosEnum.OK)
      {
        ConfigHTTP.token = dto.token;
      }

      return dto;
    }
  }
}
