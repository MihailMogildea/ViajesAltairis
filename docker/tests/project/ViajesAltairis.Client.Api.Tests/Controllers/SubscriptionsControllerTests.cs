using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MediatR;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Subscriptions.Commands.Subscribe;
using ViajesAltairis.Application.Features.Client.Subscriptions.Queries.GetMySubscription;
using ViajesAltairis.Application.Features.Client.Subscriptions.Queries.GetSubscriptionPlans;
using ViajesAltairis.Client.Api.Tests.Fixtures;
using ViajesAltairis.Client.Api.Tests.Helpers;

namespace ViajesAltairis.Client.Api.Tests.Controllers;

public class SubscriptionsControllerTests : IClassFixture<ClientApiFactory>
{
    private readonly ClientApiFactory _factory;

    public SubscriptionsControllerTests(ClientApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetPlans_ReturnsOk_Anonymous()
    {
        var client = _factory.CreateClient();
        _factory.MockMediator
            .Send(Arg.Any<GetSubscriptionPlansQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetSubscriptionPlansResponse { Plans = [] });

        var response = await client.GetAsync("/api/subscriptions/plans");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMySubscription_Returns401_WhenNoToken()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/subscriptions/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMySubscription_ReturnsOk_WhenAuthenticated()
    {
        var client = _factory.CreateClient().WithClientAuth();
        _factory.MockMediator
            .Send(Arg.Any<GetMySubscriptionQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetMySubscriptionResponse { IsActive = false });

        var response = await client.GetAsync("/api/subscriptions/me");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Subscribe_Returns401_WhenNoToken()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/subscriptions", new { SubscriptionTypeId = 1 });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Subscribe_ReturnsOk_WhenAuthenticated()
    {
        var client = _factory.CreateClient().WithClientAuth();
        _factory.MockMediator
            .Send(Arg.Any<SubscribeCommand>(), Arg.Any<CancellationToken>())
            .Returns(new SubscribeResponse { SubscriptionId = 1, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(1) });

        var response = await client.PostAsJsonAsync("/api/subscriptions", new { SubscriptionTypeId = 1 });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
