using System.Globalization;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Utils
{
    public static class FormatoNumero
    {
        private static readonly NumberFormatInfo Simbolos = new()
        {
            NumberDecimalSeparator = ",",
            NumberGroupSeparator = ".",
            NegativeSign = "-",
        };

        public static string Formatear(double valor)
        {
            if (!double.IsFinite(valor))
                return "—";
            if (valor == 0)
                return "0";

            string patron = Math.Abs(valor) >= 1 ? "#,##0.####" : "0.##########";
            string texto = valor.ToString(patron, Simbolos);

            return texto is "0" or "-0" ? "≈ 0" : texto;
        }
    }
}
