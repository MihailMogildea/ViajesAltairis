using FluentValidation;
using ViajesAltairis.Application.Features.Admin.Reviews.Commands;

namespace ViajesAltairis.Application.Features.Admin.Reviews.Validators;

public class SetReviewVisibleValidator : AbstractValidator<SetReviewVisibleCommand>
{
    public SetReviewVisibleValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
