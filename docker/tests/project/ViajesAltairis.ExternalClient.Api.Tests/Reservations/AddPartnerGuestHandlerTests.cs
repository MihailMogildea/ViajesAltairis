using ViajesAltairis.Application.Features.ExternalClient.Reservations.Commands;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.ExternalClient.Api.Tests.Helpers;

namespace ViajesAltairis.ExternalClient.Api.Tests.Reservations;

public class AddPartnerGuestHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _db;
    private readonly IReservationApiClient _reservationApi;
    private readonly AddPartnerGuestHandler _handler;

    public AddPartnerGuestHandlerTests()
    {
        _db = new SqliteTestDatabase();
        _db.CreateSchema().SeedReferenceData();
        _reservationApi = Substitute.For<IReservationApiClient>();
        _handler = new AddPartnerGuestHandler(_db, _reservationApi);

        _db.Execute("""
            INSERT INTO business_partner (id, company_name, enabled) VALUES (1, 'Acme Travel', 1), (2, 'Other Corp', 1);
            INSERT INTO user (id, email, password_hash, enabled, user_type_id, business_partner_id, first_name, last_name)
                VALUES (1, 'agent@acme.com', 'hash', 1, 3, 1, 'Jane', 'Agent');
            INSERT INTO reservation (id, reservation_code, status_id, booked_by_user_id, currency_id,
                owner_first_name, owner_last_name, owner_email)
                VALUES (100, 'RES-001', 1, 1, 1, 'John', 'Doe', 'john@test.com');
            """);
    }

    [Fact]
    public async Task AddGuest_ValidOwnership_DelegatesToApiClient()
    {
        var cmd = new AddPartnerGuestCommand(100, 5, "Alice", "Wonder", null, null) { BusinessPartnerId = 1 };

        await _handler.Handle(cmd, CancellationToken.None);

        await _reservationApi.Received(1).AddGuestAsync(100, 5, "Alice", "Wonder", null, null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddGuest_WrongPartner_ThrowsInvalidOperation()
    {
        var cmd = new AddPartnerGuestCommand(100, 5, "Alice", "Wonder", null, null) { BusinessPartnerId = 2 };

        var act = () => _handler.Handle(cmd, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*does not belong*");
    }

    [Fact]
    public async Task AddGuest_PassesEmailAndPhone()
    {
        var cmd = new AddPartnerGuestCommand(100, 5, "Bob", "Guest", "bob@test.com", "+34600111222")
        {
            BusinessPartnerId = 1
        };

        await _handler.Handle(cmd, CancellationToken.None);

        await _reservationApi.Received(1).AddGuestAsync(
            100, 5, "Bob", "Guest", "bob@test.com", "+34600111222", Arg.Any<CancellationToken>());
    }

    public void Dispose() => _db.Dispose();
}
