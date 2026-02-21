namespace ViajesAltairis.Application.Features.Admin.ReviewResponses.Dtos;

public record CreateReviewResponseRequest(long ReviewId, long UserId, string Comment);
