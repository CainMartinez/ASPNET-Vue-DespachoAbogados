using AbogadosAPI.DTOs;
using AbogadosAPI.Models;

namespace AbogadosAPI.Services;

/// <summary>
/// Interfaz del servicio de expedientes
/// </summary>
/// <remarks>
/// Define las operaciones disponibles para gestionar expedientes jurídicos
/// </remarks>
public interface IExpedienteService
{
    /// <summary>
    /// Obtiene todos los expedientes con información completa
    /// </summary>
    /// <returns>Colección de expedientes con sus relaciones</returns>
    Task<IEnumerable<ExpedienteDto>> GetAllAsync();
    
    /// <summary>
    /// Obtiene un resumen de todos los expedientes
    /// </summary>
    /// <returns>Colección de expedientes con información básica</returns>
    Task<IEnumerable<ExpedienteResumenDto>> GetResumenAsync();
    
    /// <summary>
    /// Obtiene un expediente por su identificador
    /// </summary>
    /// <param name="id">Identificador único del expediente</param>
    /// <returns>Expediente si existe, null en caso contrario</returns>
    Task<ExpedienteDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// Crea un nuevo expediente
    /// </summary>
    /// <param name="expedienteDto">Datos del expediente a crear</param>
    /// <returns>Expediente creado con su identificador asignado</returns>
    Task<ExpedienteDto> CreateAsync(ExpedienteCreateDto expedienteDto);
    
    /// <summary>
    /// Actualiza un expediente existente
    /// </summary>
    /// <param name="id">Identificador del expediente</param>
    /// <param name="expedienteDto">Nuevos datos del expediente</param>
    /// <returns>Expediente actualizado si existe, null en caso contrario</returns>
    Task<ExpedienteDto?> UpdateAsync(int id, ExpedienteUpdateDto expedienteDto);
    
    /// <summary>
    /// Elimina un expediente
    /// </summary>
    /// <param name="id">Identificador del expediente</param>
    /// <returns>True si se eliminó correctamente, false si no existe</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Cambia el estado de un expediente
    /// </summary>
    /// <param name="id">Identificador del expediente</param>
    /// <param name="estadoDto">Nuevo estado y observaciones</param>
    /// <returns>Expediente actualizado si existe, null en caso contrario</returns>
    Task<ExpedienteDto?> CambiarEstadoAsync(int id, ExpedienteCambiarEstadoDto estadoDto);
    
    /// <summary>
    /// Obtiene todos los expedientes de un cliente
    /// </summary>
    /// <param name="clienteId">Identificador del cliente</param>
    /// <returns>Colección de expedientes del cliente</returns>
    Task<IEnumerable<ExpedienteDto>> GetByClienteIdAsync(int clienteId);
    
    /// <summary>
    /// Obtiene expedientes por estado
    /// </summary>
    /// <param name="estado">Estado del expediente a buscar</param>
    /// <returns>Colección de expedientes con el estado especificado</returns>
    Task<IEnumerable<ExpedienteDto>> GetByEstadoAsync(Estado estado);
    
    /// <summary>
    /// Busca expedientes por término de búsqueda
    /// </summary>
    /// <param name="searchTerm">Término a buscar en número, asunto, tipo o número de procedimiento</param>
    /// <returns>Colección de expedientes que coinciden con la búsqueda</returns>
    Task<IEnumerable<ExpedienteDto>> SearchAsync(string searchTerm);
}
