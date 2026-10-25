using MOV_CON_UNI_DOTNET_RESTFUL.Models.data;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.Enums;
using MOV_CON_UNI_DOTNET_RESTFUL.Utils;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Controllers
{
    public static partial class ValidadorConversion
    {
        public const int MaxEnteros = 9;
        public const int MaxDecimales = 6;
        public const double ValorMaximo = 999_999_999.999999;

        private static readonly Dictionary<string, double> CeroAbsoluto = new()
        {
            ["°C"] = -273.15,
            ["°F"] = -459.67,
            ["K"] = 0,
            ["°N"] = -90.1395,
            ["°Ré"] = -218.52,
        };

        [GeneratedRegex(@"^-?(\d+([.,]\d*)?|[.,]\d+)$")]
        private static partial Regex NumeroValido();

        public static string FiltrarValor(string? texto, bool permiteNegativos)
        {
            if (string.IsNullOrEmpty(texto))
                return "";

            var limpio = new StringBuilder(texto.Length);
            bool haySeparador = false;
            int enteros = 0, decimales = 0;

            foreach (char c in texto)
            {
                if (c == '-')
                {
                    if (permiteNegativos && limpio.Length == 0)
                        limpio.Append(c);
                }
                else if (c is '.' or ',')
                {
                    if (!haySeparador)
                    {
                        haySeparador = true;
                        limpio.Append(c);
                    }
                }
                else if (char.IsAsciiDigit(c))
                {
                    if (haySeparador && decimales < MaxDecimales)
                    {
                        limpio.Append(c);
                        decimales++;
                    }
                    else if (!haySeparador && enteros < MaxEnteros)
                    {
                        limpio.Append(c);
                        enteros++;
                    }
                }
            }

            return limpio.ToString();
        }

        public static ValidacionResult Validar(Magnitud? magnitud, string? texto, string? origen, string? destino)
        {
            if (magnitud is null)
                return ValidacionResult.Error("Seleccione qué desea convertir");

            ValidacionResult valor = ValidarValor(texto);
            if (!valor.EsValido)
                return valor;

            if (string.IsNullOrWhiteSpace(origen))
                return ValidacionResult.Error("Seleccione la unidad de origen");
            if (string.IsNullOrWhiteSpace(destino))
                return ValidacionResult.Error("Seleccione la unidad de destino");

            if (valor.Valor < 0 && !magnitud.Value.PermiteNegativos())
                return ValidacionResult.Error($"No se permiten valores negativos en {magnitud.Value.Nombre()}");

            if (magnitud == Magnitud.Temperatura
                && CeroAbsoluto.TryGetValue(origen.Normalize(NormalizationForm.FormC), out double cero)
                && valor.Valor < cero)
                return ValidacionResult.Error(
                    $"La temperatura no puede ser menor al cero absoluto ({FormatoNumero.Formatear(cero)} {origen})");

            return valor;
        }

        private static ValidacionResult ValidarValor(string? texto)
        {
            if (string.IsNullOrEmpty(texto))
                return ValidacionResult.Error("Ingrese un valor para convertir");
            if (texto.Any(char.IsWhiteSpace))
                return ValidacionResult.Error("El valor no debe contener espacios");

            if (!NumeroValido().IsMatch(texto))
                return ValidacionResult.Error(MotivoFormatoInvalido(texto));

            string sinSigno = texto.TrimStart('-');
            int posSeparador = sinSigno.IndexOfAny(['.', ',']);
            int enteros = posSeparador < 0 ? sinSigno.Length : posSeparador;
            int decimales = posSeparador < 0 ? 0 : sinSigno.Length - posSeparador - 1;

            if (enteros > MaxEnteros)
                return ValidacionResult.Error($"El valor admite como máximo {MaxEnteros} dígitos enteros");
            if (decimales > MaxDecimales)
                return ValidacionResult.Error($"El valor admite como máximo {MaxDecimales} decimales");

            string normalizado = texto.Replace(',', '.');
            if (!double.TryParse(normalizado, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture, out double numero)
                || !double.IsFinite(numero))
                return ValidacionResult.Error("El valor no es un número válido");

            if (Math.Abs(numero) > ValorMaximo)
                return ValidacionResult.Error("El valor está fuera del rango permitido");

            return ValidacionResult.Ok(numero == 0 ? 0 : numero);
        }

        private static string MotivoFormatoInvalido(string texto)
        {
            if (texto.Trim('-', '.', ',').Length == 0)
                return "Ingrese un número, no solo un signo o un separador";
            if (texto.Count(c => c is '.' or ',') > 1)
                return "Use un solo separador decimal (punto o coma)";
            if (texto.Contains('e', StringComparison.OrdinalIgnoreCase) && texto.Any(char.IsAsciiDigit))
                return "No se admite notación científica";
            if (texto.Any(char.IsLetter))
                return "El valor solo puede contener números";
            return "El valor no es un número válido";
        }
    }
}
