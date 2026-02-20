using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.UserTypes.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.UserTypes.Queries;

public record GetUserTypesQuery : IRequest<IEnumerable<UserTypeDto>>;

public class GetUserTypesHandler : IRequestHandler<GetUserTypesQuery, IEnumerable<UserTypeDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetUserTypesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<UserTypeDto>> Handle(GetUserTypesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<UserTypeDto>(
            "SELECT id AS Id, name AS Name, created_at AS CreatedAt FROM user_type ORDER BY name");
    }
}
