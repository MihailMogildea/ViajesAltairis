namespace ViajesAltairis.Application.Features.Admin.Users.Dtos;

public record UserDto(long Id, long UserTypeId, string Email, string FirstName, string LastName, string? Phone, string? TaxId, string? Address, string? City, string? PostalCode, string? Country, long? LanguageId, long? BusinessPartnerId, long? ProviderId, decimal Discount, bool Enabled, DateTime CreatedAt);
