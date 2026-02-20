using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.ExternalClient.Reservations.Commands;

public record CancelPartnerReservationCommand(long ReservationId, string? Reason) : IRequest
{
    public long CancelledByUserId { get; init; }
    public long BusinessPartnerId { get; init; }
}

public class CancelPartnerReservationHandler : IRequestHandler<CancelPartnerReservationCommand>
{
    private readonly IDbConnectionFactory _db;
    private readonly IReservationApiClient _reservationApi;

    public CancelPartnerReservationHandler(IDbConnectionFactory db, IReservationApiClient reservationApi)
    {
        _db = db;
        _reservationApi = reservationApi;
    }

    public async Task Handle(CancelPartnerReservationCommand request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();

        const string checkSql = @"
            SELECT COUNT(*)
            FROM reservation r
            JOIN user u ON u.id = r.booked_by_user_id
            WHERE r.id = @ReservationId AND u.business_partner_id = @BusinessPartnerId";

        var exists = await connection.ExecuteScalarAsync<int>(checkSql, new
        {
            request.ReservationId,
            request.BusinessPartnerId
        });

        if (exists == 0)
            throw new InvalidOperationException("Reservation not found or does not belong to your organization.");

        await _reservationApi.CancelAsync(
            request.ReservationId, request.CancelledByUserId, request.Reason, cancellationToken);
    }
}
