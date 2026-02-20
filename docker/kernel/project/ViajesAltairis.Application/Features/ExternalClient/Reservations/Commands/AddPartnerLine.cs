using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.ExternalClient.Reservations.Commands;

public record AddPartnerLineCommand(
    long ReservationId, long RoomConfigurationId, long BoardTypeId,
    DateOnly CheckIn, DateOnly CheckOut, int GuestCount) : IRequest<long>
{
    public long BusinessPartnerId { get; init; }
}

public class AddPartnerLineHandler : IRequestHandler<AddPartnerLineCommand, long>
{
    private readonly IDbConnectionFactory _db;
    private readonly IReservationApiClient _reservationApi;

    public AddPartnerLineHandler(IDbConnectionFactory db, IReservationApiClient reservationApi)
    {
        _db = db;
        _reservationApi = reservationApi;
    }

    public async Task<long> Handle(AddPartnerLineCommand request, CancellationToken cancellationToken)
    {
        await VerifyOwnership(request.ReservationId, request.BusinessPartnerId);

        return await _reservationApi.AddLineAsync(
            request.ReservationId,
            request.RoomConfigurationId,
            request.BoardTypeId,
            request.CheckIn.ToDateTime(TimeOnly.MinValue),
            request.CheckOut.ToDateTime(TimeOnly.MinValue),
            request.GuestCount,
            cancellationToken);
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
