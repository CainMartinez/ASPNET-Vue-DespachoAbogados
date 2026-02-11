using AbogadosAPI.DTOs;

namespace AbogadosAPI.Services;

/// <summary>
/// Interfaz del servicio de clientes
/// </summary>
/// <remarks>
/// Define las operaciones disponibles para gestionar clientes del despacho de abogados
/// </remarks>
public interface IClienteService
{
    /// <summary>
    /// Obtiene todos los clientes
    /// </summary>
    /// <returns>Colección de clientes registrados en el sistema</returns>
    Task<IEnumerable<ClienteDto>> GetAllAsync();
    
    /// <summary>
    /// Obtiene un cliente por su identificador
    /// </summary>
    /// <param name="id">Identificador único del cliente</param>
    /// <returns>Cliente si existe, null en caso contrario</returns>
    Task<ClienteDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// Crea un nuevo cliente
    /// </summary>
    /// <param name="clienteDto">Datos del cliente a crear</param>
    /// <returns>Cliente creado con su identificador asignado</returns>
    Task<ClienteDto> CreateAsync(ClienteCreateDto clienteDto);
    
    /// <summary>
    /// Actualiza un cliente existente
    /// </summary>
    /// <param name="id">Identificador del cliente</param>
    /// <param name="clienteDto">Nuevos datos del cliente</param>
    /// <returns>Cliente actualizado si existe, null en caso contrario</returns>
    Task<ClienteDto?> UpdateAsync(int id, ClienteUpdateDto clienteDto);
    
    /// <summary>
    /// Elimina un cliente
    /// </summary>
    /// <param name="id">Identificador del cliente</param>
    /// <returns>True si se eliminó correctamente, false si no existe</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Busca clientes por término de búsqueda
    /// </summary>
    /// <param name="searchTerm">Término a buscar en nombre, apellidos, DNI/CIF o email</param>
    /// <returns>Colección de clientes que coinciden con la búsqueda</returns>
    Task<IEnumerable<ClienteDto>> SearchAsync(string searchTerm);
}
