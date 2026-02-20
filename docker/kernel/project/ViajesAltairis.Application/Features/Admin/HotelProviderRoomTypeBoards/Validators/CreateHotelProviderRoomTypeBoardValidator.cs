using FluentValidation;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeBoards.Commands;

namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeBoards.Validators;

public class CreateHotelProviderRoomTypeBoardValidator : AbstractValidator<CreateHotelProviderRoomTypeBoardCommand>
{
    public CreateHotelProviderRoomTypeBoardValidator()
    {
        RuleFor(x => x.HotelProviderRoomTypeId).GreaterThan(0);
        RuleFor(x => x.BoardTypeId).GreaterThan(0);
        RuleFor(x => x.PricePerNight).GreaterThanOrEqualTo(0);
    }
}
