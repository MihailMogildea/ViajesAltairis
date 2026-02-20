using FluentValidation;
using ViajesAltairis.Application.Features.Admin.Users.Commands;

namespace ViajesAltairis.Application.Features.Admin.Users.Validators;

public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.UserTypeId).GreaterThan(0);
        RuleFor(x => x.Email).NotEmpty().MaximumLength(200);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
    }
}
