using FluentValidation;
using ViajesAltairis.Application.Features.Admin.BoardTypes.Commands;

namespace ViajesAltairis.Application.Features.Admin.BoardTypes.Validators;

public class CreateBoardTypeValidator : AbstractValidator<CreateBoardTypeCommand>
{
    public CreateBoardTypeValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
    }
}
