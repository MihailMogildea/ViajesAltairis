using FluentValidation;
using ViajesAltairis.Application.Features.Admin.Hotels.Commands;

namespace ViajesAltairis.Application.Features.Admin.Hotels.Validators;

public class UpdateHotelValidator : AbstractValidator<UpdateHotelCommand>
{
    public UpdateHotelValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CityId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Stars).InclusiveBetween((byte)1, (byte)5);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Email).MaximumLength(150);
        RuleFor(x => x.Phone).MaximumLength(50);
        RuleFor(x => x.CheckInTime).NotEmpty();
        RuleFor(x => x.CheckOutTime).NotEmpty();
        RuleFor(x => x.Margin).GreaterThanOrEqualTo(0);
    }
}
