using ec.edu.monster.vista;

namespace ec.edu.monster.pruebas
{
  // Vistas en memoria: guardan lo que el controlador les pide mostrar y
  // permiten simular lo que haría el usuario.

  public class VistaLoginFalsa : ILoginVista
  {
    private Func<Task> accionIngresar = () => Task.CompletedTask;

    public string Usuario { get; set; } = "";
    public string Clave { get; set; } = "";
    public string? Error { get; private set; }
    public bool MenuAbierto { get; private set; }

    public void MostrarError(string mensaje) => Error = mensaje;
    public void AbrirMenu() => MenuAbierto = true;
    public void AlIngresar(Func<Task> accion) => accionIngresar = accion;

    public Task PresionarIngresar() => accionIngresar();
  }

  public class VistaConversionFalsa : IConversionVista
  {
    private Func<Task> accionConvertir = () => Task.CompletedTask;
    private Func<Task> accionCambiarMagnitud = () => Task.CompletedTask;

    public IReadOnlyList<string> Magnitudes { get; private set; } = [];
    public IReadOnlyList<string> Unidades { get; private set; } = [];
    public string? Magnitud { get; set; }
    public string Valor { get; set; } = "";
    public string? UnidadOrigen { get; set; }
    public string? UnidadDestino { get; set; }
    public string? Resultado { get; private set; }
    public string? Error { get; private set; }

    public void MostrarMagnitudes(IReadOnlyList<string> magnitudes)
    {
      Magnitudes = magnitudes;
      Magnitud = magnitudes.Count > 0 ? magnitudes[0] : null;
    }

    // Igual que el panel real: origen la primera unidad, destino la segunda.
    public void MostrarUnidades(IReadOnlyList<string> unidades)
    {
      Unidades = unidades;
      UnidadOrigen = unidades.Count > 0 ? unidades[0] : null;
      UnidadDestino = unidades.Count > 1 ? unidades[1] : UnidadOrigen;
    }

    public void MostrarResultado(string texto) => Resultado = texto;
    public void MostrarError(string mensaje) => Error = mensaje;

    public void LimpiarMensajes()
    {
      Resultado = null;
      Error = null;
    }

    public void AlConvertir(Func<Task> accion) => accionConvertir = accion;
    public void AlCambiarMagnitud(Func<Task> accion) => accionCambiarMagnitud = accion;

    public Task SeleccionarMagnitud(string magnitud)
    {
      Magnitud = magnitud;
      return accionCambiarMagnitud();
    }

    public Task PresionarConvertir() => accionConvertir();
  }

  public class VistaCatalogoFalsa : ICatalogoVista
  {
    private Func<Task> accionCambiarMagnitud = () => Task.CompletedTask;

    public IReadOnlyList<string> Magnitudes { get; private set; } = [];
    public IReadOnlyList<string> Unidades { get; private set; } = [];
    public string? Magnitud { get; set; }
    public string? Error { get; private set; }

    public void MostrarMagnitudes(IReadOnlyList<string> magnitudes)
    {
      Magnitudes = magnitudes;
      Magnitud = magnitudes.Count > 0 ? magnitudes[0] : null;
    }

    public void MostrarUnidades(IReadOnlyList<string> unidades) => Unidades = unidades;
    public void MostrarError(string mensaje) => Error = mensaje;
    public void LimpiarMensajes() => Error = null;
    public void AlCambiarMagnitud(Func<Task> accion) => accionCambiarMagnitud = accion;

    public Task SeleccionarMagnitud(string magnitud)
    {
      Magnitud = magnitud;
      return accionCambiarMagnitud();
    }
  }

  public class VistaEstadoFalsa : IEstadoVista
  {
    private Func<Task> accionRevisar = () => Task.CompletedTask;

    public bool? Disponible { get; private set; }

    public void MostrarEstado(bool disponible) => Disponible = disponible;
    public void AlRevisarEstado(Func<Task> accion) => accionRevisar = accion;

    public Task Revisar() => accionRevisar();
  }
}
