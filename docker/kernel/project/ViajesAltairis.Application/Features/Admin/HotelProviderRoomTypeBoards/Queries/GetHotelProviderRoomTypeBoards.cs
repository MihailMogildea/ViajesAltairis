using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeBoards.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeBoards.Queries;

public record GetHotelProviderRoomTypeBoardsQuery : IRequest<IEnumerable<HotelProviderRoomTypeBoardDto>>;

public class GetHotelProviderRoomTypeBoardsHandler : IRequestHandler<GetHotelProviderRoomTypeBoardsQuery, IEnumerable<HotelProviderRoomTypeBoardDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetHotelProviderRoomTypeBoardsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<HotelProviderRoomTypeBoardDto>> Handle(GetHotelProviderRoomTypeBoardsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<HotelProviderRoomTypeBoardDto>(
            "SELECT id AS Id, hotel_provider_room_type_id AS HotelProviderRoomTypeId, board_type_id AS BoardTypeId, price_per_night AS PricePerNight, enabled AS Enabled FROM hotel_provider_room_type_board ORDER BY id");
    }
}
