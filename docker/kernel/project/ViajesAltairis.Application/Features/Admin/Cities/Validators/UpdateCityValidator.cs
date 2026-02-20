using FluentValidation;
using ViajesAltairis.Application.Features.Admin.Cities.Commands;

namespace ViajesAltairis.Application.Features.Admin.Cities.Validators;

public class UpdateCityValidator : AbstractValidator<UpdateCityCommand>
{
    public UpdateCityValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.AdministrativeDivisionId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
    }
}
