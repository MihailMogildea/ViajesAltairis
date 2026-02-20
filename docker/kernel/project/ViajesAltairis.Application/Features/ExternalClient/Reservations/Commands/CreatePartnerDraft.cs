using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.ExternalClient.Reservations.Commands;

public record CreatePartnerDraftCommand(
    string OwnerFirstName, string OwnerLastName, string OwnerEmail,
    string? OwnerPhone, string? OwnerTaxId,
    string CurrencyCode, string? PromoCode) : IRequest<long>
{
    public long BookedByUserId { get; init; }
}

public class CreatePartnerDraftHandler : IRequestHandler<CreatePartnerDraftCommand, long>
{
    private readonly IReservationApiClient _reservationApi;

    public CreatePartnerDraftHandler(IReservationApiClient reservationApi)
    {
        _reservationApi = reservationApi;
    }

    public async Task<long> Handle(CreatePartnerDraftCommand request, CancellationToken cancellationToken)
    {
        return await _reservationApi.CreateDraftAsync(
            request.BookedByUserId,
            request.CurrencyCode,
            request.PromoCode,
            ownerFirstName: request.OwnerFirstName,
            ownerLastName: request.OwnerLastName,
            ownerEmail: request.OwnerEmail,
            ownerPhone: request.OwnerPhone,
            ownerTaxId: request.OwnerTaxId,
            cancellationToken: cancellationToken);
    }
}
