using System.Net;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Enums;
using ViajesAltairis.Reservations.Api.Tests.Fixtures;

namespace ViajesAltairis.Reservations.Api.Tests;

public class RemoveReservationLineTests : IntegrationTestBase
{
    public RemoveReservationLineTests(CustomWebApplicationFactory factory) : base(factory) { }

    private Reservation CreateReservationWithLine()
    {
        var line = new ReservationLine
        {
            Id = 10,
            ReservationId = 1,
            Subtotal = 300m,
            TaxAmount = 30m,
            MarginAmount = 24m,
            DiscountAmount = 10m,
            TotalPrice = 344m,
        };
        return new Reservation
        {
            Id = 1,
            ReservationCode = "RES-TEST-001",
            StatusId = (long)ReservationStatusEnum.Draft,
            BookedByUserId = 1,
            OwnerFirstName = "John",
            OwnerLastName = "Doe",
            Subtotal = 300m,
            TaxAmount = 30m,
            MarginAmount = 24m,
            DiscountAmount = 10m,
            TotalPrice = 344m,
            CurrencyId = 1,
            ExchangeRateId = 1,
            ReservationLines = new List<ReservationLine> { line },
        };
    }

    [Fact]
    public async Task RemoveLine_HappyPath_Returns204()
    {
        var reservation = CreateReservationWithLine();
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var response = await DeleteAsync("/api/reservations/1/lines/10");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        reservation.ReservationLines.Should().BeEmpty();
        // After removing the only line, totals = 0
        reservation.Subtotal.Should().Be(0);
        reservation.TotalPrice.Should().Be(0);
    }

    [Fact]
    public async Task RemoveLine_LastLine_ZeroTotals()
    {
        var reservation = CreateReservationWithLine();
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var response = await DeleteAsync("/api/reservations/1/lines/10");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        reservation.Subtotal.Should().Be(0);
        reservation.TaxAmount.Should().Be(0);
        reservation.MarginAmount.Should().Be(0);
        reservation.DiscountAmount.Should().Be(0);
        reservation.TotalPrice.Should().Be(0);
    }

    [Fact]
    public async Task RemoveLine_NonDraft_Returns500()
    {
        var reservation = CreateReservationWithLine();
        reservation.StatusId = (long)ReservationStatusEnum.Confirmed;
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);

        var response = await DeleteAsync("/api/reservations/1/lines/10");

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
