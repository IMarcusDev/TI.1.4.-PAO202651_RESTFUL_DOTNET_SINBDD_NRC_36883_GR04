using MOV_CON_UNI_DOTNET_RESTFUL.Config;
using System.Text;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Services
{
    public record RespuestaHttp(int Codigo, string? Razon, string Cuerpo)
    {
        public bool EsExitosa => Codigo is >= 200 and <= 299;
    }

    public class ClienteRest
    {
        private readonly HttpClient _http;

        public string BaseUrl { get; }

        public ClienteRest(string baseUrl, HttpMessageHandler? manejador = null)
        {
            BaseUrl = baseUrl.EndsWith('/') ? baseUrl : baseUrl + "/";
            _http = manejador is null ? new HttpClient() : new HttpClient(manejador);
            _http.BaseAddress = new Uri(BaseUrl);
            _http.Timeout = RestConfig.Timeout;
        }

        public Task<RespuestaHttp> GetAsync(string ruta, string? token)
            => EnviarAsync(new HttpRequestMessage(HttpMethod.Get, ruta), token);

        public Task<RespuestaHttp> PostJsonAsync(string ruta, string json, string? token)
        {
            var peticion = new HttpRequestMessage(HttpMethod.Post, ruta)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
            };
            return EnviarAsync(peticion, token);
        }

        private async Task<RespuestaHttp> EnviarAsync(HttpRequestMessage peticion, string? token)
        {
            using (peticion)
            {
                peticion.Headers.Accept.ParseAdd("application/json");
                if (token is not null)
                    peticion.Headers.TryAddWithoutValidation("Authorization", token);

                using HttpResponseMessage respuesta = await _http.SendAsync(peticion);
                string cuerpo = await respuesta.Content.ReadAsStringAsync();
                return new RespuestaHttp((int)respuesta.StatusCode, respuesta.ReasonPhrase, cuerpo);
            }
        }
    }
}
