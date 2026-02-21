using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.UserSubscriptions.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.UserSubscriptions.Queries;

public record GetUserSubscriptionsQuery(long? UserId = null) : IRequest<IEnumerable<UserSubscriptionDto>>;

public class GetUserSubscriptionsHandler : IRequestHandler<GetUserSubscriptionsQuery, IEnumerable<UserSubscriptionDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetUserSubscriptionsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<UserSubscriptionDto>> Handle(GetUserSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        var sql = @"SELECT us.id AS Id, us.user_id AS UserId, u.email AS UserEmail,
                           us.subscription_type_id AS SubscriptionTypeId,
                           us.start_date AS StartDate, us.end_date AS EndDate, us.active AS Active,
                           us.created_at AS CreatedAt, us.updated_at AS UpdatedAt
                    FROM user_subscription us
                    JOIN user u ON u.id = us.user_id";
        if (request.UserId.HasValue)
            sql += " WHERE us.user_id = @UserId";
        sql += " ORDER BY us.created_at DESC";
        return await connection.QueryAsync<UserSubscriptionDto>(sql, new { request.UserId });
    }
}
