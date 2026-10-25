namespace ec.edu.monster.vista
{
  /// <summary>
  /// Contrato entre la pantalla de catálogo y su controlador.
  /// </summary>
  public interface ICatalogoVista
  {
    /// <summary>Muestra las magnitudes y deja elegida la primera, sin avisar al controlador.</summary>
    void MostrarMagnitudes(IReadOnlyList<string> magnitudes);

    string? Magnitud { get; }

    void MostrarUnidades(IReadOnlyList<string> unidades);

    void MostrarError(string mensaje);

    /// <summary>Oculta el error anterior antes de cargar otro catálogo.</summary>
    void LimpiarMensajes();

    void AlCambiarMagnitud(Func<Task> accion);
  }
}
