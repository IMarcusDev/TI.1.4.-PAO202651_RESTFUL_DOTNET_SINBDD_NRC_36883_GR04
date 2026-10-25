using MOV_CON_UNI_DOTNET_RESTFUL.Models.data;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.DTO;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.Interface;
using MOV_CON_UNI_DOTNET_RESTFUL.Services;
using System.Text.Json;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Models.Repositories
{
    public class LoginRepository(ClienteRest cliente) : ILoginRepository
    {
        private readonly ClienteRest _cliente = cliente;

        public async Task<LoginResult> LoginAsync(string username, string password)
        {
            if (RestErrores.SinRed())
                return new LoginResult(false, RestErrores.SinConexion, null);

            try
            {
                string json = JsonSerializer.Serialize(
                    new LoginRequestDTO(username, password), JsonContexto.Default.LoginRequestDTO);

                RespuestaHttp respuesta = await _cliente.PostJsonAsync("login", json, token: null);

                LoginResponseDTO? dto = RestErrores.LeerDto(respuesta, JsonContexto.Default.LoginResponseDTO);
                if (dto == null)
                    return new LoginResult(false, RestErrores.MensajeSinDto(respuesta), null);

                if (dto.Estado != EstadosEnum.OK)
                    return new LoginResult(false, RestErrores.MensajeServidor(dto.Mensaje, "Usuario o contraseña incorrectos"), null);

                if (string.IsNullOrWhiteSpace(dto.Token))
                    return new LoginResult(false, RestErrores.RespuestaIncompleta, null);

                return new LoginResult(
                    Success: true,
                    Message: dto.Mensaje ?? "",
                    Token: dto.Token
                    );
            }
            catch (Exception ex)
            {
                return new LoginResult(false, RestErrores.Mensaje(ex), null);
            }
        }
    }
}
