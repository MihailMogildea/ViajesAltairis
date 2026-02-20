using MediatR;

namespace ViajesAltairis.Application.Features.Client.Auth.Commands.Register;

public class RegisterCommand : IRequest<RegisterResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public long? CountryId { get; set; }
    public long PreferredLanguageId { get; set; }
    public long PreferredCurrencyId { get; set; }
}

public class RegisterResponse
{
    public long UserId { get; set; }
    public string Token { get; set; } = string.Empty;
}
