using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelImages.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelImages.Queries;

public record GetHotelImagesQuery : IRequest<IEnumerable<HotelImageDto>>;

public class GetHotelImagesHandler : IRequestHandler<GetHotelImagesQuery, IEnumerable<HotelImageDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetHotelImagesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<HotelImageDto>> Handle(GetHotelImagesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<HotelImageDto>(
            "SELECT id AS Id, hotel_id AS HotelId, url AS Url, alt_text AS AltText, sort_order AS SortOrder, created_at AS CreatedAt FROM hotel_image ORDER BY hotel_id, sort_order");
    }
}
