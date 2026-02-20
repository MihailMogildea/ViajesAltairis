using ViajesAltairis.Application.Features.ExternalClient.Reservations.Commands;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.ExternalClient.Api.Tests.Reservations;

public class CreatePartnerDraftHandlerTests
{
    private readonly IReservationApiClient _reservationApi = Substitute.For<IReservationApiClient>();
    private readonly CreatePartnerDraftHandler _handler;

    public CreatePartnerDraftHandlerTests()
    {
        _handler = new CreatePartnerDraftHandler(_reservationApi);
    }

    [Fact]
    public async Task CreateDraft_ValidCommand_DelegatesToApiClient()
    {
        _reservationApi.CreateDraftAsync(
            Arg.Any<long>(), Arg.Any<string>(), Arg.Any<string?>(),
            Arg.Any<long?>(), Arg.Any<string?>(), Arg.Any<string?>(),
            Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<string?>(),
            Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<string?>(),
            Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(42L);

        var cmd = new CreatePartnerDraftCommand("John", "Doe", "john@test.com", "+34600000000", "B123", "EUR", null)
        {
            BookedByUserId = 10
        };

        var result = await _handler.Handle(cmd, CancellationToken.None);

        result.Should().Be(42);
    }

    [Fact]
    public async Task CreateDraft_VerifiesCorrectParametersPassed()
    {
        _reservationApi.CreateDraftAsync(
            Arg.Any<long>(), Arg.Any<string>(), Arg.Any<string?>(),
            Arg.Any<long?>(), Arg.Any<string?>(), Arg.Any<string?>(),
            Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<string?>(),
            Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<string?>(),
            Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(1L);

        var cmd = new CreatePartnerDraftCommand("Jane", "Smith", "jane@test.com", null, null, "USD", "PROMO10")
        {
            BookedByUserId = 5
        };

        await _handler.Handle(cmd, CancellationToken.None);

        await _reservationApi.Received(1).CreateDraftAsync(
            5, "USD", "PROMO10",
            ownerFirstName: "Jane",
            ownerLastName: "Smith",
            ownerEmail: "jane@test.com",
            ownerPhone: null,
            ownerTaxId: null,
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
