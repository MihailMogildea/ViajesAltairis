using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Enums;
using ViajesAltairis.Reservations.Api.Tests.Fixtures;
using ViajesAltairis.Reservations.Api.Tests.Helpers;
using ViajesAltairis.ReservationsApi.Controllers;

namespace ViajesAltairis.Reservations.Api.Tests;

public class AddReservationLineTests : IntegrationTestBase
{
    public AddReservationLineTests(CustomWebApplicationFactory factory) : base(factory) { }

    private Reservation CreateDraftReservation(long id = 1, long? promoCodeId = null) => new()
    {
        Id = id,
        ReservationCode = "RES-TEST-001",
        StatusId = (long)ReservationStatusEnum.Draft,
        BookedByUserId = 1,
        OwnerFirstName = "John",
        OwnerLastName = "Doe",
        Subtotal = 0,
        TaxAmount = 0,
        MarginAmount = 0,
        DiscountAmount = 0,
        TotalPrice = 0,
        CurrencyId = 1,
        ExchangeRateId = 1,
        PromoCodeId = promoCodeId,
        ReservationLines = new List<ReservationLine>(),
    };

    private DapperMockHelper SetupBasicLineDapper(
        decimal pricePerNight = 100m, decimal boardPrice = 20m,
        decimal providerMargin = 5m, decimal hotelMargin = 3m,
        int quantity = 10, int bookedCount = 0,
        decimal? seasonalMargin = null,
        decimal userDiscount = 0, long? businessPartnerId = null,
        decimal bpDiscount = 0, decimal subscriptionDiscount = 0,
        decimal promoDiscountPct = 0, decimal promoDiscountAmt = 0,
        bool hasTaxes = true)
    {
        var helper = new DapperMockHelper();

        // 1) Room configuration
        helper.EnqueueSingleRow(
            ("id", (long)1), ("price_per_night", pricePerNight), ("quantity", quantity),
            ("hotel_provider_id", (long)1), ("currency_id", (long)1),
            ("provider_id", (long)1), ("hotel_id", (long)1),
            ("provider_margin", providerMargin), ("hotel_margin", hotelMargin),
            ("city_id", (long)1), ("administrative_division_id", (long)1), ("country_id", (long)1));

        // 2) Board price
        helper.EnqueueSingleRow(("price_per_night", boardPrice));

        // 3) Booked count (scalar)
        helper.EnqueueScalar(bookedCount);

        // 4) Seasonal margin
        if (seasonalMargin.HasValue)
            helper.EnqueueSingleRow(("margin", seasonalMargin.Value));
        else
            helper.EnqueueEmptyQuery("margin");

        // 5) User info for discounts
        helper.EnqueueSingleRow(("discount", userDiscount), ("business_partner_id", businessPartnerId ?? (object)DBNull.Value));

        // 6) Business partner discount (if applicable)
        if (businessPartnerId.HasValue)
            helper.EnqueueSingleRow(("discount", bpDiscount));

        // 7) Subscription discount
        if (subscriptionDiscount > 0)
            helper.EnqueueSingleRow(("discount", subscriptionDiscount));
        else
            helper.EnqueueEmptyQuery("discount");

        // 8) Promo code (if promoCodeId set on reservation)
        if (promoDiscountPct > 0 || promoDiscountAmt > 0)
            helper.EnqueueSingleRow(("discount_percentage", promoDiscountPct), ("discount_amount", promoDiscountAmt));

        // 9) Taxes
        if (hasTaxes)
        {
            helper.EnqueueMultiRow(
                ["tax_type_id", "rate", "is_percentage", "specificity"],
                new object[] { (long)1, 10m, true, 1 }); // 10% VAT at city level
        }
        else
        {
            helper.EnqueueMultiRow(["tax_type_id", "rate", "is_percentage", "specificity"]);
        }

        return helper;
    }

    [Fact]
    public async Task AddLine_HappyPath_Returns201()
    {
        var reservation = CreateDraftReservation();
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // price=100, board=20, 3 nights => subtotal = 360
        // margins: 5% + 3% = 8% => margin = 28.8
        // no discounts => discount = 0
        // tax: 10% of (360 + 28.8) = 38.88
        // total = 360 + 28.8 - 0 + 38.88 = 427.68
        var helper = SetupBasicLineDapper();
        Factory.SetupDapperConnection(helper);

        var checkIn = DateTime.UtcNow.Date.AddDays(30);
        var checkOut = checkIn.AddDays(3);
        var request = new AddLineRequest(1, 1, checkIn, checkOut, 2);

        var response = await PostAsync("/api/reservations/1/lines", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        reservation.ReservationLines.Should().HaveCount(1);
        var line = reservation.ReservationLines.First();
        line.NumNights.Should().Be(3);
        line.Subtotal.Should().Be(360m);
        line.MarginAmount.Should().Be(28.8m);
    }

    [Fact]
    public async Task AddLine_MarginsApplied()
    {
        var reservation = CreateDraftReservation();
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var helper = SetupBasicLineDapper(providerMargin: 10m, hotelMargin: 5m);
        Factory.SetupDapperConnection(helper);

        var checkIn = DateTime.UtcNow.Date.AddDays(30);
        var checkOut = checkIn.AddDays(2);
        var request = new AddLineRequest(1, 1, checkIn, checkOut, 2);

        var response = await PostAsync("/api/reservations/1/lines", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var line = reservation.ReservationLines.First();
        // subtotal = (100 + 20) * 2 = 240; margin = 240 * 15% = 36
        line.MarginAmount.Should().Be(36m);
    }

    [Fact]
    public async Task AddLine_SeasonalMarginAdded()
    {
        var reservation = CreateDraftReservation();
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // provider=5%, hotel=3%, seasonal=2% => total margin = 10%
        var helper = SetupBasicLineDapper(seasonalMargin: 2m);
        Factory.SetupDapperConnection(helper);

        var checkIn = DateTime.UtcNow.Date.AddDays(30);
        var checkOut = checkIn.AddDays(2);
        var request = new AddLineRequest(1, 1, checkIn, checkOut, 2);

        var response = await PostAsync("/api/reservations/1/lines", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var line = reservation.ReservationLines.First();
        // subtotal = 240; margin = 240 * 10% = 24
        line.MarginAmount.Should().Be(24m);
    }

    [Fact]
    public async Task AddLine_DiscountStack_AllSources()
    {
        var reservation = CreateDraftReservation(promoCodeId: 10);
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // user=2%, bp=3%, subscription=5%, promo=1% => total discount = 11%
        var helper = SetupBasicLineDapper(
            userDiscount: 2m, businessPartnerId: 1, bpDiscount: 3m,
            subscriptionDiscount: 5m, promoDiscountPct: 1m, hasTaxes: false);
        Factory.SetupDapperConnection(helper);

        var checkIn = DateTime.UtcNow.Date.AddDays(30);
        var checkOut = checkIn.AddDays(2);
        var request = new AddLineRequest(1, 1, checkIn, checkOut, 2);

        var response = await PostAsync("/api/reservations/1/lines", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var line = reservation.ReservationLines.First();
        // subtotal = 240; margin = 240 * 8% = 19.2
        // discount = (240 + 19.2) * 11% = 28.512
        line.DiscountAmount.Should().Be(28.512m);
    }

    [Fact]
    public async Task AddLine_PromoFixedAmount_AppliedAtHeader()
    {
        var reservation = CreateDraftReservation(promoCodeId: 10);
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var helper = SetupBasicLineDapper(promoDiscountPct: 0, promoDiscountAmt: 50m, hasTaxes: false);
        Factory.SetupDapperConnection(helper);

        var checkIn = DateTime.UtcNow.Date.AddDays(30);
        var checkOut = checkIn.AddDays(2);
        var request = new AddLineRequest(1, 1, checkIn, checkOut, 2);

        var response = await PostAsync("/api/reservations/1/lines", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        // header.DiscountAmount includes the fixed 50
        reservation.DiscountAmount.Should().Be(reservation.ReservationLines.Sum(l => l.DiscountAmount) + 50m);
        // header.TotalPrice has the fixed 50 subtracted
        reservation.TotalPrice.Should().Be(reservation.ReservationLines.Sum(l => l.TotalPrice) - 50m);
    }

    [Fact]
    public async Task AddLine_TaxHierarchy_MostSpecificWins()
    {
        var reservation = CreateDraftReservation();
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var helper = new DapperMockHelper();
        // Room config
        helper.EnqueueSingleRow(
            ("id", (long)1), ("price_per_night", 100m), ("quantity", 10),
            ("hotel_provider_id", (long)1), ("currency_id", (long)1),
            ("provider_id", (long)1), ("hotel_id", (long)1),
            ("provider_margin", 0m), ("hotel_margin", 0m),
            ("city_id", (long)1), ("administrative_division_id", (long)1), ("country_id", (long)1));
        // Board
        helper.EnqueueSingleRow(("price_per_night", 0m));
        // Availability
        helper.EnqueueScalar(0);
        // No seasonal margin
        helper.EnqueueEmptyQuery("margin");
        // User with no discounts
        helper.EnqueueSingleRow(("discount", 0m), ("business_partner_id", (object)DBNull.Value));
        // No subscription
        helper.EnqueueEmptyQuery("discount");
        // Taxes: same tax_type_id, city (specificity=1) and country (specificity=3) => city wins
        helper.EnqueueMultiRow(
            ["tax_type_id", "rate", "is_percentage", "specificity"],
            new object[] { (long)1, 8m, true, 1 },   // city-level 8%
            new object[] { (long)1, 21m, true, 3 });  // country-level 21% (should be skipped)

        Factory.SetupDapperConnection(helper);

        var checkIn = DateTime.UtcNow.Date.AddDays(30);
        var checkOut = checkIn.AddDays(1);
        var request = new AddLineRequest(1, 1, checkIn, checkOut, 1);

        var response = await PostAsync("/api/reservations/1/lines", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var line = reservation.ReservationLines.First();
        // subtotal = 100 * 1 = 100; no margin, no discount
        // tax = 100 * 8% = 8 (city wins over country for same tax_type)
        line.TaxAmount.Should().Be(8m);
    }

    [Fact]
    public async Task AddLine_MultipleTaxTypes_Summed()
    {
        var reservation = CreateDraftReservation();
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var helper = new DapperMockHelper();
        helper.EnqueueSingleRow(
            ("id", (long)1), ("price_per_night", 200m), ("quantity", 10),
            ("hotel_provider_id", (long)1), ("currency_id", (long)1),
            ("provider_id", (long)1), ("hotel_id", (long)1),
            ("provider_margin", 0m), ("hotel_margin", 0m),
            ("city_id", (long)1), ("administrative_division_id", (long)1), ("country_id", (long)1));
        helper.EnqueueSingleRow(("price_per_night", 0m));
        helper.EnqueueScalar(0);
        helper.EnqueueEmptyQuery("margin");
        helper.EnqueueSingleRow(("discount", 0m), ("business_partner_id", (object)DBNull.Value));
        helper.EnqueueEmptyQuery("discount");
        // Two different tax types — both applied
        helper.EnqueueMultiRow(
            ["tax_type_id", "rate", "is_percentage", "specificity"],
            new object[] { (long)1, 10m, true, 1 },  // VAT 10%
            new object[] { (long)2, 2m, true, 1 });   // tourism tax 2%

        Factory.SetupDapperConnection(helper);

        var checkIn = DateTime.UtcNow.Date.AddDays(30);
        var checkOut = checkIn.AddDays(1);
        var request = new AddLineRequest(1, 1, checkIn, checkOut, 1);

        var response = await PostAsync("/api/reservations/1/lines", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var line = reservation.ReservationLines.First();
        // subtotal = 200; tax = 200 * 10% + 200 * 2% = 20 + 4 = 24
        line.TaxAmount.Should().Be(24m);
    }

    [Fact]
    public async Task AddLine_CurrencyConversion_ConverterCalled()
    {
        // Reservation currency = 2, room currency = 1 → conversion needed
        var reservation = CreateDraftReservation();
        reservation.CurrencyId = 2;
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        Factory.CurrencyConverter.ConvertAsync(100m, 1, 2, Arg.Any<CancellationToken>())
            .Returns((110m, (long)5));
        Factory.CurrencyConverter.ConvertAsync(20m, 1, 2, Arg.Any<CancellationToken>())
            .Returns((22m, (long)5));

        var helper = new DapperMockHelper();
        helper.EnqueueSingleRow(
            ("id", (long)1), ("price_per_night", 100m), ("quantity", 10),
            ("hotel_provider_id", (long)1), ("currency_id", (long)1), // different from reservation
            ("provider_id", (long)1), ("hotel_id", (long)1),
            ("provider_margin", 0m), ("hotel_margin", 0m),
            ("city_id", (long)1), ("administrative_division_id", (long)1), ("country_id", (long)1));
        helper.EnqueueSingleRow(("price_per_night", 20m));
        helper.EnqueueScalar(0);
        helper.EnqueueEmptyQuery("margin");
        helper.EnqueueSingleRow(("discount", 0m), ("business_partner_id", (object)DBNull.Value));
        helper.EnqueueEmptyQuery("discount");
        helper.EnqueueMultiRow(["tax_type_id", "rate", "is_percentage", "specificity"]);
        Factory.SetupDapperConnection(helper);

        var checkIn = DateTime.UtcNow.Date.AddDays(30);
        var checkOut = checkIn.AddDays(1);
        var request = new AddLineRequest(1, 1, checkIn, checkOut, 1);

        var response = await PostAsync("/api/reservations/1/lines", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await Factory.CurrencyConverter.Received(2)
            .ConvertAsync(Arg.Any<decimal>(), 1, 2, Arg.Any<CancellationToken>());
        var line = reservation.ReservationLines.First();
        line.PricePerNight.Should().Be(110m);
        line.BoardPricePerNight.Should().Be(22m);
    }

    [Fact]
    public async Task AddLine_NoAvailability_Returns500()
    {
        var reservation = CreateDraftReservation();
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);

        var helper = SetupBasicLineDapper(quantity: 5, bookedCount: 5);
        Factory.SetupDapperConnection(helper);

        var checkIn = DateTime.UtcNow.Date.AddDays(30);
        var checkOut = checkIn.AddDays(2);
        var request = new AddLineRequest(1, 1, checkIn, checkOut, 2);

        var response = await PostAsync("/api/reservations/1/lines", request);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError); // InvalidOperationException
    }

    [Fact]
    public async Task AddLine_NonDraft_Returns500()
    {
        var reservation = CreateDraftReservation();
        reservation.StatusId = (long)ReservationStatusEnum.Confirmed;
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);

        var checkIn = DateTime.UtcNow.Date.AddDays(30);
        var checkOut = checkIn.AddDays(2);
        var request = new AddLineRequest(1, 1, checkIn, checkOut, 2);

        var response = await PostAsync("/api/reservations/1/lines", request);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task AddLine_RoomNotFound_ReturnsError()
    {
        var reservation = CreateDraftReservation();
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);

        var helper = new DapperMockHelper();
        helper.EnqueueEmptyQuery("id", "price_per_night", "quantity", "hotel_provider_id",
            "currency_id", "provider_id", "hotel_id", "provider_margin", "hotel_margin",
            "city_id", "administrative_division_id", "country_id");
        Factory.SetupDapperConnection(helper);

        var checkIn = DateTime.UtcNow.Date.AddDays(30);
        var checkOut = checkIn.AddDays(2);
        var request = new AddLineRequest(1, 1, checkIn, checkOut, 2);

        var response = await PostAsync("/api/reservations/1/lines", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound); // KeyNotFoundException
    }

    [Fact]
    public async Task AddLine_HeaderTotalsRecalculated()
    {
        var existingLine = new ReservationLine
        {
            Id = 100,
            ReservationId = 1,
            Subtotal = 500m,
            TaxAmount = 50m,
            MarginAmount = 40m,
            DiscountAmount = 20m,
            TotalPrice = 570m,
        };
        var reservation = CreateDraftReservation();
        reservation.ReservationLines = new List<ReservationLine> { existingLine };
        reservation.Subtotal = 500m;
        reservation.TaxAmount = 50m;
        reservation.MarginAmount = 40m;
        reservation.DiscountAmount = 20m;
        reservation.TotalPrice = 570m;

        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var helper = SetupBasicLineDapper(hasTaxes: false);
        Factory.SetupDapperConnection(helper);

        var checkIn = DateTime.UtcNow.Date.AddDays(30);
        var checkOut = checkIn.AddDays(2);
        var request = new AddLineRequest(1, 1, checkIn, checkOut, 2);

        var response = await PostAsync("/api/reservations/1/lines", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        reservation.ReservationLines.Should().HaveCount(2);
        // Header = sum of both lines
        reservation.Subtotal.Should().Be(reservation.ReservationLines.Sum(l => l.Subtotal));
        reservation.TaxAmount.Should().Be(reservation.ReservationLines.Sum(l => l.TaxAmount));
    }
}
