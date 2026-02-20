using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Reservations.Queries;

public record GetReservationLineInfoQuery(long LineId) : IRequest<ReservationLineInfoResult?>;

public class GetReservationLineInfoHandler : IRequestHandler<GetReservationLineInfoQuery, ReservationLineInfoResult?>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetReservationLineInfoHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ReservationLineInfoResult?> Handle(GetReservationLineInfoQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var result = await Dapper.SqlMapper.QuerySingleOrDefaultAsync<dynamic>(
            connection,
            """
            SELECT rl.id AS reservation_line_id, rl.reservation_id,
                   hp.hotel_id, r.booked_by_user_id AS user_id
            FROM reservation_line rl
            JOIN reservation r ON r.id = rl.reservation_id
            JOIN hotel_provider_room_type hprt ON hprt.id = rl.hotel_provider_room_type_id
            JOIN hotel_provider hp ON hp.id = hprt.hotel_provider_id
            WHERE rl.id = @LineId
            """,
            new { request.LineId });

        if (result is null) return null;

        return new ReservationLineInfoResult(
            (long)result.reservation_line_id,
            (long)result.reservation_id,
            (long)result.hotel_id,
            (long)result.user_id);
    }
}
