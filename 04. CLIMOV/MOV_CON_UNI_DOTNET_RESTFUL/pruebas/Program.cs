using MOV_CON_UNI_DOTNET_RESTFUL.Pruebas;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var suites = new List<(string Titulo, Ejecutor Ejecutor)>
{
    ("Validador de conversión", PruebasValidador.Crear()),
    ("Formato del resultado", PruebasFormato.Crear()),
    ("Errores centrales (RestErrores)", PruebasErrores.Crear()),
    ("Repositorios y controladores (HTTP simulado)", PruebasRepositorios.Crear()),
};

int posicion = Array.IndexOf(args, "--contrato");
if (posicion >= 0)
{
    string url = posicion + 1 < args.Length ? args[posicion + 1] : "http://127.0.0.1:5013/";
    suites.Add(($"Contrato con el servidor real ({url})", PruebasContrato.Crear(url)));
}

int total = 0, fallos = 0;
foreach (var (titulo, ejecutor) in suites)
{
    var (t, f) = await ejecutor.EjecutarAsync(titulo);
    total += t;
    fallos += f;
}

Console.WriteLine($"\nResultado: {total - fallos}/{total} pruebas correctas, {fallos} fallidas.");
return fallos == 0 ? 0 : 1;
