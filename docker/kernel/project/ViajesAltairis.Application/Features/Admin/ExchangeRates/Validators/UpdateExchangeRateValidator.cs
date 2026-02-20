using FluentValidation;
using ViajesAltairis.Application.Features.Admin.ExchangeRates.Commands;

namespace ViajesAltairis.Application.Features.Admin.ExchangeRates.Validators;

public class UpdateExchangeRateValidator : AbstractValidator<UpdateExchangeRateCommand>
{
    public UpdateExchangeRateValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CurrencyId).GreaterThan(0);
        RuleFor(x => x.RateToEur).GreaterThan(0);
    }
}
