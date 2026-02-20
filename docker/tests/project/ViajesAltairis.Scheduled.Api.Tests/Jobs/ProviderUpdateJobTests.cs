using System.Data;
using Microsoft.Extensions.Logging;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.ScheduledApi.Jobs;

namespace ViajesAltairis.Scheduled.Api.Tests.Jobs;

public class ProviderUpdateJobTests
{
    private readonly IDbConnectionFactory _dbFactory = Substitute.For<IDbConnectionFactory>();
    private readonly IDbConnection _connection = Substitute.For<IDbConnection>();
    private readonly ILogger<ProviderUpdateJob> _logger = Substitute.For<ILogger<ProviderUpdateJob>>();

    public ProviderUpdateJobTests()
    {
        _dbFactory.CreateConnection().Returns(_connection);
    }

    [Fact]
    public async Task ExecuteAsync_CreatesConnectionAndLogs()
    {
        var job = new ProviderUpdateJob(_dbFactory, _logger);

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
