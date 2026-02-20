using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.AmenityCategories;

[Collection("AdminApi")]
public class AmenityCategoriesControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public AmenityCategoriesControllerTests(AdminApiFactory factory)
    {
        _factory = factory;
        _client = TestAuthHelper.CreateAuthenticatedClient(factory);
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/amenitycategories");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        var name = $"AmenCat {Guid.NewGuid().ToString()[..6]}";
        var response = await _client.PostAsJsonAsync("/api/amenitycategories", new { Name = name });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<AmenityCategoryResponse>();
        body!.Id.Should().BeGreaterThan(0);
        body.Name.Should().Be(name);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCreated()
    {
        var name = $"AmenCat {Guid.NewGuid().ToString()[..6]}";
        var createResponse = await _client.PostAsJsonAsync("/api/amenitycategories", new { Name = name });
        var created = await createResponse.Content.ReadFromJsonAsync<AmenityCategoryResponse>();

        var response = await _client.GetAsync($"/api/amenitycategories/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<AmenityCategoryResponse>();
        body!.Id.Should().Be(created.Id);
        body.Name.Should().Be(name);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/amenitycategories", new
        {
            Name = $"Before {Guid.NewGuid().ToString()[..6]}"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<AmenityCategoryResponse>();

        var newName = $"After {Guid.NewGuid().ToString()[..6]}";
        var updateResponse = await _client.PutAsJsonAsync($"/api/amenitycategories/{created!.Id}", new
        {
            Name = newName
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await updateResponse.Content.ReadFromJsonAsync<AmenityCategoryResponse>();
        body!.Name.Should().Be(newName);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/amenitycategories", new
        {
            Name = $"Del {Guid.NewGuid().ToString()[..6]}"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<AmenityCategoryResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/amenitycategories/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/amenitycategories/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/amenitycategories");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record AmenityCategoryResponse(long Id, string Name);
}
