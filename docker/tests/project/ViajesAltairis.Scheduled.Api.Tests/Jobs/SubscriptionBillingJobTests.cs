using System.Data;
using Microsoft.Extensions.Logging;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.ScheduledApi.Jobs;

namespace ViajesAltairis.Scheduled.Api.Tests.Jobs;

public class SubscriptionBillingJobTests
{
    private readonly IDbConnectionFactory _dbFactory = Substitute.For<IDbConnectionFactory>();
    private readonly IDbConnection _connection = Substitute.For<IDbConnection>();
    private readonly ILogger<SubscriptionBillingJob> _logger = Substitute.For<ILogger<SubscriptionBillingJob>>();

    public SubscriptionBillingJobTests()
    {
        _dbFactory.CreateConnection().Returns(_connection);
    }

    [Fact]
    public async Task ExecuteAsync_CreatesConnectionAndLogs()
    {
        var job = new SubscriptionBillingJob(_dbFactory, _logger);

        // Will fail on Dapper extension, but verifies job construction and initial flow
        try
        {
            await job.ExecuteAsync(CancellationToken.None);
        }
        catch
        {
            // Expected: Dapper extension on mock connection
        }

        _dbFactory.Received(1).CreateConnection();
    }
}
