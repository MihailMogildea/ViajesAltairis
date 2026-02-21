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
                p.margin AS ProviderMargin,
                h.margin AS HotelMargin,
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

        var rows = (await connection.QueryAsync<AvailabilityRow>(sql, new
        {
            request.HotelId,
            CheckIn = request.CheckIn.ToString("yyyy-MM-dd"),
            CheckOut = request.CheckOut.ToString("yyyy-MM-dd")
        })).ToList();

        // Seasonal margin lookup
        var adminDivId = await connection.ExecuteScalarAsync<long?>(
            "SELECT c.administrative_division_id FROM hotel h JOIN city c ON c.id = h.city_id WHERE h.id = @HotelId",
            new { request.HotelId });

        decimal seasonalMarginPct = 0;
        if (adminDivId.HasValue)
        {
            var checkInMmDd = request.CheckIn.ToString("MM-dd");
            var checkOutMmDd = request.CheckOut.ToString("MM-dd");
            seasonalMarginPct = await connection.ExecuteScalarAsync<decimal?>(
                @"SELECT sm.margin FROM seasonal_margin sm
                  WHERE sm.administrative_division_id = @AdminDivId
                    AND (CASE WHEN sm.start_month_day <= sm.end_month_day THEN
                           @CheckInMmDd <= sm.end_month_day AND @CheckOutMmDd >= sm.start_month_day
                         ELSE
                           @CheckInMmDd >= sm.start_month_day OR @CheckOutMmDd <= sm.end_month_day
                         END)
                  ORDER BY sm.margin DESC LIMIT 1",
                new { AdminDivId = adminDivId.Value, CheckInMmDd = checkInMmDd, CheckOutMmDd = checkOutMmDd }) ?? 0;
        }

        // Apply provider + hotel + seasonal margins
        var rooms = rows.Select(r =>
        {
            var marginFactor = 1 + (r.ProviderMargin + r.HotelMargin + seasonalMarginPct) / 100m;
            return new RoomAvailabilityDto
            {
                HotelProviderRoomTypeId = r.HotelProviderRoomTypeId,
                RoomTypeName = r.RoomTypeName,
                ProviderName = r.ProviderName,
                TotalRooms = r.TotalRooms,
                PricePerNight = Math.Round(r.PricePerNight * marginFactor, 2),
                CurrencyCode = r.CurrencyCode,
                AvailableRooms = r.AvailableRooms,
            };
        }).ToList();

        return new GetAvailabilityResponse(request.HotelId, rooms);
    }

    private class AvailabilityRow
    {
        public long HotelProviderRoomTypeId { get; set; }
        public string RoomTypeName { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public int TotalRooms { get; set; }
        public decimal PricePerNight { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public decimal ProviderMargin { get; set; }
        public decimal HotelMargin { get; set; }
        public int AvailableRooms { get; set; }
    }
}
