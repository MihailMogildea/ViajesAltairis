using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Reservations.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Reservations.Queries;

public record GetReservationGuestsQuery(long ReservationId) : IRequest<IEnumerable<ReservationGuestAdminDto>>;

public class GetReservationGuestsHandler : IRequestHandler<GetReservationGuestsQuery, IEnumerable<ReservationGuestAdminDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetReservationGuestsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<ReservationGuestAdminDto>> Handle(GetReservationGuestsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<ReservationGuestAdminDto>(
            @"SELECT
                guest_id AS GuestId, reservation_line_id AS ReservationLineId,
                first_name AS FirstName, last_name AS LastName,
                email AS Email, phone AS Phone,
                hotel_name AS HotelName, room_type_name AS RoomTypeName
              FROM v_reservation_guest_list
              WHERE reservation_id = @ReservationId
              ORDER BY reservation_line_id, guest_id",
            new { request.ReservationId });
    }
}
