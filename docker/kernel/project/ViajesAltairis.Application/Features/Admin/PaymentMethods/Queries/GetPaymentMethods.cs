using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.PaymentMethods.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.PaymentMethods.Queries;

public record GetPaymentMethodsQuery : IRequest<IEnumerable<PaymentMethodDto>>;

public class GetPaymentMethodsHandler : IRequestHandler<GetPaymentMethodsQuery, IEnumerable<PaymentMethodDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetPaymentMethodsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<PaymentMethodDto>> Handle(GetPaymentMethodsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<PaymentMethodDto>(
            "SELECT id AS Id, name AS Name, min_days_before_checkin AS MinDaysBeforeCheckin, enabled AS Enabled, created_at AS CreatedAt FROM payment_method ORDER BY name");
    }
}
