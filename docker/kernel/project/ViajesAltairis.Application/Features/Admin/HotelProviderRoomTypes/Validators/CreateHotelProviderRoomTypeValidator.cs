using FluentValidation;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypes.Commands;

namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypes.Validators;

public class CreateHotelProviderRoomTypeValidator : AbstractValidator<CreateHotelProviderRoomTypeCommand>
{
    public CreateHotelProviderRoomTypeValidator()
    {
        RuleFor(x => x.HotelProviderId).GreaterThan(0);
        RuleFor(x => x.RoomTypeId).GreaterThan(0);
        RuleFor(x => x.Capacity).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.PricePerNight).GreaterThan(0);
        RuleFor(x => x.CurrencyId).GreaterThan(0);
        RuleFor(x => x.ExchangeRateId).GreaterThan(0);
    }
}
