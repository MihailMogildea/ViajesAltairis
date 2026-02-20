using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.ExternalClient.Hotels.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.ExternalClient.Hotels.Queries;

public record GetAvailabilityQuery(long HotelId, DateOnly CheckIn, DateOnly CheckOut) : IRequest<GetAvailabilityResponse>;

public record GetAvailabilityResponse(long HotelId, List<RoomAvailabilityDto> Rooms);

public class GetAvailabilityHandler : IRequestHandler<GetAvailabilityQuery, GetAvailabilityResponse>
{
    private readonly IDbConnectionFactory _db;

    public GetAvailabilityHandler(IDbConnectionFactory db) => _db = db;

    public async Task<GetAvailabilityResponse> Handle(GetAvailabilityQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();

        const string sql = @"
            SELECT
                hprt.id AS HotelProviderRoomTypeId,
                rt.name AS RoomTypeName,
                p.name AS ProviderName,
                hprt.quantity AS TotalRooms,
                hprt.price_per_night AS PricePerNight,
                cur.iso_code AS CurrencyCode,
                hprt.quantity - COALESCE(booked.rooms_booked, 0) AS AvailableRooms
            FROM hotel_provider_room_type hprt
            JOIN hotel_provider hp ON hp.id = hprt.hotel_provider_id
            JOIN hotel h ON h.id = hp.hotel_id
            JOIN provider p ON p.id = hp.provider_id
            JOIN room_type rt ON rt.id = hprt.room_type_id
            JOIN currency cur ON cur.id = hprt.currency_id
            LEFT JOIN (
                SELECT rl.hotel_provider_room_type_id,
                       SUM(rl.num_rooms) AS rooms_booked
                FROM reservation_line rl
                JOIN reservation r ON r.id = rl.reservation_id
                WHERE r.status_id BETWEEN 1 AND 4
                  AND rl.check_in_date < @CheckOut
                  AND rl.check_out_date > @CheckIn
                GROUP BY rl.hotel_provider_room_type_id
            ) booked ON booked.hotel_provider_room_type_id = hprt.id
            WHERE h.id = @HotelId AND hprt.enabled = TRUE AND h.enabled = TRUE
            HAVING AvailableRooms > 0
            ORDER BY rt.name, p.name";

        var rooms = (await connection.QueryAsync<RoomAvailabilityDto>(sql, new
        {
            request.HotelId,
            CheckIn = request.CheckIn.ToString("yyyy-MM-dd"),
            CheckOut = request.CheckOut.ToString("yyyy-MM-dd")
        })).ToList();

        return new GetAvailabilityResponse(request.HotelId, rooms);
    }
}
