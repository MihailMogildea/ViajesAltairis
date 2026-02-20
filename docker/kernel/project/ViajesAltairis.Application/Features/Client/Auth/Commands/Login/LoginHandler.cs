using MediatR;
using Microsoft.Extensions.Configuration;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Auth.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;

    public LoginHandler(IUserRepository userRepository, IPasswordService passwordService, IJwtTokenService jwtTokenService, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _jwtTokenService = jwtTokenService;
        _configuration = configuration;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null || user.UserTypeId != 5 || !user.Enabled)
            throw new UnauthorizedAccessException("Invalid email or password.");

        if (!_passwordService.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        var token = _jwtTokenService.GenerateToken(user.Id, user.Email, "Client");
        var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationInMinutes"] ?? "60");

        return new LoginResponse
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
        };
    }
}
