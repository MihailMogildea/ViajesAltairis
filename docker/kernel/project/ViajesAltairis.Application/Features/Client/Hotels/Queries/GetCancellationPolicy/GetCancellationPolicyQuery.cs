using MediatR;

namespace ViajesAltairis.Application.Features.Client.Hotels.Queries.GetCancellationPolicy;

public class GetCancellationPolicyQuery : IRequest<GetCancellationPolicyResponse>
{
    public long HotelId { get; set; }
}

public class GetCancellationPolicyResponse
{
    public List<CancellationPolicyDto> Policies { get; set; } = new();
}

public class CancellationPolicyDto
{
    public int HoursBeforeCheckIn { get; set; }
    public decimal PenaltyPercentage { get; set; }
}
