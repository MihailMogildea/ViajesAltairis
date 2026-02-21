using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ViajesAltairis.Application.Features.Client.Hotels.Queries.GetRoomAvailability;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Client.Api.Tests.Fixtures;

namespace ViajesAltairis.Client.Api.Tests.Handlers.Hotels;

public class GetRoomAvailabilityHandlerTests
{
    private static GetRoomAvailabilityHandler CreateHandler(
        IDbConnectionFactory factory,
        IProvidersApiClient? providersApiClient = null,
        ITranslationService? translationService = null,
        ICurrencyConverter? currencyConverter = null)
    {
        translationService ??= CreateTranslationService();
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.LanguageId.Returns(1L);
        currentUser.CurrencyCode.Returns("EUR");
        currencyConverter ??= Substitute.For<ICurrencyConverter>();
        providersApiClient ??= Substitute.For<IProvidersApiClient>();

        return new GetRoomAvailabilityHandler(
            factory, translationService, currentUser, currencyConverter,
            Substitute.For<ICacheService>(), providersApiClient,
            Substitute.For<ILogger<GetRoomAvailabilityHandler>>());
    }

    private static ITranslationService CreateTranslationService()
    {
        var svc = Substitute.For<ITranslationService>();
        svc.ResolveAsync(Arg.Any<string>(), Arg.Any<IEnumerable<long>>(), Arg.Any<long>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<long, string>());
        return svc;
    }

    private static void CreateRoomTables(Microsoft.Data.Sqlite.SqliteConnection conn)
    {
        // The handler queries v_hotel_room_catalog (a view in MariaDB).
        // In SQLite tests we create it as a plain table with the columns the handler SELECTs + filters on.
        TestDbHelper.CreateTable(conn, "v_hotel_room_catalog",
            "hotel_id INTEGER, hotel_provider_room_type_id INTEGER, room_type_id INTEGER, " +
            "room_type_name TEXT, capacity INTEGER, quantity INTEGER, price_per_night REAL, currency_code TEXT, " +
            "provider_margin REAL DEFAULT 0, hotel_margin REAL DEFAULT 0, " +
            "provider_type_id INTEGER, provider_id INTEGER, enabled INTEGER DEFAULT 1");

        TestDbHelper.CreateTable(conn, "v_room_board_option",
            "hotel_provider_room_type_id INTEGER, board_type_id INTEGER, board_type_name TEXT, price_per_night REAL, enabled INTEGER");

        // currency table is needed for the currency-conversion path
        TestDbHelper.CreateTable(conn, "currency", "id INTEGER PRIMARY KEY, iso_code TEXT");

        // Tables referenced by handler for booking subtraction, images, and seasonal margins
        TestDbHelper.CreateTable(conn, "reservation_line",
            "id INTEGER PRIMARY KEY, reservation_id INTEGER, hotel_provider_room_type_id INTEGER, " +
            "check_in_date TEXT, check_out_date TEXT, num_rooms INTEGER DEFAULT 1, num_guests INTEGER");
        TestDbHelper.CreateTable(conn, "reservation",
            "id INTEGER PRIMARY KEY, status_id INTEGER");
        TestDbHelper.CreateTable(conn, "room_image",
            "id INTEGER PRIMARY KEY, hotel_provider_room_type_id INTEGER, url TEXT, sort_order INTEGER");
        TestDbHelper.CreateTable(conn, "hotel",
            "id INTEGER PRIMARY KEY, city_id INTEGER");
        TestDbHelper.CreateTable(conn, "city",
            "id INTEGER PRIMARY KEY, administrative_division_id INTEGER");
        TestDbHelper.CreateTable(conn, "seasonal_margin",
            "id INTEGER PRIMARY KEY, administrative_division_id INTEGER, start_month_day TEXT, end_month_day TEXT, margin REAL");
    }

    [Fact]
    public async Task Handle_ReturnsInternalRoomsWithBoards()
    {
        using var conn = TestDbHelper.CreateConnection();
        CreateRoomTables(conn);
        TestDbHelper.Execute(conn, """
            INSERT INTO currency VALUES (1, 'EUR');
            INSERT INTO v_hotel_room_catalog VALUES (1, 10, 1, 'Standard', 2, 5, 100.00, 'EUR', 0, 0, 1, 1, 1);
            INSERT INTO v_hotel_room_catalog VALUES (1, 11, 2, 'Suite',    4, 2, 250.00, 'EUR', 0, 0, 1, 1, 1);
            INSERT INTO v_room_board_option VALUES (10, 1, 'Room Only',  100.00, 1);
            INSERT INTO v_room_board_option VALUES (10, 2, 'Breakfast',  130.00, 1);
            INSERT INTO v_room_board_option VALUES (11, 1, 'Room Only',  250.00, 1);
        """);

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);

        var handler = CreateHandler(factory);
        var result = await handler.Handle(new GetRoomAvailabilityQuery
        {
            HotelId = 1, CheckIn = DateTime.UtcNow, CheckOut = DateTime.UtcNow.AddDays(3), Guests = 2
        }, CancellationToken.None);

        result.Rooms.Should().HaveCount(2);
        result.Rooms.First(r => r.RoomTypeName == "Standard").BoardOptions.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_FiltersBy_GuestCapacity()
    {
        using var conn = TestDbHelper.CreateConnection();
        CreateRoomTables(conn);
        TestDbHelper.Execute(conn, """
            INSERT INTO currency VALUES (1, 'EUR');
            INSERT INTO v_hotel_room_catalog VALUES (1, 10, 1, 'Standard', 2, 5, 100.00, 'EUR', 0, 0, 1, 1, 1);
            INSERT INTO v_hotel_room_catalog VALUES (1, 11, 2, 'Suite',    4, 2, 250.00, 'EUR', 0, 0, 1, 1, 1);
        """);

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);

        var handler = CreateHandler(factory);
        var result = await handler.Handle(new GetRoomAvailabilityQuery
        {
            HotelId = 1, CheckIn = DateTime.UtcNow, CheckOut = DateTime.UtcNow.AddDays(3), Guests = 3
        }, CancellationToken.None);

        result.Rooms.Should().HaveCount(1);
        result.Rooms.First().RoomTypeName.Should().Be("Suite");
    }

    [Fact]
    public async Task Handle_ReturnsDbQuantity_ForInternalRooms()
    {
        // The overlay approach returns the DB quantity directly (no booking subtraction in this handler).
        using var conn = TestDbHelper.CreateConnection();
        CreateRoomTables(conn);
        TestDbHelper.Execute(conn, """
            INSERT INTO currency VALUES (1, 'EUR');
            INSERT INTO v_hotel_room_catalog VALUES (1, 10, 1, 'Standard', 2, 5, 100.00, 'EUR', 0, 0, 1, 1, 1);
            INSERT INTO v_room_board_option VALUES (10, 1, 'Room Only', 100.00, 1);
        """);

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);

        var handler = CreateHandler(factory);
        var result = await handler.Handle(new GetRoomAvailabilityQuery
        {
            HotelId = 1,
            CheckIn = new DateTime(2026, 3, 2),
            CheckOut = new DateTime(2026, 3, 4),
            Guests = 2
        }, CancellationToken.None);

        result.Rooms.Should().HaveCount(1);
        result.Rooms.First().AvailableRooms.Should().Be(5);
    }

    [Fact]
    public async Task Handle_ExternalProvider_OverlaysLiveAvailability()
    {
        using var conn = TestDbHelper.CreateConnection();
        CreateRoomTables(conn);
        TestDbHelper.Execute(conn, """
            INSERT INTO currency VALUES (1, 'EUR');
            INSERT INTO v_hotel_room_catalog VALUES (1, 20, 1, 'single', 2, 5, 100.00, 'EUR', 0, 0, 2, 6, 1);
            INSERT INTO v_room_board_option VALUES (20, 1, 'room_only',          100.00, 1);
            INSERT INTO v_room_board_option VALUES (20, 2, 'bed_and_breakfast',   130.00, 1);
        """);

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);

        var providersApiClient = Substitute.For<IProvidersApiClient>();
        providersApiClient.GetHotelAvailabilityAsync(6, 1, Arg.Any<DateTime>(), Arg.Any<DateTime>(), 2, Arg.Any<CancellationToken>())
            .Returns(new List<ExternalRoomAvailability>
            {
                new("single", 2, 110.00m, 3, new List<ExternalBoardOption>
                {
                    new("room_only", 110.00m),
                    new("bed_and_breakfast", 145.00m)
                })
            });

        var handler = CreateHandler(factory, providersApiClient);
        var result = await handler.Handle(new GetRoomAvailabilityQuery
        {
            HotelId = 1, CheckIn = DateTime.UtcNow, CheckOut = DateTime.UtcNow.AddDays(3), Guests = 2
        }, CancellationToken.None);

        result.Rooms.Should().HaveCount(1);
        var room = result.Rooms.First();
        room.AvailableRooms.Should().Be(3); // overlaid from live data
        room.BoardOptions.Should().HaveCount(2);
        room.BoardOptions.First(b => b.BoardTypeName == "room_only").PricePerNight.Should().Be(110.00m);
        room.BoardOptions.First(b => b.BoardTypeName == "bed_and_breakfast").PricePerNight.Should().Be(145.00m);
    }

    [Fact]
    public async Task Handle_ExternalProvider_FallsBackToDbOnFailure()
    {
        using var conn = TestDbHelper.CreateConnection();
        CreateRoomTables(conn);
        TestDbHelper.Execute(conn, """
            INSERT INTO currency VALUES (1, 'EUR');
            INSERT INTO v_hotel_room_catalog VALUES (1, 20, 1, 'Standard', 2, 5, 100.00, 'EUR', 0, 0, 2, 6, 1);
            INSERT INTO v_room_board_option VALUES (20, 1, 'Room Only', 100.00, 1);
        """);

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);

        var providersApiClient = Substitute.For<IProvidersApiClient>();
        providersApiClient.GetHotelAvailabilityAsync(Arg.Any<long>(), Arg.Any<long>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Throws(new HttpRequestException("Provider unreachable"));

        var handler = CreateHandler(factory, providersApiClient);
        var result = await handler.Handle(new GetRoomAvailabilityQuery
        {
            HotelId = 1, CheckIn = DateTime.UtcNow, CheckOut = DateTime.UtcNow.AddDays(3), Guests = 2
        }, CancellationToken.None);

        // External provider failed — room still returned with DB data (graceful fallback)
        result.Rooms.Should().HaveCount(1);
        result.Rooms.First().AvailableRooms.Should().Be(5); // DB quantity preserved
    }
}
