using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.AdministrativeDivisionTypes;

[Collection("AdminApi")]
public class AdministrativeDivisionTypesControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public AdministrativeDivisionTypesControllerTests(AdminApiFactory factory)
    {
        _factory = factory;
        _client = TestAuthHelper.CreateAuthenticatedClient(factory);
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/administrativedivisiontypes");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        var name = $"AdminDiv {Guid.NewGuid().ToString()[..6]}";
        var response = await _client.PostAsJsonAsync("/api/administrativedivisiontypes", new { Name = name });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<AdministrativeDivisionTypeResponse>();
        body!.Id.Should().BeGreaterThan(0);
        body.Name.Should().Be(name);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCreated()
    {
        var name = $"AdminDiv {Guid.NewGuid().ToString()[..6]}";
        var createResponse = await _client.PostAsJsonAsync("/api/administrativedivisiontypes", new { Name = name });
        var created = await createResponse.Content.ReadFromJsonAsync<AdministrativeDivisionTypeResponse>();

        var response = await _client.GetAsync($"/api/administrativedivisiontypes/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<AdministrativeDivisionTypeResponse>();
        body!.Id.Should().Be(created.Id);
        body.Name.Should().Be(name);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/administrativedivisiontypes", new
        {
            Name = $"Before {Guid.NewGuid().ToString()[..6]}"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<AdministrativeDivisionTypeResponse>();

        var newName = $"After {Guid.NewGuid().ToString()[..6]}";
        var updateResponse = await _client.PutAsJsonAsync($"/api/administrativedivisiontypes/{created!.Id}", new
        {
            Name = newName
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await updateResponse.Content.ReadFromJsonAsync<AdministrativeDivisionTypeResponse>();
        body!.Name.Should().Be(newName);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/administrativedivisiontypes", new
        {
            Name = $"Del {Guid.NewGuid().ToString()[..6]}"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<AdministrativeDivisionTypeResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/administrativedivisiontypes/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/administrativedivisiontypes/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/administrativedivisiontypes");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record AdministrativeDivisionTypeResponse(long Id, string Name);
}
