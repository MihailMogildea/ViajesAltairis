using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Reservations.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Reservations.Queries;

public record GetReservationLinesQuery(long ReservationId) : IRequest<IEnumerable<ReservationLineAdminDto>>;

public class GetReservationLinesHandler : IRequestHandler<GetReservationLinesQuery, IEnumerable<ReservationLineAdminDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetReservationLinesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<ReservationLineAdminDto>> Handle(GetReservationLinesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<ReservationLineAdminDto>(
            @"SELECT
                reservation_line_id AS ReservationLineId, reservation_id AS ReservationId,
                hotel_id AS HotelId, hotel_name AS HotelName,
                room_type_id AS RoomTypeId, room_type_name AS RoomTypeName,
                board_type_id AS BoardTypeId, board_type_name AS BoardTypeName,
                provider_id AS ProviderId, provider_name AS ProviderName,
                check_in_date AS CheckInDate, check_out_date AS CheckOutDate,
                num_rooms AS NumRooms, num_guests AS NumGuests,
                price_per_night AS PricePerNight, board_price_per_night AS BoardPricePerNight,
                num_nights AS NumNights, subtotal AS Subtotal, tax_amount AS TaxAmount,
                margin_amount AS MarginAmount, discount_amount AS DiscountAmount,
                total_price AS TotalPrice, currency_code AS CurrencyCode
              FROM v_reservation_line_detail
              WHERE reservation_id = @ReservationId
              ORDER BY reservation_line_id",
            new { request.ReservationId });
    }
}
