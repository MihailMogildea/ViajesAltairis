using FluentValidation;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeBoards.Commands;

namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeBoards.Validators;

public class UpdateHotelProviderRoomTypeBoardValidator : AbstractValidator<UpdateHotelProviderRoomTypeBoardCommand>
{
    public UpdateHotelProviderRoomTypeBoardValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.HotelProviderRoomTypeId).GreaterThan(0);
        RuleFor(x => x.BoardTypeId).GreaterThan(0);
        RuleFor(x => x.PricePerNight).GreaterThanOrEqualTo(0);
    }
}
