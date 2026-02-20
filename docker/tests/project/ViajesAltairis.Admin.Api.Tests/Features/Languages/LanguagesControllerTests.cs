using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.Languages;

[Collection("AdminApi")]
public class LanguagesControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public LanguagesControllerTests(AdminApiFactory factory)
    {
        _factory = factory;
        _client = TestAuthHelper.CreateAuthenticatedClient(factory);
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/languages");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        var iso = TestAuthHelper.UniqueIso(2).ToLower();
        var response = await _client.PostAsJsonAsync("/api/languages", new
        {
            IsoCode = iso,
            Name = $"Lang {iso}"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<LanguageResponse>();
        body!.Id.Should().BeGreaterThan(0);
        body.IsoCode.Should().Be(iso);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCreated()
    {
        var iso = TestAuthHelper.UniqueIso(2).ToLower();
        var createResponse = await _client.PostAsJsonAsync("/api/languages", new
        {
            IsoCode = iso,
            Name = $"Get Test {iso}"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<LanguageResponse>();

        var response = await _client.GetAsync($"/api/languages/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<LanguageResponse>();
        body!.Id.Should().Be(created.Id);
        body.IsoCode.Should().Be(iso);
    }

    [Fact]
    public async Task GetById_NonExistent_Returns404()
    {
        var response = await _client.GetAsync("/api/languages/999999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var iso = TestAuthHelper.UniqueIso(2).ToLower();
        var createResponse = await _client.PostAsJsonAsync("/api/languages", new
        {
            IsoCode = iso,
            Name = "Before Update"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<LanguageResponse>();

        var newIso = TestAuthHelper.UniqueIso(2).ToLower();
        var updateResponse = await _client.PutAsJsonAsync($"/api/languages/{created!.Id}", new
        {
            IsoCode = newIso,
            Name = "After Update"
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await updateResponse.Content.ReadFromJsonAsync<LanguageResponse>();
        body!.Name.Should().Be("After Update");
        body.IsoCode.Should().Be(newIso);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var iso = TestAuthHelper.UniqueIso(2).ToLower();
        var createResponse = await _client.PostAsJsonAsync("/api/languages", new
        {
            IsoCode = iso,
            Name = "Delete Test"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<LanguageResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/languages/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/languages/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WithEmptyName_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/languages", new
        {
            IsoCode = "zz",
            Name = ""
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/languages");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record LanguageResponse(long Id, string IsoCode, string Name, DateTime CreatedAt);
}
