using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeBoards.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeBoards.Queries;

public record GetHotelProviderRoomTypeBoardByIdQuery(long Id) : IRequest<HotelProviderRoomTypeBoardDto?>;

public class GetHotelProviderRoomTypeBoardByIdHandler : IRequestHandler<GetHotelProviderRoomTypeBoardByIdQuery, HotelProviderRoomTypeBoardDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetHotelProviderRoomTypeBoardByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<HotelProviderRoomTypeBoardDto?> Handle(GetHotelProviderRoomTypeBoardByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<HotelProviderRoomTypeBoardDto>(
            "SELECT id AS Id, hotel_provider_room_type_id AS HotelProviderRoomTypeId, board_type_id AS BoardTypeId, price_per_night AS PricePerNight, enabled AS Enabled FROM hotel_provider_room_type_board WHERE id = @Id",
            new { request.Id });
    }
}
