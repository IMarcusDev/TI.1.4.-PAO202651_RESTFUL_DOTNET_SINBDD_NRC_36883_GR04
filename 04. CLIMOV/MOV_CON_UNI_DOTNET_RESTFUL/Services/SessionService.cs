namespace MOV_CON_UNI_DOTNET_RESTFUL.Services
{
    public class SessionService
    {
        public string? Token { get; private set; }
        public string? Usuario { get; private set; }

        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);

        public void Iniciar(string usuario, string token)
        {
            Usuario = usuario;
            Token = token;
        }

        public void Clear()
        {
            Usuario = null;
            Token = null;
        }
    }
}
