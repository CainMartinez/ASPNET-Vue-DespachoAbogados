using Microsoft.AspNetCore.Mvc;
using AbogadosAPI.DTOs;
using AbogadosAPI.Services;

namespace AbogadosAPI.Controllers;

/// <summary>
/// Controlador para gestionar documentos adjuntos a expedientes
/// </summary>
/// <remarks>
/// Proporciona endpoints para operaciones CRUD de documentos y consultas especializadas
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class DocumentosController : ControllerBase
{
    private readonly IDocumentoService _documentoService;
    private readonly ILogger<DocumentosController> _logger;

    /// <summary>
    /// Constructor del controlador de documentos
    /// </summary>
    /// <param name="documentoService">Servicio de documentos</param>
    /// <param name="logger">Logger para registrar eventos</param>
    public DocumentosController(IDocumentoService documentoService, ILogger<DocumentosController> logger)
    {
        _documentoService = documentoService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los documentos
    /// </summary>
    /// <returns>Lista de documentos</returns>
    /// <response code="200">Documentos obtenidos exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DocumentoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DocumentoDto>>> GetAll()
    {
        try
        {
            var documentos = await _documentoService.GetAllAsync();
            return Ok(documentos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener documentos");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene un documento por su ID
    /// </summary>
    /// <param name="id">Identificador del documento</param>
    /// <returns>Documento solicitado</returns>
    /// <response code="200">Documento encontrado</response>
    /// <response code="404">Documento no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DocumentoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DocumentoDto>> GetById(int id)
    {
        try
        {
            var documento = await _documentoService.GetByIdAsync(id);
            
            if (documento == null)
            {
                return NotFound(new { mensaje = $"Documento con ID {id} no encontrado" });
            }

            return Ok(documento);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el documento {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Crea un nuevo documento
    /// </summary>
    /// <param name="documentoDto">Datos del documento a crear</param>
    /// <returns>Documento creado</returns>
    /// <response code="201">Documento creado exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(DocumentoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DocumentoDto>> Create([FromBody] DocumentoCreateDto documentoDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var documento = await _documentoService.CreateAsync(documentoDto);
            return CreatedAtAction(nameof(GetById), new { id = documento.Id }, documento);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el documento");
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza un documento existente
    /// </summary>
    /// <param name="id">Identificador del documento</param>
    /// <param name="documentoDto">Nuevos datos del documento</param>
    /// <returns>Documento actualizado</returns>
    /// <response code="200">Documento actualizado exitosamente</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="404">Documento no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(DocumentoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DocumentoDto>> Update(int id, [FromBody] DocumentoUpdateDto documentoDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var documento = await _documentoService.UpdateAsync(id, documentoDto);
            
            if (documento == null)
            {
                return NotFound(new { mensaje = $"Documento con ID {id} no encontrado" });
            }

            return Ok(documento);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el documento {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Elimina un documento
    /// </summary>
    /// <param name="id">Identificador del documento</param>
    /// <returns>Resultado de la eliminación</returns>
    /// <response code="204">Documento eliminado exitosamente</response>
    /// <response code="404">Documento no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var resultado = await _documentoService.DeleteAsync(id);
            
            if (!resultado)
            {
                return NotFound(new { mensaje = $"Documento con ID {id} no encontrado" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el documento {Id}", id);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene documentos por expediente
    /// </summary>
    /// <param name="expedienteId">Identificador del expediente</param>
    /// <returns>Lista de documentos del expediente</returns>
    /// <response code="200">Documentos obtenidos exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("expediente/{expedienteId}")]
    [ProducesResponseType(typeof(IEnumerable<DocumentoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DocumentoDto>>> GetByExpediente(int expedienteId)
    {
        try
        {
            var documentos = await _documentoService.GetByExpedienteIdAsync(expedienteId);
            return Ok(documentos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener documentos del expediente {ExpedienteId}", expedienteId);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene documentos por tipo
    /// </summary>
    /// <param name="tipo">Tipo de documento</param>
    /// <returns>Lista de documentos del tipo especificado</returns>
    /// <response code="200">Documentos obtenidos exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("tipo/{tipo}")]
    [ProducesResponseType(typeof(IEnumerable<DocumentoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<DocumentoDto>>> GetByTipo(string tipo)
    {
        try
        {
            var documentos = await _documentoService.GetByTipoAsync(tipo);
            return Ok(documentos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener documentos por tipo {Tipo}", tipo);
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }
}
