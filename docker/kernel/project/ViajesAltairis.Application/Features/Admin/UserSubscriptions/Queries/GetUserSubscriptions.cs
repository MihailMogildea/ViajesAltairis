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
        var sql = @"SELECT id AS Id, user_id AS UserId, subscription_type_id AS SubscriptionTypeId,
                           start_date AS StartDate, end_date AS EndDate, active AS Active,
                           created_at AS CreatedAt, updated_at AS UpdatedAt
                    FROM user_subscription";
        if (request.UserId.HasValue)
            sql += " WHERE user_id = @UserId";
        sql += " ORDER BY created_at DESC";
        return await connection.QueryAsync<UserSubscriptionDto>(sql, new { request.UserId });
    }
}
