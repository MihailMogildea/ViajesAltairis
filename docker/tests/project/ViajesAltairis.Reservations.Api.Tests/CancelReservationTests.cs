using System.Net;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Enums;
using ViajesAltairis.Reservations.Api.Tests.Fixtures;
using ViajesAltairis.Reservations.Api.Tests.Helpers;
using ViajesAltairis.ReservationsApi.Controllers;

namespace ViajesAltairis.Reservations.Api.Tests;

public class CancelReservationTests : IntegrationTestBase
{
    public CancelReservationTests(CustomWebApplicationFactory factory) : base(factory) { }

    private Reservation CreateConfirmedReservation()
    {
        var line = new ReservationLine
        {
            Id = 10,
            ReservationId = 1,
            HotelProviderRoomTypeId = 1,
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(30)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(33)),
            NumGuests = 2,
            TotalPrice = 400m,
        };
        return new Reservation
        {
            Id = 1,
            ReservationCode = "RES-TEST-001",
            StatusId = (long)ReservationStatusEnum.Confirmed,
            BookedByUserId = 1,
            OwnerFirstName = "John",
            OwnerLastName = "Doe",
            TotalPrice = 400m,
            CurrencyId = 1,
            ExchangeRateId = 1,
            ReservationLines = new List<ReservationLine> { line },
        };
    }

    [Fact]
    public async Task Cancel_FreePeriod_Returns204_NoPenalty()
    {
        var reservation = CreateConfirmedReservation();
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
        Factory.PaymentService.ProcessRefundAsync(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new RefundResult(true));

        var helper = new DapperMockHelper();
        // Policy: 48h free cancellation — check-in is 30 days away, so within free period
        helper.EnqueueSingleRow(("free_cancellation_hours", 48), ("penalty_percentage", 50m));
        // Provider info for line
        helper.EnqueueSingleRow(("provider_id", (long)1), ("provider_type", "internal"));
        // Payment reference
        helper.EnqueueScalar("PAY-123");
        Factory.SetupDapperConnection(helper);

        var request = new CancelRequest(1, "Changed plans");
        var response = await PostAsync("/api/reservations/1/cancel", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        reservation.StatusId.Should().Be((long)ReservationStatusEnum.Cancelled);
        await Factory.CancellationRepository.Received(1)
            .AddAsync(Arg.Is<Cancellation>(c =>
                c.PenaltyAmount == 0 && c.RefundAmount == 400m),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Cancel_OutsideFreePeriod_PenaltyApplied()
    {
        var reservation = CreateConfirmedReservation();
        // Check-in is only 1 hour away
        reservation.ReservationLines.First().CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var helper = new DapperMockHelper();
        // free_cancellation_hours=48 → we're at 0 hours, so penalty applies
        helper.EnqueueSingleRow(("free_cancellation_hours", 48), ("penalty_percentage", 50m));
        helper.EnqueueSingleRow(("provider_id", (long)1), ("provider_type", "internal"));
        helper.EnqueueScalar("PAY-123");
        Factory.SetupDapperConnection(helper);
        Factory.PaymentService.ProcessRefundAsync(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new RefundResult(true));

        var request = new CancelRequest(1, null);
        var response = await PostAsync("/api/reservations/1/cancel", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        await Factory.CancellationRepository.Received(1)
            .AddAsync(Arg.Is<Cancellation>(c =>
                c.PenaltyPercentage == 50m &&
                c.PenaltyAmount == 200m &&
                c.RefundAmount == 200m),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Cancel_CancellationRecordCreated()
    {
        var reservation = CreateConfirmedReservation();
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
        Factory.PaymentService.ProcessRefundAsync(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new RefundResult(true));

        var helper = new DapperMockHelper();
        helper.EnqueueSingleRow(("free_cancellation_hours", 48), ("penalty_percentage", 0m));
        helper.EnqueueSingleRow(("provider_id", (long)1), ("provider_type", "internal"));
        helper.EnqueueScalar("PAY-123");
        Factory.SetupDapperConnection(helper);

        var request = new CancelRequest(5, "Customer request");
        var response = await PostAsync("/api/reservations/1/cancel", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        await Factory.CancellationRepository.Received(1)
            .AddAsync(Arg.Is<Cancellation>(c =>
                c.ReservationId == 1 &&
                c.CancelledByUserId == 5 &&
                c.Reason == "Customer request" &&
                c.CurrencyId == 1),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Cancel_ConfirmedWithPayment_RefundCalled()
    {
        var reservation = CreateConfirmedReservation();
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
        Factory.PaymentService.ProcessRefundAsync(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new RefundResult(true));

        var helper = new DapperMockHelper();
        helper.EnqueueSingleRow(("free_cancellation_hours", 48), ("penalty_percentage", 0m));
        helper.EnqueueSingleRow(("provider_id", (long)1), ("provider_type", "internal"));
        helper.EnqueueScalar("PAY-123"); // payment reference found
        Factory.SetupDapperConnection(helper);

        var request = new CancelRequest(1, null);
        var response = await PostAsync("/api/reservations/1/cancel", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        await Factory.PaymentService.Received(1)
            .ProcessRefundAsync(1, "PAY-123", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Cancel_ExternalProvider_CancelBookingCalled()
    {
        var reservation = CreateConfirmedReservation();
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
        Factory.ProviderReservationService.CancelBookingAsync(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(true);
        Factory.PaymentService.ProcessRefundAsync(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new RefundResult(true));

        var helper = new DapperMockHelper();
        helper.EnqueueSingleRow(("free_cancellation_hours", 48), ("penalty_percentage", 0m));
        helper.EnqueueSingleRow(("provider_id", (long)1), ("provider_type", "external")); // external provider
        helper.EnqueueScalar("PAY-123");
        Factory.SetupDapperConnection(helper);

        var request = new CancelRequest(1, null);
        var response = await PostAsync("/api/reservations/1/cancel", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        await Factory.ProviderReservationService.Received(1)
            .CancelBookingAsync(1, "RES-TEST-001", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Cancel_InternalProvider_NoCancelCall()
    {
        var reservation = CreateConfirmedReservation();
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
        Factory.PaymentService.ProcessRefundAsync(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new RefundResult(true));

        var helper = new DapperMockHelper();
        helper.EnqueueSingleRow(("free_cancellation_hours", 48), ("penalty_percentage", 0m));
        helper.EnqueueSingleRow(("provider_id", (long)1), ("provider_type", "internal")); // internal
        helper.EnqueueScalar("PAY-123");
        Factory.SetupDapperConnection(helper);

        var request = new CancelRequest(1, null);
        var response = await PostAsync("/api/reservations/1/cancel", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        await Factory.ProviderReservationService.DidNotReceive()
            .CancelBookingAsync(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Cancel_DraftReservation_NoProviderOrRefund()
    {
        var reservation = CreateConfirmedReservation();
        reservation.StatusId = (long)ReservationStatusEnum.Draft; // draft → no external calls
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var helper = new DapperMockHelper();
        // No policy (could be null)
        helper.EnqueueEmptyQuery("free_cancellation_hours", "penalty_percentage");
        Factory.SetupDapperConnection(helper);

        var request = new CancelRequest(1, null);
        var response = await PostAsync("/api/reservations/1/cancel", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        await Factory.ProviderReservationService.DidNotReceive()
            .CancelBookingAsync(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        await Factory.PaymentService.DidNotReceive()
            .ProcessRefundAsync(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Cancel_AlreadyCancelled_Returns500()
    {
        var reservation = CreateConfirmedReservation();
        reservation.StatusId = (long)ReservationStatusEnum.Cancelled;
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);

        var request = new CancelRequest(1, null);
        var response = await PostAsync("/api/reservations/1/cancel", request);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Cancel_AlreadyCompleted_Returns500()
    {
        var reservation = CreateConfirmedReservation();
        reservation.StatusId = (long)ReservationStatusEnum.Completed;
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);

        var request = new CancelRequest(1, null);
        var response = await PostAsync("/api/reservations/1/cancel", request);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
