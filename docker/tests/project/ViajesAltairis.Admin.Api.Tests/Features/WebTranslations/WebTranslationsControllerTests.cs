using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;

namespace ViajesAltairis.Admin.Api.Tests.Features.WebTranslations;

[Collection("AdminApi")]
public class WebTranslationsControllerTests
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;

    public WebTranslationsControllerTests(AdminApiFactory factory)
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
        var response = await _client.GetAsync("/api/webtranslations");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        var languageId = await CreateLanguageAsync();
        var key = $"web.test.{Guid.NewGuid().ToString()[..8]}";

        var response = await _client.PostAsJsonAsync("/api/webtranslations", new
        {
            TranslationKey = key,
            LanguageId = languageId,
            Value = $"Value {Guid.NewGuid().ToString()[..6]}"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<WebTranslationResponse>();
        body!.Id.Should().BeGreaterThan(0);
        body.TranslationKey.Should().Be(key);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsCreated()
    {
        var languageId = await CreateLanguageAsync();
        var key = $"web.test.{Guid.NewGuid().ToString()[..8]}";
        var value = $"Value {Guid.NewGuid().ToString()[..6]}";

        var createResponse = await _client.PostAsJsonAsync("/api/webtranslations", new
        {
            TranslationKey = key,
            LanguageId = languageId,
            Value = value
        });
        var created = await createResponse.Content.ReadFromJsonAsync<WebTranslationResponse>();

        var response = await _client.GetAsync($"/api/webtranslations/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<WebTranslationResponse>();
        body!.Id.Should().Be(created.Id);
        body.TranslationKey.Should().Be(key);
    }

    [Fact]
    public async Task Update_ReturnsOk()
    {
        var languageId = await CreateLanguageAsync();
        var key = $"web.test.{Guid.NewGuid().ToString()[..8]}";

        var createResponse = await _client.PostAsJsonAsync("/api/webtranslations", new
        {
            TranslationKey = key,
            LanguageId = languageId,
            Value = "Before Update"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<WebTranslationResponse>();

        var updateResponse = await _client.PutAsJsonAsync($"/api/webtranslations/{created!.Id}", new
        {
            TranslationKey = key,
            LanguageId = languageId,
            Value = "After Update"
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await updateResponse.Content.ReadFromJsonAsync<WebTranslationResponse>();
        body!.Value.Should().Be("After Update");
    }

    [Fact]
    public async Task Delete_ReturnsNoContent()
    {
        var languageId = await CreateLanguageAsync();
        var key = $"web.test.{Guid.NewGuid().ToString()[..8]}";

        var createResponse = await _client.PostAsJsonAsync("/api/webtranslations", new
        {
            TranslationKey = key,
            LanguageId = languageId,
            Value = $"Del {Guid.NewGuid().ToString()[..6]}"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<WebTranslationResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/webtranslations/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/webtranslations/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PublicGet_AllowsAnonymous()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/webtranslations/public");
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsync("/api/webtranslations", new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record IdResponse(long Id);
    private record WebTranslationResponse(long Id, string TranslationKey, long LanguageId, string Value, DateTime CreatedAt);
}
