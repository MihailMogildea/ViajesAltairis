using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelProviders.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelProviders.Queries;

public record GetHotelProviderByIdQuery(long Id) : IRequest<HotelProviderDto?>;

public class GetHotelProviderByIdHandler : IRequestHandler<GetHotelProviderByIdQuery, HotelProviderDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetHotelProviderByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<HotelProviderDto?> Handle(GetHotelProviderByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<HotelProviderDto>(
            "SELECT id AS Id, hotel_id AS HotelId, provider_id AS ProviderId, enabled AS Enabled, created_at AS CreatedAt FROM hotel_provider WHERE id = @Id",
            new { request.Id });
    }
}
