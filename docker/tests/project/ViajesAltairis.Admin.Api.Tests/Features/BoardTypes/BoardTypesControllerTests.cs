using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.BoardTypes;

[Collection("AdminApi")]
public class BoardTypesControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public BoardTypesControllerTests(AdminApiFactory factory)
    {
        _factory = factory;
        _client = TestAuthHelper.CreateAuthenticatedClient(factory);
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/boardtypes");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        var name = $"Board {Guid.NewGuid().ToString()[..6]}";
        var response = await _client.PostAsJsonAsync("/api/boardtypes", new { Name = name });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<BoardTypeResponse>();
        body!.Id.Should().BeGreaterThan(0);
        body.Name.Should().Be(name);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCreated()
    {
        var name = $"Board {Guid.NewGuid().ToString()[..6]}";
        var createResponse = await _client.PostAsJsonAsync("/api/boardtypes", new { Name = name });
        var created = await createResponse.Content.ReadFromJsonAsync<BoardTypeResponse>();

        var response = await _client.GetAsync($"/api/boardtypes/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<BoardTypeResponse>();
        body!.Id.Should().Be(created.Id);
        body.Name.Should().Be(name);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/boardtypes", new
        {
            Name = $"Before {Guid.NewGuid().ToString()[..6]}"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<BoardTypeResponse>();

        var newName = $"After {Guid.NewGuid().ToString()[..6]}";
        var updateResponse = await _client.PutAsJsonAsync($"/api/boardtypes/{created!.Id}", new
        {
            Name = newName
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await updateResponse.Content.ReadFromJsonAsync<BoardTypeResponse>();
        body!.Name.Should().Be(newName);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/boardtypes", new
        {
            Name = $"Del {Guid.NewGuid().ToString()[..6]}"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<BoardTypeResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/boardtypes/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/boardtypes/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/boardtypes");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record BoardTypeResponse(long Id, string Name);
}
