using ec.edu.monster.controlador;
using ec.edu.monster.modelo;
using ec.edu.monster.vista;

namespace ec.edu.monster
{
  /// <summary>
  /// Punto de entrada: crea el modelo una sola vez y conecta cada ventana con sus
  /// controladores. Maneja la navegación entre el login y la ventana principal.
  /// </summary>
  public sealed class App : ApplicationContext
  {
    private readonly ConversorServicio servicio;
    private readonly IEstadoServidor estado;

    private App(ConversorServicio servicio, IEstadoServidor estado)
    {
      this.servicio = servicio;
      this.estado = estado;
      MostrarLogin();
    }

    [STAThread]
    public static void Main()
    {
      ApplicationConfiguration.Initialize();

      string url = Configuracion.LeerUrlServidor(AppContext.BaseDirectory);
      var servicio = ClienteRest.CrearServicio(url, new Sesion());
      Application.Run(new App(servicio, new EstadoServidor(url)));
    }

    private void MostrarLogin()
    {
      var login = new LoginFrame(MostrarPrincipal);
      _ = new LoginControlador(login, servicio);
      _ = new EstadoControlador(login.Estado, estado);
      Abrir(login);
    }

    private void MostrarPrincipal()
    {
      var principal = new PrincipalFrame(() =>
      {
        try
        {
          servicio.CerrarSesion();
        }
        finally
        {
          MostrarLogin();
        }
      });

      var conversion = new ConversionControlador(principal.ConversionPanel, servicio);
      var catalogo = new CatalogoControlador(principal.CatalogoPanel, servicio);
      _ = new EstadoControlador(principal.Estado, estado);

      principal.Shown += async (_, _) =>
      {
        await conversion.IniciarAsync();
        await catalogo.IniciarAsync();
      };
      Abrir(principal);
    }

    // La nueva ventana pasa a ser la principal antes de cerrar la anterior,
    // así la aplicación no termina al cambiar de pantalla.
    private void Abrir(Form ventana)
    {
      MainForm = ventana;
      ventana.Show();
    }
  }
}
