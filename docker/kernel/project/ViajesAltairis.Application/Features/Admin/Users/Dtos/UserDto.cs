namespace ViajesAltairis.Application.Features.Admin.Users.Dtos;

public class UserDto
{
    public long Id { get; init; }
    public long UserTypeId { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? Phone { get; init; }
    public string? TaxId { get; init; }
    public string? Address { get; init; }
    public string? City { get; init; }
    public string? PostalCode { get; init; }
    public string? Country { get; init; }
    public long? LanguageId { get; init; }
    public long? BusinessPartnerId { get; init; }
    public long? ProviderId { get; init; }
    public decimal Discount { get; init; }
    public bool Enabled { get; init; }
    public DateTime CreatedAt { get; init; }
}
