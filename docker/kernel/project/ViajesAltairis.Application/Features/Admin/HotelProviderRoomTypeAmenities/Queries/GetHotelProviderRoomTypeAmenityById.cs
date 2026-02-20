using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeAmenities.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeAmenities.Queries;

public record GetHotelProviderRoomTypeAmenityByIdQuery(long Id) : IRequest<HotelProviderRoomTypeAmenityDto?>;

public class GetHotelProviderRoomTypeAmenityByIdHandler : IRequestHandler<GetHotelProviderRoomTypeAmenityByIdQuery, HotelProviderRoomTypeAmenityDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetHotelProviderRoomTypeAmenityByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<HotelProviderRoomTypeAmenityDto?> Handle(GetHotelProviderRoomTypeAmenityByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<HotelProviderRoomTypeAmenityDto>(
            "SELECT id AS Id, hotel_provider_room_type_id AS HotelProviderRoomTypeId, amenity_id AS AmenityId, created_at AS CreatedAt FROM hotel_provider_room_type_amenity WHERE id = @Id",
            new { request.Id });
    }
}
