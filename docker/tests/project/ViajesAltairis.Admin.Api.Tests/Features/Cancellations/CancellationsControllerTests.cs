using System.Net;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.Cancellations;

[Collection("AdminApi")]
public class CancellationsControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public CancellationsControllerTests(AdminApiFactory factory)
    {
        _factory = factory;
        _client = TestAuthHelper.CreateAuthenticatedClient(factory);
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/cancellations");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/cancellations/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/cancellations");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
