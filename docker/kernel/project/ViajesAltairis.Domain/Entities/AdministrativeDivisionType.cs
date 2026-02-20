namespace ViajesAltairis.Domain.Entities;

public class AdministrativeDivisionType : BaseEntity
{
    public string Name { get; set; } = null!;

    public ICollection<AdministrativeDivision> AdministrativeDivisions { get; set; } = [];
}
