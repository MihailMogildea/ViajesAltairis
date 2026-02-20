using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.PaymentTransactions.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.PaymentTransactions.Queries;

public record GetPaymentTransactionsQuery : IRequest<IEnumerable<PaymentTransactionDto>>;

public class GetPaymentTransactionsHandler : IRequestHandler<GetPaymentTransactionsQuery, IEnumerable<PaymentTransactionDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetPaymentTransactionsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<PaymentTransactionDto>> Handle(GetPaymentTransactionsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<PaymentTransactionDto>(
            @"SELECT id AS Id, reservation_id AS ReservationId, payment_method_id AS PaymentMethodId,
                     transaction_reference AS TransactionReference, amount AS Amount,
                     currency_id AS CurrencyId, exchange_rate_id AS ExchangeRateId,
                     status AS Status, created_at AS CreatedAt, updated_at AS UpdatedAt
              FROM payment_transaction ORDER BY created_at DESC");
    }
}
