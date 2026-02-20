using FluentAssertions;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Reference.Queries.GetWebTranslations;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Client.Api.Tests.Fixtures;

namespace ViajesAltairis.Client.Api.Tests.Handlers.Reference;

public class GetWebTranslationsHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsClientTranslationsOnly()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "web_translation", "id INTEGER PRIMARY KEY, language_id INTEGER, translation_key TEXT, value TEXT");
        TestDbHelper.Execute(conn, @"
            INSERT INTO web_translation VALUES (1, 1, 'client.welcome', 'Welcome');
            INSERT INTO web_translation VALUES (2, 1, 'client.search', 'Search');
            INSERT INTO web_translation VALUES (3, 1, 'admin.dashboard', 'Dashboard');
            INSERT INTO web_translation VALUES (4, 2, 'client.welcome', 'Bienvenido');
        ");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.LanguageId.Returns(1L);

        var handler = new GetWebTranslationsHandler(factory, currentUser);
        var result = await handler.Handle(new GetWebTranslationsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().ContainKey("client.welcome").WhoseValue.Should().Be("Welcome");
        result.Should().ContainKey("client.search");
        result.Should().NotContainKey("admin.dashboard");
    }
}
