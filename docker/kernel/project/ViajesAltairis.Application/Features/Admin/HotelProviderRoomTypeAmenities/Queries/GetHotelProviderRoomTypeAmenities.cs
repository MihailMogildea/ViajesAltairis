using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeAmenities.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeAmenities.Queries;

public record GetHotelProviderRoomTypeAmenitiesQuery(long? HotelProviderRoomTypeId = null) : IRequest<IEnumerable<HotelProviderRoomTypeAmenityDto>>;

public class GetHotelProviderRoomTypeAmenitiesHandler : IRequestHandler<GetHotelProviderRoomTypeAmenitiesQuery, IEnumerable<HotelProviderRoomTypeAmenityDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetHotelProviderRoomTypeAmenitiesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<HotelProviderRoomTypeAmenityDto>> Handle(GetHotelProviderRoomTypeAmenitiesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        var sql = "SELECT id AS Id, hotel_provider_room_type_id AS HotelProviderRoomTypeId, amenity_id AS AmenityId, created_at AS CreatedAt FROM hotel_provider_room_type_amenity";
        if (request.HotelProviderRoomTypeId.HasValue)
        {
            sql += " WHERE hotel_provider_room_type_id = @HotelProviderRoomTypeId";
            return await connection.QueryAsync<HotelProviderRoomTypeAmenityDto>(sql, new { request.HotelProviderRoomTypeId });
        }
        return await connection.QueryAsync<HotelProviderRoomTypeAmenityDto>(sql + " ORDER BY id");
    }
}
