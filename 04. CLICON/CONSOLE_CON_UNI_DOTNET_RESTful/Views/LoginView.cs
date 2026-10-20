using CONSOLE_CON_UNI_DOTNET_SOAP.Controllers;
using CONSOLE_CON_UNI_DOTNET_SOAP.Models.HTTP;
using CONSOLE_CON_UNI_DOTNET_SOAP.Views.Utils;
using LoginServiceRef;
using System;
using System.Collections.Generic;
using System.Text;

namespace CONSOLE_CON_UNI_DOTNET_SOAP.Views
{
  internal class LoginView
  {
    public static bool UserHasLogin()
    {
      var ctrl = new LoginController();

      // Username
      MessagesUtils.PrintMessage("Username: ");
      string username = Console.ReadLine() ?? string.Empty;

      // Password
      MessagesUtils.PrintMessage("Password: ");
      string password = Console.ReadLine() ?? string.Empty;

      LoginResponseDTO? res = ctrl.Login(username, password);

      if (res is null)
      {
        MessagesUtils.PrintMessage("No se recibió respuesta del servicio.");
        return false;
      }

      MessagesUtils.PrintMessage(res.mensaje ?? string.Empty);
      Thread.Sleep(TimeSpan.FromSeconds(3));

      return res.estado == EstadosEnum.OK;
    }
  }
}
