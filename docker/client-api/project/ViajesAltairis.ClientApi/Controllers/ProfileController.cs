using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Client.Profile.Commands.ChangePassword;
using ViajesAltairis.Application.Features.Client.Profile.Commands.UpdateProfile;
using ViajesAltairis.Application.Features.Client.Profile.Queries.GetProfile;

namespace ViajesAltairis.ClientApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Client")]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _mediator.Send(new GetProfileQuery());
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateProfileCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }
}
