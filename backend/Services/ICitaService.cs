using AbogadosAPI.DTOs;

namespace AbogadosAPI.Services;

/// <summary>
/// Interfaz del servicio de citas
/// </summary>
/// <remarks>
/// Define las operaciones disponibles para gestionar citas y reuniones relacionadas con expedientes
/// </remarks>
public interface ICitaService
{
    /// <summary>
    /// Obtiene todas las citas
    /// </summary>
    /// <returns>Colección de citas ordenadas por fecha de inicio</returns>
    Task<IEnumerable<CitaDto>> GetAllAsync();
    
    /// <summary>
    /// Obtiene una cita por su identificador
    /// </summary>
    /// <param name="id">Identificador único de la cita</param>
    /// <returns>Cita si existe, null en caso contrario</returns>
    Task<CitaDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// Crea una nueva cita
    /// </summary>
    /// <param name="citaDto">Datos de la cita a crear</param>
    /// <returns>Cita creada con su identificador asignado</returns>
    Task<CitaDto> CreateAsync(CitaCreateDto citaDto);
    
    /// <summary>
    /// Actualiza una cita existente
    /// </summary>
    /// <param name="id">Identificador de la cita</param>
    /// <param name="citaDto">Nuevos datos de la cita</param>
    /// <returns>Cita actualizada si existe, null en caso contrario</returns>
    Task<CitaDto?> UpdateAsync(int id, CitaUpdateDto citaDto);
    
    /// <summary>
    /// Elimina una cita
    /// </summary>
    /// <param name="id">Identificador de la cita</param>
    /// <returns>True si se eliminó correctamente, false si no existe</returns>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Marca una cita como completada o pendiente
    /// </summary>
    /// <param name="id">Identificador de la cita</param>
    /// <param name="completada">True para marcar como completada, false para pendiente</param>
    /// <returns>Cita actualizada si existe, null en caso contrario</returns>
    Task<CitaDto?> MarcarCompletadaAsync(int id, bool completada);
    
    /// <summary>
    /// Obtiene todas las citas de un expediente
    /// </summary>
    /// <param name="expedienteId">Identificador del expediente</param>
    /// <returns>Colección de citas del expediente ordenadas por fecha de inicio</returns>
    Task<IEnumerable<CitaDto>> GetByExpedienteIdAsync(int expedienteId);
    
    /// <summary>
    /// Obtiene citas en un rango de fechas
    /// </summary>
    /// <param name="fechaInicio">Fecha inicial del rango</param>
    /// <param name="fechaFin">Fecha final del rango</param>
    /// <returns>Colección de citas dentro del rango de fechas</returns>
    Task<IEnumerable<CitaDto>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin);
    
    /// <summary>
    /// Obtiene las citas pendientes (no completadas y futuras)
    /// </summary>
    /// <returns>Colección de citas pendientes ordenadas por fecha de inicio</returns>
    Task<IEnumerable<CitaDto>> GetPendientesAsync();
}
