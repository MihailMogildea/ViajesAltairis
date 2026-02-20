using Dapper;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.ScheduledApi.Jobs;

public class SubscriptionBillingJob
{
    private readonly IDbConnectionFactory _db;
    private readonly ILogger<SubscriptionBillingJob> _logger;

    public SubscriptionBillingJob(IDbConnectionFactory db, ILogger<SubscriptionBillingJob> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Subscription billing started");

        // TODO: Query active subscriptions due for billing
        // TODO: Process billing for each subscription
        // TODO: Update subscription status and next billing date

        await Task.CompletedTask;

        using var connection = _db.CreateConnection();
        await connection.ExecuteAsync(
            "UPDATE job_schedule SET last_executed_at = NOW() WHERE job_key = 'subscription-billing'");

        _logger.LogInformation("Subscription billing completed");
    }
}
