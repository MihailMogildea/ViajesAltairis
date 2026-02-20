namespace ViajesAltairis.Application.Features.Admin.HotelBlackouts.Dtos;

public record HotelBlackoutDto(long Id, long HotelId, DateOnly StartDate, DateOnly EndDate, string? Reason, DateTime CreatedAt);
