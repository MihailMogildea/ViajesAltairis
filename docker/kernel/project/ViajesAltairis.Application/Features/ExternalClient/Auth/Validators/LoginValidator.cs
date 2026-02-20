using FluentValidation;
using ViajesAltairis.Application.Features.ExternalClient.Auth.Commands;

namespace ViajesAltairis.Application.Features.ExternalClient.Auth.Validators;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
