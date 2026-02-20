using FluentValidation;
using ViajesAltairis.Application.Features.Admin.AmenityCategories.Commands;

namespace ViajesAltairis.Application.Features.Admin.AmenityCategories.Validators;

public class UpdateAmenityCategoryValidator : AbstractValidator<UpdateAmenityCategoryCommand>
{
    public UpdateAmenityCategoryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
