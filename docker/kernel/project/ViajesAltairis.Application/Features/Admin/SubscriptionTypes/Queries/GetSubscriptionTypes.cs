using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Queries;

public record GetSubscriptionTypesQuery : IRequest<IEnumerable<SubscriptionTypeDto>>;

public class GetSubscriptionTypesHandler : IRequestHandler<GetSubscriptionTypesQuery, IEnumerable<SubscriptionTypeDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetSubscriptionTypesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<SubscriptionTypeDto>> Handle(GetSubscriptionTypesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<SubscriptionTypeDto>(
            @"SELECT id AS Id, name AS Name, price_per_month AS PricePerMonth, discount AS Discount,
                     currency_id AS CurrencyId, enabled AS Enabled, created_at AS CreatedAt, updated_at AS UpdatedAt
              FROM subscription_type ORDER BY name");
    }
}
