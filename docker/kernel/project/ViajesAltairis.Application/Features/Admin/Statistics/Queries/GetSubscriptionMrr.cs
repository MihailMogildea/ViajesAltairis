using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Statistics.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Statistics.Queries;

public record GetSubscriptionMrrQuery : IRequest<IEnumerable<SubscriptionMrrDto>>;

public class GetSubscriptionMrrHandler : IRequestHandler<GetSubscriptionMrrQuery, IEnumerable<SubscriptionMrrDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetSubscriptionMrrHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<SubscriptionMrrDto>> Handle(GetSubscriptionMrrQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<SubscriptionMrrDto>(
            """
            SELECT st.name AS SubscriptionName, c.iso_code AS CurrencyCode,
                   COUNT(us.id) AS ActiveCount,
                   COUNT(us.id) * st.price_per_month AS MonthlyRevenue
            FROM subscription_type st
            JOIN currency c ON c.id = st.currency_id
            LEFT JOIN user_subscription us ON us.subscription_type_id = st.id AND us.active = 1
            WHERE st.enabled = 1
            GROUP BY st.id, st.name, c.iso_code, st.price_per_month
            ORDER BY MonthlyRevenue DESC
            """);
    }
}
