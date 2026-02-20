using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelBlackouts.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelBlackouts.Queries;

public record GetHotelBlackoutByIdQuery(long Id) : IRequest<HotelBlackoutDto?>;

public class GetHotelBlackoutByIdHandler : IRequestHandler<GetHotelBlackoutByIdQuery, HotelBlackoutDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetHotelBlackoutByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<HotelBlackoutDto?> Handle(GetHotelBlackoutByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<HotelBlackoutDto>(
            "SELECT id AS Id, hotel_id AS HotelId, start_date AS StartDate, end_date AS EndDate, reason AS Reason, created_at AS CreatedAt FROM hotel_blackout WHERE id = @Id",
            new { request.Id });
    }
}
