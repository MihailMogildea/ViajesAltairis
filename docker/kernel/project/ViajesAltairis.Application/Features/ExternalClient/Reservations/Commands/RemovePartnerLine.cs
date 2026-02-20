using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.ExternalClient.Reservations.Commands;

public record RemovePartnerLineCommand(long ReservationId, long LineId) : IRequest
{
    public long BusinessPartnerId { get; init; }
}

public class RemovePartnerLineHandler : IRequestHandler<RemovePartnerLineCommand>
{
    private readonly IDbConnectionFactory _db;
    private readonly IReservationApiClient _reservationApi;

    public RemovePartnerLineHandler(IDbConnectionFactory db, IReservationApiClient reservationApi)
    {
        _db = db;
        _reservationApi = reservationApi;
    }

    public async Task Handle(RemovePartnerLineCommand request, CancellationToken cancellationToken)
    {
        await VerifyOwnership(request.ReservationId, request.BusinessPartnerId);

        await _reservationApi.RemoveLineAsync(
            request.ReservationId, request.LineId, cancellationToken);
    }

    private async Task VerifyOwnership(long reservationId, long businessPartnerId)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SELECT COUNT(*) FROM reservation r
            JOIN user u ON u.id = r.booked_by_user_id
            WHERE r.id = @ReservationId AND u.business_partner_id = @BusinessPartnerId";

        var count = await connection.ExecuteScalarAsync<int>(sql, new { ReservationId = reservationId, BusinessPartnerId = businessPartnerId });
        if (count == 0)
            throw new InvalidOperationException("Reservation not found or does not belong to your organization.");
    }
}
