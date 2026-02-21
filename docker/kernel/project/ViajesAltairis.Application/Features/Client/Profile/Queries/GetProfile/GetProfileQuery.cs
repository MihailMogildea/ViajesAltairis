using MediatR;

namespace ViajesAltairis.Application.Features.Client.Profile.Queries.GetProfile;

public class GetProfileQuery : IRequest<GetProfileResponse>
{
}

public class GetProfileResponse
{
    public long Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Country { get; set; }
    public string PreferredLanguage { get; set; } = string.Empty;
    public string PreferredCurrency { get; set; } = string.Empty;
    public decimal Discount { get; set; }
    public string? SubscriptionType { get; set; }
    public decimal? SubscriptionDiscount { get; set; }
    public DateTime CreatedAt { get; set; }
}
