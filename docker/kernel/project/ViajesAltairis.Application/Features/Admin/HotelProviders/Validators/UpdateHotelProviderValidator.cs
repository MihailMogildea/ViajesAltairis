using FluentValidation;
using ViajesAltairis.Application.Features.Admin.HotelProviders.Commands;

namespace ViajesAltairis.Application.Features.Admin.HotelProviders.Validators;

public class UpdateHotelProviderValidator : AbstractValidator<UpdateHotelProviderCommand>
{
    public UpdateHotelProviderValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.HotelId).GreaterThan(0);
        RuleFor(x => x.ProviderId).GreaterThan(0);
    }
}
