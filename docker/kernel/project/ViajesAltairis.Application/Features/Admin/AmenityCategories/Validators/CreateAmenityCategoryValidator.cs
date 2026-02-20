using FluentValidation;
using ViajesAltairis.Application.Features.Admin.AmenityCategories.Commands;

namespace ViajesAltairis.Application.Features.Admin.AmenityCategories.Validators;

public class CreateAmenityCategoryValidator : AbstractValidator<CreateAmenityCategoryCommand>
{
    public CreateAmenityCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
