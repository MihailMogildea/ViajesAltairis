namespace ViajesAltairis.Application.Features.Admin.CancellationPolicies.Dtos;

public record CreateCancellationPolicyRequest(long HotelId, int FreeCancellationHours, decimal PenaltyPercentage);
