using ec.edu.monster.modelo.dto;

namespace ec.edu.monster.modelo.http
{
  /// <summary>
  /// Conversión y catálogo del servidor; ambas exigen el token de la sesión.
  /// </summary>
  public interface IConversorApi
  {
    Task<ConversorResponseDTO> ConvertAsync(string token, string type, double value, string from, string to);

    Task<CatalogResponseDTO> CatalogAsync(string token, string type);
  }
}
