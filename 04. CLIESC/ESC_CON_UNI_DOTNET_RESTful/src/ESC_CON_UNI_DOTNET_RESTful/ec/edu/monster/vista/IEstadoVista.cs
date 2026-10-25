namespace ec.edu.monster.vista
{
  /// <summary>
  /// Indicador del servidor (punto verde o rojo). La ventana llama a la acción
  /// registrada al abrirse y luego cada pocos segundos.
  /// </summary>
  public interface IEstadoVista
  {
    void MostrarEstado(bool disponible);

    void AlRevisarEstado(Func<Task> accion);
  }
}
