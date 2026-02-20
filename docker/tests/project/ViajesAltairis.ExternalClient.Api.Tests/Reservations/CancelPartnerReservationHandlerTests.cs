using ViajesAltairis.Application.Features.ExternalClient.Reservations.Commands;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.ExternalClient.Api.Tests.Helpers;

namespace ViajesAltairis.ExternalClient.Api.Tests.Reservations;

public class CancelPartnerReservationHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _db;
    private readonly IReservationApiClient _reservationApi;
    private readonly CancelPartnerReservationHandler _handler;

    public CancelPartnerReservationHandlerTests()
    {
        _db = new SqliteTestDatabase();
        _db.CreateSchema().SeedReferenceData();
        _reservationApi = Substitute.For<IReservationApiClient>();
        _handler = new CancelPartnerReservationHandler(_db, _reservationApi);

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
    public async Task Cancel_ValidOwnership_DelegatesToApiClient()
    {
        var cmd = new CancelPartnerReservationCommand(100, "Changed plans")
        {
            CancelledByUserId = 1,
            BusinessPartnerId = 1
        };

        await _handler.Handle(cmd, CancellationToken.None);

        await _reservationApi.Received(1).CancelAsync(100, 1, "Changed plans", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Cancel_WrongPartner_ThrowsInvalidOperation()
    {
        var cmd = new CancelPartnerReservationCommand(100, null)
        {
            CancelledByUserId = 1,
            BusinessPartnerId = 2
        };

        var act = () => _handler.Handle(cmd, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*does not belong*");
    }

    [Fact]
    public async Task Cancel_PassesCancelledByUserId()
    {
        var cmd = new CancelPartnerReservationCommand(100, null)
        {
            CancelledByUserId = 77,
            BusinessPartnerId = 1
        };

        await _handler.Handle(cmd, CancellationToken.None);

        await _reservationApi.Received(1).CancelAsync(100, 77, null, Arg.Any<CancellationToken>());
    }

    public void Dispose() => _db.Dispose();
}
