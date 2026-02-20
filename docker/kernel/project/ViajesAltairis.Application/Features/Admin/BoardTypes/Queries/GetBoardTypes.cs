using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.BoardTypes.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.BoardTypes.Queries;

public record GetBoardTypesQuery : IRequest<IEnumerable<BoardTypeDto>>;

public class GetBoardTypesHandler : IRequestHandler<GetBoardTypesQuery, IEnumerable<BoardTypeDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetBoardTypesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<BoardTypeDto>> Handle(GetBoardTypesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<BoardTypeDto>(
            "SELECT id AS Id, name AS Name FROM board_type ORDER BY name");
    }
}
