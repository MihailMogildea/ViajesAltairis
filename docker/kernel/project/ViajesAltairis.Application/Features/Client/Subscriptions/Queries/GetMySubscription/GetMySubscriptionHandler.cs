using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Subscriptions.Queries.GetMySubscription;

public class GetMySubscriptionHandler : IRequestHandler<GetMySubscriptionQuery, GetMySubscriptionResponse>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ICurrentUserService _currentUser;
    private readonly ITranslationService _translationService;

    public GetMySubscriptionHandler(
        IDbConnectionFactory connectionFactory,
        ICurrentUserService currentUser,
        ITranslationService translationService)
    {
        _connectionFactory = connectionFactory;
        _currentUser = currentUser;
        _translationService = translationService;
    }

    public async Task<GetMySubscriptionResponse> Handle(GetMySubscriptionQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT
                user_subscription_id AS SubscriptionId,
                subscription_type_id AS SubscriptionTypeId,
                subscription_type_name AS PlanName,
                subscription_discount AS Discount,
                start_date AS StartDate,
                end_date AS EndDate,
                active AS IsActive
            FROM v_user_subscription_status
            WHERE user_id = @UserId AND active = TRUE
            ORDER BY start_date DESC
            LIMIT 1
            """;

        var subscription = await connection.QuerySingleOrDefaultAsync<GetMySubscriptionResponse>(sql, new { UserId = _currentUser.UserId!.Value });

        if (subscription == null)
            return new GetMySubscriptionResponse { IsActive = false };

        var langId = _currentUser.LanguageId;
        if (langId != 1 && subscription.SubscriptionTypeId.HasValue)
        {
            var names = await _translationService.ResolveAsync(
                "subscription_type", new[] { subscription.SubscriptionTypeId.Value }, langId, "name", cancellationToken);
            if (names.TryGetValue(subscription.SubscriptionTypeId.Value, out var n))
                subscription.PlanName = n;
        }

        return subscription;
    }
}
