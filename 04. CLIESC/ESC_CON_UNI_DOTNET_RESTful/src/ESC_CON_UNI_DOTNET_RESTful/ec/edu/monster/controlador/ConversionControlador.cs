using ec.edu.monster.modelo;
using ec.edu.monster.vista;

namespace ec.edu.monster.controlador
{
  /// <summary>
  /// Maneja la pantalla de conversión: carga las unidades de la magnitud elegida,
  /// valida los datos y muestra el resultado con formato.
  /// </summary>
  public class ConversionControlador
  {
    private readonly IConversionVista vista;
    private readonly ConversorServicio servicio;

    public ConversionControlador(IConversionVista vista, ConversorServicio servicio)
    {
      this.vista = vista;
      this.servicio = servicio;
      vista.AlCambiarMagnitud(CargarUnidadesAsync);
      vista.AlConvertir(ConvertirAsync);
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
        // Nunca quedan elegibles las unidades de la magnitud anterior.
        vista.MostrarUnidades([]);
        vista.MostrarError(e.Message);
      }
    }

    private async Task ConvertirAsync()
    {
      vista.LimpiarMensajes();
      string? magnitud = vista.Magnitud;
      string? desde = vista.UnidadOrigen;
      string? hacia = vista.UnidadDestino;

      string? error = ValidadorConversion.Validar(vista.Valor, magnitud, desde, hacia);
      if (error != null)
      {
        vista.MostrarError(error);
        return;
      }

      double valor = ValidadorConversion.LeerValor(vista.Valor)!.Value;
      try
      {
        double resultado = await servicio.ConvertirAsync(magnitud!, valor, desde!, hacia!);
        vista.MostrarResultado(FormatoResultado.Formatear(valor, desde!, resultado, hacia!));
      }
      catch (ServicioException e)
      {
        vista.MostrarError(e.Message);
      }
    }
  }
}
