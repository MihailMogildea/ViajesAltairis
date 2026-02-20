using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Statistics.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Statistics.Queries;

public record GetActiveSubscriptionsQuery : IRequest<ActiveSubscriptionsDto>;

public class GetActiveSubscriptionsHandler : IRequestHandler<GetActiveSubscriptionsQuery, ActiveSubscriptionsDto>
{
    private readonly IDbConnectionFactory _db;
    public GetActiveSubscriptionsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<ActiveSubscriptionsDto> Handle(GetActiveSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleAsync<ActiveSubscriptionsDto>(
            """
            SELECT
                (SELECT COUNT(*) FROM user_subscription WHERE active = 1) AS ActiveCount,
                (SELECT COUNT(*) FROM user WHERE user_type_id = 5 AND enabled = 1) AS TotalUsers,
                CASE WHEN (SELECT COUNT(*) FROM user WHERE user_type_id = 5 AND enabled = 1) > 0
                     THEN ROUND((SELECT COUNT(*) FROM user_subscription WHERE active = 1) * 100.0
                          / (SELECT COUNT(*) FROM user WHERE user_type_id = 5 AND enabled = 1), 2)
                     ELSE 0 END AS SubscriptionRate
            """);
    }
}
