namespace ViajesAltairis.Domain.Entities;

public class UserType : BaseEntity
{
    public string Name { get; set; } = null!;

    public ICollection<User> Users { get; set; } = [];
}
