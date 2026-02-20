namespace ViajesAltairis.Application.Features.ExternalClient.Auth.Dtos;

public record LoginResult(long UserId, string Email, long BusinessPartnerId, string PartnerName);
