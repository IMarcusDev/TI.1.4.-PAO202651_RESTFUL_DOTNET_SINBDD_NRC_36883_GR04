namespace WS_CON_UNI_DOTNET_RESTful.Models.Interfaces
{
  public interface LoginRepository
  {
    public abstract string ValidateCredentials(string username, string password);
  }
}
