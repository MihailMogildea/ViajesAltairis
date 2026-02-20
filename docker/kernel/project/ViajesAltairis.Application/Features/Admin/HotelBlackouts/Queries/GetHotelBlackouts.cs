using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelBlackouts.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelBlackouts.Queries;

public record GetHotelBlackoutsQuery : IRequest<IEnumerable<HotelBlackoutDto>>;

public class GetHotelBlackoutsHandler : IRequestHandler<GetHotelBlackoutsQuery, IEnumerable<HotelBlackoutDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetHotelBlackoutsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<HotelBlackoutDto>> Handle(GetHotelBlackoutsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<HotelBlackoutDto>(
            "SELECT id AS Id, hotel_id AS HotelId, start_date AS StartDate, end_date AS EndDate, reason AS Reason, created_at AS CreatedAt FROM hotel_blackout ORDER BY start_date");
    }
}
