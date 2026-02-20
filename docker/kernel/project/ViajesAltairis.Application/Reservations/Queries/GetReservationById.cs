using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Reservations.Queries;

public record GetReservationByIdQuery(long ReservationId) : IRequest<ReservationDetailResult?>;

public class GetReservationByIdHandler : IRequestHandler<GetReservationByIdQuery, ReservationDetailResult?>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetReservationByIdHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ReservationDetailResult?> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var reservation = await Dapper.SqlMapper.QuerySingleOrDefaultAsync<dynamic>(
            connection,
            """
            SELECT r.id, r.booked_by_user_id, rs.name AS status, r.created_at, r.total_price, r.discount_amount,
                   c.iso_code AS currency_code, er.rate_to_eur AS exchange_rate, pc.code AS promo_code
            FROM reservation r
            JOIN reservation_status rs ON rs.id = r.status_id
            JOIN currency c ON c.id = r.currency_id
            JOIN exchange_rate er ON er.id = r.exchange_rate_id
            LEFT JOIN promo_code pc ON pc.id = r.promo_code_id
            WHERE r.id = @Id
            """,
            new { Id = request.ReservationId });

        if (reservation is null)
            return null;

        var lines = await Dapper.SqlMapper.QueryAsync<dynamic>(
            connection,
            """
            SELECT rl.id, h.name AS hotel_name, rt.name AS room_type, bt.name AS board_type,
                   rl.check_in_date AS check_in, rl.check_out_date AS check_out,
                   rl.num_guests AS guest_count, rl.total_price AS line_total
            FROM reservation_line rl
            JOIN hotel_provider_room_type hprt ON hprt.id = rl.hotel_provider_room_type_id
            JOIN hotel_provider hp ON hp.id = hprt.hotel_provider_id
            JOIN hotel h ON h.id = hp.hotel_id
            JOIN room_type rt ON rt.id = hprt.room_type_id
            JOIN board_type bt ON bt.id = rl.board_type_id
            WHERE rl.reservation_id = @ReservationId
            """,
            new { ReservationId = request.ReservationId });

        var lineResults = new List<ReservationLineResult>();
        foreach (var line in lines)
        {
            var guests = await Dapper.SqlMapper.QueryAsync<dynamic>(
                connection,
                "SELECT id, first_name, last_name FROM reservation_guest WHERE reservation_line_id = @LineId",
                new { LineId = (long)line.id });

            var guestResults = guests.Select(g => new ReservationGuestResult(
                (long)g.id, (string)g.first_name, (string)g.last_name, null)).ToList();

            lineResults.Add(new ReservationLineResult(
                (long)line.id,
                (string)line.hotel_name,
                (string)line.room_type,
                (string)line.board_type,
                ((DateOnly)line.check_in).ToDateTime(TimeOnly.MinValue),
                ((DateOnly)line.check_out).ToDateTime(TimeOnly.MinValue),
                (int)line.guest_count,
                (decimal)line.line_total,
                guestResults));
        }

        return new ReservationDetailResult(
            (long)reservation.id,
            (long)reservation.booked_by_user_id,
            (string)reservation.status,
            (DateTime)reservation.created_at,
            (decimal)reservation.total_price,
            (decimal)reservation.discount_amount,
            (string)reservation.currency_code,
            (decimal)reservation.exchange_rate,
            reservation.promo_code as string,
            lineResults);
    }
}
