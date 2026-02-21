using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Infrastructure.Services;

public class InvoicePdfGenerator : IInvoicePdfGenerator
{
    private static readonly Dictionary<string, string> FallbackLabels = new()
    {
        ["invoice"] = "Invoice",
        ["issued"] = "Issued",
        ["status"] = "Status",
        ["paid"] = "Paid",
        ["bill_to"] = "Bill to",
        ["tax_id"] = "Tax ID",
        ["hotel"] = "Hotel",
        ["room"] = "Room",
        ["board"] = "Board",
        ["check_in"] = "Check-in",
        ["check_out"] = "Check-out",
        ["guests"] = "Guests",
        ["total"] = "Total",
        ["subtotal"] = "Subtotal",
        ["discount"] = "Discount",
        ["tax"] = "Tax",
        ["exchange_rate"] = "Exchange rate",
        ["footer"] = "Thank you for your business.",
    };

    private Dictionary<string, string> _labels = FallbackLabels;

    private string L(string key) =>
        _labels.TryGetValue(key, out var val) ? val : FallbackLabels.GetValueOrDefault(key, key);

    public byte[] Generate(InvoicePdfData data, Dictionary<string, string> labels)
    {
        _labels = labels.Count > 0 ? labels : FallbackLabels;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginHorizontal(40);
                page.MarginVertical(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(c => ComposeHeader(c, data));
                page.Content().Element(c => ComposeContent(c, data));
                page.Footer().Element(c => ComposeFooter(c));
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container, InvoicePdfData data)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(left =>
                {
                    left.Item().Text("Viajes Altairis S.L.").Bold().FontSize(16);
                    left.Item().Text("Calle Altair 7, 28001 Madrid");
                    left.Item().Text("CIF: B12345678");
                });

                row.RelativeItem().AlignRight().Column(right =>
                {
                    right.Item().Text($"{L("invoice")} {data.InvoiceNumber}").Bold().FontSize(14);
                    right.Item().Text($"{L("issued")}: {data.IssuedAt:dd/MM/yyyy}");
                    right.Item().Text($"{L("status")}: {data.Status}");
                    if (data.PaidAt.HasValue)
                        right.Item().Text($"{L("paid")}: {data.PaidAt.Value:dd/MM/yyyy}");
                });
            });

            col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

            col.Item().Column(billTo =>
            {
                billTo.Item().Text($"{L("bill_to")}:").SemiBold();
                billTo.Item().Text(data.CustomerName);
                if (!string.IsNullOrEmpty(data.CustomerEmail))
                    billTo.Item().Text(data.CustomerEmail);
                if (!string.IsNullOrEmpty(data.CustomerAddress))
                    billTo.Item().Text(data.CustomerAddress);
                var cityLine = string.Join(", ",
                    new[] { data.CustomerPostalCode, data.CustomerCity, data.CustomerCountry }
                        .Where(s => !string.IsNullOrEmpty(s)));
                if (!string.IsNullOrEmpty(cityLine))
                    billTo.Item().Text(cityLine);
                if (!string.IsNullOrEmpty(data.CustomerTaxId))
                    billTo.Item().Text($"{L("tax_id")}: {data.CustomerTaxId}");
            });

            col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
        });
    }

    private void ComposeContent(IContainer container, InvoicePdfData data)
    {
        container.PaddingVertical(5).Column(col =>
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.ConstantColumn(50);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    var style = TextStyle.Default.SemiBold().FontSize(9);

                    header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(4).Text(L("hotel")).Style(style);
                    header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(4).Text(L("room")).Style(style);
                    header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(4).Text(L("board")).Style(style);
                    header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(4).Text(L("check_in")).Style(style);
                    header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(4).Text(L("check_out")).Style(style);
                    header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(4).AlignCenter().Text(L("guests")).Style(style);
                    header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(4).AlignRight().Text(L("total")).Style(style);
                });

                foreach (var line in data.Lines)
                {
                    var cellStyle = TextStyle.Default.FontSize(9);

                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(4).Text(line.HotelName).Style(cellStyle);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(4).Text(line.RoomType).Style(cellStyle);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(4).Text(line.BoardType).Style(cellStyle);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(4).Text(line.CheckIn.ToString("dd/MM/yyyy")).Style(cellStyle);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(4).Text(line.CheckOut.ToString("dd/MM/yyyy")).Style(cellStyle);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(4).AlignCenter().Text(line.GuestCount.ToString()).Style(cellStyle);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(4).AlignRight().Text($"{line.LineTotal:N2} {data.CurrencyCode}").Style(cellStyle);
                }
            });

            col.Item().PaddingTop(15).AlignRight().Width(250).Column(summary =>
            {
                SummaryRow(summary, L("subtotal"), data.Subtotal, data.CurrencyCode);
                if (data.DiscountAmount != 0)
                    SummaryRow(summary, L("discount"), -data.DiscountAmount, data.CurrencyCode);
                SummaryRow(summary, L("tax"), data.TaxAmount, data.CurrencyCode);

                summary.Item().PaddingVertical(4).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                summary.Item().Row(row =>
                {
                    row.RelativeItem().Text(L("total")).Bold().FontSize(12);
                    row.RelativeItem().AlignRight().Text($"{data.TotalAmount:N2} {data.CurrencyCode}").Bold().FontSize(12);
                });
            });

            if (data.ExchangeRateToEur != 1m)
            {
                col.Item().PaddingTop(15).Text(
                    $"{L("exchange_rate")}: 1 {data.CurrencyCode} = {data.ExchangeRateToEur:N6} EUR")
                    .FontSize(8).FontColor(Colors.Grey.Medium);
            }
        });
    }

    private static void SummaryRow(ColumnDescriptor col, string label, decimal amount, string currency)
    {
        col.Item().Row(row =>
        {
            row.RelativeItem().Text(label);
            row.RelativeItem().AlignRight().Text($"{amount:N2} {currency}");
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(L("footer")).FontSize(9).FontColor(Colors.Grey.Medium);
    }
}
