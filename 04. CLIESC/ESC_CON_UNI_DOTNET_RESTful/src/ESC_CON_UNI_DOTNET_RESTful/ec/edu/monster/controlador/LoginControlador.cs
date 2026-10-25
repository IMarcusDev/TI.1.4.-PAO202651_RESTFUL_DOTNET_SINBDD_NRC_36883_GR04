using ec.edu.monster.modelo;
using ec.edu.monster.vista;

namespace ec.edu.monster.controlador
{
  /// <summary>
  /// Maneja la pantalla de login: valida los datos, pide al modelo iniciar sesión
  /// y le dice a la vista qué mostrar después.
  /// </summary>
  public class LoginControlador
  {
    public const string CredencialesVacias = "Ingrese usuario y contraseña";

    private readonly ILoginVista vista;
    private readonly ConversorServicio servicio;

    public LoginControlador(ILoginVista vista, ConversorServicio servicio)
    {
      this.vista = vista;
      this.servicio = servicio;
      vista.AlIngresar(IngresarAsync);
    }

    private async Task IngresarAsync()
    {
      string usuario = vista.Usuario;
      string clave = vista.Clave;

      if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(clave))
      {
        vista.MostrarError(CredencialesVacias);
        return;
      }

      try
      {
        await servicio.IniciarSesionAsync(usuario.Trim(), clave);
        vista.AbrirMenu();
      }
      catch (ServicioException e)
      {
        vista.MostrarError(e.Message);
      }
    }
  }
}
