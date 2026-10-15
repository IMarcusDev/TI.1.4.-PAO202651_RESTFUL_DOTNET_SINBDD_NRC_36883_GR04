namespace WS_CON_UNI_DOTNET_SOAP.Models.Interfaces
{
  public interface LoginRepository
  {
    public abstract string ValidateCredentials(string username, string password);
  }
}
