using FluentValidation;
using ViajesAltairis.Application.Features.Admin.HotelImages.Commands;

namespace ViajesAltairis.Application.Features.Admin.HotelImages.Validators;

public class CreateHotelImageValidator : AbstractValidator<CreateHotelImageCommand>
{
    public CreateHotelImageValidator()
    {
        RuleFor(x => x.HotelId).GreaterThan(0);
        RuleFor(x => x.Url).NotEmpty().MaximumLength(500);
        RuleFor(x => x.AltText).MaximumLength(200);
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
    }
}
