namespace ViajesAltairis.Application.Features.Admin.Reviews.Dtos;

public record ReviewDto(
    long Id,
    long ReservationId,
    long UserId,
    long HotelId,
    byte Rating,
    string? Title,
    string? Comment,
    bool Visible,
    DateTime CreatedAt,
    DateTime UpdatedAt);
