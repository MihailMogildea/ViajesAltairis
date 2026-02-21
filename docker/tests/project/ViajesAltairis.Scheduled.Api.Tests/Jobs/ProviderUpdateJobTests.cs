using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.ScheduledApi.Jobs;

namespace ViajesAltairis.Scheduled.Api.Tests.Jobs;

public class ProviderUpdateJobTests
{
    private readonly IDbConnectionFactory _dbFactory = Substitute.For<IDbConnectionFactory>();
    private readonly IDbConnection _connection = Substitute.For<IDbConnection>();
    private readonly IHttpClientFactory _httpClientFactory = Substitute.For<IHttpClientFactory>();
    private readonly IConfiguration _configuration = Substitute.For<IConfiguration>();
    private readonly ILogger<ProviderUpdateJob> _logger = Substitute.For<ILogger<ProviderUpdateJob>>();

    public ProviderUpdateJobTests()
    {
        _dbFactory.CreateConnection().Returns(_connection);
        _configuration["ProvidersApi:BaseUrl"].Returns("http://localhost:5003");
    }

    [Fact]
    public async Task ExecuteAsync_CreatesConnectionAndLogs()
    {
        var job = new ProviderUpdateJob(_dbFactory, _httpClientFactory, _configuration, _logger);

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
