using System.Net;
using FluentAssertions;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ViajesAltairis.Application.Features.Client.Hotels.Queries.GetCancellationPolicy;
using ViajesAltairis.Application.Features.Client.Hotels.Queries.GetHotelDetail;
using ViajesAltairis.Application.Features.Client.Hotels.Queries.GetHotelReviews;
using ViajesAltairis.Application.Features.Client.Hotels.Queries.GetRoomAvailability;
using ViajesAltairis.Application.Features.Client.Hotels.Queries.SearchHotels;
using ViajesAltairis.Client.Api.Tests.Fixtures;

namespace ViajesAltairis.Client.Api.Tests.Controllers;

public class HotelsControllerTests : IClassFixture<ClientApiFactory>
{
    private readonly HttpClient _client;
    private readonly ClientApiFactory _factory;

    public HotelsControllerTests(ClientApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Search_ReturnsOk_Anonymous()
    {
        _factory.MockMediator
            .Send(Arg.Any<SearchHotelsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new SearchHotelsResponse { Hotels = [], TotalCount = 0 });

        var response = await _client.GetAsync("/api/hotels?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDetail_ReturnsOk_Anonymous()
    {
        _factory.MockMediator
            .Send(Arg.Any<GetHotelDetailQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetHotelDetailResponse { Id = 1, Name = "Test Hotel", Stars = 4 });

        var response = await _client.GetAsync("/api/hotels/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDetail_Returns404_WhenNotFound()
    {
        _factory.MockMediator
            .Send(Arg.Any<GetHotelDetailQuery>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new KeyNotFoundException("Hotel 999 not found."));

        var response = await _client.GetAsync("/api/hotels/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAvailability_ReturnsOk_Anonymous()
    {
        _factory.MockMediator
            .Send(Arg.Any<GetRoomAvailabilityQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetRoomAvailabilityResponse { Rooms = [] });

        var response = await _client.GetAsync("/api/hotels/1/availability?checkIn=2026-03-01&checkOut=2026-03-05&guests=2");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetReviews_ReturnsOk_Anonymous()
    {
        _factory.MockMediator
            .Send(Arg.Any<GetHotelReviewsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetHotelReviewsResponse { Reviews = [], TotalCount = 0, AverageRating = 0 });

        var response = await _client.GetAsync("/api/hotels/1/reviews?page=1&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCancellationPolicy_ReturnsOk_Anonymous()
    {
        _factory.MockMediator
            .Send(Arg.Any<GetCancellationPolicyQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetCancellationPolicyResponse { Policies = [] });

        var response = await _client.GetAsync("/api/hotels/1/cancellation-policy");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Search_WithFilters_PassesQueryParams()
    {
        _factory.MockMediator
            .Send(Arg.Any<SearchHotelsQuery>(), Arg.Any<CancellationToken>())
            .Returns(new SearchHotelsResponse { Hotels = [], TotalCount = 0 });

        var response = await _client.GetAsync("/api/hotels?countryId=1&stars=5&page=2&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
