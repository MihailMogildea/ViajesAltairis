using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.BoardTypes.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.BoardTypes.Queries;

public record GetBoardTypeByIdQuery(long Id) : IRequest<BoardTypeDto?>;

public class GetBoardTypeByIdHandler : IRequestHandler<GetBoardTypeByIdQuery, BoardTypeDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetBoardTypeByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<BoardTypeDto?> Handle(GetBoardTypeByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<BoardTypeDto>(
            "SELECT id AS Id, name AS Name FROM board_type WHERE id = @Id",
            new { request.Id });
    }
}
