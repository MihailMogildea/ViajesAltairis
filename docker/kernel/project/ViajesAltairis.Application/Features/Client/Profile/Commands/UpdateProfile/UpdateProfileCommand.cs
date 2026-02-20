using MediatR;

namespace ViajesAltairis.Application.Features.Client.Profile.Commands.UpdateProfile;

public class UpdateProfileCommand : IRequest<UpdateProfileResponse>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public long? CountryId { get; set; }
    public long? PreferredLanguageId { get; set; }
    public long? PreferredCurrencyId { get; set; }
}

public class UpdateProfileResponse
{
    public long UserId { get; set; }
}
