using ec.edu.monster.modelo;
using ec.edu.monster.vista;

namespace ec.edu.monster.controlador
{
  /// <summary>
  /// Mantiene al día el indicador del servidor: verde si responde, rojo si no.
  /// </summary>
  public class EstadoControlador
  {
    private readonly IEstadoVista vista;
    private readonly IEstadoServidor estado;

    public EstadoControlador(IEstadoVista vista, IEstadoServidor estado)
    {
      this.vista = vista;
      this.estado = estado;
      vista.AlRevisarEstado(RevisarAsync);
    }

    private async Task RevisarAsync()
    {
      vista.MostrarEstado(await estado.DisponibleAsync());
    }
  }
}
