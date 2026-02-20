using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.Amenities;

[Collection("AdminApi")]
public class AmenitiesControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public AmenitiesControllerTests(AdminApiFactory factory)
    {
        _factory = factory;
        _client = TestAuthHelper.CreateAuthenticatedClient(factory);
    }

    private async Task<long> CreateAmenityCategoryAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/amenitycategories", new
        {
            Name = $"Cat {Guid.NewGuid().ToString()[..6]}"
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        return body!.Id;
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/amenities");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        var categoryId = await CreateAmenityCategoryAsync();
        var name = $"Amenity {Guid.NewGuid().ToString()[..6]}";

        var response = await _client.PostAsJsonAsync("/api/amenities", new
        {
            CategoryId = categoryId,
            Name = name
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<AmenityResponse>();
        body!.Id.Should().BeGreaterThan(0);
        body.Name.Should().Be(name);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCreated()
    {
        var categoryId = await CreateAmenityCategoryAsync();
        var name = $"Amenity {Guid.NewGuid().ToString()[..6]}";

        var createResponse = await _client.PostAsJsonAsync("/api/amenities", new
        {
            CategoryId = categoryId,
            Name = name
        });
        var created = await createResponse.Content.ReadFromJsonAsync<AmenityResponse>();

        var response = await _client.GetAsync($"/api/amenities/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<AmenityResponse>();
        body!.Id.Should().Be(created.Id);
        body.Name.Should().Be(name);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var categoryId = await CreateAmenityCategoryAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/amenities", new
        {
            CategoryId = categoryId,
            Name = $"Before {Guid.NewGuid().ToString()[..6]}"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<AmenityResponse>();

        var newName = $"After {Guid.NewGuid().ToString()[..6]}";
        var updateResponse = await _client.PutAsJsonAsync($"/api/amenities/{created!.Id}", new
        {
            CategoryId = categoryId,
            Name = newName
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await updateResponse.Content.ReadFromJsonAsync<AmenityResponse>();
        body!.Name.Should().Be(newName);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var categoryId = await CreateAmenityCategoryAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/amenities", new
        {
            CategoryId = categoryId,
            Name = $"Del {Guid.NewGuid().ToString()[..6]}"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<AmenityResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/amenities/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/amenities/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/amenities");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record IdResponse(long Id);
    private record AmenityResponse(long Id, long CategoryId, string Name);
}
