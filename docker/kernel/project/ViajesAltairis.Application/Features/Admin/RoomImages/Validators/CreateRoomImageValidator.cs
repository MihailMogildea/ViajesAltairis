using FluentValidation;
using ViajesAltairis.Application.Features.Admin.RoomImages.Commands;

namespace ViajesAltairis.Application.Features.Admin.RoomImages.Validators;

public class CreateRoomImageValidator : AbstractValidator<CreateRoomImageCommand>
{
    public CreateRoomImageValidator()
    {
        RuleFor(x => x.HotelProviderRoomTypeId).GreaterThan(0);
        RuleFor(x => x.Url).NotEmpty().MaximumLength(500);
        RuleFor(x => x.AltText).MaximumLength(200);
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
    }
}
