using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.PaymentTransactions.Queries;

public record PaymentTransactionStatusDto(long Id, string Name);

public record GetPaymentTransactionStatusesQuery : IRequest<IEnumerable<PaymentTransactionStatusDto>>;

public class GetPaymentTransactionStatusesHandler : IRequestHandler<GetPaymentTransactionStatusesQuery, IEnumerable<PaymentTransactionStatusDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetPaymentTransactionStatusesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<PaymentTransactionStatusDto>> Handle(GetPaymentTransactionStatusesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<PaymentTransactionStatusDto>(
            "SELECT id AS Id, name AS Name FROM payment_transaction_status ORDER BY id");
    }
}
