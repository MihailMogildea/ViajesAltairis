using FluentValidation;
using ViajesAltairis.Application.Features.Admin.PaymentMethods.Commands;

namespace ViajesAltairis.Application.Features.Admin.PaymentMethods.Validators;

public class UpdatePaymentMethodValidator : AbstractValidator<UpdatePaymentMethodCommand>
{
    public UpdatePaymentMethodValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MinDaysBeforeCheckin).GreaterThanOrEqualTo(0);
    }
}
