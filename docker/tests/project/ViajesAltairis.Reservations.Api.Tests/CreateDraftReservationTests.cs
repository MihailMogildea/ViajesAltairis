using System.Net;
using System.Net.Http.Json;
using ViajesAltairis.Application.Reservations.Commands;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Enums;
using ViajesAltairis.Reservations.Api.Tests.Fixtures;
using ViajesAltairis.Reservations.Api.Tests.Helpers;

namespace ViajesAltairis.Reservations.Api.Tests;

public class CreateDraftReservationTests : IntegrationTestBase
{
    public CreateDraftReservationTests(CustomWebApplicationFactory factory) : base(factory) { }

    private User CreateTestUser(long id = 1) => new()
    {
        Id = id,
        FirstName = "John",
        LastName = "Doe",
        Email = "john@test.com",
        Phone = "+34600000001",
        TaxId = "12345678A",
        Address = "123 Main St",
        City = "Madrid",
        PostalCode = "28001",
        Country = "ES",
        PasswordHash = "hash",
        Discount = 0,
        Enabled = true,
    };

    private DapperMockHelper SetupDapperForValidDraft(bool withPromo = false)
    {
        var helper = new DapperMockHelper();
        // 1) Currency lookup
        helper.EnqueueSingleRow(("id", (long)1));
        // 2) Exchange rate lookup
        helper.EnqueueSingleRow(("id", (long)1));
        // 3) Promo code lookup (if requested)
        if (withPromo)
            helper.EnqueueSingleRow(("id", (long)10));
        return helper;
    }

    [Fact]
    public async Task CreateDraft_SelfBooking_Returns201()
    {
        var user = CreateTestUser();
        Factory.UserGenericRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        var helper = SetupDapperForValidDraft();
        Factory.SetupDapperConnection(helper);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var command = new CreateDraftReservationCommand(1, "EUR", null);
        var response = await PostAsync("/api/reservations/draft", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await Factory.ReservationRepository.Received(1)
            .AddAsync(Arg.Is<Reservation>(r =>
                r.BookedByUserId == 1 &&
                r.OwnerFirstName == "John" &&
                r.StatusId == (long)ReservationStatusEnum.Draft),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateDraft_OnBehalf_UsesOwnerUserFields()
    {
        var booker = CreateTestUser(1);
        var owner = CreateTestUser(2);
        owner.FirstName = "Jane";
        owner.LastName = "Smith";

        Factory.UserGenericRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(booker);
        Factory.UserGenericRepository.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(owner);

        var helper = SetupDapperForValidDraft();
        Factory.SetupDapperConnection(helper);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var command = new CreateDraftReservationCommand(1, "EUR", null, OwnerUserId: 2);
        var response = await PostAsync("/api/reservations/draft", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await Factory.ReservationRepository.Received(1)
            .AddAsync(Arg.Is<Reservation>(r =>
                r.OwnerUserId == 2 &&
                r.OwnerFirstName == "Jane" &&
                r.OwnerLastName == "Smith"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateDraft_WalkIn_UsesProvidedFields()
    {
        var booker = CreateTestUser();
        Factory.UserGenericRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(booker);

        var helper = SetupDapperForValidDraft();
        Factory.SetupDapperConnection(helper);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var command = new CreateDraftReservationCommand(
            1, "EUR", null,
            OwnerFirstName: "Walk", OwnerLastName: "In",
            OwnerEmail: "walkin@test.com");
        var response = await PostAsync("/api/reservations/draft", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await Factory.ReservationRepository.Received(1)
            .AddAsync(Arg.Is<Reservation>(r =>
                r.OwnerUserId == null &&
                r.OwnerFirstName == "Walk" &&
                r.OwnerLastName == "In" &&
                r.OwnerEmail == "walkin@test.com"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateDraft_WithPromoCode_SetsPromoCodeId()
    {
        var user = CreateTestUser();
        Factory.UserGenericRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        var helper = SetupDapperForValidDraft(withPromo: true);
        Factory.SetupDapperConnection(helper);
        Factory.UnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var command = new CreateDraftReservationCommand(1, "EUR", "PROMO10");
        var response = await PostAsync("/api/reservations/draft", command);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await Factory.ReservationRepository.Received(1)
            .AddAsync(Arg.Is<Reservation>(r => r.PromoCodeId == 10),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateDraft_InvalidCurrency_ReturnsError()
    {
        var user = CreateTestUser();
        Factory.UserGenericRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        var helper = new DapperMockHelper();
        helper.EnqueueEmptyQuery("id"); // Currency not found
        Factory.SetupDapperConnection(helper);

        var command = new CreateDraftReservationCommand(1, "XXX", null);
        var response = await PostAsync("/api/reservations/draft", command);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound); // KeyNotFoundException
    }

    [Fact]
    public async Task CreateDraft_ExpiredPromo_ReturnsError()
    {
        var user = CreateTestUser();
        Factory.UserGenericRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        var helper = new DapperMockHelper();
        helper.EnqueueSingleRow(("id", (long)1)); // Currency
        helper.EnqueueSingleRow(("id", (long)1)); // Exchange rate
        helper.EnqueueEmptyQuery("id"); // Promo not found (expired)
        Factory.SetupDapperConnection(helper);

        var command = new CreateDraftReservationCommand(1, "EUR", "EXPIRED");
        var response = await PostAsync("/api/reservations/draft", command);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound); // KeyNotFoundException
    }

    [Fact]
    public async Task CreateDraft_NoExchangeRate_ReturnsError()
    {
        var user = CreateTestUser();
        Factory.UserGenericRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        var helper = new DapperMockHelper();
        helper.EnqueueSingleRow(("id", (long)1)); // Currency found
        helper.EnqueueEmptyQuery("id"); // No active exchange rate
        Factory.SetupDapperConnection(helper);

        var command = new CreateDraftReservationCommand(1, "EUR", null);
        var response = await PostAsync("/api/reservations/draft", command);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError); // InvalidOperationException
    }

    [Fact]
    public async Task CreateDraft_InvalidUser_ReturnsError()
    {
        Factory.UserGenericRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((User?)null);

        var command = new CreateDraftReservationCommand(1, "EUR", null);
        var response = await PostAsync("/api/reservations/draft", command);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound); // KeyNotFoundException
    }
}
