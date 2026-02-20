using System.Net;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Enums;
using ViajesAltairis.Reservations.Api.Tests.Fixtures;
using ViajesAltairis.ReservationsApi.Controllers;

namespace ViajesAltairis.Reservations.Api.Tests;

public class AddReservationGuestTests : IntegrationTestBase
{
    public AddReservationGuestTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task AddGuest_HappyPath_Returns204()
    {
        var line = new ReservationLine
        {
            Id = 10,
            ReservationId = 1,
            ReservationGuests = new List<ReservationGuest>(),
        };
        var reservation = new Reservation
        {
            Id = 1,
            ReservationCode = "RES-TEST-001",
            StatusId = (long)ReservationStatusEnum.Draft,
            BookedByUserId = 1,
            OwnerFirstName = "John",
            OwnerLastName = "Doe",
            CurrencyId = 1,
            ExchangeRateId = 1,
            ReservationLines = new List<ReservationLine> { line },
        };
        Factory.ReservationRepository.GetWithLinesAsync(1, Arg.Any<CancellationToken>()).Returns(reservation);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var request = new AddGuestRequest("Alice", "Wonder", "alice@test.com", null);
        var response = await PostAsync("/api/reservations/1/lines/10/guests", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        line.ReservationGuests.Should().HaveCount(1);
        line.ReservationGuests.First().FirstName.Should().Be("Alice");
    }

    [Fact]
    public async Task AddGuest_ReservationNotFound_ReturnsError()
    {
        Factory.ReservationRepository.GetWithLinesAsync(999, Arg.Any<CancellationToken>()).Returns((Reservation?)null);

        var request = new AddGuestRequest("Alice", "Wonder", null, null);
        var response = await PostAsync("/api/reservations/999/lines/10/guests", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound); // KeyNotFoundException
    }
}
