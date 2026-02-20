namespace ViajesAltairis.Application.Features.Admin.Statistics.Dtos;

public class CancellationStatsDto
{
    public int CancellationCount { get; init; }
    public int TotalReservations { get; init; }
    public decimal CancellationRate { get; init; }
    public decimal TotalPenalty { get; init; }
    public decimal TotalRefund { get; init; }
}
