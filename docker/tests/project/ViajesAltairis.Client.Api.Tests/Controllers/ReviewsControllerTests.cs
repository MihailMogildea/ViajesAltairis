using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MediatR;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Reviews.Commands.SubmitReview;
using ViajesAltairis.Client.Api.Tests.Fixtures;
using ViajesAltairis.Client.Api.Tests.Helpers;

namespace ViajesAltairis.Client.Api.Tests.Controllers;

public class ReviewsControllerTests : IClassFixture<ClientApiFactory>
{
    private readonly ClientApiFactory _factory;

    public ReviewsControllerTests(ClientApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Submit_Returns401_WhenNoToken()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/reviews",
            new { ReservationLineId = 1, Rating = 5, Title = "Great", Comment = "Loved it" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Submit_Returns201_WhenAuthenticated()
    {
        var client = _factory.CreateClient().WithClientAuth();
        _factory.MockMediator
            .Send(Arg.Any<SubmitReviewCommand>(), Arg.Any<CancellationToken>())
            .Returns(42L);

        var response = await client.PostAsJsonAsync("/api/reviews",
            new { ReservationLineId = 1, Rating = 5, Title = "Great", Comment = "Loved it" });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
