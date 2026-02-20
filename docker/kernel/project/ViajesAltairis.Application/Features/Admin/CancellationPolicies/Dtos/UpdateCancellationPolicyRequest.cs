namespace ViajesAltairis.Application.Features.Admin.CancellationPolicies.Dtos;

public record UpdateCancellationPolicyRequest(long HotelId, int FreeCancellationHours, decimal PenaltyPercentage);
