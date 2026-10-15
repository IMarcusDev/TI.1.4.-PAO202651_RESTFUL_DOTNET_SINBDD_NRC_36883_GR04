using WS_CON_UNI_DOTNET_RESTful.Models.Interfaces;
using WS_CON_UNI_DOTNET_RESTful.Models.Repositories;
using WS_CON_UNI_DOTNET_RESTful.Services;

namespace WS_CON_UNI_DOTNET_RESTful.Controllers
{
  public class LoginController
  {
    public static string Login(string username, string password)
    {
      LoginRepository repository = new LoginImplRepository();
    
      return repository.ValidateCredentials(username, password);
    }
  
    public static bool ValidateUser(string token)
    {
      return TokenService.ValidateToken(token);
    }
  }
}
