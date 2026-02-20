namespace ViajesAltairis.Application.Features.Admin.AdministrativeDivisions.Dtos;

public record AdministrativeDivisionDto(long Id, long CountryId, long? ParentId, string Name, long TypeId, byte Level, bool Enabled, DateTime CreatedAt);
