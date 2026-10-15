using WS_CON_UNI_DOTNET_SOAP.Models.Interfaces;
using WS_CON_UNI_DOTNET_SOAP.Services;

namespace WS_CON_UNI_DOTNET_SOAP.Models.Repositories
{
  public class LoginImplRepository : LoginRepository
  {
    public string ValidateCredentials(string username, string password)
    {
      // No DB
      if (!"user".Equals(username) || !"1234".Equals(password))
      {
        throw new Exception("Invalid User");
      }

      return TokenService.GenerateToken();
    }
  }
}
