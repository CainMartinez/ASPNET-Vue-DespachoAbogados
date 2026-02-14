using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using AbogadosAPI.Data;
using AbogadosAPI.Models;
using Microsoft.EntityFrameworkCore;
using static AbogadosAPI.Services.Reports.PdfReportHelpers;

namespace AbogadosAPI.Services.Reports;

/// <summary>
/// Servicio para generar el reporte de expedientes agrupados por estado
/// </summary>
public class ExpedientesPorEstadoReportService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ExpedientesPorEstadoReportService> _logger;

    public ExpedientesPorEstadoReportService(ApplicationDbContext context, ILogger<ExpedientesPorEstadoReportService> logger)
    {
        _context = context;
        _logger = logger;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    /// <summary>
    /// Genera el informe de expedientes agrupados por estado
    /// </summary>
    public async Task<byte[]> GenerarAsync()
    {
        _logger.LogInformation("Generando informe de expedientes por estado");

        var expedientes = await _context.Expedientes
            .Include(e => e.Cliente)
            .Include(e => e.Actuaciones)
            .OrderBy(e => e.Estado)
            .ThenByDescending(e => e.FechaApertura)
            .ToListAsync();

        var grupos = expedientes.GroupBy(e => e.Estado).OrderBy(g => g.Key).ToList();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(BgPrimary);
                page.DefaultTextStyle(x => x.FontSize(9).FontColor(TextPrimary));

                page.Header().Element(c => ComponerHeader(c, "Expedientes por Estado",
                    $"Resumen ejecutivo - {expedientes.Count} expedientes en {grupos.Count} estados"));

                page.Content().PaddingTop(20).Column(column =>
                {
                    // Tarjetas de estadísticas por estado
                    column.Item().Row(row =>
                    {
                        foreach (var estado in Enum.GetValues<Estado>())
                        {
                            var count = expedientes.Count(e => e.Estado == estado);
                            row.RelativeItem().Padding(4).Element(c => TarjetaEstado(c, estado, count));
                        }
                    });

                    column.Item().PaddingTop(15);

                    foreach (var grupo in grupos)
                    {
                        var estadoNombre = FormatearEstado(grupo.Key);
                        var colorEstado = ObtenerColorEstado(grupo.Key);

                        // Encabezado del grupo con color del estado
                        column.Item().PaddingTop(12).Row(row =>
                        {
                            row.ConstantItem(5).Height(18).Background(colorEstado);
                            row.RelativeItem().PaddingLeft(8).AlignMiddle()
                                .Text($"{estadoNombre} ({grupo.Count()} expedientes)")
                                .FontSize(11).Bold().FontColor(colorEstado);
                        });

                        column.Item().PaddingTop(8);

                        // Tabla del grupo
                        column.Item().Background(White).Border(1).BorderColor(BorderColor).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1.5f);  // Numero
                                columns.RelativeColumn(3f);    // Asunto
                                columns.RelativeColumn(2f);    // Cliente
                                columns.RelativeColumn(1.5f);  // Fecha Inicio
                                columns.ConstantColumn(45);    // Actuaciones
                            });

                            table.Header(header =>
                            {
                                CeldaHeaderTabla(header.Cell(), "NUMERO");
                                CeldaHeaderTabla(header.Cell(), "ASUNTO");
                                CeldaHeaderTabla(header.Cell(), "CLIENTE");
                                CeldaHeaderTabla(header.Cell(), "F. INICIO");
                                CeldaHeaderTabla(header.Cell(), "ACT.");
                            });

                            var items = grupo.ToList();
                            for (int i = 0; i < items.Count; i++)
                            {
                                var exp = items[i];

                                CeldaDatoTabla(table.Cell(), i)
                                    .Text(exp.NumeroExpediente).FontSize(8).SemiBold().FontColor(AccentGoldDark);
                                CeldaDatoTabla(table.Cell(), i)
                                    .Text(exp.Asunto.Length > 45 ? exp.Asunto.Substring(0, 42) + "..." : exp.Asunto).FontSize(8);
                                CeldaDatoTabla(table.Cell(), i)
                                    .Text(exp.Cliente != null ? $"{exp.Cliente.Nombre} {exp.Cliente.Apellidos}" : "-").FontSize(8);
                                CeldaDatoTabla(table.Cell(), i)
                                    .Text(exp.FechaApertura.ToString("dd/MM/yyyy")).FontSize(8);

                                var actCount = exp.Actuaciones.Count;
                                var actCell = CeldaDatoTabla(table.Cell(), i).AlignCenter();
                                if (actCount > 0)
                                    actCell.Text(actCount.ToString()).FontSize(9).Bold().FontColor(AccentGoldDark);
                                else
                                    actCell.Text("0").FontSize(8).FontColor(TextMuted);
                            }
                        });

                        column.Item().PaddingTop(5);
                    }

                    column.Item().PaddingTop(15);

                    // Nota al pie
                    column.Item().Element(c => NotaAlPie(c,
                        $"Este informe muestra {expedientes.Count} expedientes distribuidos en {grupos.Count} estados diferentes. Datos actualizados al {DateTime.Now:dd/MM/yyyy HH:mm}."));
                });

                page.Footer().Element(ComponerFooter);
            });
        });

        return document.GeneratePdf();
    }

    /// <summary>
    /// Genera una tarjeta de estadísticas específica para estados
    /// </summary>
    private static void TarjetaEstado(IContainer container, Estado estado, int count)
    {
        var color = ObtenerColorEstado(estado);
        var bgColor = ObtenerColorFondoEstado(estado);
        var nombre = FormatearEstado(estado);

        container.Background(bgColor).Border(1).BorderColor(color)
            .Padding(8).Column(col =>
            {
                col.Item().AlignCenter().Text(nombre.ToUpper()).FontSize(7).Bold().FontColor(color);
                col.Item().PaddingTop(3).AlignCenter().Text(count.ToString()).FontSize(16).Bold().FontColor(color);
            });
    }
}
