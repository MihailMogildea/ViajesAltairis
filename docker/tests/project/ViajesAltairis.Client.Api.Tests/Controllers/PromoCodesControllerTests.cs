using System.Net;
using FluentAssertions;
using MediatR;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.PromoCodes.Queries.ValidatePromoCode;
using ViajesAltairis.Client.Api.Tests.Fixtures;
using ViajesAltairis.Client.Api.Tests.Helpers;

namespace ViajesAltairis.Client.Api.Tests.Controllers;

public class PromoCodesControllerTests : IClassFixture<ClientApiFactory>
{
    private readonly ClientApiFactory _factory;

    public PromoCodesControllerTests(ClientApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Validate_Returns401_WhenNoToken()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/promocodes/validate?code=SUMMER2026");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Validate_ReturnsOk_WhenAuthenticated()
    {
        var client = _factory.CreateClient().WithClientAuth();
        _factory.MockMediator
            .Send(Arg.Any<ValidatePromoCodeQuery>(), Arg.Any<CancellationToken>())
            .Returns(new ValidatePromoCodeResponse { IsValid = true, DiscountPercentage = 10, Message = "Valid" });

        var response = await client.GetAsync("/api/promocodes/validate?code=SUMMER2026");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
