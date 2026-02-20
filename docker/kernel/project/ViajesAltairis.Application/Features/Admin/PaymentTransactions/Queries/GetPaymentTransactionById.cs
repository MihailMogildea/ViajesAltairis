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
            @"SELECT id AS Id, reservation_id AS ReservationId, payment_method_id AS PaymentMethodId,
                     transaction_reference AS TransactionReference, amount AS Amount,
                     currency_id AS CurrencyId, exchange_rate_id AS ExchangeRateId,
                     status AS Status, created_at AS CreatedAt, updated_at AS UpdatedAt
              FROM payment_transaction WHERE id = @Id",
            new { request.Id });
    }
}
