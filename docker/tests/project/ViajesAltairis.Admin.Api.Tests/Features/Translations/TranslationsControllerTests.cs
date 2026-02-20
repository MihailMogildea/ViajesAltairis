using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.Translations;

[Collection("AdminApi")]
public class TranslationsControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public TranslationsControllerTests(AdminApiFactory factory)
    {
        _factory = factory;
        _client = TestAuthHelper.CreateAuthenticatedClient(factory);
    }

    private async Task<long> CreateLanguageAsync()
    {
        var iso = TestAuthHelper.UniqueIso(2).ToLower();
        var response = await _client.PostAsJsonAsync("/api/languages", new
        {
            IsoCode = iso,
            Name = $"Lang {iso}"
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        return body!.Id;
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/translations");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        var languageId = await CreateLanguageAsync();
        var value = $"Translation {Guid.NewGuid().ToString()[..6]}";

        var response = await _client.PostAsJsonAsync("/api/translations", new
        {
            EntityType = "hotel",
            EntityId = 1L,
            Field = "name",
            LanguageId = languageId,
            Value = value
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<TranslationResponse>();
        body!.Id.Should().BeGreaterThan(0);
        body.Value.Should().Be(value);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCreated()
    {
        var languageId = await CreateLanguageAsync();
        var value = $"Translation {Guid.NewGuid().ToString()[..6]}";

        var createResponse = await _client.PostAsJsonAsync("/api/translations", new
        {
            EntityType = "hotel",
            EntityId = 1L,
            Field = "description",
            LanguageId = languageId,
            Value = value
        });
        var created = await createResponse.Content.ReadFromJsonAsync<TranslationResponse>();

        var response = await _client.GetAsync($"/api/translations/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<TranslationResponse>();
        body!.Id.Should().Be(created.Id);
        body.Value.Should().Be(value);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var languageId = await CreateLanguageAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/translations", new
        {
            EntityType = "hotel",
            EntityId = 1L,
            Field = "name",
            LanguageId = languageId,
            Value = "Before Update"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<TranslationResponse>();

        var updateResponse = await _client.PutAsJsonAsync($"/api/translations/{created!.Id}", new
        {
            EntityType = "hotel",
            EntityId = 1L,
            Field = "name",
            LanguageId = languageId,
            Value = "After Update"
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await updateResponse.Content.ReadFromJsonAsync<TranslationResponse>();
        body!.Value.Should().Be("After Update");
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var languageId = await CreateLanguageAsync();

        var createResponse = await _client.PostAsJsonAsync("/api/translations", new
        {
            EntityType = "hotel",
            EntityId = 1L,
            Field = "name",
            LanguageId = languageId,
            Value = $"Del {Guid.NewGuid().ToString()[..6]}"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<TranslationResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/translations/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/translations/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/translations");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record IdResponse(long Id);
    private record TranslationResponse(long Id, string EntityType, long EntityId, string Field, long LanguageId, string Value, DateTime CreatedAt);
}
