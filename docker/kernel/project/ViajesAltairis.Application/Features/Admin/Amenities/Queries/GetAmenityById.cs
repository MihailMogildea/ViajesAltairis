using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Amenities.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Amenities.Queries;

public record GetAmenityByIdQuery(long Id) : IRequest<AmenityDto?>;

public class GetAmenityByIdHandler : IRequestHandler<GetAmenityByIdQuery, AmenityDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetAmenityByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<AmenityDto?> Handle(GetAmenityByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<AmenityDto>(
            "SELECT id AS Id, category_id AS CategoryId, name AS Name, created_at AS CreatedAt FROM amenity WHERE id = @Id",
            new { request.Id });
    }
}
