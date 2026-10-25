using MOV_CON_UNI_DOTNET_RESTFUL.Utils;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Pruebas
{
    public static class PruebasFormato
    {
        public static Ejecutor Crear()
        {
            var e = new Ejecutor();

            (double Valor, string Esperado)[] casos =
            [
                (0, "0"),
                (-0.0, "0"),
                (1, "1"),
                (-40, "-40"),
                (0.5, "0,5"),
                (247.10516301527602, "247,1052"),
                (1234.56789, "1.234,5679"),
                (1234567.891, "1.234.567,891"),
                (-1234567.5, "-1.234.567,5"),
                (0.00000254, "0,00000254"),
                (0.1234567891234, "0,1234567891"),
                (1e-12, "≈ 0"),
                (-1e-12, "≈ 0"),
                (double.NaN, "—"),
                (double.PositiveInfinity, "—"),
            ];
            foreach (var (valor, esperado) in casos)
                e.Caso($"Formatear {valor:R} → «{esperado}»", () => Afirmar.Igual(esperado, FormatoNumero.Formatear(valor)));

            e.Caso("Número grande sin notación científica", () =>
            {
                string texto = FormatoNumero.Formatear(6.047999999999994E+17);
                Afirmar.Verdadero(!texto.Contains('E') && !texto.Contains('e'), texto);
                Afirmar.EmpiezaCon("604.79", texto);
                Afirmar.Verdadero(texto.Length > 12, "debía activar la letra pequeña (más de 12 caracteres)");
            });

            return e;
        }
    }
}
