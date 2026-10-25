namespace MOV_CON_UNI_DOTNET_RESTFUL.Models.data
{
    public static class NombresUnidades
    {
        private static readonly Dictionary<string, string> Nombres = new()
        {
            ["m"] = "Metro",
            ["cm"] = "Centímetro",
            ["ft"] = "Pie",
            ["yd"] = "Yarda",
            ["mi"] = "Milla",
            ["g"] = "Gramo",
            ["kg"] = "Kilogramo",
            ["oz"] = "Onza",
            ["lb"] = "Libra",
            ["qq"] = "Quintal",
            ["°C"] = "Celsius",
            ["°F"] = "Fahrenheit",
            ["K"] = "Kelvin",
            ["°N"] = "Newton",
            ["°Ré"] = "Réaumur",
            ["mL"] = "Mililitro",
            ["L"] = "Litro",
            ["m3"] = "Metro cúbico",
            ["ft3"] = "Pie cúbico",
            ["gal"] = "Galón",
            ["ms"] = "Milisegundo",
            ["s"] = "Segundo",
            ["min"] = "Minuto",
            ["h"] = "Hora",
            ["d"] = "Día",
            ["w"] = "Semana",
            ["cm2"] = "Centímetro cuadrado",
            ["m2"] = "Metro cuadrado",
            ["km2"] = "Kilómetro cuadrado",
            ["ha"] = "Hectárea",
            ["ft2"] = "Pie cuadrado",
            ["ac"] = "Acre",
        };

        public static string Nombre(string codigo) => Nombres.GetValueOrDefault(codigo, codigo);
    }
}
