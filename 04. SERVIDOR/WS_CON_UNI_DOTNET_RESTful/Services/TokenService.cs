using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WS_CON_UNI_DOTNET_SOAP.Services
{
  public class TokenService
  {
    private static readonly HashSet<string> tokensActivos = new HashSet<string>();

    public static string GenerateToken()
    {
      string token = Guid.NewGuid().ToString();

      tokensActivos.Add(token);

      return token;
    }

    public static bool ValidateToken(string token)
    {
      return tokensActivos.Contains(token);
    }
  }
}
