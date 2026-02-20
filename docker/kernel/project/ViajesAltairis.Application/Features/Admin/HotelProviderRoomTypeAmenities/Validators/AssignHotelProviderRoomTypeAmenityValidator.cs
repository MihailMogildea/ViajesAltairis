using FluentValidation;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeAmenities.Commands;

namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeAmenities.Validators;

public class AssignHotelProviderRoomTypeAmenityValidator : AbstractValidator<AssignHotelProviderRoomTypeAmenityCommand>
{
    public AssignHotelProviderRoomTypeAmenityValidator()
    {
        RuleFor(x => x.HotelProviderRoomTypeId).GreaterThan(0);
        RuleFor(x => x.AmenityId).GreaterThan(0);
    }
}
