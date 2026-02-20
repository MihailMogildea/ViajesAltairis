using FluentAssertions;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Subscriptions.Queries.GetMySubscription;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Client.Api.Tests.Fixtures;

namespace ViajesAltairis.Client.Api.Tests.Handlers.Subscriptions;

public class GetMySubscriptionHandlerTests
{
    [Fact]
    public async Task Handle_ActiveSubscription_ReturnsIt()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "v_user_subscription_status", @"
            user_subscription_id INTEGER, user_id INTEGER, subscription_type_id INTEGER,
            subscription_type_name TEXT, subscription_discount REAL,
            start_date TEXT, end_date TEXT, active INTEGER");
        TestDbHelper.Execute(conn, @"
            INSERT INTO v_user_subscription_status VALUES (1, 8, 2, 'Premium', 15.00, '2026-01-01', '2026-02-01', 1);
        ");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns(8L);
        currentUser.LanguageId.Returns(1L);
        var translationService = Substitute.For<ITranslationService>();

        var handler = new GetMySubscriptionHandler(factory, currentUser, translationService);
        var result = await handler.Handle(new GetMySubscriptionQuery(), CancellationToken.None);

        result.IsActive.Should().BeTrue();
        result.PlanName.Should().Be("Premium");
        result.Discount.Should().Be(15.00m);
    }

    [Fact]
    public async Task Handle_NoSubscription_ReturnsInactive()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "v_user_subscription_status", @"
            user_subscription_id INTEGER, user_id INTEGER, subscription_type_id INTEGER,
            subscription_type_name TEXT, subscription_discount REAL,
            start_date TEXT, end_date TEXT, active INTEGER");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns(8L);
        currentUser.LanguageId.Returns(1L);
        var translationService = Substitute.For<ITranslationService>();

        var handler = new GetMySubscriptionHandler(factory, currentUser, translationService);
        var result = await handler.Handle(new GetMySubscriptionQuery(), CancellationToken.None);

        result.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Spanish_TranslatesPlanName()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "v_user_subscription_status", @"
            user_subscription_id INTEGER, user_id INTEGER, subscription_type_id INTEGER,
            subscription_type_name TEXT, subscription_discount REAL,
            start_date TEXT, end_date TEXT, active INTEGER");
        TestDbHelper.Execute(conn, @"
            INSERT INTO v_user_subscription_status VALUES (1, 8, 2, 'Premium', 15.00, '2026-01-01', '2026-02-01', 1);
        ");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns(8L);
        currentUser.LanguageId.Returns(2L);
        var translationService = Substitute.For<ITranslationService>();
        translationService.ResolveAsync("subscription_type", Arg.Any<IEnumerable<long>>(), 2L, "name", Arg.Any<CancellationToken>())
            .Returns(new Dictionary<long, string> { [2] = "Prémium" });

        var handler = new GetMySubscriptionHandler(factory, currentUser, translationService);
        var result = await handler.Handle(new GetMySubscriptionQuery(), CancellationToken.None);

        result.PlanName.Should().Be("Prémium");
    }
}
