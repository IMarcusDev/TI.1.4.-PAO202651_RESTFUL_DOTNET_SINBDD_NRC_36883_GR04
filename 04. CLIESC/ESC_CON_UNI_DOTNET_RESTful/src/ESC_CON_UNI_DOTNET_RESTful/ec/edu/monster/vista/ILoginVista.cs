namespace ec.edu.monster.vista
{
  /// <summary>
  /// Contrato entre la pantalla de login y su controlador. La ventana WinForms lo
  /// implementa; las pruebas usan una vista falsa, así el controlador no depende de WinForms.
  /// </summary>
  public interface ILoginVista
  {
    string Usuario { get; }

    string Clave { get; }

    void MostrarError(string mensaje);

    void AbrirMenu();

    void AlIngresar(Func<Task> accion);
  }
}
