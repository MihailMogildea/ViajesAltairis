using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ViajesAltairis.Application.Features.Client.Reservations.Commands.AddReservationGuest;
using ViajesAltairis.Application.Features.Client.Reservations.Commands.AddReservationLine;
using ViajesAltairis.Application.Features.Client.Reservations.Commands.CancelReservation;
using ViajesAltairis.Application.Features.Client.Reservations.Commands.CreateDraftReservation;
using ViajesAltairis.Application.Features.Client.Reservations.Commands.RemoveReservationLine;
using ViajesAltairis.Application.Features.Client.Reservations.Commands.SubmitReservation;
using ViajesAltairis.Application.Features.Client.Reservations.Queries.GetMyReservations;
using ViajesAltairis.Application.Features.Client.Reservations.Queries.GetReservationDetail;
using ViajesAltairis.Client.Api.Tests.Fixtures;
using ViajesAltairis.Client.Api.Tests.Helpers;

namespace ViajesAltairis.Client.Api.Tests.Controllers;

public class ReservationsControllerTests : IClassFixture<ClientApiFactory>
{
    private readonly ClientApiFactory _factory;

    public ReservationsControllerTests(ClientApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetMyReservations_Returns401_WhenNoToken()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/reservations");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMyReservations_ReturnsOk_WhenAuthenticated()
    {
        var client = _factory.CreateClient().WithClientAuth();
        _factory.MockMediator
            .Send(Arg.Any<GetMyReservationsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetMyReservationsResponse { Reservations = [], TotalCount = 0 });

        var response = await client.GetAsync("/api/reservations");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDetail_ReturnsOk_WhenAuthenticated()
    {
        var client = _factory.CreateClient().WithClientAuth();
        _factory.MockMediator
            .Send(Arg.Any<GetReservationDetailQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetReservationDetailResponse { Id = 1, Status = "draft", Lines = [] });

        var response = await client.GetAsync("/api/reservations/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDetail_Returns404_WhenNotFound()
    {
        var client = _factory.CreateClient().WithClientAuth();
        _factory.MockMediator
            .Send(Arg.Any<GetReservationDetailQuery>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new KeyNotFoundException("Reservation 999 not found."));

        var response = await client.GetAsync("/api/reservations/999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateDraft_Returns201_WhenAuthenticated()
    {
        var client = _factory.CreateClient().WithClientAuth();
        _factory.MockMediator
            .Send(Arg.Any<CreateDraftReservationCommand>(), Arg.Any<CancellationToken>())
            .Returns(42L);

        var response = await client.PostAsJsonAsync("/api/reservations",
            new { CurrencyCode = "EUR" });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task AddLine_ReturnsOk_WhenAuthenticated()
    {
        var client = _factory.CreateClient().WithClientAuth();
        _factory.MockMediator
            .Send(Arg.Any<AddReservationLineCommand>(), Arg.Any<CancellationToken>())
            .Returns(10L);

        var response = await client.PostAsJsonAsync("/api/reservations/1/lines", new
        {
            RoomConfigurationId = 1L,
            BoardTypeId = 1L,
            CheckIn = "2026-03-01",
            CheckOut = "2026-03-05",
            GuestCount = 2
        });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RemoveLine_Returns204_WhenAuthenticated()
    {
        var client = _factory.CreateClient().WithClientAuth();
        _factory.MockMediator
            .Send(Arg.Any<RemoveReservationLineCommand>(), Arg.Any<CancellationToken>())
            .Returns(Unit.Value);

        var response = await client.DeleteAsync("/api/reservations/1/lines/10");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task AddGuest_ReturnsOk_WhenAuthenticated()
    {
        var client = _factory.CreateClient().WithClientAuth();
        _factory.MockMediator
            .Send(Arg.Any<AddReservationGuestCommand>(), Arg.Any<CancellationToken>())
            .Returns(Unit.Value);

        var response = await client.PostAsJsonAsync("/api/reservations/1/lines/10/guests",
            new { FirstName = "Jane", LastName = "Doe" });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Submit_ReturnsOk_WhenAuthenticated()
    {
        var client = _factory.CreateClient().WithClientAuth();
        _factory.MockMediator
            .Send(Arg.Any<SubmitReservationCommand>(), Arg.Any<CancellationToken>())
            .Returns(new SubmitReservationResponse { ReservationId = 1, Status = "pending", TotalAmount = 100, Currency = "EUR" });

        var response = await client.PostAsJsonAsync("/api/reservations/1/submit",
            new { PaymentMethodId = 1L });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Cancel_Returns204_WhenAuthenticated()
    {
        var client = _factory.CreateClient().WithClientAuth();
        _factory.MockMediator
            .Send(Arg.Any<CancelReservationCommand>(), Arg.Any<CancellationToken>())
            .Returns(Unit.Value);

        var response = await client.PostAsJsonAsync("/api/reservations/1/cancel",
            new { Reason = "Changed plans" });
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
