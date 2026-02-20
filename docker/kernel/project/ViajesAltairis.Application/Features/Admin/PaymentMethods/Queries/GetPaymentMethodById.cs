using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.PaymentMethods.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.PaymentMethods.Queries;

public record GetPaymentMethodByIdQuery(long Id) : IRequest<PaymentMethodDto?>;

public class GetPaymentMethodByIdHandler : IRequestHandler<GetPaymentMethodByIdQuery, PaymentMethodDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetPaymentMethodByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<PaymentMethodDto?> Handle(GetPaymentMethodByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<PaymentMethodDto>(
            "SELECT id AS Id, name AS Name, min_days_before_checkin AS MinDaysBeforeCheckin, enabled AS Enabled, created_at AS CreatedAt FROM payment_method WHERE id = @Id",
            new { request.Id });
    }
}
