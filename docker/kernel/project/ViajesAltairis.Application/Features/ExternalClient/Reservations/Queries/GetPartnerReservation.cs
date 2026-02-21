using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.ExternalClient.Reservations.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.ExternalClient.Reservations.Queries;

public record GetPartnerReservationQuery(long ReservationId, long BusinessPartnerId) : IRequest<ReservationDetailDto?>;

public class GetPartnerReservationHandler : IRequestHandler<GetPartnerReservationQuery, ReservationDetailDto?>
{
    private readonly IDbConnectionFactory _db;

    public GetPartnerReservationHandler(IDbConnectionFactory db) => _db = db;

    public async Task<ReservationDetailDto?> Handle(GetPartnerReservationQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();

        const string headerSql = @"
            SELECT rs.reservation_id AS ReservationId, rs.reservation_code AS ReservationCode,
                   rs.status_name AS StatusName,
                   rs.owner_first_name AS OwnerFirstName, rs.owner_last_name AS OwnerLastName,
                   rs.owner_email AS OwnerEmail, rs.owner_phone AS OwnerPhone, rs.owner_tax_id AS OwnerTaxId,
                   rs.subtotal AS Subtotal, rs.tax_amount AS TaxAmount,
                   rs.discount_amount AS DiscountAmount,
                   rs.total_price AS TotalPrice, rs.currency_code AS CurrencyCode,
                   rs.promo_code AS PromoCode, rs.notes AS Notes, rs.created_at AS CreatedAt
            FROM v_reservation_summary rs
            JOIN user u ON u.id = rs.booked_by_user_id
            WHERE rs.reservation_id = @ReservationId AND u.business_partner_id = @BusinessPartnerId";

        var header = await connection.QuerySingleOrDefaultAsync<ReservationHeaderRow>(headerSql, new
        {
            request.ReservationId,
            request.BusinessPartnerId
        });

        if (header is null)
            return null;

        const string linesSql = @"
            SELECT reservation_line_id AS ReservationLineId,
                   hotel_name AS HotelName, room_type_name AS RoomTypeName,
                   board_type_name AS BoardTypeName,
                   check_in_date AS CheckInDate, check_out_date AS CheckOutDate,
                   num_rooms AS NumRooms, num_guests AS NumGuests,
                   price_per_night AS PricePerNight, board_price_per_night AS BoardPricePerNight,
                   num_nights AS NumNights, total_price AS TotalPrice,
                   currency_code AS CurrencyCode
            FROM v_reservation_line_detail
            WHERE reservation_id = @ReservationId";

        var lines = (await connection.QueryAsync<LineRow>(linesSql, new { request.ReservationId })).ToList();

        var lineIds = lines.Select(l => l.ReservationLineId).ToList();
        var guestsByLine = new Dictionary<long, List<GuestDto>>();

        if (lineIds.Count > 0)
        {
            const string guestsSql = @"
                SELECT guest_id AS GuestId, reservation_line_id AS ReservationLineId,
                       first_name AS FirstName, last_name AS LastName,
                       email AS Email, phone AS Phone
                FROM v_reservation_guest_list
                WHERE reservation_line_id IN @LineIds";

            var guests = await connection.QueryAsync<GuestRow>(guestsSql, new { LineIds = lineIds });
            guestsByLine = guests.GroupBy(g => g.ReservationLineId)
                                 .ToDictionary(g => g.Key, g => g.Select(x => new GuestDto(
                                     x.GuestId, x.FirstName, x.LastName, x.Email, x.Phone)).ToList());
        }

        var lineDtos = lines.Select(l => new ReservationLineDto(
            l.ReservationLineId, l.HotelName, l.RoomTypeName, l.BoardTypeName,
            l.CheckInDate, l.CheckOutDate, l.NumRooms, l.NumGuests,
            l.PricePerNight, l.BoardPricePerNight, l.NumNights, l.TotalPrice, l.CurrencyCode,
            guestsByLine.GetValueOrDefault(l.ReservationLineId, [])
        )).ToList();

        return new ReservationDetailDto(
            header.ReservationId, header.ReservationCode, header.StatusName,
            header.OwnerFirstName, header.OwnerLastName, header.OwnerEmail,
            header.OwnerPhone, header.OwnerTaxId,
            header.Subtotal, header.TaxAmount, header.DiscountAmount,
            header.TotalPrice, header.CurrencyCode, header.PromoCode, header.Notes,
            header.CreatedAt, lineDtos);
    }

    private class ReservationHeaderRow
    {
        public long ReservationId { get; set; }
        public string ReservationCode { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string OwnerFirstName { get; set; } = string.Empty;
        public string OwnerLastName { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;
        public string? OwnerPhone { get; set; }
        public string? OwnerTaxId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalPrice { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public string? PromoCode { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    private class LineRow
    {
        public long ReservationLineId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public string RoomTypeName { get; set; } = string.Empty;
        public string BoardTypeName { get; set; } = string.Empty;
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public int NumRooms { get; set; }
        public int NumGuests { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal BoardPricePerNight { get; set; }
        public int NumNights { get; set; }
        public decimal TotalPrice { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
    }

    private class GuestRow
    {
        public long GuestId { get; set; }
        public long ReservationLineId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
