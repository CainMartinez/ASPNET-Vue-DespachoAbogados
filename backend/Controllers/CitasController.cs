using Microsoft.AspNetCore.Mvc;
using AbogadosAPI.DTOs;
using AbogadosAPI.Services;

namespace AbogadosAPI.Controllers;

/// <summary>
/// Controlador para gestionar citas
/// </summary>
/// <remarks>
/// Proporciona endpoints para operaciones CRUD de citas y reuniones, y consultas especializadas
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class CitasController : ControllerBase
{
    private readonly ICitaService _citaService;
    private readonly ILogger<CitasController> _logger;

    /// <summary>
    /// Constructor del controlador de citas
    /// </summary>
    /// <param name="citaService">Servicio de citas</param>
    /// <param name="logger">Logger para registrar eventos</param>
    public CitasController(ICitaService citaService, ILogger<CitasController> logger)
    {
        _citaService = citaService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las citas
    /// </summary>
    /// <returns>Lista de citas ordenadas por fecha de inicio</returns>
    /// <response code="200">Citas obtenidas exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CitaDto>>> GetAll()
    {
        try
        {
            var citas = await _citaService.GetAllAsync();
            return Ok(citas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener citas");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene una cita por su ID
    /// </summary>
    /// <param name="id">Identificador de la cita</param>
    /// <returns>Cita solicitada</returns>
    /// <response code="200">Cita encontrada</response>
    /// <response code="404">Cita no encontrada</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<CitaDto>> GetById(int id)
    {
        try
        {
            var cita = await _citaService.GetByIdAsync(id);
            
            if (cita == null)
            {
                return NotFound(new { mensaje = $"Cita con ID {id} no encontrada" });
            }

            return Ok(cita);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la cita {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Crea una nueva cita
    /// </summary>
    /// <param name="citaDto">Datos de la cita a crear</param>
    /// <returns>Cita creada</returns>
    /// <response code="201">Cita creada exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost]
    public async Task<ActionResult<CitaDto>> Create([FromBody] CitaCreateDto citaDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cita = await _citaService.CreateAsync(citaDto);
            return CreatedAtAction(nameof(GetById), new { id = cita.Id }, cita);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la cita");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza una cita existente
    /// </summary>
    /// <param name="id">Identificador de la cita</param>
    /// <param name="citaDto">Nuevos datos de la cita</param>
    /// <returns>Cita actualizada</returns>
    /// <response code="200">Cita actualizada exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="404">Cita no encontrada</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<CitaDto>> Update(int id, [FromBody] CitaUpdateDto citaDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cita = await _citaService.UpdateAsync(id, citaDto);
            
            if (cita == null)
            {
                return NotFound(new { mensaje = $"Cita con ID {id} no encontrada" });
            }

            return Ok(cita);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la cita {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Elimina una cita
    /// </summary>
    /// <param name="id">Identificador de la cita</param>
    /// <returns>Resultado de la eliminación</returns>
    /// <response code="204">Cita eliminada exitosamente</response>
    /// <response code="404">Cita no encontrada</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var resultado = await _citaService.DeleteAsync(id);
            
            if (!resultado)
            {
                return NotFound(new { mensaje = $"Cita con ID {id} no encontrada" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la cita {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Marca una cita como completada o pendiente
    /// </summary>
    /// <param name="id">Identificador de la cita</param>
    /// <param name="completada">True para marcar como completada, false para pendiente</param>
    /// <returns>Cita actualizada</returns>
    /// <response code="200">Estado de cita actualizado exitosamente</response>
    /// <response code="404">Cita no encontrada</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPatch("{id}/completada")]
    public async Task<ActionResult<CitaDto>> MarcarCompletada(int id, [FromBody] bool completada)
    {
        try
        {
            var cita = await _citaService.MarcarCompletadaAsync(id, completada);
            
            if (cita == null)
            {
                return NotFound(new { mensaje = $"Cita con ID {id} no encontrada" });
            }

            return Ok(cita);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al marcar cita como completada {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene citas por expediente
    /// </summary>
    /// <param name="expedienteId">Identificador del expediente</param>
    /// <returns>Lista de citas del expediente</returns>
    /// <response code="200">Citas obtenidas exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("expediente/{expedienteId}")]
    public async Task<ActionResult<IEnumerable<CitaDto>>> GetByExpediente(int expedienteId)
    {
        try
        {
            var citas = await _citaService.GetByExpedienteIdAsync(expedienteId);
            return Ok(citas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener citas del expediente {ExpedienteId}", expedienteId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene citas por rango de fechas
    /// </summary>
    /// <param name="fechaInicio">Fecha inicial del rango</param>
    /// <param name="fechaFin">Fecha final del rango</param>
    /// <returns>Lista de citas dentro del rango de fechas</returns>
    /// <response code="200">Citas obtenidas exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("rango-fechas")]
    public async Task<ActionResult<IEnumerable<CitaDto>>> GetByFechaRango(
        [FromQuery] DateTime fechaInicio,
        [FromQuery] DateTime fechaFin)
    {
        try
        {
            var citas = await _citaService.GetByFechaRangoAsync(fechaInicio, fechaFin);
            return Ok(citas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener citas por rango de fechas");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene citas pendientes (no completadas y futuras)
    /// </summary>
    /// <returns>Lista de citas pendientes</returns>
    /// <response code="200">Citas pendientes obtenidas exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("pendientes")]
    public async Task<ActionResult<IEnumerable<CitaDto>>> GetPendientes()
    {
        try
        {
            var citas = await _citaService.GetPendientesAsync();
            return Ok(citas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener citas pendientes");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }
}
