using FluentAssertions;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Hotels.Queries.SearchHotels;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Client.Api.Tests.Fixtures;

namespace ViajesAltairis.Client.Api.Tests.Handlers.Hotels;

public class SearchHotelsHandlerTests
{
    private SearchHotelsHandler CreateHandler(IDbConnectionFactory factory, long langId = 1, string currency = "EUR")
    {
        var translationService = Substitute.For<ITranslationService>();
        translationService.ResolveAsync(Arg.Any<string>(), Arg.Any<IEnumerable<long>>(), Arg.Any<long>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<long, string>());
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.LanguageId.Returns(langId);
        currentUser.CurrencyCode.Returns(currency);
        var currencyConverter = Substitute.For<ICurrencyConverter>();
        var cacheService = Substitute.For<ICacheService>();

        return new SearchHotelsHandler(factory, translationService, currentUser, currencyConverter, cacheService);
    }

    private (Microsoft.Data.Sqlite.SqliteConnection conn, IDbConnectionFactory factory) SetupDb()
    {
        var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "v_hotel_card", @"
            hotel_id INTEGER, hotel_name TEXT, stars INTEGER,
            city_id INTEGER, city_name TEXT, city_image_url TEXT,
            country_id INTEGER, country_name TEXT,
            avg_rating REAL, review_count INTEGER, enabled INTEGER");
        TestDbHelper.CreateTable(conn, "v_hotel_room_catalog", @"
            hotel_id INTEGER, hotel_provider_room_type_id INTEGER, room_type_id INTEGER,
            room_type_name TEXT, capacity INTEGER, quantity INTEGER,
            price_per_night REAL, currency_code TEXT,
            provider_margin REAL DEFAULT 0, hotel_margin REAL DEFAULT 0,
            enabled INTEGER");
        TestDbHelper.CreateTable(conn, "exchange_rate", @"
            id INTEGER PRIMARY KEY, currency_id INTEGER, rate_to_eur REAL,
            valid_from TEXT, valid_to TEXT");
        TestDbHelper.CreateTable(conn, "hotel_image", "id INTEGER PRIMARY KEY, hotel_id INTEGER, url TEXT, sort_order INTEGER");
        TestDbHelper.CreateTable(conn, "currency", "id INTEGER PRIMARY KEY, iso_code TEXT");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);
        return (conn, factory);
    }

    [Fact]
    public async Task Handle_NoFilters_ReturnsAllEnabled()
    {
        var (conn, factory) = SetupDb();
        TestDbHelper.Execute(conn, @"
            INSERT INTO v_hotel_card VALUES (1, 'Hotel A', 4, 1, 'Madrid', NULL, 1, 'Spain', 4.5, 10, 1);
            INSERT INTO v_hotel_card VALUES (2, 'Hotel B', 3, 1, 'Madrid', NULL, 1, 'Spain', 3.5, 5, 1);
            INSERT INTO v_hotel_card VALUES (3, 'Hotel C', 5, 2, 'Paris', NULL, 2, 'France', 0, 0, 0);
        ");

        var handler = CreateHandler(factory);
        var result = await handler.Handle(new SearchHotelsQuery { Page = 1, PageSize = 20 }, CancellationToken.None);

        result.TotalCount.Should().Be(2);
        result.Hotels.Should().HaveCount(2);
        conn.Dispose();
    }

    [Fact]
    public async Task Handle_FilterByStars_ReturnsMatching()
    {
        var (conn, factory) = SetupDb();
        TestDbHelper.Execute(conn, @"
            INSERT INTO v_hotel_card VALUES (1, 'Hotel A', 4, 1, 'Madrid', NULL, 1, 'Spain', 4.5, 10, 1);
            INSERT INTO v_hotel_card VALUES (2, 'Hotel B', 5, 1, 'Madrid', NULL, 1, 'Spain', 3.5, 5, 1);
        ");

        var handler = CreateHandler(factory);
        var result = await handler.Handle(new SearchHotelsQuery { Stars = 5, Page = 1, PageSize = 20 }, CancellationToken.None);

        result.TotalCount.Should().Be(1);
        result.Hotels.First().Name.Should().Be("Hotel B");
        conn.Dispose();
    }

    [Fact]
    public async Task Handle_Pagination_RespectsPageSize()
    {
        var (conn, factory) = SetupDb();
        for (int i = 1; i <= 5; i++)
            TestDbHelper.Execute(conn, $"INSERT INTO v_hotel_card VALUES ({i}, 'Hotel {i}', 4, 1, 'Madrid', NULL, 1, 'Spain', 4.0, 1, 1)");

        var handler = CreateHandler(factory);
        var result = await handler.Handle(new SearchHotelsQuery { Page = 1, PageSize = 2 }, CancellationToken.None);

        result.TotalCount.Should().Be(5);
        result.Hotels.Should().HaveCount(2);
        conn.Dispose();
    }
}
