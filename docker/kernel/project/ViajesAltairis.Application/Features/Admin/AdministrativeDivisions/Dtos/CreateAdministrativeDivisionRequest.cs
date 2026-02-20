namespace ViajesAltairis.Application.Features.Admin.AdministrativeDivisions.Dtos;

public record CreateAdministrativeDivisionRequest(long CountryId, long? ParentId, string Name, long TypeId, byte Level);
