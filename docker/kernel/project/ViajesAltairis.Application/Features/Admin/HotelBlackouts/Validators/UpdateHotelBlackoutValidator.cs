using FluentValidation;
using ViajesAltairis.Application.Features.Admin.HotelBlackouts.Commands;

namespace ViajesAltairis.Application.Features.Admin.HotelBlackouts.Validators;

public class UpdateHotelBlackoutValidator : AbstractValidator<UpdateHotelBlackoutCommand>
{
    public UpdateHotelBlackoutValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.HotelId).GreaterThan(0);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate);
    }
}
