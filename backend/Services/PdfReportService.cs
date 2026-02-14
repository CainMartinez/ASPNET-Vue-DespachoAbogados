using AbogadosAPI.Services.Reports;

namespace AbogadosAPI.Services;

/// <summary>
/// Fachada para generación de reportes PDF. 
/// Delega a servicios especializados ubicados en Services/Reports/
/// </summary>
public class PdfReportService
{
    private readonly ClientesReportService _clientesReport;
    private readonly ExpedientesPorEstadoReportService _expedientesReport;
    private readonly ActuacionesPorExpedienteReportService _actuacionesReport;
    private readonly ILogger<PdfReportService> _logger;

    public PdfReportService(
        ClientesReportService clientesReport,
        ExpedientesPorEstadoReportService expedientesReport,
        ActuacionesPorExpedienteReportService actuacionesReport,
        ILogger<PdfReportService> logger)
    {
        _clientesReport = clientesReport;
        _expedientesReport = expedientesReport;
        _actuacionesReport = actuacionesReport;
        _logger = logger;
    }

    /// <summary>Genera el informe de listado de clientes</summary>
    public async Task<byte[]> GenerarInformeClientesAsync()
    {
        _logger.LogInformation("Delegando generación de informe de clientes");
        return await _clientesReport.GenerarAsync();
    }

    /// <summary>Genera el informe de expedientes agrupados por estado</summary>
    public async Task<byte[]> GenerarInformeExpedientesPorEstadoAsync()
    {
        _logger.LogInformation("Delegando generación de informe de expedientes por estado");
        return await _expedientesReport.GenerarAsync();
    }

    /// <summary>Genera el informe de actuaciones agrupadas por expediente</summary>
    public async Task<byte[]> GenerarInformeActuacionesPorExpedienteAsync()
    {
        _logger.LogInformation("Delegando generación de informe de actuaciones por expediente");
        return await _actuacionesReport.GenerarAsync();
    }
}
