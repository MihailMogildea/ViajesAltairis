using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.PaymentTransactionFees.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.PaymentTransactionFees.Queries;

public record GetPaymentTransactionFeesQuery : IRequest<IEnumerable<PaymentTransactionFeeDto>>;

public class GetPaymentTransactionFeesHandler : IRequestHandler<GetPaymentTransactionFeesQuery, IEnumerable<PaymentTransactionFeeDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetPaymentTransactionFeesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<PaymentTransactionFeeDto>> Handle(GetPaymentTransactionFeesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<PaymentTransactionFeeDto>(
            @"SELECT id AS Id, payment_transaction_id AS PaymentTransactionId, fee_type AS FeeType,
                     fee_amount AS FeeAmount, fee_percentage AS FeePercentage, fixed_fee_amount AS FixedFeeAmount,
                     currency_id AS CurrencyId, description AS Description, created_at AS CreatedAt
              FROM payment_transaction_fee ORDER BY created_at DESC");
    }
}
