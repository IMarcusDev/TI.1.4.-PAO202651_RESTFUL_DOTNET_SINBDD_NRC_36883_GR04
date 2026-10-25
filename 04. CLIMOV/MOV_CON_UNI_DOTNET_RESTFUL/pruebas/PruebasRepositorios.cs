using MOV_CON_UNI_DOTNET_RESTFUL.Controllers;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.data;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.Enums;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.Repositories;
using MOV_CON_UNI_DOTNET_RESTFUL.Services;
using System.Net.Sockets;
using System.Text.Json;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Pruebas
{
    public static class PruebasRepositorios
    {
        private const string Url = "http://servidor.prueba:5013/";
        private const string Token = "11111111-2222-3333-4444-555555555555";

        public static Ejecutor Crear()
        {
            var e = new Ejecutor();

            e.Caso("Login: envía POST /login con usuario y contraseña, sin token", async () =>
            {
                var red = ManejadorSimulado.Responde(200, $$"""{"token":"{{Token}}","estado":0,"mensaje":"Ingreso de Sesión Exitoso!"}""");
                LoginResult r = await new LoginRepository(Cliente(red)).LoginAsync("user", "1234");
                Afirmar.Verdadero(r.Success, r.Message);
                Afirmar.Igual(Token, r.Token);
                var p = red.Peticiones.Single();
                Afirmar.Igual("POST /login", $"{p.Metodo} {p.Ruta}");
                Afirmar.Igual(null, p.Token, "token");
                Afirmar.Igual("""{"username":"user","password":"1234"}""", p.Cuerpo);
            });
            e.Caso("Login: 401 «Credenciales Incorrectas» → mensaje del servidor", async () =>
            {
                var red = ManejadorSimulado.Responde(401, """{"token":"","estado":1,"mensaje":"Credenciales Incorrectas"}""");
                LoginResult r = await new LoginRepository(Cliente(red)).LoginAsync("user", "x");
                Afirmar.Verdadero(!r.Success, "debía fallar");
                Afirmar.Igual("Credenciales Incorrectas", r.Message);
            });
            e.Caso("Login: OK sin token → respuesta incompleta", async () =>
            {
                var red = ManejadorSimulado.Responde(200, """{"token":"","estado":0,"mensaje":"ok"}""");
                LoginResult r = await new LoginRepository(Cliente(red)).LoginAsync("user", "1234");
                Afirmar.Igual(RestErrores.RespuestaIncompleta, r.Message);
            });
            e.Caso("Login: sin red → no llama al servidor", async () =>
            {
                var red = ManejadorSimulado.Responde(200, "{}");
                RestErrores.SimularSinRed = true;
                try
                {
                    LoginResult r = await new LoginRepository(Cliente(red)).LoginAsync("user", "1234");
                    Afirmar.Igual(RestErrores.SinConexion, r.Message);
                    Afirmar.Igual(0, red.Peticiones.Count, "peticiones");
                }
                finally
                {
                    RestErrores.SimularSinRed = false;
                }
            });
            e.Caso("Login: conexión rechazada → no lanza, devuelve mensaje", async () =>
            {
                var red = ManejadorSimulado.Lanza(new HttpRequestException("x", new SocketException((int)SocketError.ConnectionRefused)));
                LoginResult r = await new LoginRepository(Cliente(red)).LoginAsync("user", "1234");
                Afirmar.EmpiezaCon("No se encontró el servidor en", r.Message);
            });

            e.Caso("Catálogo: GET /catalog/Temperatura con el token en Authorization (sin Bearer)", async () =>
            {
                var red = ManejadorSimulado.Responde(200, """{"resultado":["°C","°F","K","°N","°Ré"],"estado":0,"mensaje":"ok"}""");
                CatalogResult r = await new CatalogRepository(Cliente(red)).GetCatalogAsync(Token, Magnitud.Temperatura);
                Afirmar.Igual("°C,°F,K,°N,°Ré", string.Join(",", r.Unidades));
                var p = red.Peticiones.Single();
                Afirmar.Igual("GET /catalog/Temperatura", $"{p.Metodo} {p.Ruta}");
                Afirmar.Igual(Token, p.Token, "token");
            });
            e.Caso("Catálogo: 401 «Token Inválido» → SesionExpirada", async () =>
            {
                var red = ManejadorSimulado.Responde(401, """{"resultado":[],"estado":1,"mensaje":"Token Inválido"}""");
                CatalogResult r = await new CatalogRepository(Cliente(red)).GetCatalogAsync(Token, Magnitud.Area);
                Afirmar.Verdadero(r.SesionExpirada && !r.Success, "debía marcar sesión expirada");
            });
            e.Caso("Catálogo: 400 «Tipo de Catálogo Inexistente» → error, no sesión", async () =>
            {
                var red = ManejadorSimulado.Responde(400, """{"resultado":[],"estado":1,"mensaje":"Tipo de Catálogo Inexistente"}""");
                CatalogResult r = await new CatalogRepository(Cliente(red)).GetCatalogAsync(Token, Magnitud.Area);
                Afirmar.Igual("Tipo de Catálogo Inexistente", r.Message);
                Afirmar.Verdadero(!r.SesionExpirada, "no era sesión");
            });
            e.Caso("Catálogo: OK sin unidades → mensaje con la magnitud", async () =>
            {
                var red = ManejadorSimulado.Responde(200, """{"resultado":[],"estado":0,"mensaje":"ok"}""");
                CatalogResult r = await new CatalogRepository(Cliente(red)).GetCatalogAsync(Token, Magnitud.Area);
                Afirmar.Igual("El servidor no devolvió unidades para Área.", r.Message);
            });

            e.Caso("Conversión: POST /converter con JSON y unidades Unicode intactas", async () =>
            {
                var red = ManejadorSimulado.Responde(200, """{"resultado":80,"estado":0,"mensaje":"Conversión Exitosa"}""");
                var conversion = new Conversion(Magnitud.Temperatura, 100, "°C", "°Ré");
                ConvertResult r = await new ConvertRepository(Cliente(red)).ConvertAsync(Token, conversion);
                Afirmar.Cerca(80, r.Value);
                var p = red.Peticiones.Single();
                Afirmar.Igual("POST /converter", $"{p.Metodo} {p.Ruta}");
                Afirmar.Igual(Token, p.Token, "token");
                using JsonDocument json = JsonDocument.Parse(p.Cuerpo!);
                Afirmar.Igual("Temperatura", json.RootElement.GetProperty("type").GetString());
                Afirmar.Igual(100.0, json.RootElement.GetProperty("value").GetDouble());
                Afirmar.Igual("°C", json.RootElement.GetProperty("from").GetString());
                Afirmar.Igual("°Ré", json.RootElement.GetProperty("to").GetString());
            });
            e.Caso("Conversión: 401 «No se pudo hacer la conversión» → error, NO sesión expirada", async () =>
            {
                var red = ManejadorSimulado.Responde(401, """{"resultado":0,"estado":1,"mensaje":"No se pudo hacer la conversión"}""");
                ConvertResult r = await new ConvertRepository(Cliente(red)).ConvertAsync(Token, new Conversion(Magnitud.Area, 1, "m2", "ha"));
                Afirmar.Igual("No se pudo hacer la conversión", r.Message);
                Afirmar.Verdadero(!r.SesionExpirada && !r.Success, "no debía ser sesión expirada");
            });
            e.Caso("Conversión: OK sin «resultado» → respuesta incompleta (no 0)", async () =>
            {
                var red = ManejadorSimulado.Responde(200, """{"estado":0,"mensaje":"ok"}""");
                ConvertResult r = await new ConvertRepository(Cliente(red)).ConvertAsync(Token, new Conversion(Magnitud.Area, 1, "m2", "ha"));
                Afirmar.Igual(RestErrores.RespuestaIncompleta, r.Message);
            });
            e.Caso("Conversión: 500 con HTML → el servidor no pudo procesar", async () =>
            {
                var red = ManejadorSimulado.Responde(500, "<html/>", "text/html");
                ConvertResult r = await new ConvertRepository(Cliente(red)).ConvertAsync(Token, new Conversion(Magnitud.Area, 1, "m2", "ha"));
                Afirmar.Igual("El servidor no pudo procesar la solicitud: HTTP 500 Internal Server Error", r.Message);
            });
            e.Caso("Conversión: timeout → mensaje de 15 segundos", async () =>
            {
                var red = ManejadorSimulado.Lanza(new TaskCanceledException("x", new TimeoutException()));
                ConvertResult r = await new ConvertRepository(Cliente(red)).ConvertAsync(Token, new Conversion(Magnitud.Area, 1, "m2", "ha"));
                Afirmar.EmpiezaCon("El servidor no respondió en 15 segundos. Intente de nuevo.", r.Message);
            });

            (string? Usuario, string? Clave, string Mensaje)[] login =
            [
                (null, "1234", "Ingrese el usuario"),
                ("", "1234", "Ingrese el usuario"),
                ("   ", "1234", "Ingrese el usuario"),
                ("user", null, "Ingrese la contraseña"),
                ("user", "   ", "Ingrese la contraseña"),
                (new string('u', 51), "1234", "El usuario no puede superar 50 caracteres"),
                ("user", new string('c', 65), "La contraseña no puede superar 64 caracteres"),
            ];
            foreach (var (usuario, clave, mensaje) in login)
                e.Caso($"LoginController: «{Recortar(usuario)}»/«{Recortar(clave)}» → «{mensaje}» sin llamar a la red", async () =>
                {
                    var red = ManejadorSimulado.Responde(200, "{}");
                    var control = new LoginController(new LoginRepository(Cliente(red)), new SessionService());
                    LoginResult r = await control.LoginAsync(usuario, clave);
                    Afirmar.Igual(mensaje, r.Message);
                    Afirmar.Igual(0, red.Peticiones.Count, "peticiones");
                });
            e.Caso("LoginController: 50 y 64 caracteres sí se aceptan", async () =>
            {
                var red = ManejadorSimulado.Responde(401, """{"token":"","estado":1,"mensaje":"Credenciales Incorrectas"}""");
                var control = new LoginController(new LoginRepository(Cliente(red)), new SessionService());
                LoginResult r = await control.LoginAsync(new string('u', 50), new string('c', 64));
                Afirmar.Igual("Credenciales Incorrectas", r.Message);
            });
            e.Caso("LoginController: aplica Trim y guarda la sesión", async () =>
            {
                var red = ManejadorSimulado.Responde(200, $$"""{"token":"{{Token}}","estado":0,"mensaje":"ok"}""");
                var sesion = new SessionService();
                LoginResult r = await new LoginController(new LoginRepository(Cliente(red)), sesion).LoginAsync("  user ", " 1234  ");
                Afirmar.Verdadero(r.Success, r.Message);
                Afirmar.Igual("""{"username":"user","password":"1234"}""", red.Peticiones.Single().Cuerpo);
                Afirmar.Igual(Token, sesion.Token);
                Afirmar.Igual("user", sesion.Usuario);
            });

            e.Caso("ConversionController: origen == destino no llama al servidor", async () =>
            {
                var red = ManejadorSimulado.Responde(500, "");
                ConversionController control = ControlConversion(red, ConSesion());
                ConvertResult r = await control.ConvertAsync(Magnitud.Longitud, "2,5", "m", "m");
                Afirmar.Verdadero(r.Success, r.Message);
                Afirmar.Igual("Origen y destino son la misma unidad; el valor no cambia.", r.Message);
                Afirmar.Igual(2.5, r.Value);
                Afirmar.Igual(0, red.Peticiones.Count, "peticiones");
            });
            e.Caso("ConversionController: origen == destino igual valida antes (negativo en Masa)", async () =>
            {
                var red = ManejadorSimulado.Responde(500, "");
                ConvertResult r = await ControlConversion(red, ConSesion()).ConvertAsync(Magnitud.Masa, "-1", "kg", "kg");
                Afirmar.Igual("No se permiten valores negativos en Masa", r.Message);
            });
            e.Caso("ConversionController: token rechazado → limpia la sesión y avisa", async () =>
            {
                var red = ManejadorSimulado.Responde(401, """{"resultado":0,"estado":1,"mensaje":"Token Inválido"}""");
                SessionService sesion = ConSesion();
                ConvertResult r = await ControlConversion(red, sesion).ConvertAsync(Magnitud.Area, "1", "km2", "ac");
                Afirmar.Verdadero(r.SesionExpirada, "debía marcar sesión expirada");
                Afirmar.Igual(ControladorConSesion.MensajeSesionExpirada, r.Message);
                Afirmar.Verdadero(!sesion.IsAuthenticated, "la sesión debía quedar vacía");
            });
            e.Caso("ConversionController: sin sesión → expirada sin llamar a la red", async () =>
            {
                var red = ManejadorSimulado.Responde(200, "{}");
                ConvertResult r = await ControlConversion(red, new SessionService()).ConvertAsync(Magnitud.Area, "1", "km2", "ac");
                Afirmar.Verdadero(r.SesionExpirada, "debía marcar sesión expirada");
                Afirmar.Igual(0, red.Peticiones.Count, "peticiones");
            });
            e.Caso("ConversionController: conversión OK trae el detalle", async () =>
            {
                var red = ManejadorSimulado.Responde(200, """{"resultado":247.10516301527602,"estado":0,"mensaje":"Conversión Exitosa"}""");
                ConvertResult r = await ControlConversion(red, ConSesion()).ConvertAsync(Magnitud.Area, "1", "km2", "ac");
                Afirmar.Verdadero(r.Conversion is { From: "km2", To: "ac" }, "faltaba el detalle");
                Afirmar.Cerca(247.10516301527602, r.Conversion!.Result);
            });

            e.Caso("CatalogController: las 6 magnitudes en paralelo con nombres en español", async () =>
            {
                var red = new ManejadorSimulado(_ => ManejadorSimulado.Respuesta(200, """{"resultado":["m","cm"],"estado":0,"mensaje":"ok"}"""));
                CatalogoCompletoResult r = await new CatalogController(new CatalogRepository(Cliente(red)), ConSesion()).ObtenerCatalogoAsync();
                Afirmar.Verdadero(r.Success, r.Message);
                Afirmar.Igual(6, r.Grupos.Count, "grupos");
                Afirmar.Igual(6, red.Peticiones.Count, "peticiones");
                Afirmar.Igual("Metro", r.Grupos[0][0].Nombre);
            });
            e.Caso("CatalogController: si una magnitud falla no hay catálogo a medias", async () =>
            {
                var red = new ManejadorSimulado(p => p.RequestUri!.AbsolutePath.EndsWith("/Masa")
                    ? ManejadorSimulado.Respuesta(400, """{"resultado":[],"estado":1,"mensaje":"Tipo de Catálogo Inexistente"}""")
                    : ManejadorSimulado.Respuesta(200, """{"resultado":["m"],"estado":0,"mensaje":"ok"}"""));
                CatalogoCompletoResult r = await new CatalogController(new CatalogRepository(Cliente(red)), ConSesion()).ObtenerCatalogoAsync();
                Afirmar.Verdadero(!r.Success, "debía fallar");
                Afirmar.Igual(0, r.Grupos.Count, "grupos");
                Afirmar.Igual("Tipo de Catálogo Inexistente", r.Message);
            });
            e.Caso("CatalogController: token rechazado → limpia la sesión", async () =>
            {
                var red = ManejadorSimulado.Responde(401, """{"resultado":[],"estado":1,"mensaje":"Token Inválido"}""");
                SessionService sesion = ConSesion();
                CatalogoCompletoResult r = await new CatalogController(new CatalogRepository(Cliente(red)), sesion).ObtenerCatalogoAsync();
                Afirmar.Verdadero(r.SesionExpirada && !sesion.IsAuthenticated, "debía expirar la sesión");
                Afirmar.Igual(ControladorConSesion.MensajeSesionExpirada, r.Message);
            });

            return e;
        }

        private static ClienteRest Cliente(ManejadorSimulado red) => new(Url, red);

        private static SessionService ConSesion()
        {
            var sesion = new SessionService();
            sesion.Iniciar("user", Token);
            return sesion;
        }

        private static ConversionController ControlConversion(ManejadorSimulado red, SessionService sesion)
            => new(new ConvertRepository(Cliente(red)), new CatalogRepository(Cliente(red)), sesion);

        private static string Recortar(string? texto) => texto is { Length: > 10 } ? $"{texto[..3]}…({texto.Length})" : texto ?? "null";
    }
}
