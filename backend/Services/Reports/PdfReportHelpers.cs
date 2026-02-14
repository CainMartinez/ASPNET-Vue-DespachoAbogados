using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using AbogadosAPI.Models;

namespace AbogadosAPI.Services.Reports;

/// <summary>
/// Componentes compartidos y estilos para reportes PDF premium
/// </summary>
public static class PdfReportHelpers
{
    // ═══════════════════════════════════════════════════════════════
    //  PALETA DE COLORES PREMIUM (misma que la web)
    // ═══════════════════════════════════════════════════════════════

    public static readonly string PrimaryBrown = "#5D4E37";
    public static readonly string SecondaryBrown = "#8B7355";
    public static readonly string LightBrown = "#A0826D";
    public static readonly string AccentGold = "#D4AF37";
    public static readonly string AccentGoldLight = "#E8C961";
    public static readonly string AccentGoldDark = "#B8941F";
    public static readonly string TextPrimary = "#3E2723";
    public static readonly string TextSecondary = "#5D4E37";
    public static readonly string TextMuted = "#8B7355";
    public static readonly string BgPrimary = "#FAF8F3";
    public static readonly string BgSecondary = "#F5F1E8";
    public static readonly string BorderColor = "#D4C5B0";
    public static readonly string White = "#FFFFFF";

    // ═══════════════════════════════════════════════════════════════
    //  FORMATEO DE ESTADO
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Convierte el enum Estado a texto legible con espacios
    /// </summary>
    public static string FormatearEstado(Estado estado)
    {
        return estado switch
        {
            Estado.Abierto => "Abierto",
            Estado.EnTramite => "En Trámite",
            Estado.Suspendido => "Suspendido",
            Estado.Archivado => "Archivado",
            Estado.Cerrado => "Cerrado",
            _ => estado.ToString()
        };
    }

    /// <summary>
    /// Obtiene el color asociado a un estado de expediente
    /// </summary>
    public static string ObtenerColorEstado(Estado estado)
    {
        return estado switch
        {
            Estado.Abierto => "#2563EB",      // Azul vibrante
            Estado.EnTramite => "#16A34A",     // Verde
            Estado.Suspendido => AccentGold,   // Dorado
            Estado.Archivado => TextMuted,     // Gris marrón
            Estado.Cerrado => "#DC2626",       // Rojo
            _ => TextMuted
        };
    }

    /// <summary>
    /// Obtiene el color de fondo asociado a un estado
    /// </summary>
    public static string ObtenerColorFondoEstado(Estado estado)
    {
        return estado switch
        {
            Estado.Abierto => "#DBEAFE",
            Estado.EnTramite => "#DCFCE7",
            Estado.Suspendido => "#FEF9C3",
            Estado.Archivado => "#F3F4F6",
            Estado.Cerrado => "#FEE2E2",
            _ => BgSecondary
        };
    }

    // ═══════════════════════════════════════════════════════════════
    //  COMPONENTES DE DISEÑO
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Header premium reutilizable para todos los reportes
    /// </summary>
    public static void ComponerHeader(IContainer container, string titulo, string subtitulo)
    {
        container
            .Background(PrimaryBrown)
            .Padding(0)
            .Column(column =>
            {
                // Barra dorada superior decorativa
                column.Item().Height(4).Background(AccentGold);

                column.Item().Padding(20).Column(inner =>
                {
                    // Título del despacho
                    inner.Item().Column(titleCol =>
                    {
                        titleCol.Item().Text("DESPACHO DE ABOGADOS")
                            .FontSize(24).Bold().FontColor(AccentGold)
                            .LetterSpacing(0.1f);
                        titleCol.Item().PaddingTop(3)
                            .Text("Gestion Juridica Profesional")
                            .FontSize(9).FontColor(LightBrown)
                            .LetterSpacing(0.12f);
                    });

                    // Separador dorado
                    inner.Item().PaddingVertical(10).LineHorizontal(1.5f).LineColor(AccentGold);

                    // Título del informe y metadatos
                    inner.Item().Row(row =>
                    {
                        row.RelativeItem().Column(left =>
                        {
                            left.Item().Text(titulo)
                                .FontSize(16).Bold().FontColor(White);
                            left.Item().PaddingTop(3).Text(subtitulo)
                                .FontSize(9).FontColor(LightBrown);
                        });
                        row.ConstantItem(160).AlignRight().AlignBottom().Column(right =>
                        {
                            right.Item().AlignRight().Text($"{DateTime.Now:dd/MM/yyyy}")
                                .FontSize(9).FontColor(AccentGoldLight);
                            right.Item().AlignRight().Text($"{DateTime.Now:HH:mm} hrs")
                                .FontSize(8).FontColor(LightBrown);
                        });
                    });
                });

                // Barra dorada inferior decorativa
                column.Item().Height(3).Background(AccentGold);
            });
    }

    /// <summary>
    /// Footer premium reutilizable
    /// </summary>
    public static void ComponerFooter(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().LineHorizontal(0.75f).LineColor(BorderColor);
            col.Item().PaddingTop(8).Row(row =>
            {
                row.RelativeItem().AlignLeft()
                    .Text("Despacho de Abogados - Documento confidencial")
                    .FontSize(7).FontColor(TextMuted).Italic();
                row.RelativeItem().AlignCenter().Text(x =>
                {
                    x.Span("Pagina ").FontSize(7).FontColor(TextMuted);
                    x.CurrentPageNumber().FontSize(7).FontColor(TextPrimary).Bold();
                    x.Span(" de ").FontSize(7).FontColor(TextMuted);
                    x.TotalPages().FontSize(7).FontColor(TextPrimary).Bold();
                });
                row.RelativeItem().AlignRight()
                    .Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                    .FontSize(7).FontColor(TextMuted);
            });
        });
    }

    /// <summary>
    /// Celda de encabezado de tabla premium
    /// </summary>
    public static void CeldaHeaderTabla(IContainer cell, string texto)
    {
        cell.Background(PrimaryBrown)
            .BorderBottom(2).BorderColor(AccentGold)
            .Padding(8)
            .Text(texto)
            .FontSize(8).Bold().FontColor(AccentGold)
            .LetterSpacing(0.04f);
    }

    /// <summary>
    /// Celda de dato de tabla con zebra striping
    /// </summary>
    public static IContainer CeldaDatoTabla(IContainer cell, int index)
    {
        var bgColor = index % 2 == 0 ? White : BgSecondary;
        return cell.Background(bgColor)
            .BorderBottom(0.5f).BorderColor(BorderColor)
            .Padding(7);
    }

    /// <summary>
    /// Tarjeta de estadística
    /// </summary>
    public static void TarjetaStat(IContainer container, string label, string valor)
    {
        container.Border(1).BorderColor(BorderColor)
            .Background(White)
            .Column(col =>
            {
                // Barra dorada superior como acento
                col.Item().Height(3).Background(AccentGold);
                col.Item().Padding(10).Column(inner =>
                {
                    inner.Item().AlignCenter()
                        .Text(valor).FontSize(22).Bold().FontColor(PrimaryBrown);
                    inner.Item().PaddingTop(4).AlignCenter()
                        .Text(label).FontSize(7).FontColor(TextMuted)
                        .LetterSpacing(0.08f);
                });
            });
    }

    /// <summary>
    /// Nota al pie del reporte
    /// </summary>
    public static void NotaAlPie(IContainer container, string texto)
    {
        container.Background(BgSecondary).Border(1).BorderColor(BorderColor)
            .Padding(10).Row(row =>
            {
                row.AutoItem().PaddingRight(8).AlignMiddle()
                    .Width(3).Height(20).Background(AccentGold);
                row.RelativeItem()
                    .Text(texto)
                    .FontSize(7.5f).FontColor(TextMuted).Italic();
            });
    }

    /// <summary>
    /// Encabezado de sección con barra lateral dorada
    /// </summary>
    public static void EncabezadoSeccion(IContainer container, string titulo)
    {
        container.Row(row =>
        {
            row.AutoItem().Width(4).Height(16).Background(AccentGold);
            row.AutoItem().PaddingLeft(8)
                .Text(titulo)
                .FontSize(13).Bold().FontColor(PrimaryBrown);
        });
    }
}
