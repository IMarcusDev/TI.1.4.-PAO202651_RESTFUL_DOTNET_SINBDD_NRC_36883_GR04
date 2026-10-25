using ec.edu.monster.modelo;
using ec.edu.monster.vista;

namespace ec.edu.monster.controlador
{
  /// <summary>
  /// Maneja la pantalla de catálogo: lista las unidades de la magnitud elegida.
  /// </summary>
  public class CatalogoControlador
  {
    private readonly ICatalogoVista vista;
    private readonly ConversorServicio servicio;

    public CatalogoControlador(ICatalogoVista vista, ConversorServicio servicio)
    {
      this.vista = vista;
      this.servicio = servicio;
      vista.AlCambiarMagnitud(CargarUnidadesAsync);
    }

    /// <summary>Muestra las magnitudes y carga las unidades de la primera.</summary>
    public Task IniciarAsync()
    {
      vista.MostrarMagnitudes(Magnitudes.Todas);
      return CargarUnidadesAsync();
    }

    private async Task CargarUnidadesAsync()
    {
      vista.LimpiarMensajes();
      string? magnitud = vista.Magnitud;

      if (!Magnitudes.Existe(magnitud))
      {
        vista.MostrarUnidades([]);
        return;
      }

      try
      {
        vista.MostrarUnidades(await servicio.CatalogoAsync(magnitud!));
      }
      catch (ServicioException e)
      {
        // Nunca quedan en pantalla las unidades de la magnitud anterior.
        vista.MostrarUnidades([]);
        vista.MostrarError(e.Message);
      }
    }
  }
}
