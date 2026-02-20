namespace ViajesAltairis.Application.Features.Admin.CancellationPolicies.Dtos;

public record CancellationPolicyDto(long Id, long HotelId, int FreeCancellationHours, decimal PenaltyPercentage, bool Enabled, DateTime CreatedAt);
