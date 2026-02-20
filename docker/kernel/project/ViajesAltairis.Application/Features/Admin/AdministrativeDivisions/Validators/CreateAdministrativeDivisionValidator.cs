using FluentValidation;
using ViajesAltairis.Application.Features.Admin.AdministrativeDivisions.Commands;

namespace ViajesAltairis.Application.Features.Admin.AdministrativeDivisions.Validators;

public class CreateAdministrativeDivisionValidator : AbstractValidator<CreateAdministrativeDivisionCommand>
{
    public CreateAdministrativeDivisionValidator()
    {
        RuleFor(x => x.CountryId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.TypeId).GreaterThan(0);
    }
}
