using ViajesAltairis.Application.Features.ExternalClient.Reservations.Queries;
using ViajesAltairis.ExternalClient.Api.Tests.Helpers;

namespace ViajesAltairis.ExternalClient.Api.Tests.Reservations;

public class GetPartnerReservationsHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _db;
    private readonly GetPartnerReservationsHandler _handler;

    public GetPartnerReservationsHandlerTests()
    {
        _db = new SqliteTestDatabase();
        _db.CreateSchema().SeedReferenceData().CreateBookingViews();
        _handler = new GetPartnerReservationsHandler(_db);

        _db.Execute("""
            INSERT INTO business_partner (id, company_name, enabled) VALUES (1, 'Acme Travel', 1), (2, 'Beta Tours', 1);
            INSERT INTO user (id, email, password_hash, enabled, user_type_id, business_partner_id, first_name, last_name)
                VALUES (1, 'agent@acme.com', 'hash', 1, 3, 1, 'Jane', 'Agent'),
                       (2, 'agent@beta.com', 'hash', 1, 3, 2, 'Bob', 'Beta');
            INSERT INTO reservation (id, reservation_code, status_id, booked_by_user_id, currency_id,
                owner_first_name, owner_last_name, owner_email, subtotal, tax_amount, total_price, created_at)
                VALUES
                    (1, 'RES-001', 1, 1, 1, 'John', 'Doe', 'john@test.com', 100, 21, 121, '2026-01-15 10:00:00'),
                    (2, 'RES-002', 3, 1, 1, 'Alice', 'Smith', 'alice@test.com', 200, 42, 242, '2026-01-16 10:00:00'),
                    (3, 'RES-003', 2, 2, 2, 'Charlie', 'Brown', 'charlie@test.com', 150, 31.5, 181.5, '2026-01-17 10:00:00');
            """);
    }

    [Fact]
    public async Task GetReservations_ReturnsOnlyPartnerReservations()
    {
        var query = new GetPartnerReservationsQuery(1, null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.TotalCount.Should().Be(2);
        result.Reservations.Should().HaveCount(2);
        result.Reservations.Should().AllSatisfy(r =>
            r.ReservationCode.Should().BeOneOf("RES-001", "RES-002"));
    }

    [Fact]
    public async Task GetReservations_FiltersByStatusId()
    {
        var query = new GetPartnerReservationsQuery(1, StatusId: 3);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.TotalCount.Should().Be(1);
        result.Reservations.Should().ContainSingle()
            .Which.ReservationCode.Should().Be("RES-002");
    }

    [Fact]
    public async Task GetReservations_InvalidStatusId_ThrowsArgumentException()
    {
        var query = new GetPartnerReservationsQuery(1, StatusId: 7);

        var act = () => _handler.Handle(query, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*between 1 and 6*");
    }

    [Fact]
    public async Task GetReservations_Pagination_ReturnsCorrectPage()
    {
        var query = new GetPartnerReservationsQuery(1, null, Page: 1, PageSize: 1);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.TotalCount.Should().Be(2);
        result.Reservations.Should().HaveCount(1);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(1);
    }

    public void Dispose() => _db.Dispose();
}
