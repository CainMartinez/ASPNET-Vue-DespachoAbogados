using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using AbogadosAPI.Data;
using Microsoft.EntityFrameworkCore;
using static AbogadosAPI.Services.Reports.PdfReportHelpers;

namespace AbogadosAPI.Services.Reports;

/// <summary>
/// Servicio para generar el reporte de actuaciones agrupadas por expediente
/// </summary>
public class ActuacionesPorExpedienteReportService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ActuacionesPorExpedienteReportService> _logger;

    public ActuacionesPorExpedienteReportService(ApplicationDbContext context, ILogger<ActuacionesPorExpedienteReportService> logger)
    {
        _context = context;
        _logger = logger;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    /// <summary>
    /// Genera el informe de actuaciones agrupadas por expediente
    /// </summary>
    public async Task<byte[]> GenerarAsync()
    {
        _logger.LogInformation("Generando informe de actuaciones por expediente");

        var expedientes = await _context.Expedientes
            .Include(e => e.Cliente)
            .Include(e => e.Actuaciones)
            .Where(e => e.Actuaciones.Any())
            .OrderBy(e => e.NumeroExpediente)
            .ToListAsync();

        var totalActuaciones = expedientes.Sum(e => e.Actuaciones.Count);
        var tiposActuacion = expedientes.SelectMany(e => e.Actuaciones).Select(a => a.TipoActuacion).Distinct().Count();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(BgPrimary);
                page.DefaultTextStyle(x => x.FontSize(9).FontColor(TextPrimary));

                page.Header().Element(c => ComponerHeader(c, "Actuaciones por Expediente",
                    $"Registro de actividad - {totalActuaciones} actuaciones en {expedientes.Count} expedientes"));

                page.Content().PaddingTop(20).Column(column =>
                {
                    // Tarjetas de estadísticas
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Padding(4).Element(c => TarjetaStat(c, "EXPEDIENTES ACTIVOS", expedientes.Count.ToString()));
                        row.RelativeItem().Padding(4).Element(c => TarjetaStat(c, "TOTAL ACTUACIONES", totalActuaciones.ToString()));
                        row.RelativeItem().Padding(4).Element(c => TarjetaStat(c, "TIPOS DE ACTUACION", tiposActuacion.ToString()));
                        row.RelativeItem().Padding(4).Element(c => TarjetaStat(c, "MEDIA ACT/EXP", expedientes.Count > 0 ? $"{(totalActuaciones / (double)expedientes.Count):F1}" : "0"));
                    });

                    column.Item().PaddingTop(15);

                    foreach (var expediente in expedientes)
                    {
                        var estadoNombre = FormatearEstado(expediente.Estado);
                        var colorEstado = ObtenerColorEstado(expediente.Estado);

                        // Cabecera del expediente
                        column.Item().PaddingTop(12).Background(PrimaryBrown).Padding(10).Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text(expediente.NumeroExpediente).FontSize(11).Bold().FontColor(AccentGold);
                                col.Item().PaddingTop(2).Text(expediente.Asunto).FontSize(9).FontColor(White);
                            });
                            row.ConstantItem(80).AlignRight().AlignMiddle().Column(col =>
                            {
                                col.Item().AlignRight().Text($"{expediente.Actuaciones.Count} actuaciones").FontSize(8).FontColor(AccentGoldLight);
                            });
                        });

                        // Info del cliente y estado
                        column.Item().Background(BgSecondary).BorderBottom(1).BorderColor(BorderColor).Padding(8).Row(row =>
                        {
                            row.RelativeItem().Text(text =>
                            {
                                text.Span("Cliente: ").FontSize(8).FontColor(TextMuted);
                                text.Span(expediente.Cliente != null ? $"{expediente.Cliente.Nombre} {expediente.Cliente.Apellidos}" : "Sin cliente").FontSize(8).SemiBold();
                            });
                            row.ConstantItem(100).AlignRight().Row(subrow =>
                            {
                                subrow.ConstantItem(4).Height(14).Background(colorEstado);
                                subrow.RelativeItem().PaddingLeft(5)
                                    .Text(estadoNombre).FontSize(8).SemiBold().FontColor(colorEstado);
                            });
                        });

                        // Tabla de actuaciones
                        column.Item().Background(White).Border(1).BorderColor(BorderColor).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(28);   // #
                                columns.RelativeColumn(1.5f); // Fecha
                                columns.RelativeColumn(1.5f); // Tipo
                                columns.RelativeColumn(4f);   // Descripción
                            });

                            table.Header(header =>
                            {
                                CeldaHeaderTabla(header.Cell(), "#");
                                CeldaHeaderTabla(header.Cell(), "FECHA");
                                CeldaHeaderTabla(header.Cell(), "TIPO");
                                CeldaHeaderTabla(header.Cell(), "DESCRIPCION");
                            });

                            var actuaciones = expediente.Actuaciones.OrderByDescending(a => a.FechaActuacion).ToList();
                            for (int i = 0; i < actuaciones.Count; i++)
                            {
                                var act = actuaciones[i];

                                CeldaDatoTabla(table.Cell(), i).AlignCenter()
                                    .Text((i + 1).ToString()).FontSize(8).FontColor(TextMuted);
                                CeldaDatoTabla(table.Cell(), i)
                                    .Text(act.FechaActuacion.ToString("dd/MM/yyyy")).FontSize(8);
                                CeldaDatoTabla(table.Cell(), i)
                                    .Text(act.TipoActuacion).FontSize(8).SemiBold().FontColor(SecondaryBrown);
                                CeldaDatoTabla(table.Cell(), i)
                                    .Text(act.Descripcion.Length > 70 ? act.Descripcion.Substring(0, 67) + "..." : act.Descripcion).FontSize(8);
                            }
                        });

                        column.Item().PaddingTop(8);
                    }

                    column.Item().PaddingTop(15);

                    // Nota al pie
                    column.Item().Element(c => NotaAlPie(c,
                        $"Este informe detalla {totalActuaciones} actuaciones distribuidas en {expedientes.Count} expedientes activos. Datos actualizados al {DateTime.Now:dd/MM/yyyy HH:mm}."));
                });

                page.Footer().Element(ComponerFooter);
            });
        });

        return document.GeneratePdf();
    }
}
