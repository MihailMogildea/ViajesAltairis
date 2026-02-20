using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Reference.Queries.GetLanguages;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Client.Api.Tests.Fixtures;

namespace ViajesAltairis.Client.Api.Tests.Handlers.Reference;

public class GetLanguagesHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsLanguages()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "language", "id INTEGER PRIMARY KEY, iso_code TEXT, name TEXT");
        TestDbHelper.Execute(conn, "INSERT INTO language VALUES (1, 'en', 'English'), (2, 'es', 'Español')");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);

        var handler = new GetLanguagesHandler(factory);
        var result = await handler.Handle(new GetLanguagesQuery(), CancellationToken.None);

        result.Languages.Should().HaveCount(2);
        result.Languages[0].Code.Should().Be("en");
    }

    [Fact]
    public async Task Handle_EmptyTable_ReturnsEmptyList()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "language", "id INTEGER PRIMARY KEY, iso_code TEXT, name TEXT");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);

        var handler = new GetLanguagesHandler(factory);
        var result = await handler.Handle(new GetLanguagesQuery(), CancellationToken.None);

        result.Languages.Should().BeEmpty();
    }
}
