using ViajesAltairis.Application.Features.ExternalClient.Hotels.Queries;
using ViajesAltairis.ExternalClient.Api.Tests.Helpers;

namespace ViajesAltairis.ExternalClient.Api.Tests.Hotels;

public class GetHotelDetailHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _db;
    private readonly GetHotelDetailHandler _handler;

    public GetHotelDetailHandlerTests()
    {
        _db = new SqliteTestDatabase();
        _db.CreateSchema().SeedReferenceData().CreateHotelViews();
        _handler = new GetHotelDetailHandler(_db);

        _db.Execute("""
            INSERT INTO hotel (id, name, stars, address, email, phone, check_in_time, check_out_time,
                latitude, longitude, enabled, city_id)
                VALUES (1, 'Grand Hotel', 5, '123 Main St', 'info@grand.com', '+34900000000',
                        '14:00', '11:00', 41.3851, 2.1734, 1, 1);
            INSERT INTO hotel (id, name, stars, address, enabled, city_id)
                VALUES (2, 'Disabled Hotel', 3, '456 Side St', 0, 1);
            INSERT INTO provider (id, name) VALUES (1, 'Hotelbeds');
            INSERT INTO hotel_provider (id, hotel_id, provider_id) VALUES (1, 1, 1);
            INSERT INTO hotel_provider_room_type (id, hotel_provider_id, room_type_id, currency_id, quantity, price_per_night, capacity, enabled)
                VALUES (1, 1, 1, 1, 5, 100.00, 2, 1),
                       (2, 1, 2, 1, 3, 150.00, 3, 1);
            INSERT INTO hotel_provider_room_type_board (id, hotel_provider_room_type_id, board_type_id, price_per_night, enabled)
                VALUES (1, 1, 1, 0.00, 1),
                       (2, 1, 2, 15.00, 1),
                       (3, 2, 2, 20.00, 1);
            INSERT INTO amenity_category (id, name) VALUES (1, 'General'), (2, 'Room');
            INSERT INTO amenity (id, name, category_id) VALUES (1, 'WiFi', 1), (2, 'Pool', 1), (3, 'Mini Bar', 2);
            INSERT INTO hotel_amenity (id, hotel_id, amenity_id) VALUES (1, 1, 1), (2, 1, 2);
            INSERT INTO cancellation_policy (id, hotel_id, free_cancellation_hours, penalty_percentage, enabled)
                VALUES (1, 1, 48, 10.0, 1);
            INSERT INTO user (id, email, password_hash, enabled, user_type_id, first_name, last_name)
                VALUES (1, 'reviewer@test.com', 'hash', 1, 1, 'Test', 'User');
            INSERT INTO review (id, hotel_id, user_id, rating, visible) VALUES (1, 1, 1, 9, 1);
            """);
    }

    [Fact]
    public async Task GetDetail_ReturnsHotelWithRoomsAndAmenities()
    {
        var query = new GetHotelDetailQuery(1);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.HotelId.Should().Be(1);
        result.HotelName.Should().Be("Grand Hotel");
        result.Stars.Should().Be(5);
        result.Address.Should().Be("123 Main St");
        result.CheckInTime.Should().Be(new TimeOnly(14, 0));
        result.CheckOutTime.Should().Be(new TimeOnly(11, 0));
        result.AvgRating.Should().Be(9m);
        result.ReviewCount.Should().Be(1);
        result.FreeCancellationHours.Should().Be(48);
        result.PenaltyPercentage.Should().Be(10m);

        result.Rooms.Should().HaveCount(2);
        var standard = result.Rooms.First(r => r.RoomTypeName == "Standard");
        standard.PricePerNight.Should().Be(100m);
        standard.BoardOptions.Should().HaveCount(2);

        result.Amenities.Should().HaveCount(2);
        result.Amenities.Should().Contain(a => a.AmenityName == "WiFi");
        result.Amenities.Should().Contain(a => a.AmenityName == "Pool");
    }

    [Fact]
    public async Task GetDetail_DisabledHotel_ReturnsNull()
    {
        var query = new GetHotelDetailQuery(2);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetDetail_NotFound_ReturnsNull()
    {
        var query = new GetHotelDetailQuery(999);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }

    public void Dispose() => _db.Dispose();
}
