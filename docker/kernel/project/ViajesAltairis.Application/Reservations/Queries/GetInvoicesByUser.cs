using FluentValidation;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Reservations.Queries;

public record GetInvoicesByUserQuery(long UserId, int Page, int PageSize) : IRequest<InvoiceListResult>;

public class GetInvoicesByUserHandler : IRequestHandler<GetInvoicesByUserQuery, InvoiceListResult>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetInvoicesByUserHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<InvoiceListResult> Handle(GetInvoicesByUserQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        var offset = (request.Page - 1) * request.PageSize;

        var totalCount = await Dapper.SqlMapper.ExecuteScalarAsync<int>(
            connection,
            """
            SELECT COUNT(*) FROM invoice i
            JOIN reservation r ON r.id = i.reservation_id
            WHERE (r.booked_by_user_id = @UserId OR r.owner_user_id = @UserId)
            """,
            new { request.UserId });

        var invoices = await Dapper.SqlMapper.QueryAsync<dynamic>(
            connection,
            """
            SELECT i.id, i.invoice_number, i.status_id, ins.name AS status, i.total_amount,
                   c.iso_code AS currency, i.created_at
            FROM invoice i
            JOIN reservation r ON r.id = i.reservation_id
            JOIN invoice_status ins ON ins.id = i.status_id
            JOIN currency c ON c.id = r.currency_id
            WHERE (r.booked_by_user_id = @UserId OR r.owner_user_id = @UserId)
            ORDER BY i.created_at DESC
            LIMIT @PageSize OFFSET @Offset
            """,
            new { request.UserId, request.PageSize, Offset = offset });

        var summaries = invoices.Select(inv => new InvoiceSummaryResult(
            (long)inv.id,
            (string)inv.invoice_number,
            (long)inv.status_id,
            (string)inv.status,
            (decimal)inv.total_amount,
            (string)inv.currency,
            (DateTime)inv.created_at)).ToList();

        return new InvoiceListResult(summaries, totalCount);
    }
}

public class GetInvoicesByUserValidator : AbstractValidator<GetInvoicesByUserQuery>
{
    public GetInvoicesByUserValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
