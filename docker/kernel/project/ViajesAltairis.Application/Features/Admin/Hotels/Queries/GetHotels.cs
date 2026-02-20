using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Hotels.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Hotels.Queries;

public record GetHotelsQuery : IRequest<IEnumerable<HotelDto>>;

public class GetHotelsHandler : IRequestHandler<GetHotelsQuery, IEnumerable<HotelDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetHotelsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<HotelDto>> Handle(GetHotelsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<HotelDto>(
            "SELECT id AS Id, city_id AS CityId, name AS Name, stars AS Stars, address AS Address, email AS Email, phone AS Phone, check_in_time AS CheckInTime, check_out_time AS CheckOutTime, latitude AS Latitude, longitude AS Longitude, margin AS Margin, enabled AS Enabled, created_at AS CreatedAt FROM hotel ORDER BY name");
    }
}
