using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Reservations.Queries;

public record GetInvoiceByIdQuery(long InvoiceId, long UserId) : IRequest<InvoiceDetailResult?>;

public class GetInvoiceByIdHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDetailResult?>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetInvoiceByIdHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<InvoiceDetailResult?> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var invoice = await Dapper.SqlMapper.QuerySingleOrDefaultAsync<dynamic>(
            connection,
            """
            SELECT i.id, i.invoice_number, ins.name AS status,
                   i.subtotal, i.tax_amount, i.total_amount,
                   c.iso_code AS currency, er.rate_to_eur AS exchange_rate_to_eur,
                   i.created_at, i.updated_at, i.reservation_id
            FROM invoice i
            JOIN reservation r ON r.id = i.reservation_id
            JOIN invoice_status ins ON ins.id = i.status_id
            JOIN currency c ON c.id = r.currency_id
            JOIN exchange_rate er ON er.id = r.exchange_rate_id
            WHERE i.id = @InvoiceId AND r.booked_by_user_id = @UserId
            """,
            new { request.InvoiceId, request.UserId });

        if (invoice is null) return null;

        return new InvoiceDetailResult(
            (long)invoice.id,
            (string)invoice.invoice_number,
            (string)invoice.status,
            (decimal)invoice.subtotal,
            (decimal)invoice.tax_amount,
            (decimal)invoice.total_amount,
            (string)invoice.currency,
            (decimal)invoice.exchange_rate_to_eur,
            (DateTime)invoice.created_at,
            invoice.updated_at as DateTime?,
            (long)invoice.reservation_id);
    }
}
