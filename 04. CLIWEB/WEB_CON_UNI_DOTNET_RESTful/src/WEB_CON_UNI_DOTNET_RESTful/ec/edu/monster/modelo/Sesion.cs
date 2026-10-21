namespace ec.edu.monster.modelo
{
  /// <summary>
  /// Estado de la sesión del usuario: inicio, estado activo y cierre.
  /// </summary>
  public class Sesion
  {
    public string? Usuario { get; private set; }
    public string? Token { get; private set; }

    public void Iniciar(string usuario, string token)
    {
      Usuario = usuario;
      Token = token;
    }

    public void Cerrar()
    {
      Usuario = null;
      Token = null;
    }

    public bool EstaActiva()
    {
      return Token != null;
    }
  }
}
