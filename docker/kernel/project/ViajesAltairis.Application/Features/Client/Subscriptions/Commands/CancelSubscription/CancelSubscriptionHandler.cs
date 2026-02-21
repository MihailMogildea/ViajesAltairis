using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Subscriptions.Commands.CancelSubscription;

public class CancelSubscriptionHandler : IRequestHandler<CancelSubscriptionCommand, Unit>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IDbConnectionFactory _connectionFactory;

    public CancelSubscriptionHandler(ICurrentUserService currentUser, IDbConnectionFactory connectionFactory)
    {
        _currentUser = currentUser;
        _connectionFactory = connectionFactory;
    }

    public async Task<Unit> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        var userId = _currentUser.UserId!.Value;

        var rows = await connection.ExecuteAsync(
            "UPDATE user_subscription SET active = FALSE, end_date = @Today, updated_at = @Now WHERE user_id = @UserId AND active = TRUE",
            new { Today = DateOnly.FromDateTime(DateTime.UtcNow), Now = DateTime.UtcNow, UserId = userId });

        if (rows == 0)
            throw new KeyNotFoundException("No active subscription found.");

        return Unit.Value;
    }
}
