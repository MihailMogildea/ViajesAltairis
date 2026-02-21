namespace ViajesAltairis.Application.Interfaces;

public interface ICurrentUserService
{
    long? UserId { get; }
    string? Email { get; }
    string? UserType { get; }
    long? BusinessPartnerId { get; }
    long LanguageId { get; }
    string CurrencyCode { get; }
}
