using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Reservations.Commands.CreateDraftReservation;

public class CreateDraftReservationHandler : IRequestHandler<CreateDraftReservationCommand, long>
{
    private readonly IReservationApiClient _reservationApi;
    private readonly ICurrentUserService _currentUser;

    public CreateDraftReservationHandler(IReservationApiClient reservationApi, ICurrentUserService currentUser)
    {
        _reservationApi = reservationApi;
        _currentUser = currentUser;
    }

    public async Task<long> Handle(CreateDraftReservationCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;
        return await _reservationApi.CreateDraftAsync(
            userId,
            request.CurrencyCode,
            request.PromoCode,
            ownerUserId: userId,
            cancellationToken: cancellationToken);
    }
}
