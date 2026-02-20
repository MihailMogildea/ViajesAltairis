using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.RoomImages.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.RoomImages.Queries;

public record GetRoomImageByIdQuery(long Id) : IRequest<RoomImageDto?>;

public class GetRoomImageByIdHandler : IRequestHandler<GetRoomImageByIdQuery, RoomImageDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetRoomImageByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<RoomImageDto?> Handle(GetRoomImageByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<RoomImageDto>(
            "SELECT id AS Id, hotel_provider_room_type_id AS HotelProviderRoomTypeId, url AS Url, alt_text AS AltText, sort_order AS SortOrder, created_at AS CreatedAt FROM room_image WHERE id = @Id",
            new { request.Id });
    }
}
