using FluentAssertions;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Hotels.Queries.GetHotelDetail;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Client.Api.Tests.Fixtures;

namespace ViajesAltairis.Client.Api.Tests.Handlers.Hotels;

public class GetHotelDetailHandlerTests
{
    [Fact]
    public async Task Handle_ExistingHotel_ReturnsDetail()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "v_hotel_detail", @"
            hotel_id INTEGER, hotel_name TEXT, stars INTEGER, address TEXT,
            city_id INTEGER, city_name TEXT, country_id INTEGER, country_name TEXT,
            latitude REAL, longitude REAL, phone TEXT, email TEXT,
            check_in_time TEXT, check_out_time TEXT, avg_rating REAL, review_count INTEGER,
            free_cancellation_hours INTEGER, enabled INTEGER");
        TestDbHelper.CreateTable(conn, "hotel_image", "id INTEGER PRIMARY KEY, hotel_id INTEGER, url TEXT, sort_order INTEGER");
        TestDbHelper.CreateTable(conn, "v_hotel_amenity_list", "hotel_id INTEGER, amenity_id INTEGER, amenity_name TEXT");
        TestDbHelper.Execute(conn, @"
            INSERT INTO v_hotel_detail VALUES (1, 'Grand Hotel', 5, 'Calle 1', 1, 'Madrid', 1, 'Spain', 40.4, -3.7, '+34 91', 'h@hotel.com', '14:00', '12:00', 4.5, 20, 48, 1);
            INSERT INTO hotel_image VALUES (1, 1, 'https://img.com/1.jpg', 1);
            INSERT INTO hotel_image VALUES (2, 1, 'https://img.com/2.jpg', 2);
            INSERT INTO v_hotel_amenity_list VALUES (1, 1, 'WiFi'), (1, 2, 'Pool');
        ");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);
        var translationService = Substitute.For<ITranslationService>();
        translationService.ResolveAsync(Arg.Any<string>(), Arg.Any<IEnumerable<long>>(), Arg.Any<long>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<long, string>());
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.LanguageId.Returns(1L);

        var handler = new GetHotelDetailHandler(factory, translationService, currentUser, Substitute.For<ICacheService>());
        var result = await handler.Handle(new GetHotelDetailQuery { HotelId = 1 }, CancellationToken.None);

        result.Name.Should().Be("Grand Hotel");
        result.Images.Should().HaveCount(2);
        result.Amenities.Should().HaveCount(2);
        result.FreeCancellationHours.Should().Be(48);
    }

    [Fact]
    public async Task Handle_NonExistentHotel_ThrowsKeyNotFound()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "v_hotel_detail", @"
            hotel_id INTEGER, hotel_name TEXT, stars INTEGER, address TEXT,
            city_id INTEGER, city_name TEXT, country_id INTEGER, country_name TEXT,
            latitude REAL, longitude REAL, phone TEXT, email TEXT,
            check_in_time TEXT, check_out_time TEXT, avg_rating REAL, review_count INTEGER,
            free_cancellation_hours INTEGER, enabled INTEGER");
        TestDbHelper.CreateTable(conn, "hotel_image", "id INTEGER PRIMARY KEY, hotel_id INTEGER, url TEXT, sort_order INTEGER");
        TestDbHelper.CreateTable(conn, "v_hotel_amenity_list", "hotel_id INTEGER, amenity_id INTEGER, amenity_name TEXT");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);
        var translationService = Substitute.For<ITranslationService>();
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.LanguageId.Returns(1L);

        var handler = new GetHotelDetailHandler(factory, translationService, currentUser, Substitute.For<ICacheService>());
        var act = () => handler.Handle(new GetHotelDetailQuery { HotelId = 999 }, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_Spanish_TranslatesSummaryAndAmenities()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "v_hotel_detail", @"
            hotel_id INTEGER, hotel_name TEXT, stars INTEGER, address TEXT,
            city_id INTEGER, city_name TEXT, country_id INTEGER, country_name TEXT,
            latitude REAL, longitude REAL, phone TEXT, email TEXT,
            check_in_time TEXT, check_out_time TEXT, avg_rating REAL, review_count INTEGER,
            free_cancellation_hours INTEGER, enabled INTEGER");
        TestDbHelper.CreateTable(conn, "hotel_image", "id INTEGER PRIMARY KEY, hotel_id INTEGER, url TEXT, sort_order INTEGER");
        TestDbHelper.CreateTable(conn, "v_hotel_amenity_list", "hotel_id INTEGER, amenity_id INTEGER, amenity_name TEXT");
        TestDbHelper.Execute(conn, @"
            INSERT INTO v_hotel_detail VALUES (1, 'Hotel Madrid', 4, 'Calle 1', 1, 'Madrid', 1, 'Spain', 40.4, -3.7, NULL, NULL, NULL, NULL, 4.0, 5, NULL, 1);
            INSERT INTO v_hotel_amenity_list VALUES (1, 1, 'WiFi');
        ");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);
        var translationService = Substitute.For<ITranslationService>();
        translationService.ResolveAsync("hotel", Arg.Any<IEnumerable<long>>(), 2L, "summary", Arg.Any<CancellationToken>())
            .Returns(new Dictionary<long, string> { [1] = "Un gran hotel en Madrid" });
        translationService.ResolveAsync("city", Arg.Any<IEnumerable<long>>(), 2L, "name", Arg.Any<CancellationToken>())
            .Returns(new Dictionary<long, string>());
        translationService.ResolveAsync("country", Arg.Any<IEnumerable<long>>(), 2L, "name", Arg.Any<CancellationToken>())
            .Returns(new Dictionary<long, string> { [1] = "España" });
        translationService.ResolveAsync("amenity", Arg.Any<IEnumerable<long>>(), 2L, "name", Arg.Any<CancellationToken>())
            .Returns(new Dictionary<long, string>());
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.LanguageId.Returns(2L);

        var handler = new GetHotelDetailHandler(factory, translationService, currentUser, Substitute.For<ICacheService>());
        var result = await handler.Handle(new GetHotelDetailQuery { HotelId = 1 }, CancellationToken.None);

        result.Summary.Should().Be("Un gran hotel en Madrid");
        result.Country.Should().Be("España");
    }
}
