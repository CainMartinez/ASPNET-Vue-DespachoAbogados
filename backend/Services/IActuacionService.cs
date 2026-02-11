using AbogadosAPI.DTOs;

namespace AbogadosAPI.Services;

/// <summary>
/// Interfaz del servicio de actuaciones
/// </summary>
/// <remarks>
/// Define las operaciones disponibles para gestionar actuaciones procesales de expedientes
/// </remarks>
public interface IActuacionService
{
    /// <summary>
    /// Obtiene todas las actuaciones
    /// </summary>
    /// <returns>Colección de actuaciones ordenadas por fecha descendente</returns>
    Task<IEnumerable<ActuacionDto>> GetAllAsync();
    
    /// <summary>
    /// Obtiene una actuación por su identificador
    /// </summary>
    /// <param name="id">Identificador único de la actuación</param>
    /// <returns>Actuación si existe, null en caso contrario</returns>
    Task<ActuacionDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// Crea una nueva actuación
    /// </summary>
    /// <param name="actuacionDto">Datos de la actuación a crear</param>
    /// <returns>Actuación creada con su identificador asignado</returns>
    Task<ActuacionDto> CreateAsync(ActuacionCreateDto actuacionDto);
    
    /// <summary>
    /// Actualiza una actuación existente
    /// </summary>
    /// <param name="id">Identificador de la actuación</param>
    /// <param name="actuacionDto">Nuevos datos de la actuación</param>
    /// <returns>Actuación actualizada si existe, null en caso contrario</returns>
    Task<ActuacionDto?> UpdateAsync(int id, ActuacionUpdateDto actuacionDto);
    
    /// <summary>
    /// Elimina una actuación
    /// </summary>
    /// <param name="id">Identificador de la actuación</param>
    /// <returns>True si se eliminó correctamente, false si no existe</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Obtiene todas las actuaciones de un expediente
    /// </summary>
    /// <param name="expedienteId">Identificador del expediente</param>
    /// <returns>Colección de actuaciones del expediente ordenadas por fecha descendente</returns>
    Task<IEnumerable<ActuacionDto>> GetByExpedienteIdAsync(int expedienteId);
    
    /// <summary>
    /// Obtiene actuaciones en un rango de fechas
    /// </summary>
    /// <param name="fechaInicio">Fecha inicial del rango</param>
    /// <param name="fechaFin">Fecha final del rango</param>
    /// <returns>Colección de actuaciones dentro del rango de fechas</returns>
    Task<IEnumerable<ActuacionDto>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin);
}
