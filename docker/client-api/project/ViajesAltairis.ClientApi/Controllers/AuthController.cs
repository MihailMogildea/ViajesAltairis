using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Client.Auth.Commands.ForgotPassword;
using ViajesAltairis.Application.Features.Client.Auth.Commands.Login;
using ViajesAltairis.Application.Features.Client.Auth.Commands.Register;
using ViajesAltairis.Application.Features.Client.Auth.Commands.ResetPassword;

namespace ViajesAltairis.ClientApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }
}
