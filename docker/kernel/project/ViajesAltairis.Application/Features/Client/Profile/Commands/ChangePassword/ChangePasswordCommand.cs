using MediatR;

namespace ViajesAltairis.Application.Features.Client.Profile.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest<Unit>
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
