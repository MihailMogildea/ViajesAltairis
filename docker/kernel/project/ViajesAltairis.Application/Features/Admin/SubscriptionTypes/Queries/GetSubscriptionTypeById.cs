using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Queries;

public record GetSubscriptionTypeByIdQuery(long Id) : IRequest<SubscriptionTypeDto?>;

public class GetSubscriptionTypeByIdHandler : IRequestHandler<GetSubscriptionTypeByIdQuery, SubscriptionTypeDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetSubscriptionTypeByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<SubscriptionTypeDto?> Handle(GetSubscriptionTypeByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SubscriptionTypeDto>(
            @"SELECT id AS Id, name AS Name, price_per_month AS PricePerMonth, discount AS Discount,
                     currency_id AS CurrencyId, enabled AS Enabled, created_at AS CreatedAt, updated_at AS UpdatedAt
              FROM subscription_type WHERE id = @Id",
            new { request.Id });
    }
}
