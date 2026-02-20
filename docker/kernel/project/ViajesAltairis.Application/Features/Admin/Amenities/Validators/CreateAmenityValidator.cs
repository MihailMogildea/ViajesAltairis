using FluentValidation;
using ViajesAltairis.Application.Features.Admin.Amenities.Commands;

namespace ViajesAltairis.Application.Features.Admin.Amenities.Validators;

public class CreateAmenityValidator : AbstractValidator<CreateAmenityCommand>
{
    public CreateAmenityValidator()
    {
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
