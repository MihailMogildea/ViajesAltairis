namespace ViajesAltairis.Application.Features.Admin.HotelBlackouts.Dtos;

public record UpdateHotelBlackoutRequest(long HotelId, DateOnly StartDate, DateOnly EndDate, string? Reason);
