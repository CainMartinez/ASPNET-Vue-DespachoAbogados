using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using AbogadosAPI.Data;
using Microsoft.EntityFrameworkCore;
using static AbogadosAPI.Services.Reports.PdfReportHelpers;

namespace AbogadosAPI.Services.Reports;

/// <summary>
/// Servicio para generar el reporte de listado de clientes
/// </summary>
public class ClientesReportService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ClientesReportService> _logger;

    public ClientesReportService(ApplicationDbContext context, ILogger<ClientesReportService> logger)
    {
        _context = context;
        _logger = logger;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    /// <summary>
    /// Genera el informe de listado de clientes
    /// </summary>
    public async Task<byte[]> GenerarAsync()
    {
        _logger.LogInformation("Generando informe de clientes");

        var clientes = await _context.Clientes
            .Include(c => c.Expedientes)
            .OrderBy(c => c.Apellidos)
            .ThenBy(c => c.Nombre)
            .ToListAsync();

        var totalExp = clientes.Sum(c => c.Expedientes.Count);
        var ciudades = clientes.Select(c => c.Ciudad).Where(c => !string.IsNullOrEmpty(c)).Distinct().Count();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(BgPrimary);
                page.DefaultTextStyle(x => x.FontSize(9).FontColor(TextPrimary));

                page.Header().Element(c => ComponerHeader(c, "Informe de Clientes",
                    $"Directorio completo - {clientes.Count} clientes registrados"));

                page.Content().PaddingTop(20).Column(column =>
                {
                    // Tarjetas de estadísticas
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Padding(4).Element(c => TarjetaStat(c, "TOTAL CLIENTES", clientes.Count.ToString()));
                        row.RelativeItem().Padding(4).Element(c => TarjetaStat(c, "EXPEDIENTES", totalExp.ToString()));
                        row.RelativeItem().Padding(4).Element(c => TarjetaStat(c, "CIUDADES", ciudades.ToString()));
                        row.RelativeItem().Padding(4).Element(c => TarjetaStat(c, "MEDIA EXP/CLIENTE", clientes.Count > 0 ? $"{(totalExp / (double)clientes.Count):F1}" : "0"));
                    });

                    column.Item().PaddingTop(18);

                    // Encabezado de sección
                    column.Item().Element(c => EncabezadoSeccion(c, "Directorio de Clientes"));

                    column.Item().PaddingTop(8);

                    // Tabla de clientes
                    column.Item().Background(White).Border(1).BorderColor(BorderColor).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(28);   // #
                            columns.RelativeColumn(3);    // Nombre
                            columns.RelativeColumn(1.8f); // DNI/CIF
                            columns.RelativeColumn(1.8f); // Teléfono
                            columns.RelativeColumn(2.8f); // Email
                            columns.RelativeColumn(1.5f); // Ciudad
                            columns.ConstantColumn(45);   // Exp
                        });

                        table.Header(header =>
                        {
                            CeldaHeaderTabla(header.Cell(), "#");
                            CeldaHeaderTabla(header.Cell(), "NOMBRE COMPLETO");
                            CeldaHeaderTabla(header.Cell(), "DNI / CIF");
                            CeldaHeaderTabla(header.Cell(), "TELEFONO");
                            CeldaHeaderTabla(header.Cell(), "EMAIL");
                            CeldaHeaderTabla(header.Cell(), "CIUDAD");
                            CeldaHeaderTabla(header.Cell(), "EXP.");
                        });

                        for (int i = 0; i < clientes.Count; i++)
                        {
                            var cliente = clientes[i];

                            CeldaDatoTabla(table.Cell(), i).AlignCenter()
                                .Text((i + 1).ToString()).FontSize(8).FontColor(TextMuted);
                            CeldaDatoTabla(table.Cell(), i)
                                .Text($"{cliente.Nombre} {cliente.Apellidos}").FontSize(8.5f).SemiBold();
                            CeldaDatoTabla(table.Cell(), i)
                                .Text(cliente.DniCif).FontSize(8);
                            CeldaDatoTabla(table.Cell(), i)
                                .Text(cliente.Telefono ?? "-").FontSize(8);
                            CeldaDatoTabla(table.Cell(), i)
                                .Text(cliente.Email ?? "-").FontSize(7.5f).FontColor(SecondaryBrown);
                            CeldaDatoTabla(table.Cell(), i)
                                .Text(cliente.Ciudad ?? "-").FontSize(8);

                            var expCount = cliente.Expedientes.Count;
                            var expCell = CeldaDatoTabla(table.Cell(), i).AlignCenter();
                            if (expCount > 0)
                                expCell.Text(expCount.ToString()).FontSize(9).Bold().FontColor(AccentGoldDark);
                            else
                                expCell.Text("0").FontSize(8).FontColor(TextMuted);
                        }
                    });

                    column.Item().PaddingTop(15);

                    // Nota al pie
                    column.Item().Element(c => NotaAlPie(c,
                        $"Este informe contiene {clientes.Count} clientes registrados con un total de {totalExp} expedientes asociados. Datos actualizados al {DateTime.Now:dd/MM/yyyy HH:mm}."));
                });

                page.Footer().Element(ComponerFooter);
            });
        });

        return document.GeneratePdf();
    }
}
