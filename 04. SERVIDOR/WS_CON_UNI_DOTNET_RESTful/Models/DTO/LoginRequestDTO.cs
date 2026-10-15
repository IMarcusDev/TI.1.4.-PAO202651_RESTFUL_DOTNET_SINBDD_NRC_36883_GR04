namespace WS_CON_UNI_DOTNET_RESTful.Models.DTO
{
  public class LoginRequestDTO
  {
    public string? username { get; set; }
    public string? password { get; set; }

    public LoginRequestDTO() { }

    public LoginRequestDTO(string username, string password)
    {
      this.username = username;
      this.password = password;
    }
  }
}
