using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Amenities.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Amenities.Queries;

public record GetAmenitiesQuery : IRequest<IEnumerable<AmenityDto>>;

public class GetAmenitiesHandler : IRequestHandler<GetAmenitiesQuery, IEnumerable<AmenityDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetAmenitiesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<AmenityDto>> Handle(GetAmenitiesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<AmenityDto>(
            "SELECT id AS Id, category_id AS CategoryId, name AS Name, created_at AS CreatedAt FROM amenity ORDER BY name");
    }
}
