using FluentValidation;
using ViajesAltairis.Application.Features.Admin.PromoCodes.Commands;

namespace ViajesAltairis.Application.Features.Admin.PromoCodes.Validators;

public class CreatePromoCodeValidator : AbstractValidator<CreatePromoCodeCommand>
{
    public CreatePromoCodeValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ValidTo).GreaterThanOrEqualTo(x => x.ValidFrom);
    }
}
