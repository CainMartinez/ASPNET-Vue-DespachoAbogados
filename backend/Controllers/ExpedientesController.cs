using Microsoft.AspNetCore.Mvc;
using AbogadosAPI.DTOs;
using AbogadosAPI.Models;
using AbogadosAPI.Services;

namespace AbogadosAPI.Controllers;

/// <summary>
/// Controlador para gestionar expedientes jurídicos
/// </summary>
/// <remarks>
/// Proporciona endpoints para operaciones CRUD de expedientes y consultas especializadas
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class ExpedientesController : ControllerBase
{
    private readonly IExpedienteService _expedienteService;
    private readonly ILogger<ExpedientesController> _logger;

    /// <summary>
    /// Constructor del controlador de expedientes
    /// </summary>
    /// <param name="expedienteService">Servicio de expedientes</param>
    /// <param name="logger">Logger para registrar eventos</param>
    public ExpedientesController(IExpedienteService expedienteService, ILogger<ExpedientesController> logger)
    {
        _expedienteService = expedienteService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los expedientes
    /// </summary>
    /// <returns>Lista de expedientes con información completa</returns>
    /// <response code="200">Expedientes obtenidos exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpedienteDto>>> GetAll()
    {
        try
        {
            var expedientes = await _expedienteService.GetAllAsync();
            return Ok(expedientes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener expedientes");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene un resumen de todos los expedientes
    /// </summary>
    /// <returns>Lista de expedientes con información resumida</returns>
    /// <response code="200">Resumen obtenido exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("resumen")]
    public async Task<ActionResult<IEnumerable<ExpedienteResumenDto>>> GetResumen()
    {
        try
        {
            var expedientes = await _expedienteService.GetResumenAsync();
            return Ok(expedientes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener resumen de expedientes");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene un expediente por su ID
    /// </summary>
    /// <param name="id">Identificador del expediente</param>
    /// <returns>Expediente solicitado</returns>
    /// <response code="200">Expediente encontrado</response>
    /// <response code="404">Expediente no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<ExpedienteDto>> GetById(int id)
    {
        try
        {
            var expediente = await _expedienteService.GetByIdAsync(id);
            
            if (expediente == null)
            {
                return NotFound(new { mensaje = $"Expediente con ID {id} no encontrado" });
            }

            return Ok(expediente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el expediente {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Crea un nuevo expediente
    /// </summary>
    /// <param name="expedienteDto">Datos del expediente a crear</param>
    /// <returns>Expediente creado</returns>
    /// <response code="201">Expediente creado exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost]
    public async Task<ActionResult<ExpedienteDto>> Create([FromBody] ExpedienteCreateDto expedienteDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var expediente = await _expedienteService.CreateAsync(expedienteDto);
            return CreatedAtAction(nameof(GetById), new { id = expediente.Id }, expediente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el expediente");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza un expediente existente
    /// </summary>
    /// <param name="id">Identificador del expediente</param>
    /// <param name="expedienteDto">Nuevos datos del expediente</param>
    /// <returns>Expediente actualizado</returns>
    /// <response code="200">Expediente actualizado exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="404">Expediente no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<ExpedienteDto>> Update(int id, [FromBody] ExpedienteUpdateDto expedienteDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var expediente = await _expedienteService.UpdateAsync(id, expedienteDto);
            
            if (expediente == null)
            {
                return NotFound(new { mensaje = $"Expediente con ID {id} no encontrado" });
            }

            return Ok(expediente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el expediente {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Elimina un expediente
    /// </summary>
    /// <param name="id">Identificador del expediente</param>
    /// <returns>Resultado de la eliminación</returns>
    /// <response code="204">Expediente eliminado exitosamente</response>
    /// <response code="404">Expediente no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var resultado = await _expedienteService.DeleteAsync(id);
            
            if (!resultado)
            {
                return NotFound(new { mensaje = $"Expediente con ID {id} no encontrado" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el expediente {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Cambia el estado de un expediente
    /// </summary>
    /// <param name="id">Identificador del expediente</param>
    /// <param name="estadoDto">Nuevo estado y observaciones opcionales</param>
    /// <returns>Expediente con el estado actualizado</returns>
    /// <response code="200">Estado cambiado exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="404">Expediente no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPatch("{id}/estado")]
    public async Task<ActionResult<ExpedienteDto>> CambiarEstado(int id, [FromBody] ExpedienteCambiarEstadoDto estadoDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var expediente = await _expedienteService.CambiarEstadoAsync(id, estadoDto);
            
            if (expediente == null)
            {
                return NotFound(new { mensaje = $"Expediente con ID {id} no encontrado" });
            }

            return Ok(expediente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado del expediente {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene expedientes por cliente
    /// </summary>
    /// <param name="clienteId">Identificador del cliente</param>
    /// <returns>Lista de expedientes del cliente</returns>
    /// <response code="200">Expedientes obtenidos exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("cliente/{clienteId}")]
    public async Task<ActionResult<IEnumerable<ExpedienteDto>>> GetByCliente(int clienteId)
    {
        try
        {
            var expedientes = await _expedienteService.GetByClienteIdAsync(clienteId);
            return Ok(expedientes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener expedientes del cliente {ClienteId}", clienteId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene expedientes por estado
    /// </summary>
    /// <param name="estado">Estado del expediente</param>
    /// <returns>Lista de expedientes con el estado especificado</returns>
    /// <response code="200">Expedientes obtenidos exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("estado/{estado}")]
    public async Task<ActionResult<IEnumerable<ExpedienteDto>>> GetByEstado(Estado estado)
    {
        try
        {
            var expedientes = await _expedienteService.GetByEstadoAsync(estado);
            return Ok(expedientes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener expedientes por estado {Estado}", estado);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Busca expedientes por término de búsqueda
    /// </summary>
    /// <param name="q">Término a buscar en número, asunto, tipo o número de procedimiento</param>
    /// <returns>Lista de expedientes que coinciden con la búsqueda</returns>
    /// <response code="200">Búsqueda completada exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("buscar")]
    public async Task<ActionResult<IEnumerable<ExpedienteDto>>> Search([FromQuery] string q = "")
    {
        try
        {
            var expedientes = await _expedienteService.SearchAsync(q);
            return Ok(expedientes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar expedientes con término: {SearchTerm}", q);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }
}
