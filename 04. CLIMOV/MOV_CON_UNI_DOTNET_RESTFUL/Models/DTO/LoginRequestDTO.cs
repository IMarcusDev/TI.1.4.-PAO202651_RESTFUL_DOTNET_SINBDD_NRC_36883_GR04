namespace MOV_CON_UNI_DOTNET_RESTFUL.Models.DTO
{
    public class LoginRequestDTO(string username, string password)
    {
        public string Username { get; } = username;
        public string Password { get; } = password;
    }
}
