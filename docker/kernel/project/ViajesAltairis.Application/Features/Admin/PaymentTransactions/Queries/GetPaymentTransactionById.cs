using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.PaymentTransactions.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.PaymentTransactions.Queries;

public record GetPaymentTransactionByIdQuery(long Id) : IRequest<PaymentTransactionDto?>;

public class GetPaymentTransactionByIdHandler : IRequestHandler<GetPaymentTransactionByIdQuery, PaymentTransactionDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetPaymentTransactionByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<PaymentTransactionDto?> Handle(GetPaymentTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<PaymentTransactionDto>(
            @"SELECT pt.id AS Id, pt.reservation_id AS ReservationId, pt.payment_method_id AS PaymentMethodId,
                     pt.transaction_reference AS TransactionReference, pt.amount AS Amount,
                     pt.currency_id AS CurrencyId, c.iso_code AS CurrencyCode,
                     pt.exchange_rate_id AS ExchangeRateId,
                     pt.status_id AS StatusId, pt.created_at AS CreatedAt, pt.updated_at AS UpdatedAt
              FROM payment_transaction pt
              JOIN currency c ON c.id = pt.currency_id
              WHERE pt.id = @Id",
            new { request.Id });
    }
}
