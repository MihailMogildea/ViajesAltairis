using FluentAssertions;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Subscriptions.Commands.Subscribe;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Client.Api.Tests.Fixtures;

namespace ViajesAltairis.Client.Api.Tests.Handlers.Subscriptions;

public class SubscribeHandlerTests
{
    [Fact]
    public async Task Handle_ValidPlan_CreatesSubscription()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "subscription_type", "id INTEGER PRIMARY KEY, name TEXT, price_per_month REAL, currency_id INTEGER, enabled INTEGER");
        TestDbHelper.CreateTable(conn, "currency", "id INTEGER PRIMARY KEY, iso_code TEXT");
        TestDbHelper.Execute(conn, "INSERT INTO currency VALUES (1, 'EUR')");
        TestDbHelper.CreateTable(conn, "user_subscription", @"
            id INTEGER PRIMARY KEY AUTOINCREMENT, user_id INTEGER, subscription_type_id INTEGER,
            start_date TEXT, end_date TEXT, active INTEGER, created_at TEXT, updated_at TEXT");
        TestDbHelper.Execute(conn, "INSERT INTO subscription_type VALUES (1, 'Basic', 9.99, 1, 1)");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns(8L);

        var paymentService = Substitute.For<IPaymentService>();
        paymentService.ProcessPaymentAsync(Arg.Any<PaymentRequest>(), Arg.Any<CancellationToken>())
            .Returns(new PaymentResult(true, "PAY-001"));

        var handler = new SubscribeHandler(currentUser, factory, paymentService);
        var result = await handler.Handle(new SubscribeCommand { SubscriptionTypeId = 1 }, CancellationToken.None);

        result.SubscriptionId.Should().BeGreaterThan(0);
        result.StartDate.Should().BeCloseTo(DateTime.UtcNow.Date, TimeSpan.FromDays(1));
    }

    [Fact]
    public async Task Handle_InvalidPlan_ThrowsKeyNotFound()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "subscription_type", "id INTEGER PRIMARY KEY, name TEXT, price_per_month REAL, currency_id INTEGER, enabled INTEGER");
        TestDbHelper.CreateTable(conn, "currency", "id INTEGER PRIMARY KEY, iso_code TEXT");
        TestDbHelper.Execute(conn, "INSERT INTO currency VALUES (1, 'EUR')");
        TestDbHelper.CreateTable(conn, "user_subscription", @"
            id INTEGER PRIMARY KEY AUTOINCREMENT, user_id INTEGER, subscription_type_id INTEGER,
            start_date TEXT, end_date TEXT, active INTEGER, created_at TEXT, updated_at TEXT");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns(8L);
        var paymentService = Substitute.For<IPaymentService>();

        var handler = new SubscribeHandler(currentUser, factory, paymentService);
        var act = () => handler.Handle(new SubscribeCommand { SubscriptionTypeId = 999 }, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
