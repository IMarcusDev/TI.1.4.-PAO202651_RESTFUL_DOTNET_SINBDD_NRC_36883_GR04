using MOV_CON_UNI_DOTNET_RESTFUL.Config;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.DTO;
using MOV_CON_UNI_DOTNET_RESTFUL.Services;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Models.Repositories
{
    internal static partial class RestErrores
    {
        public const string SinConexion = "Sin conexión a internet. Revise el Wi-Fi o los datos móviles.";
        public const string RespuestaIncompleta = "El servidor envió una respuesta vacía o incompleta.";
        public const string SesionExpirada = "Su sesión expiró o no es válida. Inicie sesión nuevamente.";
        public const string ResultadoNoValido = "El servidor devolvió un resultado no válido.";

        public static string Mensaje(Exception ex)
        {
            string mensaje = MensajeAmigable(ex);
#if DEBUG
            Exception causa = ex.GetBaseException();
            mensaje += $"\n[Debug] {causa.GetType().Name}: {causa.Message}";
#endif
            return mensaje;
        }

        private static string MensajeAmigable(Exception ex)
        {
            List<Exception> causas = Causas(ex);

            if (causas.Any(EsServidorNoEncontrado))
                return $"No se encontró el servidor en {RestConfig.BaseUrl}. Verifique que esté encendido y que la IP sea correcta.";
            if (causas.Any(EsTimeout))
                return $"El servidor no respondió en {RestConfig.Timeout.TotalSeconds:0} segundos. Intente de nuevo.";
            if (causas.Any(c => c is JsonException))
                return RespuestaIncompleta;
            if (causas.Any(c => c is UriFormatException))
                return "La dirección del servidor está mal configurada.";
            if (causas.Any(c => c is HttpRequestException or IOException or SocketException || c.GetType().Name == "IOException"))
                return "No se pudo comunicar con el servidor. Verifique que esté encendido y en la misma red.";
            return "Ocurrió un error inesperado al contactar al servidor.";
        }

        private static List<Exception> Causas(Exception ex)
        {
            var causas = new List<Exception>();
            for (Exception? actual = ex; actual is not null; actual = actual.InnerException)
                causas.Add(actual);
            return causas;
        }

        private static bool EsServidorNoEncontrado(Exception ex) => ex switch
        {
            SocketException s => s.SocketErrorCode is SocketError.ConnectionRefused or SocketError.HostNotFound
                or SocketError.HostUnreachable or SocketError.NetworkUnreachable or SocketError.NoData,
            HttpRequestException h => h.HttpRequestError == HttpRequestError.NameResolutionError,
            _ => ex.GetType().Name is "ConnectException" or "UnknownHostException" or "NoRouteToHostException",
        };

        private static bool EsTimeout(Exception ex)
            => ex is TimeoutException or OperationCanceledException || ex.GetType().Name == "SocketTimeoutException";

        public static T? LeerDto<T>(RespuestaHttp respuesta, JsonTypeInfo<T> tipo) where T : ResponseDTO
        {
            if (string.IsNullOrWhiteSpace(respuesta.Cuerpo))
                return null;
            try
            {
                T? dto = JsonSerializer.Deserialize(respuesta.Cuerpo, tipo);
                return dto?.Estado is null ? null : dto;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        public static string MensajeSinDto(RespuestaHttp respuesta)
            => respuesta.EsExitosa
                ? RespuestaIncompleta
                : $"El servidor no pudo procesar la solicitud: {DetalleHttp(respuesta)}";

        private static string DetalleHttp(RespuestaHttp respuesta)
        {
            try
            {
                using JsonDocument json = JsonDocument.Parse(respuesta.Cuerpo);
                if (json.RootElement.ValueKind == JsonValueKind.Object
                    && json.RootElement.TryGetProperty("title", out JsonElement titulo)
                    && titulo.ValueKind == JsonValueKind.String
                    && !string.IsNullOrWhiteSpace(titulo.GetString()))
                    return titulo.GetString()!.Trim();
            }
            catch (JsonException)
            {
            }
            return $"HTTP {respuesta.Codigo} {respuesta.Razon}".TrimEnd();
        }

        public static string MensajeServidor(string? mensaje, string porDefecto)
            => string.IsNullOrWhiteSpace(mensaje) ? porDefecto : mensaje.Trim();

        public static bool EsTokenInvalido(string? mensaje)
            => mensaje?.Contains("token", StringComparison.OrdinalIgnoreCase) == true;
    }
}
