namespace ViajesAltairis.Application.Features.Admin.ReviewResponses.Dtos;

public record ReviewResponseDto(long Id, long ReviewId, long UserId, string UserEmail, string Comment, DateTime CreatedAt);
