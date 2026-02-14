using Microsoft.AspNetCore.Mvc;
using AbogadosAPI.Services;
using AbogadosAPI.DTOs;

namespace AbogadosAPI.Controllers;

/// <summary>
/// Controlador para generación de reportes PDF con trazabilidad
/// </summary>
/// <remarks>
/// Genera informes PDF, los almacena en disco y registra cada generación
/// en la tabla Documentos para poder volver a descargarlos
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReportesController : ControllerBase
{
    private readonly PdfReportService _pdfReportService;
    private readonly IDocumentoService _documentoService;
    private readonly ILogger<ReportesController> _logger;
    private const string ReportesDir = "/app/reportes";

    public ReportesController(
        PdfReportService pdfReportService,
        IDocumentoService documentoService,
        ILogger<ReportesController> logger)
    {
        _pdfReportService = pdfReportService;
        _documentoService = documentoService;
        _logger = logger;
    }

    /// <summary>
    /// Genera un informe PDF de listado de clientes, lo almacena y registra
    /// </summary>
    [HttpGet("clientes")]
    [ProducesResponseType(typeof(DocumentoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObtenerInformeClientes()
    {
        try
        {
            _logger.LogInformation("Generando informe de clientes");
            var pdfBytes = await _pdfReportService.GenerarInformeClientesAsync();
            var fileName = $"InformeClientes_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            var documento = await GuardarReporte(pdfBytes, fileName, "Informe de Clientes",
                "Listado completo de clientes con información de contacto y expedientes asociados");

            return Ok(documento);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar informe de clientes");
            return StatusCode(500, new { mensaje = "Error al generar el informe", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Genera un informe PDF de expedientes agrupados por estado
    /// </summary>
    [HttpGet("expedientes-por-estado")]
    [ProducesResponseType(typeof(DocumentoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObtenerInformeExpedientesPorEstado()
    {
        try
        {
            _logger.LogInformation("Generando informe de expedientes por estado");
            var pdfBytes = await _pdfReportService.GenerarInformeExpedientesPorEstadoAsync();
            var fileName = $"InformeExpedientesPorEstado_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            var documento = await GuardarReporte(pdfBytes, fileName, "Informe de Expedientes por Estado",
                "Expedientes agrupados por estado con totalizaciones de actuaciones y citas");

            return Ok(documento);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar informe de expedientes por estado");
            return StatusCode(500, new { mensaje = "Error al generar el informe", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Genera un informe PDF de actuaciones agrupadas por expediente
    /// </summary>
    [HttpGet("actuaciones-por-expediente")]
    [ProducesResponseType(typeof(DocumentoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObtenerInformeActuacionesPorExpediente()
    {
        try
        {
            _logger.LogInformation("Generando informe de actuaciones por expediente");
            var pdfBytes = await _pdfReportService.GenerarInformeActuacionesPorExpedienteAsync();
            var fileName = $"InformeActuacionesPorExpediente_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            var documento = await GuardarReporte(pdfBytes, fileName, "Informe de Actuaciones por Expediente",
                "Actuaciones agrupadas por expediente con subtotales por tipo y detalle cronológico");

            return Ok(documento);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar informe de actuaciones por expediente");
            return StatusCode(500, new { mensaje = "Error al generar el informe", detalle = ex.Message });
        }
    }

    /// <summary>
    /// Descarga un reporte previamente generado por su ID de documento
    /// </summary>
    [HttpGet("descargar/{documentoId}")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DescargarReporte(int documentoId)
    {
        try
        {
            var documento = await _documentoService.GetByIdAsync(documentoId);
            if (documento == null)
                return NotFound(new { mensaje = "Documento no encontrado" });

            var filePath = documento.RutaArchivo;
            if (!System.IO.File.Exists(filePath))
                return NotFound(new { mensaje = "El archivo PDF no existe en el servidor" });

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, "application/pdf", documento.NombreArchivo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al descargar reporte {DocumentoId}", documentoId);
            return StatusCode(500, new { mensaje = "Error al descargar el reporte" });
        }
    }

    /// <summary>
    /// Guarda un PDF generado en disco y crea un registro en Documentos
    /// </summary>
    private async Task<DocumentoDto> GuardarReporte(byte[] pdfBytes, string fileName, string tipoReporte, string descripcion)
    {
        // Asegurar que el directorio existe
        Directory.CreateDirectory(ReportesDir);

        var filePath = Path.Combine(ReportesDir, fileName);
        await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);

        _logger.LogInformation("Reporte guardado en {FilePath} ({Size} bytes)", filePath, pdfBytes.Length);

        // Crear registro en la DB
        var createDto = new DocumentoCreateDto
        {
            ExpedienteId = null, // Reporte general, sin expediente asociado
            NombreArchivo = fileName,
            Descripcion = descripcion,
            TipoDocumento = tipoReporte,
            RutaArchivo = filePath,
            TamanoBytes = pdfBytes.Length,
            Extension = ".pdf",
            CargadoPor = "Sistema",
            Observaciones = $"Generado automáticamente el {DateTime.Now:dd/MM/yyyy HH:mm:ss}"
        };

        var documento = await _documentoService.CreateAsync(createDto);
        return documento;
    }
}
