namespace ViajesAltairis.Domain.Entities;

public class User : BaseEntity
{
    public long UserTypeId { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Phone { get; set; }
    public string? TaxId { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public long? LanguageId { get; set; }
    public long? BusinessPartnerId { get; set; }
    public long? ProviderId { get; set; }
    public decimal Discount { get; set; }
    public bool Enabled { get; set; }

    public UserType UserType { get; set; } = null!;
    public Language? Language { get; set; }
    public BusinessPartner? BusinessPartner { get; set; }
    public Provider? Provider { get; set; }
    public ICollection<UserHotel> UserHotels { get; set; } = [];
    public ICollection<UserSubscription> UserSubscriptions { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
}
