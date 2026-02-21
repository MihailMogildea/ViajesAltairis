using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Reservations.Queries;

public record GetInvoicePdfQuery(long InvoiceId, long UserId, long? LanguageId = null) : IRequest<InvoicePdfResult?>;

public record InvoicePdfResult(byte[] PdfBytes, string FileName);

public class GetInvoicePdfHandler : IRequestHandler<GetInvoicePdfQuery, InvoicePdfResult?>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IInvoicePdfGenerator _pdfGenerator;

    public GetInvoicePdfHandler(IDbConnectionFactory connectionFactory, IInvoicePdfGenerator pdfGenerator)
    {
        _connectionFactory = connectionFactory;
        _pdfGenerator = pdfGenerator;
    }

    public async Task<InvoicePdfResult?> Handle(GetInvoicePdfQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var invoice = await Dapper.SqlMapper.QuerySingleOrDefaultAsync<dynamic>(
            connection,
            """
            SELECT i.invoice_number, ins.name AS status,
                   i.created_at AS issued_at, i.updated_at AS paid_at,
                   i.subtotal, i.discount_amount, i.tax_amount, i.total_amount,
                   c.iso_code AS currency_code, er.rate_to_eur,
                   r.owner_first_name, r.owner_last_name, r.owner_email,
                   r.owner_address, r.owner_city, r.owner_postal_code,
                   r.owner_country, r.owner_tax_id,
                   u.language_id, l.iso_code AS language_code
            FROM invoice i
            JOIN reservation r ON r.id = i.reservation_id
            JOIN invoice_status ins ON ins.id = i.status_id
            JOIN currency c ON c.id = r.currency_id
            JOIN exchange_rate er ON er.id = r.exchange_rate_id
            JOIN user u ON u.id = @UserId
            JOIN language l ON l.id = u.language_id
            WHERE i.id = @InvoiceId AND (r.booked_by_user_id = @UserId OR r.owner_user_id = @UserId)
            """,
            new { request.InvoiceId, request.UserId });

        if (invoice is null) return null;

        long langId = request.LanguageId ?? (long)invoice.language_id;

        var labelRows = await Dapper.SqlMapper.QueryAsync<dynamic>(
            connection,
            """
            SELECT translation_key, value FROM web_translation
            WHERE language_id = @LangId AND translation_key LIKE 'pdf.invoice.%'
            """,
            new { LangId = langId });

        var labels = labelRows.ToDictionary(
            r => (string)r.translation_key.Replace("pdf.invoice.", ""),
            r => (string)r.value);

        var lines = (await Dapper.SqlMapper.QueryAsync<dynamic>(
            connection,
            """
            SELECT h.name AS hotel_name,
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
            JOIN invoice i ON i.reservation_id = rl.reservation_id
            LEFT JOIN translation trt ON trt.entity_type = 'room_type' AND trt.entity_id = rt.id AND trt.field = 'name' AND trt.language_id = @LangId
            LEFT JOIN translation tbt ON tbt.entity_type = 'board_type' AND tbt.entity_id = bt.id AND tbt.field = 'name' AND tbt.language_id = @LangId
            WHERE i.id = @InvoiceId
            """,
            new { request.InvoiceId, LangId = langId })).ToList();

        var pdfLines = lines.Select(l => new InvoicePdfLine(
            (string)l.hotel_name,
            (string)l.room_type,
            (string)l.board_type,
            (DateTime)l.check_in,
            (DateTime)l.check_out,
            (int)l.guest_count,
            (decimal)l.line_total)).ToList();

        var data = new InvoicePdfData(
            InvoiceNumber: (string)invoice.invoice_number,
            IssuedAt: (DateTime)invoice.issued_at,
            PaidAt: invoice.paid_at as DateTime?,
            Status: (string)invoice.status,
            CustomerName: $"{(string)invoice.owner_first_name} {(string)invoice.owner_last_name}",
            CustomerEmail: invoice.owner_email as string,
            CustomerAddress: invoice.owner_address as string,
            CustomerCity: invoice.owner_city as string,
            CustomerPostalCode: invoice.owner_postal_code as string,
            CustomerCountry: invoice.owner_country as string,
            CustomerTaxId: invoice.owner_tax_id as string,
            Lines: pdfLines,
            Subtotal: (decimal)invoice.subtotal,
            DiscountAmount: (decimal)invoice.discount_amount,
            TaxAmount: (decimal)invoice.tax_amount,
            TotalAmount: (decimal)invoice.total_amount,
            CurrencyCode: (string)invoice.currency_code,
            ExchangeRateToEur: (decimal)invoice.rate_to_eur);

        var pdfBytes = _pdfGenerator.Generate(data, labels);

        return new InvoicePdfResult(pdfBytes, $"{data.InvoiceNumber}.pdf");
    }
}
