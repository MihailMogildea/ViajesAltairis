using FluentValidation;
using ViajesAltairis.Application.Features.Admin.SeasonalMargins.Commands;

namespace ViajesAltairis.Application.Features.Admin.SeasonalMargins.Validators;

public class UpdateSeasonalMarginValidator : AbstractValidator<UpdateSeasonalMarginCommand>
{
    public UpdateSeasonalMarginValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.AdministrativeDivisionId).GreaterThan(0);
        RuleFor(x => x.StartMonthDay).NotEmpty().Length(5);
        RuleFor(x => x.EndMonthDay).NotEmpty().Length(5);
        RuleFor(x => x.Margin).GreaterThanOrEqualTo(0);
    }
}
