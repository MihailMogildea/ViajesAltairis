using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Statistics.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Statistics.Queries;

public record GetUsersByTypeQuery(DateTime? From, DateTime? To) : IRequest<IEnumerable<UsersByTypeDto>>;

public class GetUsersByTypeHandler : IRequestHandler<GetUsersByTypeQuery, IEnumerable<UsersByTypeDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetUsersByTypeHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<UsersByTypeDto>> Handle(GetUsersByTypeQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<UsersByTypeDto>(
            """
            SELECT ut.name AS TypeName, COUNT(u.id) AS UserCount
            FROM user_type ut
            LEFT JOIN user u ON u.user_type_id = ut.id
              AND (@From IS NULL OR u.created_at >= @From)
              AND (@To IS NULL OR u.created_at <= @To)
            GROUP BY ut.id, ut.name
            ORDER BY ut.id
            """, new { request.From, request.To });
    }
}
