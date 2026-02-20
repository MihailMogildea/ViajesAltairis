using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.AmenityCategories.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.AmenityCategories.Queries;

public record GetAmenityCategoryByIdQuery(long Id) : IRequest<AmenityCategoryDto?>;

public class GetAmenityCategoryByIdHandler : IRequestHandler<GetAmenityCategoryByIdQuery, AmenityCategoryDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetAmenityCategoryByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<AmenityCategoryDto?> Handle(GetAmenityCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<AmenityCategoryDto>(
            "SELECT id AS Id, name AS Name, created_at AS CreatedAt FROM amenity_category WHERE id = @Id",
            new { request.Id });
    }
}
