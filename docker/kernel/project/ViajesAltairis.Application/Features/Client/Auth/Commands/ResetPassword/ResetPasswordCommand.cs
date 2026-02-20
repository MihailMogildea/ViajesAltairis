using MediatR;

namespace ViajesAltairis.Application.Features.Client.Auth.Commands.ResetPassword;

public class ResetPasswordCommand : IRequest<Unit>
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
