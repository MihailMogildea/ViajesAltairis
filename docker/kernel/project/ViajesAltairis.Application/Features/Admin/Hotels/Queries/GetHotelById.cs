using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Hotels.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Hotels.Queries;

public record GetHotelByIdQuery(long Id) : IRequest<HotelDto?>;

public class GetHotelByIdHandler : IRequestHandler<GetHotelByIdQuery, HotelDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetHotelByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<HotelDto?> Handle(GetHotelByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<HotelDto>(
            "SELECT id AS Id, city_id AS CityId, name AS Name, stars AS Stars, address AS Address, email AS Email, phone AS Phone, check_in_time AS CheckInTime, check_out_time AS CheckOutTime, latitude AS Latitude, longitude AS Longitude, margin AS Margin, enabled AS Enabled, created_at AS CreatedAt FROM hotel WHERE id = @Id",
            new { request.Id });
    }
}
