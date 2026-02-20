using FluentValidation;
using ViajesAltairis.Application.Features.Admin.AdministrativeDivisions.Commands;

namespace ViajesAltairis.Application.Features.Admin.AdministrativeDivisions.Validators;

public class UpdateAdministrativeDivisionValidator : AbstractValidator<UpdateAdministrativeDivisionCommand>
{
    public UpdateAdministrativeDivisionValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CountryId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.TypeId).GreaterThan(0);
    }
}
