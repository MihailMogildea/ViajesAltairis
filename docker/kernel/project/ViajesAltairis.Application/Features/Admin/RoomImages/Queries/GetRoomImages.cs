using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.RoomImages.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.RoomImages.Queries;

public record GetRoomImagesQuery : IRequest<IEnumerable<RoomImageDto>>;

public class GetRoomImagesHandler : IRequestHandler<GetRoomImagesQuery, IEnumerable<RoomImageDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetRoomImagesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<RoomImageDto>> Handle(GetRoomImagesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<RoomImageDto>(
            "SELECT id AS Id, hotel_provider_room_type_id AS HotelProviderRoomTypeId, url AS Url, alt_text AS AltText, sort_order AS SortOrder, created_at AS CreatedAt FROM room_image ORDER BY hotel_provider_room_type_id, sort_order");
    }
}
