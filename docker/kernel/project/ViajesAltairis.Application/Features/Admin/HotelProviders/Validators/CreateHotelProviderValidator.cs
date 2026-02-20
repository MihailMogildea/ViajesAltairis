using FluentValidation;
using ViajesAltairis.Application.Features.Admin.HotelProviders.Commands;

namespace ViajesAltairis.Application.Features.Admin.HotelProviders.Validators;

public class CreateHotelProviderValidator : AbstractValidator<CreateHotelProviderCommand>
{
    public CreateHotelProviderValidator()
    {
        RuleFor(x => x.HotelId).GreaterThan(0);
        RuleFor(x => x.ProviderId).GreaterThan(0);
    }
}
