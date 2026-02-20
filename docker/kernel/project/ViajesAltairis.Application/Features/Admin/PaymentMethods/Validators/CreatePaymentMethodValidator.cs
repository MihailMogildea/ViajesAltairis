using FluentValidation;
using ViajesAltairis.Application.Features.Admin.PaymentMethods.Commands;

namespace ViajesAltairis.Application.Features.Admin.PaymentMethods.Validators;

public class CreatePaymentMethodValidator : AbstractValidator<CreatePaymentMethodCommand>
{
    public CreatePaymentMethodValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MinDaysBeforeCheckin).GreaterThanOrEqualTo(0);
    }
}
