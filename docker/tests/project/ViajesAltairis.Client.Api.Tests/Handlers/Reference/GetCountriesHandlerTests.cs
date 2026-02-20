using FluentAssertions;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Reference.Queries.GetCountries;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Client.Api.Tests.Fixtures;

namespace ViajesAltairis.Client.Api.Tests.Handlers.Reference;

public class GetCountriesHandlerTests
{
    [Fact]
    public async Task Handle_English_ReturnsWithoutTranslation()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "country", "id INTEGER PRIMARY KEY, iso_code TEXT, name TEXT, enabled INTEGER");
        TestDbHelper.Execute(conn, "INSERT INTO country VALUES (1, 'ES', 'Spain', 1), (2, 'FR', 'France', 1), (3, 'XX', 'Disabled', 0)");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);
        var translationService = Substitute.For<ITranslationService>();
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.LanguageId.Returns(1L); // English

        var handler = new GetCountriesHandler(factory, translationService, currentUser, Substitute.For<ICacheService>());
        var result = await handler.Handle(new GetCountriesQuery(), CancellationToken.None);

        result.Countries.Should().HaveCount(2);
        await translationService.DidNotReceive().ResolveAsync(Arg.Any<string>(), Arg.Any<IEnumerable<long>>(), Arg.Any<long>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Spanish_TranslatesNames()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "country", "id INTEGER PRIMARY KEY, iso_code TEXT, name TEXT, enabled INTEGER");
        TestDbHelper.Execute(conn, "INSERT INTO country VALUES (1, 'ES', 'Spain', 1)");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);
        var translationService = Substitute.For<ITranslationService>();
        translationService.ResolveAsync("country", Arg.Any<IEnumerable<long>>(), 2L, "name", Arg.Any<CancellationToken>())
            .Returns(new Dictionary<long, string> { [1] = "España" });
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.LanguageId.Returns(2L); // Spanish

        var handler = new GetCountriesHandler(factory, translationService, currentUser, Substitute.For<ICacheService>());
        var result = await handler.Handle(new GetCountriesQuery(), CancellationToken.None);

        result.Countries.First().Name.Should().Be("España");
    }
}
