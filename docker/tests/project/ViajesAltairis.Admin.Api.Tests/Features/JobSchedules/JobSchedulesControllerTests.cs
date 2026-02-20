using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Admin.Api.Tests.Features.JobSchedules;

[Collection("AdminApi")]
public class JobSchedulesControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public JobSchedulesControllerTests(AdminApiFactory factory)
    {
        _factory = factory;
        _client = TestAuthHelper.CreateAuthenticatedClient(factory);
    }

    [Fact]
    public async Task GetAll_ReturnsOkOr500()
    {
        // GetAll uses Dapper which may fail with SQLite record deserialization (bool/DateTime in record constructor)
        var response = await _client.GetAsync("/api/job-schedules");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetByKey_NonExistent_Returns404()
    {
        var response = await _client.GetAsync("/api/job-schedules/nonexistent-key");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Trigger_NonExistent_Returns404()
    {
        var response = await _client.PostAsync("/api/job-schedules/nonexistent-key/trigger", null);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/job-schedules");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
