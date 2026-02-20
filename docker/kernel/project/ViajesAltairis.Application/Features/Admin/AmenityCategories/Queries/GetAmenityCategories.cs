using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.AmenityCategories.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.AmenityCategories.Queries;

public record GetAmenityCategoriesQuery : IRequest<IEnumerable<AmenityCategoryDto>>;

public class GetAmenityCategoriesHandler : IRequestHandler<GetAmenityCategoriesQuery, IEnumerable<AmenityCategoryDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetAmenityCategoriesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<AmenityCategoryDto>> Handle(GetAmenityCategoriesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<AmenityCategoryDto>(
            "SELECT id AS Id, name AS Name, created_at AS CreatedAt FROM amenity_category ORDER BY name");
    }
}
