using MediatR;

namespace ViajesAltairis.Application.Features.Client.Reviews.Commands.SubmitReview;

public class SubmitReviewCommand : IRequest<long>
{
    public long ReservationLineId { get; set; }
    public int Rating { get; set; }
    public string? Title { get; set; }
    public string? Comment { get; set; }
}
