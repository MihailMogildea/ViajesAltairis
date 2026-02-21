using FluentValidation;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Reservations.Queries;

public record GetReservationsByUserQuery(long UserId, int Page, int PageSize, string? Status) : IRequest<ReservationListResult>;

public class GetReservationsByUserHandler : IRequestHandler<GetReservationsByUserQuery, ReservationListResult>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetReservationsByUserHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ReservationListResult> Handle(GetReservationsByUserQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        var statusFilter = string.IsNullOrWhiteSpace(request.Status) ? "" : "AND rs.name = @Status";
        var offset = (request.Page - 1) * request.PageSize;

        var countSql = $"""
            SELECT COUNT(*) FROM reservation r
            JOIN reservation_status rs ON rs.id = r.status_id
            WHERE (r.owner_user_id = @UserId OR r.booked_by_user_id = @UserId) {statusFilter}
            """;

        var totalCount = await Dapper.SqlMapper.ExecuteScalarAsync<int>(
            connection, countSql, new { request.UserId, request.Status });

        var querySql = $"""
            SELECT r.id, rs.name AS status, r.created_at, r.total_price,
                   c.iso_code AS currency_code,
                   (SELECT COUNT(*) FROM reservation_line rl WHERE rl.reservation_id = r.id) AS line_count,
                   (SELECT GROUP_CONCAT(DISTINCT h.name ORDER BY h.name SEPARATOR ', ')
                    FROM reservation_line rl
                    JOIN hotel_provider_room_type hprt ON hprt.id = rl.hotel_provider_room_type_id
                    JOIN hotel_provider hp ON hp.id = hprt.hotel_provider_id
                    JOIN hotel h ON h.id = hp.hotel_id
                    WHERE rl.reservation_id = r.id) AS hotel_names
            FROM reservation r
            JOIN reservation_status rs ON rs.id = r.status_id
            JOIN currency c ON c.id = r.currency_id
            WHERE (r.owner_user_id = @UserId OR r.booked_by_user_id = @UserId) {statusFilter}
            ORDER BY r.created_at DESC
            LIMIT @PageSize OFFSET @Offset
            """;

        var reservations = await Dapper.SqlMapper.QueryAsync<dynamic>(
            connection, querySql, new { request.UserId, request.Status, request.PageSize, Offset = offset });

        var summaries = reservations.Select(r => new ReservationSummaryResult(
            (long)r.id,
            (string)r.status,
            (DateTime)r.created_at,
            (decimal)r.total_price,
            (string)r.currency_code,
            Convert.ToInt32(r.line_count),
            (string?)r.hotel_names)).ToList();

        return new ReservationListResult(summaries, totalCount);
    }
}

public class GetReservationsByUserValidator : AbstractValidator<GetReservationsByUserQuery>
{
    public GetReservationsByUserValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
