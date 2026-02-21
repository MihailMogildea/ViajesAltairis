using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.PaymentTransactions.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.PaymentTransactions.Queries;

public record GetPaymentTransactionsQuery(DateTime? From = null, DateTime? To = null, long? StatusId = null) : IRequest<IEnumerable<PaymentTransactionDto>>;

public class GetPaymentTransactionsHandler : IRequestHandler<GetPaymentTransactionsQuery, IEnumerable<PaymentTransactionDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetPaymentTransactionsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<PaymentTransactionDto>> Handle(GetPaymentTransactionsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();

        var sql = @"SELECT pt.id AS Id, pt.reservation_id AS ReservationId, pt.payment_method_id AS PaymentMethodId,
                     pt.transaction_reference AS TransactionReference, pt.amount AS Amount,
                     pt.currency_id AS CurrencyId, c.iso_code AS CurrencyCode,
                     pt.exchange_rate_id AS ExchangeRateId,
                     pt.status_id AS StatusId, pt.created_at AS CreatedAt, pt.updated_at AS UpdatedAt
              FROM payment_transaction pt
              JOIN currency c ON c.id = pt.currency_id
              WHERE 1=1";

        if (request.From.HasValue)
            sql += " AND pt.created_at >= @From";
        if (request.To.HasValue)
            sql += " AND pt.created_at <= @To";
        if (request.StatusId.HasValue)
            sql += " AND pt.status_id = @StatusId";

        sql += " ORDER BY pt.created_at DESC";

        return await connection.QueryAsync<PaymentTransactionDto>(sql, new { request.From, request.To, request.StatusId });
    }
}
