namespace ec.edu.monster.vista
{
  /// <summary>
  /// Contrato entre la pantalla de conversión y su controlador.
  /// </summary>
  public interface IConversionVista
  {
    /// <summary>Muestra las magnitudes y deja elegida la primera, sin avisar al controlador.</summary>
    void MostrarMagnitudes(IReadOnlyList<string> magnitudes);

    /// <summary>Muestra las unidades; elige la primera como origen y la segunda como destino.</summary>
    void MostrarUnidades(IReadOnlyList<string> unidades);

    string? Magnitud { get; }

    string Valor { get; }

    string? UnidadOrigen { get; }

    string? UnidadDestino { get; }

    void MostrarResultado(string texto);

    void MostrarError(string mensaje);

    /// <summary>Oculta el resultado y el error anteriores antes de una nueva acción.</summary>
    void LimpiarMensajes();

    void AlConvertir(Func<Task> accion);

    void AlCambiarMagnitud(Func<Task> accion);
  }
}
