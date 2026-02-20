using Hangfire;
using Hangfire.InMemory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ViajesAltairis.ScheduledApi.Controllers;

namespace ViajesAltairis.Scheduled.Api.Tests.Controllers;

public class JobsControllerTests : IDisposable
{
    private readonly JobsController _controller;
    private readonly BackgroundJobServer _server;

    public JobsControllerTests()
    {
        GlobalConfiguration.Configuration.UseInMemoryStorage();
        _server = new BackgroundJobServer();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Database=test"
            })
            .Build();

        _controller = new JobsController(config);
    }

    [Theory]
    [InlineData("exchange-rate-sync")]
    [InlineData("subscription-billing")]
    [InlineData("provider-update")]
    public void Trigger_KnownJobKey_ReturnsOk(string jobKey)
    {
        var result = _controller.Trigger(jobKey);
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void Trigger_UnknownJobKey_ReturnsNotFound()
    {
        var result = _controller.Trigger("unknown-job");
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Reload_WithInvalidConnectionString_Throws()
    {
        // HangfireScheduleLoader.LoadSchedulesFromDb opens a real MySql connection,
        // so with a fake connection string it will throw. This verifies the endpoint
        // delegates to the loader correctly.
        var act = () => _controller.Reload();
        act.Should().Throw<Exception>();
    }

    public void Dispose()
    {
        _server.Dispose();
    }
}
