namespace ec.edu.monster.modelo
{
  /// <summary>
  /// Error del servidor RESTful con un mensaje listo para mostrar al usuario.
  /// </summary>
  public class ServicioException : Exception
  {
    public ServicioException(string mensaje) : base(mensaje) { }

    public ServicioException(string mensaje, Exception causa) : base(mensaje, causa) { }
  }
}
