using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelAmenities.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelAmenities.Queries;

public record GetHotelAmenityByIdQuery(long Id) : IRequest<HotelAmenityDto?>;

public class GetHotelAmenityByIdHandler : IRequestHandler<GetHotelAmenityByIdQuery, HotelAmenityDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetHotelAmenityByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<HotelAmenityDto?> Handle(GetHotelAmenityByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<HotelAmenityDto>(
            "SELECT id AS Id, hotel_id AS HotelId, amenity_id AS AmenityId, created_at AS CreatedAt FROM hotel_amenity WHERE id = @Id",
            new { request.Id });
    }
}
