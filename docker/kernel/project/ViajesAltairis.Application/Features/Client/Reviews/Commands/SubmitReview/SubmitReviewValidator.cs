using FluentValidation;

namespace ViajesAltairis.Application.Features.Client.Reviews.Commands.SubmitReview;

public class SubmitReviewValidator : AbstractValidator<SubmitReviewCommand>
{
    public SubmitReviewValidator()
    {
        RuleFor(x => x.ReservationLineId).GreaterThan(0);
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Title).MaximumLength(200).When(x => x.Title != null);
        RuleFor(x => x.Comment).MaximumLength(2000).When(x => x.Comment != null);
    }
}
