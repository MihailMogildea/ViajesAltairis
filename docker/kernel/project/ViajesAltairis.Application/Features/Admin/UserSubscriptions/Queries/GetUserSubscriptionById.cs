using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.UserSubscriptions.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.UserSubscriptions.Queries;

public record GetUserSubscriptionByIdQuery(long Id) : IRequest<UserSubscriptionDto?>;

public class GetUserSubscriptionByIdHandler : IRequestHandler<GetUserSubscriptionByIdQuery, UserSubscriptionDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetUserSubscriptionByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<UserSubscriptionDto?> Handle(GetUserSubscriptionByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<UserSubscriptionDto>(
            @"SELECT id AS Id, user_id AS UserId, subscription_type_id AS SubscriptionTypeId,
                     start_date AS StartDate, end_date AS EndDate, active AS Active,
                     created_at AS CreatedAt, updated_at AS UpdatedAt
              FROM user_subscription WHERE id = @Id",
            new { request.Id });
    }
}
