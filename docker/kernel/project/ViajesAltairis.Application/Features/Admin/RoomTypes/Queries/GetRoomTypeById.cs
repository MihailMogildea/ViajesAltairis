using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.RoomTypes.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.RoomTypes.Queries;

public record GetRoomTypeByIdQuery(long Id) : IRequest<RoomTypeDto?>;

public class GetRoomTypeByIdHandler : IRequestHandler<GetRoomTypeByIdQuery, RoomTypeDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetRoomTypeByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<RoomTypeDto?> Handle(GetRoomTypeByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<RoomTypeDto>(
            "SELECT id AS Id, name AS Name, created_at AS CreatedAt FROM room_type WHERE id = @Id",
            new { request.Id });
    }
}
