using FluentAssertions;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.PromoCodes.Queries.ValidatePromoCode;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Client.Api.Tests.Fixtures;

namespace ViajesAltairis.Client.Api.Tests.Handlers.PromoCodes;

public class ValidatePromoCodeHandlerTests
{
    [Fact]
    public async Task Handle_ValidCode_ReturnsValid()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "v_active_promo_code", @"
            promo_code_id INTEGER, code TEXT, discount_percentage REAL,
            discount_amount REAL, currency_code TEXT, valid_to TEXT");
        TestDbHelper.CreateTable(conn, "currency", "id INTEGER PRIMARY KEY, iso_code TEXT");
        TestDbHelper.Execute(conn, @"
            INSERT INTO v_active_promo_code VALUES (1, 'SUMMER2026', 10, NULL, NULL, '2026-12-31');
            INSERT INTO currency VALUES (1, 'EUR');
        ");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);
        var currencyConverter = Substitute.For<ICurrencyConverter>();
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.CurrencyCode.Returns("EUR");

        var handler = new ValidatePromoCodeHandler(factory, currencyConverter, currentUser);
        var result = await handler.Handle(new ValidatePromoCodeQuery { Code = "SUMMER2026" }, CancellationToken.None);

        result.IsValid.Should().BeTrue();
        result.DiscountPercentage.Should().Be(10);
    }

    [Fact]
    public async Task Handle_InvalidCode_ReturnsInvalid()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "v_active_promo_code", @"
            promo_code_id INTEGER, code TEXT, discount_percentage REAL,
            discount_amount REAL, currency_code TEXT, valid_to TEXT");
        TestDbHelper.CreateTable(conn, "currency", "id INTEGER PRIMARY KEY, iso_code TEXT");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);
        var currencyConverter = Substitute.For<ICurrencyConverter>();
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.CurrencyCode.Returns("EUR");

        var handler = new ValidatePromoCodeHandler(factory, currencyConverter, currentUser);
        var result = await handler.Handle(new ValidatePromoCodeQuery { Code = "INVALID" }, CancellationToken.None);

        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_AmountDiscount_ReturnsCurrencyInfo()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "v_active_promo_code", @"
            promo_code_id INTEGER, code TEXT, discount_percentage REAL,
            discount_amount REAL, currency_code TEXT, valid_to TEXT");
        TestDbHelper.CreateTable(conn, "currency", "id INTEGER PRIMARY KEY, iso_code TEXT");
        TestDbHelper.Execute(conn, @"
            INSERT INTO v_active_promo_code VALUES (2, 'FLAT50', NULL, 50, 'EUR', '2026-12-31');
            INSERT INTO currency VALUES (1, 'EUR');
        ");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);
        var currencyConverter = Substitute.For<ICurrencyConverter>();
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.CurrencyCode.Returns("EUR");

        var handler = new ValidatePromoCodeHandler(factory, currencyConverter, currentUser);
        var result = await handler.Handle(new ValidatePromoCodeQuery { Code = "FLAT50" }, CancellationToken.None);

        result.IsValid.Should().BeTrue();
        result.DiscountAmount.Should().Be(50);
        result.CurrencyCode.Should().Be("EUR");
    }

    [Fact]
    public async Task Handle_NullValidTo_ReturnsValid()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "v_active_promo_code", @"
            promo_code_id INTEGER, code TEXT, discount_percentage REAL,
            discount_amount REAL, currency_code TEXT, valid_to TEXT");
        TestDbHelper.CreateTable(conn, "currency", "id INTEGER PRIMARY KEY, iso_code TEXT");
        TestDbHelper.Execute(conn, "INSERT INTO v_active_promo_code VALUES (3, 'FOREVER', 5, NULL, NULL, NULL)");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);
        var currencyConverter = Substitute.For<ICurrencyConverter>();
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.CurrencyCode.Returns("EUR");

        var handler = new ValidatePromoCodeHandler(factory, currencyConverter, currentUser);
        var result = await handler.Handle(new ValidatePromoCodeQuery { Code = "FOREVER" }, CancellationToken.None);

        result.IsValid.Should().BeTrue();
        result.ExpiresAt.Should().BeNull();
    }
}
