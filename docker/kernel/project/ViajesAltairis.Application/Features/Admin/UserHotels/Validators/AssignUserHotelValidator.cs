using FluentValidation;
using ViajesAltairis.Application.Features.Admin.UserHotels.Commands;

namespace ViajesAltairis.Application.Features.Admin.UserHotels.Validators;

public class AssignUserHotelValidator : AbstractValidator<AssignUserHotelCommand>
{
    public AssignUserHotelValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.HotelId).GreaterThan(0);
    }
}
