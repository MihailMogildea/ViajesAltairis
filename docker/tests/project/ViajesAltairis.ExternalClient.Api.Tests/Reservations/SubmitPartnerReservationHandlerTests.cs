using ViajesAltairis.Application.Features.ExternalClient.Reservations.Commands;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.ExternalClient.Api.Tests.Helpers;

namespace ViajesAltairis.ExternalClient.Api.Tests.Reservations;

public class SubmitPartnerReservationHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _db;
    private readonly IReservationApiClient _reservationApi;
    private readonly SubmitPartnerReservationHandler _handler;

    public SubmitPartnerReservationHandlerTests()
    {
        _db = new SqliteTestDatabase();
        _db.CreateSchema().SeedReferenceData();
        _reservationApi = Substitute.For<IReservationApiClient>();
        _handler = new SubmitPartnerReservationHandler(_db, _reservationApi);

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
    public async Task Submit_ValidOwnership_DelegatesToApiClient()
    {
        var expected = new SubmitResult(100, "pending", 500m, "EUR");
        _reservationApi.SubmitAsync(
            Arg.Any<long>(), Arg.Any<long>(),
            Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<string?>(),
            Arg.Any<CancellationToken>())
            .Returns(expected);

        var cmd = new SubmitPartnerReservationCommand(100, 1, "4111111111111111", "12/28", "123", "John Doe")
        {
            BusinessPartnerId = 1
        };

        var result = await _handler.Handle(cmd, CancellationToken.None);

        result.Should().Be(expected);
    }

    [Fact]
    public async Task Submit_WrongPartner_ThrowsInvalidOperation()
    {
        var cmd = new SubmitPartnerReservationCommand(100, 1, null, null, null, null) { BusinessPartnerId = 2 };

        var act = () => _handler.Handle(cmd, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*does not belong*");
    }

    public void Dispose() => _db.Dispose();
}
