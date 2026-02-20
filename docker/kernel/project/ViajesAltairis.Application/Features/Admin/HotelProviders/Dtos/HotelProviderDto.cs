namespace ViajesAltairis.Application.Features.Admin.HotelProviders.Dtos;

public record HotelProviderDto(long Id, long HotelId, long ProviderId, bool Enabled, DateTime CreatedAt);
