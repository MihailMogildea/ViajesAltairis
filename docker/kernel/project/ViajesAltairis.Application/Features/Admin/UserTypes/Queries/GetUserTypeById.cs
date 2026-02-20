using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.UserTypes.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.UserTypes.Queries;

public record GetUserTypeByIdQuery(long Id) : IRequest<UserTypeDto?>;

public class GetUserTypeByIdHandler : IRequestHandler<GetUserTypeByIdQuery, UserTypeDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetUserTypeByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<UserTypeDto?> Handle(GetUserTypeByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<UserTypeDto>(
            "SELECT id AS Id, name AS Name, created_at AS CreatedAt FROM user_type WHERE id = @Id",
            new { request.Id });
    }
}
