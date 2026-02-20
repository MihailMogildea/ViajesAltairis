using FluentValidation;
using ViajesAltairis.Application.Features.Admin.ExchangeRates.Commands;

namespace ViajesAltairis.Application.Features.Admin.ExchangeRates.Validators;

public class CreateExchangeRateValidator : AbstractValidator<CreateExchangeRateCommand>
{
    public CreateExchangeRateValidator()
    {
        RuleFor(x => x.CurrencyId).GreaterThan(0);
        RuleFor(x => x.RateToEur).GreaterThan(0);
    }
}
