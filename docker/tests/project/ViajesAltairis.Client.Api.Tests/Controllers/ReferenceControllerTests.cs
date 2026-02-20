using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MediatR;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Reference.Queries.GetCountries;
using ViajesAltairis.Application.Features.Client.Reference.Queries.GetCurrencies;
using ViajesAltairis.Application.Features.Client.Reference.Queries.GetLanguages;
using ViajesAltairis.Application.Features.Client.Reference.Queries.GetWebTranslations;
using ViajesAltairis.Client.Api.Tests.Fixtures;

namespace ViajesAltairis.Client.Api.Tests.Controllers;

public class ReferenceControllerTests : IClassFixture<ClientApiFactory>
{
    private readonly HttpClient _client;
    private readonly ClientApiFactory _factory;

    public ReferenceControllerTests(ClientApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetLanguages_ReturnsOk_Anonymous()
    {
        _factory.MockMediator
            .Send(Arg.Any<GetLanguagesQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetLanguagesResponse
            {
                Languages = [new LanguageDto { Id = 1, Code = "en", Name = "English" }]
            });

        var response = await _client.GetAsync("/api/reference/languages");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCurrencies_ReturnsOk_Anonymous()
    {
        _factory.MockMediator
            .Send(Arg.Any<GetCurrenciesQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetCurrenciesResponse
            {
                Currencies = [new CurrencyDto { Id = 1, Code = "EUR", Name = "Euro", Symbol = "€" }]
            });

        var response = await _client.GetAsync("/api/reference/currencies");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCountries_ReturnsOk_Anonymous()
    {
        _factory.MockMediator
            .Send(Arg.Any<GetCountriesQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetCountriesResponse
            {
                Countries = [new CountryDto { Id = 1, Code = "ES", Name = "Spain" }]
            });

        var response = await _client.GetAsync("/api/reference/countries");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTranslations_ReturnsOk_Anonymous()
    {
        _factory.MockMediator
            .Send(Arg.Any<GetWebTranslationsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<string, string> { ["client.welcome"] = "Welcome" });

        var response = await _client.GetAsync("/api/reference/translations");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        body.Should().ContainKey("client.welcome");
    }

    [Fact]
    public async Task NonExistentRoute_Returns404()
    {
        var response = await _client.GetAsync("/api/reference/nonexistent");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
