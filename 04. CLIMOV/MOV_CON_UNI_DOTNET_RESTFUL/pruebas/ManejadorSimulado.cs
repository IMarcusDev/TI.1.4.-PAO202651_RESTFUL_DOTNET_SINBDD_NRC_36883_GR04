using System.Net;
using System.Text;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Pruebas
{
    public class ManejadorSimulado(Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
    {
        public record Peticion(HttpMethod Metodo, string Ruta, string? Token, string? Cuerpo);

        public List<Peticion> Peticiones { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage peticion, CancellationToken cancelacion)
        {
            string? cuerpo = peticion.Content is null ? null : await peticion.Content.ReadAsStringAsync(cancelacion);
            string? token = peticion.Headers.TryGetValues("Authorization", out var valores) ? valores.FirstOrDefault() : null;
            Peticiones.Add(new Peticion(peticion.Method, peticion.RequestUri!.AbsolutePath, token, cuerpo));
            return responder(peticion);
        }

        public static ManejadorSimulado Responde(int codigo, string cuerpo, string tipo = "application/json")
            => new(_ => Respuesta(codigo, cuerpo, tipo));

        public static ManejadorSimulado Lanza(Exception ex) => new(_ => throw ex);

        public static HttpResponseMessage Respuesta(int codigo, string cuerpo, string tipo = "application/json")
            => new((HttpStatusCode)codigo) { Content = new StringContent(cuerpo, Encoding.UTF8, tipo) };
    }
}

namespace Simulado.Java
{
    public class ConnectException(string m) : Exception(m);
    public class UnknownHostException(string m) : Exception(m);
    public class NoRouteToHostException(string m) : Exception(m);
    public class SocketTimeoutException(string m) : Exception(m);
    public class IOException(string m) : Exception(m);
}
