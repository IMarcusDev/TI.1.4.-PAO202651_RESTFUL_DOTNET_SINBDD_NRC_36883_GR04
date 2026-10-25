using ec.edu.monster.modelo.dto;

namespace ec.edu.monster.modelo.http
{
  /// <summary>
  /// Operación de login del servidor.
  /// </summary>
  public interface ILoginApi
  {
    Task<LoginResponseDTO> LoginAsync(string usuario, string clave);
  }
}
