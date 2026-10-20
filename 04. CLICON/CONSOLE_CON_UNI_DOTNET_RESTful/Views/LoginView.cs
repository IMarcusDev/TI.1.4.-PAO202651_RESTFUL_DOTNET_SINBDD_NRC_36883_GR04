using CONSOLE_CON_UNI_DOTNET_RESTful.Models.DTO;
using CONSOLE_CON_UNI_DOTNET_RESTful.Models.Enums;
using CONSOLE_CON_UNI_DOTNET_SOAP.Controllers;
using CONSOLE_CON_UNI_DOTNET_SOAP.Views.Utils;

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

      LoginResponseDTO? res = ctrl.LoginAsync(username, password).Result;

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
