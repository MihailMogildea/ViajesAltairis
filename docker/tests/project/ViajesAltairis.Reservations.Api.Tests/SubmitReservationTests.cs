using System.Net;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Enums;
using ViajesAltairis.Reservations.Api.Tests.Fixtures;
using ViajesAltairis.Reservations.Api.Tests.Helpers;
using ViajesAltairis.ReservationsApi.Controllers;

namespace ViajesAltairis.Reservations.Api.Tests;

public class SubmitReservationTests : IntegrationTestBase
{
    public SubmitReservationTests(CustomWebApplicationFactory factory) : base(factory) { }

    private Reservation CreateDraftWithLines(long? promoCodeId = null)
    {
        var line = new ReservationLine
        {
            Id = 10,
            ReservationId = 1,
            HotelProviderRoomTypeId = 1,
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(30)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(33)),
            NumGuests = 2,
            Subtotal = 360m,
            TotalPrice = 400m,
            CurrencyId = 1,
            ExchangeRateId = 1,
        };
        return new Reservation
        {
            Id = 1,
            ReservationCode = "RES-TEST-001",
            StatusId = (long)ReservationStatusEnum.Draft,
            BookedByUserId = 1,
            OwnerFirstName = "John",
            OwnerLastName = "Doe",
            Subtotal = 360m,
            TotalPrice = 400m,
            CurrencyId = 1,
            ExchangeRateId = 1,
            PromoCodeId = promoCodeId,
            ReservationLines = new List<ReservationLine> { line },
        };
    }

    private DapperMockHelper SetupSubmitDapper(
        int minDaysBeforeCheckin = 0,
        bool withPromo = false,
        bool promoExpired = false,
        string providerType = "internal")
    {
        var helper = new DapperMockHelper();

        // 1) Payment method
        helper.EnqueueSingleRow(("min_days_before_checkin", minDaysBeforeCheckin));

        // 2) Promo revalidation (if promoCodeId set)
        if (withPromo)
        {
            if (promoExpired)
                helper.EnqueueSingleRow(
                    ("valid_to", DateTime.UtcNow.AddDays(-1)),
                    ("max_uses", (object)DBNull.Value),
                    ("current_uses", 0));
            else
                helper.EnqueueSingleRow(
                    ("valid_to", DateTime.UtcNow.AddDays(30)),
                    ("max_uses", (object)DBNull.Value),
                    ("current_uses", 0));
        }

        // 3) Currency code lookup
        helper.EnqueueScalar("EUR");

        // 4) Promo increment (if applicable) — happens after payment, needs an Execute result
        if (withPromo && !promoExpired)
            helper.EnqueueExecute(1);

        // 5) Provider info per line
        helper.EnqueueSingleRow(
            ("provider_id", (long)1), ("hotel_id", (long)1), ("provider_type", providerType));

        return helper;
    }

    [Fact]
    public async Task Submit_HappyPath_Returns200()
    {
        var reservation = CreateDraftWithLines();
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
        Factory.PaymentService.ProcessPaymentAsync(Arg.Any<PaymentRequest>(), Arg.Any<CancellationToken>())
            .Returns(new PaymentResult(true, "PAY-123"));

        var helper = SetupSubmitDapper();
        Factory.SetupDapperConnection(helper);

        var request = new SubmitRequest(1, "4111111111111111", "12/28", "123", "John Doe");
        var response = await PostAsync("/api/reservations/1/submit", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        reservation.StatusId.Should().Be((long)ReservationStatusEnum.Confirmed);
        await Factory.PaymentTransactionRepository.Received(1)
            .AddAsync(Arg.Is<PaymentTransaction>(pt =>
                pt.ReservationId == 1 && pt.TransactionReference == "PAY-123"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Submit_PaymentMethodMinDays_Returns500()
    {
        var reservation = CreateDraftWithLines();
        // Check-in in 30 days, but method requires 60
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);

        var helper = new DapperMockHelper();
        helper.EnqueueSingleRow(("min_days_before_checkin", 60));
        Factory.SetupDapperConnection(helper);

        var request = new SubmitRequest(1, null, null, null, null);
        var response = await PostAsync("/api/reservations/1/submit", request);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Submit_PromoExpired_Returns500()
    {
        var reservation = CreateDraftWithLines(promoCodeId: 10);
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);

        var helper = SetupSubmitDapper(withPromo: true, promoExpired: true);
        Factory.SetupDapperConnection(helper);

        var request = new SubmitRequest(1, null, null, null, null);
        var response = await PostAsync("/api/reservations/1/submit", request);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Submit_PromoUsageIncremented()
    {
        var reservation = CreateDraftWithLines(promoCodeId: 10);
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
        Factory.PaymentService.ProcessPaymentAsync(Arg.Any<PaymentRequest>(), Arg.Any<CancellationToken>())
            .Returns(new PaymentResult(true, "PAY-123"));

        var helper = SetupSubmitDapper(withPromo: true);
        Factory.SetupDapperConnection(helper);

        var request = new SubmitRequest(1, null, null, null, null);
        var response = await PostAsync("/api/reservations/1/submit", request);

        // The Dapper Execute call for promo increment was consumed from the queue
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Submit_PaymentFails_Returns500()
    {
        var reservation = CreateDraftWithLines();
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.PaymentService.ProcessPaymentAsync(Arg.Any<PaymentRequest>(), Arg.Any<CancellationToken>())
            .Returns(new PaymentResult(false, FailureReason: "Insufficient funds"));

        var helper = new DapperMockHelper();
        helper.EnqueueSingleRow(("min_days_before_checkin", 0)); // payment method
        helper.EnqueueScalar("EUR"); // currency code
        Factory.SetupDapperConnection(helper);

        var request = new SubmitRequest(1, null, null, null, null);
        var response = await PostAsync("/api/reservations/1/submit", request);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        reservation.StatusId.Should().Be((long)ReservationStatusEnum.Draft); // unchanged
    }

    [Fact]
    public async Task Submit_ExternalProvider_BookingCreated()
    {
        var reservation = CreateDraftWithLines();
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
        Factory.PaymentService.ProcessPaymentAsync(Arg.Any<PaymentRequest>(), Arg.Any<CancellationToken>())
            .Returns(new PaymentResult(true, "PAY-123"));
        Factory.ProviderReservationService.CreateBookingAsync(Arg.Any<ProviderBookingRequest>(), Arg.Any<CancellationToken>())
            .Returns(new ProviderBookingResult(true, "EXT-456"));

        var helper = SetupSubmitDapper(providerType: "external");
        Factory.SetupDapperConnection(helper);

        var request = new SubmitRequest(1, null, null, null, null);
        var response = await PostAsync("/api/reservations/1/submit", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await Factory.ProviderReservationService.Received()
            .CreateBookingAsync(Arg.Any<ProviderBookingRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Submit_ProviderFails_RefundsPayment()
    {
        var reservation = CreateDraftWithLines();
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.PaymentService.ProcessPaymentAsync(Arg.Any<PaymentRequest>(), Arg.Any<CancellationToken>())
            .Returns(new PaymentResult(true, "PAY-123"));
        Factory.ProviderReservationService.CreateBookingAsync(Arg.Any<ProviderBookingRequest>(), Arg.Any<CancellationToken>())
            .Returns(new ProviderBookingResult(false, FailureReason: "Unavailable"));
        Factory.PaymentService.ProcessRefundAsync(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new RefundResult(true, "REF-789"));

        var helper = SetupSubmitDapper(providerType: "external");
        Factory.SetupDapperConnection(helper);

        var request = new SubmitRequest(1, null, null, null, null);
        var response = await PostAsync("/api/reservations/1/submit", request);

        // Should fail after 3 retries and refund
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        await Factory.PaymentService.Received(1)
            .ProcessRefundAsync(1, "PAY-123", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Submit_NoLines_Returns500()
    {
        var reservation = CreateDraftWithLines();
        reservation.ReservationLines.Clear();
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);

        var request = new SubmitRequest(1, null, null, null, null);
        var response = await PostAsync("/api/reservations/1/submit", request);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Submit_NonDraft_Returns500()
    {
        var reservation = CreateDraftWithLines();
        reservation.StatusId = (long)ReservationStatusEnum.Confirmed;
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);

        var request = new SubmitRequest(1, null, null, null, null);
        var response = await PostAsync("/api/reservations/1/submit", request);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
