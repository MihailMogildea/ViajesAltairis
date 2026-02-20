using FluentAssertions;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Subscriptions.Queries.GetSubscriptionPlans;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Client.Api.Tests.Fixtures;

namespace ViajesAltairis.Client.Api.Tests.Handlers.Subscriptions;

public class GetSubscriptionPlansHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsEnabledPlans()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "subscription_type", @"
            id INTEGER PRIMARY KEY, name TEXT, price_per_month REAL, discount REAL,
            currency_id INTEGER, enabled INTEGER");
        TestDbHelper.CreateTable(conn, "currency", "id INTEGER PRIMARY KEY, iso_code TEXT");
        TestDbHelper.Execute(conn, @"
            INSERT INTO currency VALUES (1, 'EUR');
            INSERT INTO subscription_type VALUES (1, 'Basic', 9.99, 5, 1, 1);
            INSERT INTO subscription_type VALUES (2, 'Premium', 19.99, 15, 1, 1);
            INSERT INTO subscription_type VALUES (3, 'Disabled', 99.99, 50, 1, 0);
        ");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);
        var translationService = Substitute.For<ITranslationService>();
        translationService.ResolveAsync(Arg.Any<string>(), Arg.Any<IEnumerable<long>>(), Arg.Any<long>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new Dictionary<long, string>());
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.LanguageId.Returns(1L);
        currentUser.CurrencyCode.Returns("EUR");
        var currencyConverter = Substitute.For<ICurrencyConverter>();

        var handler = new GetSubscriptionPlansHandler(factory, translationService, currentUser, currencyConverter, Substitute.For<ICacheService>());
        var result = await handler.Handle(new GetSubscriptionPlansQuery(), CancellationToken.None);

        result.Plans.Should().HaveCount(2);
        result.Plans.First().Name.Should().Be("Basic");
    }

    [Fact]
    public async Task Handle_Spanish_TranslatesPlanNames()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "subscription_type", @"
            id INTEGER PRIMARY KEY, name TEXT, price_per_month REAL, discount REAL,
            currency_id INTEGER, enabled INTEGER");
        TestDbHelper.CreateTable(conn, "currency", "id INTEGER PRIMARY KEY, iso_code TEXT");
        TestDbHelper.Execute(conn, @"
            INSERT INTO currency VALUES (1, 'EUR');
            INSERT INTO subscription_type VALUES (1, 'Basic', 9.99, 5, 1, 1);
        ");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);
        var translationService = Substitute.For<ITranslationService>();
        translationService.ResolveAsync("subscription_type", Arg.Any<IEnumerable<long>>(), 2L, "name", Arg.Any<CancellationToken>())
            .Returns(new Dictionary<long, string> { [1] = "Básico" });
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.LanguageId.Returns(2L);
        currentUser.CurrencyCode.Returns("EUR");
        var currencyConverter = Substitute.For<ICurrencyConverter>();

        var handler = new GetSubscriptionPlansHandler(factory, translationService, currentUser, currencyConverter, Substitute.For<ICacheService>());
        var result = await handler.Handle(new GetSubscriptionPlansQuery(), CancellationToken.None);

        result.Plans.First().Name.Should().Be("Básico");
    }
}
