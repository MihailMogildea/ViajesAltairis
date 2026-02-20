using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Statistics.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Statistics.Queries;

public record GetBookingAverageQuery(DateTime? From, DateTime? To) : IRequest<BookingAverageDto>;

public class GetBookingAverageHandler : IRequestHandler<GetBookingAverageQuery, BookingAverageDto>
{
    private readonly IDbConnectionFactory _db;
    public GetBookingAverageHandler(IDbConnectionFactory db) => _db = db;

    public async Task<BookingAverageDto> Handle(GetBookingAverageQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleAsync<BookingAverageDto>(
            """
            SELECT COALESCE(AVG(r.total_price), 0) AS AverageValue,
                   COALESCE(AVG(rl.num_nights), 0) AS AverageNights,
                   COUNT(DISTINCT r.id) AS TotalBookings
            FROM reservation r
            LEFT JOIN reservation_line rl ON rl.reservation_id = r.id
            WHERE (@From IS NULL OR r.created_at >= @From)
              AND (@To IS NULL OR r.created_at <= @To)
            """, new { request.From, request.To });
    }
}
