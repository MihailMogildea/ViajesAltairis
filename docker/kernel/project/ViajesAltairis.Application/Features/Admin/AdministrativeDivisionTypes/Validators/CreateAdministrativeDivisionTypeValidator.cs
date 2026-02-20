using FluentValidation;
using ViajesAltairis.Application.Features.Admin.AdministrativeDivisionTypes.Commands;

namespace ViajesAltairis.Application.Features.Admin.AdministrativeDivisionTypes.Validators;

public class CreateAdministrativeDivisionTypeValidator : AbstractValidator<CreateAdministrativeDivisionTypeCommand>
{
    public CreateAdministrativeDivisionTypeValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
    }
}
