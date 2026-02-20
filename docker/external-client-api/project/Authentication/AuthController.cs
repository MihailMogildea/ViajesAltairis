using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ViajesAltairis.Application.Features.ExternalClient.Auth.Commands;
using ViajesAltairis.ExternalClientApi.Configuration;

namespace ViajesAltairis.ExternalClientApi.Authentication;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly JwtTokenService _jwtService;
    private readonly JwtSettings _jwtSettings;

    public AuthController(IMediator mediator, JwtTokenService jwtService, IOptions<JwtSettings> jwtSettings)
    {
        _mediator = mediator;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _mediator.Send(new LoginCommand(request.Email, request.Password));
            var token = _jwtService.GenerateToken(result.UserId, result.Email, result.BusinessPartnerId, result.PartnerName);
            var expiresAt = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours);
            return Ok(new LoginResponse(token, expiresAt));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}

public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token, DateTime ExpiresAt);
