namespace ViajesAltairis.Application.Features.Admin.AdministrativeDivisions.Dtos;

public class AdministrativeDivisionDto
{
    public long Id { get; init; }
    public long CountryId { get; init; }
    public long? ParentId { get; init; }
    public string Name { get; init; } = null!;
    public long TypeId { get; init; }
    public int Level { get; init; }
    public bool Enabled { get; init; }
    public DateTime CreatedAt { get; init; }
}
