using System.IO.Compression;
using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Billing.Queries;

public record ExportBillingZipQuery(DateTime From, DateTime To, string Channel) : IRequest<BillingZipResult>;

public record BillingZipResult(byte[] ZipBytes, string FileName, int InvoiceCount);

public class ExportBillingZipHandler : IRequestHandler<ExportBillingZipQuery, BillingZipResult>
{
    private readonly IDbConnectionFactory _db;
    private readonly IInvoicePdfGenerator _pdfGenerator;
    private readonly ICurrentUserService _currentUser;

    public ExportBillingZipHandler(IDbConnectionFactory db, IInvoicePdfGenerator pdfGenerator, ICurrentUserService currentUser)
    {
        _db = db;
        _pdfGenerator = pdfGenerator;
        _currentUser = currentUser;
    }

    public async Task<BillingZipResult> Handle(ExportBillingZipQuery request, CancellationToken cancellationToken)
    {
        var channelPrefix = request.Channel switch
        {
            "client" => "CLI",
            "external" => "B2B",
            "staff" => "STF",
            _ => throw new ArgumentException($"Invalid channel: {request.Channel}")
        };

        var channelFilter = request.Channel switch
        {
            "client" => "AND u.user_type_id = 5 AND u.business_partner_id IS NULL",
            "external" => "AND u.business_partner_id IS NOT NULL",
            "staff" => "AND u.user_type_id IN (1,2,3,4) AND u.business_partner_id IS NULL",
            _ => throw new ArgumentException($"Invalid channel: {request.Channel}")
        };

        using var connection = _db.CreateConnection();

        // Get reservations matching channel + date range
        var reservations = (await connection.QueryAsync<dynamic>(
            $"""
            SELECT DISTINCT r.id, r.reservation_code, r.status_id,
                   r.owner_first_name, r.owner_last_name, r.owner_email,
                   r.owner_address, r.owner_city, r.owner_postal_code,
                   r.owner_country, r.owner_tax_id,
                   r.subtotal, r.tax_amount, r.discount_amount, r.total_price,
                   r.created_at,
                   c.iso_code AS currency_code, er.rate_to_eur,
                   bp.company_name AS bp_company_name, bp.tax_id AS bp_tax_id,
                   bp.address AS bp_address, bp.city AS bp_city,
                   bp.postal_code AS bp_postal_code, bp.country AS bp_country
            FROM reservation r
            JOIN user u ON u.id = r.booked_by_user_id
            JOIN currency c ON c.id = r.currency_id
            JOIN exchange_rate er ON er.id = r.exchange_rate_id
            LEFT JOIN business_partner bp ON bp.id = u.business_partner_id
            JOIN reservation_line rl ON rl.reservation_id = r.id
            WHERE r.status_id IN (3, 4, 5)
              AND rl.check_in_date >= @From
              AND rl.check_in_date <= @To
              {channelFilter}
            ORDER BY r.id
            """,
            new { request.From, request.To })).ToList();

        if (reservations.Count == 0)
        {
            var emptyFileName = $"billing-{channelPrefix}-{request.From:yyyyMMdd}-{request.To:yyyyMMdd}.zip";
            using var emptyMs = new MemoryStream();
            using (var emptyZip = new ZipArchive(emptyMs, ZipArchiveMode.Create, true))
            {
                // Empty ZIP
            }
            return new BillingZipResult(emptyMs.ToArray(), emptyFileName, 0);
        }

        var reservationIds = reservations.Select(r => (long)r.id).ToList();

        // Fetch PDF labels in admin's language
        var langId = _currentUser.LanguageId;
        var labelRows = await connection.QueryAsync<dynamic>(
            """
            SELECT translation_key, value FROM web_translation
            WHERE language_id = @LangId AND translation_key LIKE 'pdf.invoice.%'
            """,
            new { LangId = langId });

        var labels = labelRows.ToDictionary(
            r => (string)r.translation_key.Replace("pdf.invoice.", ""),
            r => (string)r.value);

        // Get all lines for these reservations (with translated room_type and board_type names)
        var lines = (await connection.QueryAsync<dynamic>(
            """
            SELECT rl.reservation_id, h.name AS hotel_name,
                   COALESCE(trt.value, rt.name) AS room_type,
                   COALESCE(tbt.value, bt.name) AS board_type,
                   rl.check_in_date AS check_in, rl.check_out_date AS check_out,
                   rl.num_guests AS guest_count, rl.total_price AS line_total
            FROM reservation_line rl
            JOIN hotel_provider_room_type hprt ON hprt.id = rl.hotel_provider_room_type_id
            JOIN hotel_provider hp ON hp.id = hprt.hotel_provider_id
            JOIN hotel h ON h.id = hp.hotel_id
            JOIN room_type rt ON rt.id = hprt.room_type_id
            JOIN board_type bt ON bt.id = rl.board_type_id
            LEFT JOIN translation trt ON trt.entity_type = 'room_type' AND trt.entity_id = rt.id AND trt.field = 'name' AND trt.language_id = @LangId
            LEFT JOIN translation tbt ON tbt.entity_type = 'board_type' AND tbt.entity_id = bt.id AND tbt.field = 'name' AND tbt.language_id = @LangId
            WHERE rl.reservation_id IN @Ids
            """,
            new { Ids = reservationIds, LangId = langId })).ToList();

        var linesByReservation = lines.GroupBy(l => (long)l.reservation_id)
            .ToDictionary(g => g.Key, g => g.ToList());

        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
        {
            foreach (var r in reservations)
            {
                var resId = (long)r.id;
                var resCode = (string)r.reservation_code;
                var invoiceNumber = $"BIL-{channelPrefix}-{resCode}";

                var isB2B = r.bp_company_name is not null;
                var customerName = isB2B
                    ? (string)r.bp_company_name
                    : $"{(string)r.owner_first_name} {(string)r.owner_last_name}";

                var pdfLines = linesByReservation.GetValueOrDefault(resId, [])
                    .Select(l => new InvoicePdfLine(
                        (string)l.hotel_name,
                        (string)l.room_type,
                        (string)l.board_type,
                        (DateTime)l.check_in,
                        (DateTime)l.check_out,
                        (int)l.guest_count,
                        (decimal)l.line_total))
                    .ToList();

                var data = new InvoicePdfData(
                    InvoiceNumber: invoiceNumber,
                    IssuedAt: DateTime.UtcNow,
                    PaidAt: null,
                    Status: "Billing",
                    CustomerName: customerName,
                    CustomerEmail: isB2B ? null : r.owner_email as string,
                    CustomerAddress: isB2B ? r.bp_address as string : r.owner_address as string,
                    CustomerCity: isB2B ? r.bp_city as string : r.owner_city as string,
                    CustomerPostalCode: isB2B ? r.bp_postal_code as string : r.owner_postal_code as string,
                    CustomerCountry: isB2B ? r.bp_country as string : r.owner_country as string,
                    CustomerTaxId: isB2B ? r.bp_tax_id as string : r.owner_tax_id as string,
                    Lines: pdfLines,
                    Subtotal: (decimal)r.subtotal,
                    DiscountAmount: (decimal)r.discount_amount,
                    TaxAmount: (decimal)r.tax_amount,
                    TotalAmount: (decimal)r.total_price,
                    CurrencyCode: (string)r.currency_code,
                    ExchangeRateToEur: (decimal)r.rate_to_eur);

                var pdfBytes = _pdfGenerator.Generate(data, labels);

                var entry = zip.CreateEntry($"{invoiceNumber}.pdf", CompressionLevel.Fastest);
                using var entryStream = entry.Open();
                entryStream.Write(pdfBytes, 0, pdfBytes.Length);
            }
        }

        var fileName = $"billing-{channelPrefix}-{request.From:yyyyMMdd}-{request.To:yyyyMMdd}.zip";
        return new BillingZipResult(ms.ToArray(), fileName, reservations.Count);
    }
}
