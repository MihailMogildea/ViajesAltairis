using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MediatR;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Profile.Commands.ChangePassword;
using ViajesAltairis.Application.Features.Client.Profile.Commands.UpdateProfile;
using ViajesAltairis.Application.Features.Client.Profile.Queries.GetProfile;
using ViajesAltairis.Client.Api.Tests.Fixtures;
using ViajesAltairis.Client.Api.Tests.Helpers;

namespace ViajesAltairis.Client.Api.Tests.Controllers;

public class ProfileControllerTests : IClassFixture<ClientApiFactory>
{
    private readonly ClientApiFactory _factory;

    public ProfileControllerTests(ClientApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetProfile_Returns401_WhenNoToken()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/profile");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProfile_ReturnsOk_WhenAuthenticated()
    {
        var client = _factory.CreateClient().WithClientAuth();
        _factory.MockMediator
            .Send(Arg.Any<GetProfileQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GetProfileResponse { Id = 8, Email = "client1@example.com", FirstName = "Juan" });

        var response = await client.GetAsync("/api/profile");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateProfile_Returns401_WhenNoToken()
    {
        var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync("/api/profile", new { FirstName = "Updated" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateProfile_ReturnsOk_WhenAuthenticated()
    {
        var client = _factory.CreateClient().WithClientAuth();
        _factory.MockMediator
            .Send(Arg.Any<UpdateProfileCommand>(), Arg.Any<CancellationToken>())
            .Returns(new UpdateProfileResponse { UserId = 8 });

        var response = await client.PutAsJsonAsync("/api/profile", new { FirstName = "Updated" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ChangePassword_Returns401_WhenNoToken()
    {
        var client = _factory.CreateClient();

        var response = await client.PutAsJsonAsync("/api/profile/password",
            new { CurrentPassword = "old", NewPassword = "new" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ChangePassword_ReturnsOk_WhenAuthenticated()
    {
        var client = _factory.CreateClient().WithClientAuth();
        _factory.MockMediator
            .Send(Arg.Any<ChangePasswordCommand>(), Arg.Any<CancellationToken>())
            .Returns(Unit.Value);

        var response = await client.PutAsJsonAsync("/api/profile/password",
            new { CurrentPassword = "password123", NewPassword = "newpass123" });

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
