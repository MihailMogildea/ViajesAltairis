using MediatR;
using Microsoft.Extensions.Configuration;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Auth.Commands.ForgotPassword;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly ICacheService _cacheService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public ForgotPasswordHandler(IUserRepository userRepository, ICacheService cacheService, IEmailService emailService, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _cacheService = cacheService;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        // Silently succeed if not found to prevent email enumeration
        if (user == null || user.UserTypeId != 5 || !user.Enabled)
            return Unit.Value;

        var resetToken = Guid.NewGuid().ToString("N");
        await _cacheService.SetAsync($"pwd-reset:{resetToken}", user.Id, TimeSpan.FromHours(1), cancellationToken);

        var baseUrl = _configuration["App:BaseUrl"] ?? "https://app.viajesaltairis.com";
        var resetLink = $"{baseUrl}/reset-password?token={resetToken}";
        await _emailService.SendEmailAsync(
            user.Email,
            "Password Reset Request",
            $"Click the following link to reset your password: {resetLink}",
            cancellationToken);

        return Unit.Value;
    }
}
