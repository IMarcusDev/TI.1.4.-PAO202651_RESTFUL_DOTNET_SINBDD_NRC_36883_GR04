namespace ec.edu.monster.controlador
{
  // Datos que cada controlador entrega a su vista.

  public class LoginVista
  {
    public string Usuario { get; set; } = "";
    public string? Error { get; set; }
  }

  public class ConversionVista
  {
    public IReadOnlyList<Opcion> Magnitudes { get; set; } = [];
    public IReadOnlyList<Opcion> Unidades { get; set; } = [];
    public string? Magnitud { get; set; }
    public string? Desde { get; set; }
    public string? Hacia { get; set; }
    public string Valor { get; set; } = "";
    public string? Resultado { get; set; }
    public string? Error { get; set; }
  }

  public class CatalogoVista
  {
    public IReadOnlyList<Opcion> Magnitudes { get; set; } = [];
    public IReadOnlyList<Opcion> Unidades { get; set; } = [];
    public string Magnitud { get; set; } = "";
    public string? Error { get; set; }
  }
}
