using FluentAssertions;
using MediatR;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Reservations.Commands.AddReservationGuest;
using ViajesAltairis.Application.Features.Client.Reservations.Commands.AddReservationLine;
using ViajesAltairis.Application.Features.Client.Reservations.Commands.CancelReservation;
using ViajesAltairis.Application.Features.Client.Reservations.Commands.CreateDraftReservation;
using ViajesAltairis.Application.Features.Client.Reservations.Commands.RemoveReservationLine;
using ViajesAltairis.Application.Features.Client.Reservations.Commands.SubmitReservation;
using ViajesAltairis.Application.Features.Client.Reservations.Queries.GetMyReservations;
using ViajesAltairis.Application.Features.Client.Reservations.Queries.GetReservationDetail;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Client.Api.Tests.Handlers.Reservations;

public class ReservationDelegationTests
{
    private readonly IReservationApiClient _reservationApi = Substitute.For<IReservationApiClient>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();

    public ReservationDelegationTests()
    {
        _currentUser.UserId.Returns(8L);
    }

    [Fact]
    public async Task CreateDraft_DelegatesToReservationApi()
    {
        _reservationApi.CreateDraftAsync(8, "EUR", null, ownerUserId: 8, cancellationToken: Arg.Any<CancellationToken>())
            .Returns(42L);

        var handler = new CreateDraftReservationHandler(_reservationApi, _currentUser);
        var result = await handler.Handle(new CreateDraftReservationCommand { CurrencyCode = "EUR" }, CancellationToken.None);

        result.Should().Be(42);
    }

    [Fact]
    public async Task AddLine_DelegatesToReservationApi()
    {
        _reservationApi.AddLineAsync(1, 10, 1, Arg.Any<DateTime>(), Arg.Any<DateTime>(), 2, Arg.Any<CancellationToken>())
            .Returns(100L);

        var handler = new AddReservationLineHandler(_reservationApi);
        var result = await handler.Handle(new AddReservationLineCommand
        {
            ReservationId = 1, RoomConfigurationId = 10, BoardTypeId = 1,
            CheckIn = DateTime.UtcNow, CheckOut = DateTime.UtcNow.AddDays(3), GuestCount = 2
        }, CancellationToken.None);

        result.Should().Be(100);
    }

    [Fact]
    public async Task RemoveLine_DelegatesToReservationApi()
    {
        var handler = new RemoveReservationLineHandler(_reservationApi);
        await handler.Handle(new RemoveReservationLineCommand { ReservationId = 1, LineId = 10 }, CancellationToken.None);

        await _reservationApi.Received(1).RemoveLineAsync(1, 10, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Submit_DelegatesToReservationApi()
    {
        _reservationApi.SubmitAsync(1, 1, null, null, null, null, Arg.Any<CancellationToken>())
            .Returns(new SubmitResult(1, "pending", 500, "EUR"));

        var handler = new SubmitReservationHandler(_reservationApi);
        var result = await handler.Handle(new SubmitReservationCommand { ReservationId = 1, PaymentMethodId = 1 }, CancellationToken.None);

        result.Status.Should().Be("pending");
        result.TotalAmount.Should().Be(500);
    }

    [Fact]
    public async Task Cancel_DelegatesToReservationApi()
    {
        var handler = new CancelReservationHandler(_reservationApi);
        await handler.Handle(new CancelReservationCommand { ReservationId = 1, UserId = 8, Reason = "Changed plans" }, CancellationToken.None);

        await _reservationApi.Received(1).CancelAsync(1, 8, "Changed plans", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddGuest_DelegatesToReservationApi()
    {
        var handler = new AddReservationGuestHandler(_reservationApi);
        await handler.Handle(new AddReservationGuestCommand
        {
            ReservationId = 1, LineId = 10, FirstName = "Jane", LastName = "Doe"
        }, CancellationToken.None);

        await _reservationApi.Received(1).AddGuestAsync(1, 10, "Jane", "Doe", null, null, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetMyReservations_DelegatesToReservationApi()
    {
        _reservationApi.GetByUserAsync(8, 1, 10, null, Arg.Any<CancellationToken>())
            .Returns(new ReservationListResult(
            [
                new ReservationSummaryResult(1, "confirmed", DateTime.UtcNow, 500, "EUR", 2)
            ], 1));

        var handler = new GetMyReservationsHandler(_reservationApi, _currentUser);
        var result = await handler.Handle(new GetMyReservationsQuery { Page = 1, PageSize = 10 }, CancellationToken.None);

        result.Reservations.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetReservationDetail_OwnReservation_ReturnsDetail()
    {
        _reservationApi.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(new ReservationDetailResult(1, 8, 8, "confirmed", DateTime.UtcNow, 500, 50, "EUR", 1, null, []));

        var handler = new GetReservationDetailHandler(_reservationApi, _currentUser);
        var result = await handler.Handle(new GetReservationDetailQuery { ReservationId = 1 }, CancellationToken.None);

        result.Id.Should().Be(1);
        result.Status.Should().Be("confirmed");
    }
}
