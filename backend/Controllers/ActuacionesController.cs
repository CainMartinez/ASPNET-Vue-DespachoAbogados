using Microsoft.AspNetCore.Mvc;
using AbogadosAPI.DTOs;
using AbogadosAPI.Services;

namespace AbogadosAPI.Controllers;

/// <summary>
/// Controlador para gestionar actuaciones
/// </summary>
/// <remarks>
/// Proporciona endpoints para operaciones CRUD de actuaciones procesales y consultas especializadas
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class ActuacionesController : ControllerBase
{
    private readonly IActuacionService _actuacionService;
    private readonly ILogger<ActuacionesController> _logger;

    /// <summary>
    /// Constructor del controlador de actuaciones
    /// </summary>
    /// <param name="actuacionService">Servicio de actuaciones</param>
    /// <param name="logger">Logger para registrar eventos</param>
    public ActuacionesController(IActuacionService actuacionService, ILogger<ActuacionesController> logger)
    {
        _actuacionService = actuacionService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las actuaciones
    /// </summary>
    /// <returns>Lista de actuaciones ordenadas por fecha descendente</returns>
    /// <response code="200">Actuaciones obtenidas exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ActuacionDto>>> GetAll()
    {
        try
        {
            var actuaciones = await _actuacionService.GetAllAsync();
            return Ok(actuaciones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener actuaciones");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene una actuación por su ID
    /// </summary>
    /// <param name="id">Identificador de la actuación</param>
    /// <returns>Actuación solicitada</returns>
    /// <response code="200">Actuación encontrada</response>
    /// <response code="404">Actuación no encontrada</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<ActuacionDto>> GetById(int id)
    {
        try
        {
            var actuacion = await _actuacionService.GetByIdAsync(id);
            
            if (actuacion == null)
            {
                return NotFound(new { mensaje = $"Actuación con ID {id} no encontrada" });
            }

            return Ok(actuacion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la actuación {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Crea una nueva actuación
    /// </summary>
    /// <param name="actuacionDto">Datos de la actuación a crear</param>
    /// <returns>Actuación creada</returns>
    /// <response code="201">Actuación creada exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost]
    public async Task<ActionResult<ActuacionDto>> Create([FromBody] ActuacionCreateDto actuacionDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var actuacion = await _actuacionService.CreateAsync(actuacionDto);
            return CreatedAtAction(nameof(GetById), new { id = actuacion.Id }, actuacion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la actuación");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza una actuación existente
    /// </summary>
    /// <param name="id">Identificador de la actuación</param>
    /// <param name="actuacionDto">Nuevos datos de la actuación</param>
    /// <returns>Actuación actualizada</returns>
    /// <response code="200">Actuación actualizada exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="404">Actuación no encontrada</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<ActuacionDto>> Update(int id, [FromBody] ActuacionUpdateDto actuacionDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var actuacion = await _actuacionService.UpdateAsync(id, actuacionDto);
            
            if (actuacion == null)
            {
                return NotFound(new { mensaje = $"Actuación con ID {id} no encontrada" });
            }

            return Ok(actuacion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la actuación {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Elimina una actuación
    /// </summary>
    /// <param name="id">Identificador de la actuación</param>
    /// <returns>Resultado de la eliminación</returns>
    /// <response code="204">Actuación eliminada exitosamente</response>
    /// <response code="404">Actuación no encontrada</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var resultado = await _actuacionService.DeleteAsync(id);
            
            if (!resultado)
            {
                return NotFound(new { mensaje = $"Actuación con ID {id} no encontrada" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la actuación {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene actuaciones por expediente
    /// </summary>
    /// <param name="expedienteId">Identificador del expediente</param>
    /// <returns>Lista de actuaciones del expediente</returns>
    /// <response code="200">Actuaciones obtenidas exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("expediente/{expedienteId}")]
    public async Task<ActionResult<IEnumerable<ActuacionDto>>> GetByExpediente(int expedienteId)
    {
        try
        {
            var actuaciones = await _actuacionService.GetByExpedienteIdAsync(expedienteId);
            return Ok(actuaciones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener actuaciones del expediente {ExpedienteId}", expedienteId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene actuaciones por rango de fechas
    /// </summary>
    /// <param name="fechaInicio">Fecha inicial del rango</param>
    /// <param name="fechaFin">Fecha final del rango</param>
    /// <returns>Lista de actuaciones dentro del rango de fechas</returns>
    /// <response code="200">Actuaciones obtenidas exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("rango-fechas")]
    public async Task<ActionResult<IEnumerable<ActuacionDto>>> GetByFechaRango(
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin)
    {
        try
        {
            var actuaciones = await _actuacionService.GetByFechaRangoAsync(fechaInicio, fechaFin);
            return Ok(actuaciones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener actuaciones por rango de fechas");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }
}
