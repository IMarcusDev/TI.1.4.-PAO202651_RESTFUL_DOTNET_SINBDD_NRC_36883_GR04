using MOV_CON_UNI_DOTNET_RESTFUL.Controllers;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.data;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.Enums;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Pruebas
{
    public static class PruebasValidador
    {
        public static Ejecutor Crear()
        {
            var e = new Ejecutor();

            (string? Texto, bool Negativos, string Esperado)[] filtros =
            [
                (null, false, ""),
                ("", false, ""),
                ("12a3", false, "123"),
                ("1.2.3", false, "1.23"),
                ("1,2.3", false, "1,23"),
                ("-5", false, "5"),
                ("-5", true, "-5"),
                ("5-", true, "5"),
                ("--5", true, "-5"),
                (" 1 2 ", false, "12"),
                ("1e5", false, "15"),
                ("+5", false, "5"),
                ("1234567890", false, "123456789"),
                ("1.12345678", false, "1.123456"),
                ("123456789012.12345678", true, "123456789.123456"),
            ];
            foreach (var (texto, negativos, esperado) in filtros)
                e.Caso($"Filtrar «{texto}» (negativos={negativos}) → «{esperado}»",
                    () => Afirmar.Igual(esperado, ValidadorConversion.FiltrarValor(texto, negativos)));

            const Magnitud L = Magnitud.Longitud;
            const Magnitud T = Magnitud.Temperatura;
            (Magnitud? Mag, string? Valor, string? Origen, string? Destino, string Mensaje)[] errores =
            [
                (null, "1", "m", "cm", "Seleccione qué desea convertir"),
                (null, "", null, null, "Seleccione qué desea convertir"),
                (L, null, "m", "cm", "Ingrese un valor para convertir"),
                (L, "", "m", "cm", "Ingrese un valor para convertir"),
                (L, " 1", "m", "cm", "El valor no debe contener espacios"),
                (L, "1 2", "m", "cm", "El valor no debe contener espacios"),
                (L, "-", "m", "cm", "Ingrese un número, no solo un signo o un separador"),
                (L, ".", "m", "cm", "Ingrese un número, no solo un signo o un separador"),
                (L, ",", "m", "cm", "Ingrese un número, no solo un signo o un separador"),
                (L, "-.", "m", "cm", "Ingrese un número, no solo un signo o un separador"),
                (L, "1.2.3", "m", "cm", "Use un solo separador decimal (punto o coma)"),
                (L, "1,2.3", "m", "cm", "Use un solo separador decimal (punto o coma)"),
                (L, "1e5", "m", "cm", "No se admite notación científica"),
                (L, "1E5", "m", "cm", "No se admite notación científica"),
                (L, "abc", "m", "cm", "El valor solo puede contener números"),
                (L, "NaN", "m", "cm", "El valor solo puede contener números"),
                (L, "Infinity", "m", "cm", "El valor solo puede contener números"),
                (L, "-Infinity", "m", "cm", "El valor solo puede contener números"),
                (L, "1$", "m", "cm", "El valor no es un número válido"),
                (L, "+5", "m", "cm", "El valor no es un número válido"),
                (L, "1-2", "m", "cm", "El valor no es un número válido"),
                (L, "1234567890", "m", "cm", "El valor admite como máximo 9 dígitos enteros"),
                (L, "1.1234567", "m", "cm", "El valor admite como máximo 6 decimales"),
                (L, "5", null, "cm", "Seleccione la unidad de origen"),
                (L, "5", "m", null, "Seleccione la unidad de destino"),
                (L, "-5", null, null, "Seleccione la unidad de origen"),
                (L, "-5", "m", "cm", "No se permiten valores negativos en Longitud"),
                (Magnitud.Area, "-1", "m2", "ha", "No se permiten valores negativos en Área"),
                (Magnitud.Tiempo, "-0,5", "s", "h", "No se permiten valores negativos en Tiempo"),
                (T, "-300", "°C", "K", "La temperatura no puede ser menor al cero absoluto (-273,15 °C)"),
                (T, "-459.68", "°F", "K", "La temperatura no puede ser menor al cero absoluto (-459,67 °F)"),
                (T, "-1", "K", "°C", "La temperatura no puede ser menor al cero absoluto (0 K)"),
                (T, "-90.14", "°N", "°C", "La temperatura no puede ser menor al cero absoluto (-90,1395 °N)"),
                (T, "-218,53", "°Ré", "°C", "La temperatura no puede ser menor al cero absoluto (-218,52 °Ré)"),
            ];
            foreach (var (mag, valor, origen, destino, mensaje) in errores)
                e.Caso($"Validar {mag?.ToString() ?? "null"} «{valor}» {origen}→{destino} → «{mensaje}»", () =>
                {
                    ValidacionResult r = ValidadorConversion.Validar(mag, valor, origen, destino);
                    Afirmar.Verdadero(!r.EsValido, "debía ser inválido");
                    Afirmar.Igual(mensaje, r.Mensaje);
                });

            e.Caso("Cero absoluto con °Ré en forma NFD", () =>
            {
                ValidacionResult r = ValidadorConversion.Validar(T, "-219", "°Ré", "°C");
                Afirmar.Verdadero(!r.EsValido && r.Mensaje.StartsWith("La temperatura no puede ser menor al cero absoluto (-218,52"), r.Mensaje);
            });

            (Magnitud Mag, string Valor, string Origen, double Esperado)[] validos =
            [
                (L, "5", "m", 5),
                (L, "5.", "m", 5),
                (L, ",5", "m", 0.5),
                (L, "2,5", "m", 2.5),
                (L, "2.5", "m", 2.5),
                (L, "-0", "m", 0),
                (L, "999999999.999999", "m", 999_999_999.999999),
                (T, "-273,15", "°C", -273.15),
                (T, "-459.67", "°F", -459.67),
                (T, "0", "K", 0),
                (T, "-90,1395", "°N", -90.1395),
                (T, "-218.52", "°Ré", -218.52),
                (T, "-40", "°C", -40),
            ];
            foreach (var (mag, valor, origen, esperado) in validos)
                e.Caso($"Válido {mag} «{valor}» {origen} = {esperado}", () =>
                {
                    ValidacionResult r = ValidadorConversion.Validar(mag, valor, origen, origen);
                    Afirmar.Verdadero(r.EsValido, $"debía ser válido: {r.Mensaje}");
                    Afirmar.Cerca(esperado, r.Valor);
                });

            e.Caso("«-0» se interpreta como 0 positivo", () =>
                Afirmar.Verdadero(!double.IsNegative(ValidadorConversion.Validar(L, "-0", "m", "cm").Valor), "quedó -0"));

            return e;
        }
    }
}
