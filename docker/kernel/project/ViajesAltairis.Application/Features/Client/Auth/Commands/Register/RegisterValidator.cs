using FluentValidation;

namespace ViajesAltairis.Application.Features.Client.Auth.Commands.Register;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PreferredLanguageId).GreaterThan(0);
        RuleFor(x => x.PreferredCurrencyId).GreaterThan(0);
    }
}
