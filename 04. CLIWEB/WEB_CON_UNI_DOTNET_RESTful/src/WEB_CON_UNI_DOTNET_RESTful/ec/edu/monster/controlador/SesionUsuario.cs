using ec.edu.monster.modelo;

namespace ec.edu.monster.controlador
{
  /// <summary>
  /// Guarda y lee la sesión del usuario en la sesión HTTP de ASP.NET Core.
  /// </summary>
  public static class SesionUsuario
  {
    private const string ClaveUsuario = "usuario";
    private const string ClaveToken = "token";

    public static bool Activa(HttpContext contexto)
    {
      return Leer(contexto).EstaActiva();
    }

    public static Sesion Leer(HttpContext contexto)
    {
      var sesion = new Sesion();
      string? usuario = contexto.Session.GetString(ClaveUsuario);
      string? token = contexto.Session.GetString(ClaveToken);

      if (usuario != null && token != null)
        sesion.Iniciar(usuario, token);

      return sesion;
    }

    public static void Guardar(HttpContext contexto, Sesion sesion)
    {
      // Se descarta todo lo anterior para no reutilizar datos de otra sesión.
      contexto.Session.Clear();

      if (sesion.EstaActiva())
      {
        contexto.Session.SetString(ClaveUsuario, sesion.Usuario!);
        contexto.Session.SetString(ClaveToken, sesion.Token!);
      }
    }

    public static void Cerrar(HttpContext contexto)
    {
      contexto.Session.Clear();
    }
  }
}
