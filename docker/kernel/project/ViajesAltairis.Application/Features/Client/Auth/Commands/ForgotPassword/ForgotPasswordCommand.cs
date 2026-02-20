using MediatR;

namespace ViajesAltairis.Application.Features.Client.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommand : IRequest<Unit>
{
    public string Email { get; set; } = string.Empty;
}
