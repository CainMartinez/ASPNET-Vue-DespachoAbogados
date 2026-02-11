using Microsoft.AspNetCore.Mvc;
using AbogadosAPI.DTOs;
using AbogadosAPI.Services;

namespace AbogadosAPI.Controllers;

/// <summary>
/// Controlador para gestionar clientes
/// </summary>
/// <remarks>
/// Proporciona endpoints para operaciones CRUD de clientes y consultas especializadas
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;
    private readonly ILogger<ClientesController> _logger;

    /// <summary>
    /// Constructor del controlador de clientes
    /// </summary>
    /// <param name="clienteService">Servicio de clientes</param>
    /// <param name="logger">Logger para registrar eventos</param>
    public ClientesController(IClienteService clienteService, ILogger<ClientesController> logger)
    {
        _clienteService = clienteService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los clientes
    /// </summary>
    /// <returns>Lista de clientes</returns>
    /// <response code="200">Clientes obtenidos exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAll()
    {
        try
        {
            var clientes = await _clienteService.GetAllAsync();
            return Ok(clientes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener clientes");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene un cliente por su ID
    /// </summary>
    /// <param name="id">Identificador del cliente</param>
    /// <returns>Cliente solicitado</returns>
    /// <response code="200">Cliente encontrado</response>
    /// <response code="404">Cliente no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<ClienteDto>> GetById(int id)
    {
        try
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            
            if (cliente == null)
            {
                return NotFound(new { mensaje = $"Cliente con ID {id} no encontrado" });
            }

            return Ok(cliente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el cliente {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Crea un nuevo cliente
    /// </summary>
    /// <param name="clienteDto">Datos del cliente a crear</param>
    /// <returns>Cliente creado</returns>
    /// <response code="201">Cliente creado exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost]
    public async Task<ActionResult<ClienteDto>> Create([FromBody] ClienteCreateDto clienteDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cliente = await _clienteService.CreateAsync(clienteDto);
            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el cliente");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza un cliente existente
    /// </summary>
    /// <param name="id">Identificador del cliente</param>
    /// <param name="clienteDto">Nuevos datos del cliente</param>
    /// <returns>Cliente actualizado</returns>
    /// <response code="200">Cliente actualizado exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="404">Cliente no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<ClienteDto>> Update(int id, [FromBody] ClienteUpdateDto clienteDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cliente = await _clienteService.UpdateAsync(id, clienteDto);
            
            if (cliente == null)
            {
                return NotFound(new { mensaje = $"Cliente con ID {id} no encontrado" });
            }

            return Ok(cliente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el cliente {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Elimina un cliente
    /// </summary>
    /// <param name="id">Identificador del cliente</param>
    /// <returns>Resultado de la eliminación</returns>
    /// <response code="204">Cliente eliminado exitosamente</response>
    /// <response code="404">Cliente no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var resultado = await _clienteService.DeleteAsync(id);
            
            if (!resultado)
            {
                return NotFound(new { mensaje = $"Cliente con ID {id} no encontrado" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el cliente {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Busca clientes por término de búsqueda
    /// </summary>
    /// <param name="q">Término a buscar en nombre, apellidos, DNI/CIF o email</param>
    /// <returns>Lista de clientes que coinciden con la búsqueda</returns>
    /// <response code="200">Búsqueda completada exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("buscar")]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> Search([FromQuery] string q = "")
    {
        try
        {
            var clientes = await _clienteService.SearchAsync(q);
            return Ok(clientes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar clientes con término: {SearchTerm}", q);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }
}
