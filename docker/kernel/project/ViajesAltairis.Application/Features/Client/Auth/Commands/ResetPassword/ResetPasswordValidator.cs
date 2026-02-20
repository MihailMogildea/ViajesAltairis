using FluentValidation;

namespace ViajesAltairis.Application.Features.Client.Auth.Commands.ResetPassword;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8);
    }
}
