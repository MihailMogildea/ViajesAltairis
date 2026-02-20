using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelImages.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelImages.Queries;

public record GetHotelImageByIdQuery(long Id) : IRequest<HotelImageDto?>;

public class GetHotelImageByIdHandler : IRequestHandler<GetHotelImageByIdQuery, HotelImageDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetHotelImageByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<HotelImageDto?> Handle(GetHotelImageByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<HotelImageDto>(
            "SELECT id AS Id, hotel_id AS HotelId, url AS Url, alt_text AS AltText, sort_order AS SortOrder, created_at AS CreatedAt FROM hotel_image WHERE id = @Id",
            new { request.Id });
    }
}
