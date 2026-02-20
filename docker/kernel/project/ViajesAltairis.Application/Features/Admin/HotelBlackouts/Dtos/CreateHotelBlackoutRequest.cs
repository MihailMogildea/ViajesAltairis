namespace ViajesAltairis.Application.Features.Admin.HotelBlackouts.Dtos;

public record CreateHotelBlackoutRequest(long HotelId, DateOnly StartDate, DateOnly EndDate, string? Reason);
