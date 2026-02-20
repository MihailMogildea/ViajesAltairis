using FluentValidation;
using ViajesAltairis.Application.Features.Admin.HotelAmenities.Commands;

namespace ViajesAltairis.Application.Features.Admin.HotelAmenities.Validators;

public class AssignHotelAmenityValidator : AbstractValidator<AssignHotelAmenityCommand>
{
    public AssignHotelAmenityValidator()
    {
        RuleFor(x => x.HotelId).GreaterThan(0);
        RuleFor(x => x.AmenityId).GreaterThan(0);
    }
}
