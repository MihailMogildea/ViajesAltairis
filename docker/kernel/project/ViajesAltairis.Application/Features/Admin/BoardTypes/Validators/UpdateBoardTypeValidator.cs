using FluentValidation;
using ViajesAltairis.Application.Features.Admin.BoardTypes.Commands;

namespace ViajesAltairis.Application.Features.Admin.BoardTypes.Validators;

public class UpdateBoardTypeValidator : AbstractValidator<UpdateBoardTypeCommand>
{
    public UpdateBoardTypeValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
    }
}
