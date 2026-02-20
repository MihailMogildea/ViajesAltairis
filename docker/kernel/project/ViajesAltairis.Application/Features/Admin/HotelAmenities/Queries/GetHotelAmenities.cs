using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelAmenities.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelAmenities.Queries;

public record GetHotelAmenitiesQuery(long? HotelId = null) : IRequest<IEnumerable<HotelAmenityDto>>;

public class GetHotelAmenitiesHandler : IRequestHandler<GetHotelAmenitiesQuery, IEnumerable<HotelAmenityDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetHotelAmenitiesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<HotelAmenityDto>> Handle(GetHotelAmenitiesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        var sql = "SELECT id AS Id, hotel_id AS HotelId, amenity_id AS AmenityId, created_at AS CreatedAt FROM hotel_amenity";
        if (request.HotelId.HasValue)
        {
            sql += " WHERE hotel_id = @HotelId";
            return await connection.QueryAsync<HotelAmenityDto>(sql, new { request.HotelId });
        }
        return await connection.QueryAsync<HotelAmenityDto>(sql + " ORDER BY id");
    }
}
