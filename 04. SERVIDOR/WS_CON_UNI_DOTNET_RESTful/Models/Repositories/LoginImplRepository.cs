using WS_CON_UNI_DOTNET_RESTful.Models.Interfaces;
using WS_CON_UNI_DOTNET_RESTful.Services;
using WS_CON_UNI_DOTNET_SOAP.Models.Interfaces;
using WS_CON_UNI_DOTNET_SOAP.Services;

namespace WS_CON_UNI_DOTNET_SOAP.Models.Repositories
{
  public class LoginImplRepository : LoginRepository
  {
    private Dictionary<string, string> users;  // HashMap<username, password>

    public LoginImplRepository()
    {
      this.users = new Dictionary<string, string>();

      // Credentials
      this.users.Add("user", "1234");
      this.users.Add("monster", "monster9");
      this.users.Add("marcos", "mamita");
      this.users.Add("carlos", "carlos9");
      this.users.Add("msosa", "msosa");
    }

    public string ValidateCredentials(string username, string password)
    {
      // No DB
      string localUsername = username;
      string? localPassword = this.users.GetValueOrDefault(username);

      if (!localUsername.Equals(username) || localPassword == null || localPassword.Equals(password))
      {
        throw new Exception("Invalid User");
      }

      return TokenService.GenerateToken();
    }
  }
}
