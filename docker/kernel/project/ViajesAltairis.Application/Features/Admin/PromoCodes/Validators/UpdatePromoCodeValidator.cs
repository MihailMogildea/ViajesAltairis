using FluentValidation;
using ViajesAltairis.Application.Features.Admin.PromoCodes.Commands;

namespace ViajesAltairis.Application.Features.Admin.PromoCodes.Validators;

public class UpdatePromoCodeValidator : AbstractValidator<UpdatePromoCodeCommand>
{
    public UpdatePromoCodeValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ValidTo).GreaterThanOrEqualTo(x => x.ValidFrom);
    }
}
