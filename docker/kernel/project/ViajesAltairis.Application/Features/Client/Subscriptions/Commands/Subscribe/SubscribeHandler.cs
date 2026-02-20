using System.Data;
using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Subscriptions.Commands.Subscribe;

public class SubscribeHandler : IRequestHandler<SubscribeCommand, SubscribeResponse>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IDbConnectionFactory _connectionFactory;

    public SubscribeHandler(ICurrentUserService currentUser, IDbConnectionFactory connectionFactory)
    {
        _currentUser = currentUser;
        _connectionFactory = connectionFactory;
    }

    public async Task<SubscribeResponse> Handle(SubscribeCommand request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        var userId = _currentUser.UserId!.Value;

        var subType = await connection.QuerySingleOrDefaultAsync<dynamic>(
            "SELECT id, name FROM subscription_type WHERE id = @Id AND enabled = TRUE",
            new { Id = request.SubscriptionTypeId }, transaction);

        if (subType == null)
            throw new KeyNotFoundException("Subscription plan not found.");

        var now = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(now);

        await connection.ExecuteAsync(
            "UPDATE user_subscription SET active = FALSE, end_date = @Today WHERE user_id = @UserId AND active = TRUE",
            new { Today = today, UserId = userId }, transaction);

        var startDate = today;
        var endDate = startDate.AddMonths(1);

        var newId = await connection.ExecuteScalarAsync<long>("""
            INSERT INTO user_subscription (user_id, subscription_type_id, start_date, end_date, active, created_at, updated_at)
            VALUES (@UserId, @SubscriptionTypeId, @StartDate, @EndDate, TRUE, @Now, @Now);
            SELECT LAST_INSERT_ID();
            """, new
        {
            UserId = userId,
            request.SubscriptionTypeId,
            StartDate = startDate,
            EndDate = endDate,
            Now = now
        }, transaction);

        transaction.Commit();

        return new SubscribeResponse
        {
            SubscriptionId = newId,
            StartDate = startDate.ToDateTime(TimeOnly.MinValue),
            EndDate = endDate.ToDateTime(TimeOnly.MinValue)
        };
    }
}
