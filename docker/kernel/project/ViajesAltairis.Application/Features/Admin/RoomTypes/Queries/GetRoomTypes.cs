using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.RoomTypes.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.RoomTypes.Queries;

public record GetRoomTypesQuery : IRequest<IEnumerable<RoomTypeDto>>;

public class GetRoomTypesHandler : IRequestHandler<GetRoomTypesQuery, IEnumerable<RoomTypeDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetRoomTypesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<RoomTypeDto>> Handle(GetRoomTypesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<RoomTypeDto>(
            "SELECT id AS Id, name AS Name, created_at AS CreatedAt FROM room_type ORDER BY name");
    }
}
