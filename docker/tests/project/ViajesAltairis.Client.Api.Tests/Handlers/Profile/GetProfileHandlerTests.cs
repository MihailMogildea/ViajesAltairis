using FluentAssertions;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Profile.Queries.GetProfile;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Client.Api.Tests.Fixtures;

namespace ViajesAltairis.Client.Api.Tests.Handlers.Profile;

public class GetProfileHandlerTests
{
    [Fact]
    public async Task Handle_ExistingUser_ReturnsProfile()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "user", @"
            id INTEGER PRIMARY KEY, email TEXT, first_name TEXT, last_name TEXT,
            phone TEXT, country TEXT, language_id INTEGER, discount REAL, created_at TEXT");
        TestDbHelper.CreateTable(conn, "language", "id INTEGER PRIMARY KEY, iso_code TEXT");
        TestDbHelper.CreateTable(conn, "currency", "id INTEGER PRIMARY KEY, iso_code TEXT");
        TestDbHelper.CreateTable(conn, "user_subscription", "id INTEGER PRIMARY KEY, user_id INTEGER, subscription_type_id INTEGER, active INTEGER");
        TestDbHelper.CreateTable(conn, "subscription_type", "id INTEGER PRIMARY KEY, name TEXT, discount REAL, currency_id INTEGER");
        TestDbHelper.Execute(conn, @"
            INSERT INTO language VALUES (1, 'en');
            INSERT INTO user VALUES (8, 'client1@example.com', 'Juan', 'Martínez', '+34 600', 'Spain', 1, 0, '2025-01-01');
        ");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns(8L);

        var handler = new GetProfileHandler(factory, currentUser);
        var result = await handler.Handle(new GetProfileQuery(), CancellationToken.None);

        result.Email.Should().Be("client1@example.com");
        result.FirstName.Should().Be("Juan");
        result.PreferredLanguage.Should().Be("en");
    }

    [Fact]
    public async Task Handle_NonExistentUser_ThrowsKeyNotFound()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "user", @"
            id INTEGER PRIMARY KEY, email TEXT, first_name TEXT, last_name TEXT,
            phone TEXT, country TEXT, language_id INTEGER, discount REAL, created_at TEXT");
        TestDbHelper.CreateTable(conn, "language", "id INTEGER PRIMARY KEY, iso_code TEXT");
        TestDbHelper.CreateTable(conn, "currency", "id INTEGER PRIMARY KEY, iso_code TEXT");
        TestDbHelper.CreateTable(conn, "user_subscription", "id INTEGER PRIMARY KEY, user_id INTEGER, subscription_type_id INTEGER, active INTEGER");
        TestDbHelper.CreateTable(conn, "subscription_type", "id INTEGER PRIMARY KEY, name TEXT, discount REAL, currency_id INTEGER");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns(999L);

        var handler = new GetProfileHandler(factory, currentUser);
        var act = () => handler.Handle(new GetProfileQuery(), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
