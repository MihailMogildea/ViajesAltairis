namespace ViajesAltairis.Application.Features.Admin.Cities.Dtos;

public record CityDto(long Id, long AdministrativeDivisionId, string Name, bool Enabled, DateTime CreatedAt);
