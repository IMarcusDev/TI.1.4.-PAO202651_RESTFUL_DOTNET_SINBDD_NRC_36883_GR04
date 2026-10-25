using MOV_CON_UNI_DOTNET_RESTFUL.Models.data;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.Interface;
using MOV_CON_UNI_DOTNET_RESTFUL.Services;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Controllers
{
    public class LoginController(ILoginRepository loginRepository, SessionService session) : ControladorConSesion(session)
    {
        public const int MaxUsuario = 50;
        public const int MaxPassword = 64;

        private readonly ILoginRepository _loginRepository = loginRepository;

        public async Task<LoginResult> LoginAsync(string? username, string? password)
        {
            string usuario = username?.Trim() ?? "";
            string clave = password?.Trim() ?? "";

            if (usuario.Length == 0)
                return new LoginResult(false, "Ingrese el usuario", null);
            if (clave.Length == 0)
                return new LoginResult(false, "Ingrese la contraseña", null);
            if (usuario.Length > MaxUsuario)
                return new LoginResult(false, $"El usuario no puede superar {MaxUsuario} caracteres", null);
            if (clave.Length > MaxPassword)
                return new LoginResult(false, $"La contraseña no puede superar {MaxPassword} caracteres", null);

            LoginResult resultado = await _loginRepository.LoginAsync(usuario, clave);

            if (resultado.Success && resultado.Token is not null)
                Session.Iniciar(usuario, resultado.Token);

            return resultado;
        }
    }
}
