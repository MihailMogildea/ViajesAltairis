using Microsoft.Extensions.Logging;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual email sending (SMTP, SendGrid, etc.)
        _logger.LogInformation("Email sent to {To}: {Subject}", to, subject);
        return Task.CompletedTask;
    }
}
