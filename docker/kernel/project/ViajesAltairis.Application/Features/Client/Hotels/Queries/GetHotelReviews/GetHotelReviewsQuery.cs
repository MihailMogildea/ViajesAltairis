using MediatR;

namespace ViajesAltairis.Application.Features.Client.Hotels.Queries.GetHotelReviews;

public class GetHotelReviewsQuery : IRequest<GetHotelReviewsResponse>
{
    public long HotelId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetHotelReviewsResponse
{
    public List<ReviewDto> Reviews { get; set; } = new();
    public int TotalCount { get; set; }
    public decimal AverageRating { get; set; }
}

public class ReviewDto
{
    public long Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Title { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ResponseComment { get; set; }
    public DateTime? ResponseDate { get; set; }
}
