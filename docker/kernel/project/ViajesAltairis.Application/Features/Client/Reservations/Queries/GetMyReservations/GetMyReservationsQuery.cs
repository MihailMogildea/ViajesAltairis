using MediatR;

namespace ViajesAltairis.Application.Features.Client.Reservations.Queries.GetMyReservations;

public class GetMyReservationsQuery : IRequest<GetMyReservationsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Status { get; set; }
}

public class GetMyReservationsResponse
{
    public List<ReservationSummaryDto> Reservations { get; set; } = new();
    public int TotalCount { get; set; }
}

public class ReservationSummaryDto
{
    public long Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int LineCount { get; set; }
}
