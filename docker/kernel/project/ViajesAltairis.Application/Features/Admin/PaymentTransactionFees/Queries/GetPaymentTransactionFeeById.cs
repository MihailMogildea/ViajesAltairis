using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.PaymentTransactionFees.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.PaymentTransactionFees.Queries;

public record GetPaymentTransactionFeeByIdQuery(long Id) : IRequest<PaymentTransactionFeeDto?>;

public class GetPaymentTransactionFeeByIdHandler : IRequestHandler<GetPaymentTransactionFeeByIdQuery, PaymentTransactionFeeDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetPaymentTransactionFeeByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<PaymentTransactionFeeDto?> Handle(GetPaymentTransactionFeeByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<PaymentTransactionFeeDto>(
            @"SELECT id AS Id, payment_transaction_id AS PaymentTransactionId, fee_type AS FeeType,
                     fee_amount AS FeeAmount, fee_percentage AS FeePercentage, fixed_fee_amount AS FixedFeeAmount,
                     currency_id AS CurrencyId, description AS Description, created_at AS CreatedAt
              FROM payment_transaction_fee WHERE id = @Id",
            new { request.Id });
    }
}
