using FluentValidation;
using ViajesAltairis.Application.Features.Admin.AdministrativeDivisionTypes.Commands;

namespace ViajesAltairis.Application.Features.Admin.AdministrativeDivisionTypes.Validators;

public class UpdateAdministrativeDivisionTypeValidator : AbstractValidator<UpdateAdministrativeDivisionTypeCommand>
{
    public UpdateAdministrativeDivisionTypeValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
    }
}
