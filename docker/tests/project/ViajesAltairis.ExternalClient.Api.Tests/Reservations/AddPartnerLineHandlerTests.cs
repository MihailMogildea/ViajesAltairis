using ViajesAltairis.Application.Features.ExternalClient.Reservations.Commands;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.ExternalClient.Api.Tests.Helpers;

namespace ViajesAltairis.ExternalClient.Api.Tests.Reservations;

public class AddPartnerLineHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _db;
    private readonly IReservationApiClient _reservationApi;
    private readonly AddPartnerLineHandler _handler;

    public AddPartnerLineHandlerTests()
    {
        _db = new SqliteTestDatabase();
        _db.CreateSchema().SeedReferenceData();
        _reservationApi = Substitute.For<IReservationApiClient>();
        _handler = new AddPartnerLineHandler(_db, _reservationApi);

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
    public async Task AddLine_ValidOwnership_DelegatesToApiClient()
    {
        _reservationApi.AddLineAsync(
            Arg.Any<long>(), Arg.Any<long>(), Arg.Any<long>(),
            Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<int>(),
            Arg.Any<CancellationToken>())
            .Returns(200L);

        var cmd = new AddPartnerLineCommand(100, 10, 1,
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5), 2)
        {
            BusinessPartnerId = 1
        };

        var result = await _handler.Handle(cmd, CancellationToken.None);

        result.Should().Be(200);
    }

    [Fact]
    public async Task AddLine_WrongPartner_ThrowsInvalidOperation()
    {
        var cmd = new AddPartnerLineCommand(100, 10, 1,
            new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 5), 2)
        {
            BusinessPartnerId = 2  // Wrong partner
        };

        var act = () => _handler.Handle(cmd, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*does not belong*");
    }

    [Fact]
    public async Task AddLine_VerifiesCorrectParametersPassed()
    {
        _reservationApi.AddLineAsync(
            Arg.Any<long>(), Arg.Any<long>(), Arg.Any<long>(),
            Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<int>(),
            Arg.Any<CancellationToken>())
            .Returns(1L);

        var checkIn = new DateOnly(2026, 7, 10);
        var checkOut = new DateOnly(2026, 7, 15);

        var cmd = new AddPartnerLineCommand(100, 55, 3, checkIn, checkOut, 4)
        {
            BusinessPartnerId = 1
        };

        await _handler.Handle(cmd, CancellationToken.None);

        await _reservationApi.Received(1).AddLineAsync(
            100, 55, 3,
            checkIn.ToDateTime(TimeOnly.MinValue),
            checkOut.ToDateTime(TimeOnly.MinValue),
            4,
            Arg.Any<CancellationToken>());
    }

    public void Dispose() => _db.Dispose();
}
