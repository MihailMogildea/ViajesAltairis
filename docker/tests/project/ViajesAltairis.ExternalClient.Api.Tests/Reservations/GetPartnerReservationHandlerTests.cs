using ViajesAltairis.Application.Features.ExternalClient.Reservations.Queries;
using ViajesAltairis.ExternalClient.Api.Tests.Helpers;

namespace ViajesAltairis.ExternalClient.Api.Tests.Reservations;

public class GetPartnerReservationHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _db;
    private readonly GetPartnerReservationHandler _handler;

    public GetPartnerReservationHandlerTests()
    {
        _db = new SqliteTestDatabase();
        _db.CreateSchema().SeedReferenceData().CreateBookingViews();
        _handler = new GetPartnerReservationHandler(_db);

        _db.Execute("""
            INSERT INTO business_partner (id, company_name, enabled) VALUES (1, 'Acme Travel', 1), (2, 'Beta Tours', 1);
            INSERT INTO user (id, email, password_hash, enabled, user_type_id, business_partner_id, first_name, last_name)
                VALUES (1, 'agent@acme.com', 'hash', 1, 3, 1, 'Jane', 'Agent'),
                       (2, 'agent@beta.com', 'hash', 1, 3, 2, 'Bob', 'Beta');
            INSERT INTO hotel (id, name, enabled, city_id) VALUES (1, 'Grand Hotel', 1, 1);
            INSERT INTO provider (id, name) VALUES (1, 'Hotelbeds');
            INSERT INTO hotel_provider (id, hotel_id, provider_id) VALUES (1, 1, 1);
            INSERT INTO hotel_provider_room_type (id, hotel_provider_id, room_type_id, currency_id, quantity, price_per_night)
                VALUES (1, 1, 1, 1, 5, 100.00);
            INSERT INTO reservation (id, reservation_code, status_id, booked_by_user_id, currency_id,
                owner_first_name, owner_last_name, owner_email, subtotal, tax_amount, discount_amount, total_price, created_at)
                VALUES (100, 'RES-100', 1, 1, 1, 'John', 'Doe', 'john@test.com', 300, 63, 0, 363, '2026-02-01 10:00:00');
            INSERT INTO reservation_line (id, reservation_id, hotel_provider_room_type_id, board_type_id,
                check_in_date, check_out_date, num_rooms, num_guests,
                price_per_night, board_price_per_night, num_nights, total_price, currency_id)
                VALUES (1, 100, 1, 2, '2026-06-01', '2026-06-04', 1, 2, 100.00, 15.00, 3, 345.00, 1);
            INSERT INTO reservation_guest (id, reservation_line_id, first_name, last_name, email, phone)
                VALUES (1, 1, 'John', 'Doe', 'john@test.com', '+34600000000'),
                       (2, 1, 'Jane', 'Doe', 'jane@test.com', null);
            """);
    }

    [Fact]
    public async Task GetReservation_ReturnsDetailWithLinesAndGuests()
    {
        var query = new GetPartnerReservationQuery(100, 1);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.ReservationId.Should().Be(100);
        result.ReservationCode.Should().Be("RES-100");
        result.OwnerFirstName.Should().Be("John");
        result.TotalPrice.Should().Be(363);
        result.Lines.Should().HaveCount(1);

        var line = result.Lines[0];
        line.HotelName.Should().Be("Grand Hotel");
        line.RoomTypeName.Should().Be("Standard");
        line.BoardTypeName.Should().Be("Bed & Breakfast");
        line.NumNights.Should().Be(3);
        line.Guests.Should().HaveCount(2);
        line.Guests[0].FirstName.Should().Be("John");
        line.Guests[1].FirstName.Should().Be("Jane");
    }

    [Fact]
    public async Task GetReservation_WrongPartner_ReturnsNull()
    {
        var query = new GetPartnerReservationQuery(100, 2);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetReservation_NotFound_ReturnsNull()
    {
        var query = new GetPartnerReservationQuery(999, 1);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }

    public void Dispose() => _db.Dispose();
}
