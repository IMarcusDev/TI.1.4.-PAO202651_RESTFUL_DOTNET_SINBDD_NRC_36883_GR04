using MOV_CON_UNI_DOTNET_RESTFUL.Models.DTO;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.Repositories;
using MOV_CON_UNI_DOTNET_RESTFUL.Services;
using System.Net.Sockets;
using System.Text.Json;
using Java = Simulado.Java;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Pruebas
{
    public static class PruebasErrores
    {
        private const string NoEncontrado = "No se encontró el servidor en http://192.168.1.2:5013/. Verifique que esté encendido y que la IP sea correcta.";
        private const string Timeout = "El servidor no respondió en 15 segundos. Intente de nuevo.";
        private const string SinComunicacion = "No se pudo comunicar con el servidor. Verifique que esté encendido y en la misma red.";
        private const string Inesperado = "Ocurrió un error inesperado al contactar al servidor.";

        public static Ejecutor Crear()
        {
            var e = new Ejecutor();

            (string Nombre, Exception Ex, string Mensaje)[] excepciones =
            [
                ("Conexión rechazada (.NET)", new HttpRequestException("x", new SocketException((int)SocketError.ConnectionRefused)), NoEncontrado),
                ("Host desconocido (.NET)", new HttpRequestException("x", new SocketException((int)SocketError.HostNotFound)), NoEncontrado),
                ("Host inalcanzable (.NET)", new HttpRequestException("x", new SocketException((int)SocketError.HostUnreachable)), NoEncontrado),
                ("Error de resolución DNS (.NET)", new HttpRequestException(HttpRequestError.NameResolutionError, "x"), NoEncontrado),
                ("ConnectException (Android)", new HttpRequestException("x", new Java.ConnectException("refused")), NoEncontrado),
                ("UnknownHostException (Android)", new Java.UnknownHostException("host"), NoEncontrado),
                ("NoRouteToHostException (Android)", new HttpRequestException("x", new Java.NoRouteToHostException("route")), NoEncontrado),
                ("Timeout de HttpClient", new TaskCanceledException("x", new TimeoutException("15 s")), Timeout),
                ("SocketTimeoutException (Android)", new HttpRequestException("x", new Java.SocketTimeoutException("t")), Timeout),
                ("JSON inválido", new JsonException("x"), RestErrores.RespuestaIncompleta),
                ("URL mal formada", new UriFormatException("x"), "La dirección del servidor está mal configurada."),
                ("HttpRequestException genérica", new HttpRequestException("x"), SinComunicacion),
                ("IOException de Java (cleartext bloqueado)", new Java.IOException("Cleartext HTTP traffic not permitted"), SinComunicacion),
                ("Cualquier otra excepción", new InvalidOperationException("x"), Inesperado),
            ];
            foreach (var (nombre, ex, mensaje) in excepciones)
                e.Caso($"Excepción: {nombre}", () => Afirmar.EmpiezaCon(mensaje, RestErrores.Mensaje(ex)));

#if DEBUG
            e.Caso("Debug agrega la causa más interna", () =>
            {
                string m = RestErrores.Mensaje(new HttpRequestException("x", new Java.ConnectException("refused")));
                Afirmar.Verdadero(m.EndsWith("\n[Debug] ConnectException: refused"), m);
            });
#endif

            e.Caso("Estado numérico 0 → OK", () =>
                Afirmar.Igual(EstadosEnum.OK, Leer(200, """{"token":"t","estado":0,"mensaje":"m"}""")?.Estado));
            e.Caso("Estado numérico 1 → ERROR (aunque llegue con 401)", () =>
                Afirmar.Igual(EstadosEnum.ERROR, Leer(401, """{"token":"","estado":1,"mensaje":"Credenciales Incorrectas"}""")?.Estado));
            e.Caso("Estado como texto también se acepta", () =>
                Afirmar.Igual(EstadosEnum.ERROR, Leer(200, """{"estado":"ERROR","mensaje":"m"}""")?.Estado));
            e.Caso("Cuerpo vacío → sin DTO", () => Afirmar.Verdadero(Leer(200, "") is null, "debía ser null"));
            e.Caso("HTML → sin DTO", () => Afirmar.Verdadero(Leer(500, "<html>error</html>") is null, "debía ser null"));
            e.Caso("Arreglo JSON → sin DTO", () => Afirmar.Verdadero(Leer(200, "[]") is null, "debía ser null"));
            e.Caso("ProblemDetails (sin estado) → sin DTO", () =>
                Afirmar.Verdadero(Leer(400, ProblemDetails) is null, "debía ser null"));

            e.Caso("2xx vacío → respuesta incompleta", () =>
                Afirmar.Igual(RestErrores.RespuestaIncompleta, RestErrores.MensajeSinDto(new RespuestaHttp(200, "OK", ""))));
            e.Caso("400 ProblemDetails → usa su título", () =>
                Afirmar.Igual("El servidor no pudo procesar la solicitud: One or more validation errors occurred.",
                    RestErrores.MensajeSinDto(new RespuestaHttp(400, "Bad Request", ProblemDetails))));
            e.Caso("404 sin cuerpo → código HTTP", () =>
                Afirmar.Igual("El servidor no pudo procesar la solicitud: HTTP 404 Not Found",
                    RestErrores.MensajeSinDto(new RespuestaHttp(404, "Not Found", ""))));
            e.Caso("500 HTML → código HTTP", () =>
                Afirmar.Igual("El servidor no pudo procesar la solicitud: HTTP 500 Internal Server Error",
                    RestErrores.MensajeSinDto(new RespuestaHttp(500, "Internal Server Error", "<html/>"))));

            e.Caso("«Token Inválido» → sesión expirada", () => Afirmar.Verdadero(RestErrores.EsTokenInvalido("Token Inválido"), "debía detectarse"));
            e.Caso("«No se pudo hacer la conversión» (llega con 401) → NO es sesión", () =>
                Afirmar.Verdadero(!RestErrores.EsTokenInvalido("No se pudo hacer la conversión"), "no debía detectarse"));
            e.Caso("«Credenciales Incorrectas» (llega con 401) → NO es sesión", () =>
                Afirmar.Verdadero(!RestErrores.EsTokenInvalido("Credenciales Incorrectas"), "no debía detectarse"));
            e.Caso("Mensaje nulo → NO es sesión", () => Afirmar.Verdadero(!RestErrores.EsTokenInvalido(null), "no debía detectarse"));

            e.Caso("Mensaje vacío del servidor → mensaje por defecto", () =>
                Afirmar.Igual("Por defecto", RestErrores.MensajeServidor("  ", "Por defecto")));

            return e;
        }

        private const string ProblemDetails =
            """{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.1","title":"One or more validation errors occurred.","status":400}""";

        private static LoginResponseDTO? Leer(int codigo, string cuerpo)
            => RestErrores.LeerDto(new RespuestaHttp(codigo, null, cuerpo), JsonContexto.Default.LoginResponseDTO);
    }
}
