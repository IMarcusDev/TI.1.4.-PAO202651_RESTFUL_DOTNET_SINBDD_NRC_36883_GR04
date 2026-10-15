using WS_CON_UNI_DOTNET_SOAP.Models.Interfaces;
using WS_CON_UNI_DOTNET_SOAP.Models.Repositories;
using WS_CON_UNI_DOTNET_SOAP.Services;

namespace WS_CON_UNI_DOTNET_SOAP.Controllers
{
  public class LoginController
  {
    public static string login(string username, string password)
    {
      LoginRepository repository = new LoginImplRepository();
    
      return repository.ValidateCredentials(username, password);
    }
  
    public static bool validateUser(string token)
    {
      return TokenService.ValidateToken(token);
    }
  }
}
