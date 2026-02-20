using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelProviders.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelProviders.Queries;

public record GetHotelProvidersQuery : IRequest<IEnumerable<HotelProviderDto>>;

public class GetHotelProvidersHandler : IRequestHandler<GetHotelProvidersQuery, IEnumerable<HotelProviderDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetHotelProvidersHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<HotelProviderDto>> Handle(GetHotelProvidersQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<HotelProviderDto>(
            "SELECT id AS Id, hotel_id AS HotelId, provider_id AS ProviderId, enabled AS Enabled, created_at AS CreatedAt FROM hotel_provider ORDER BY id");
    }
}
