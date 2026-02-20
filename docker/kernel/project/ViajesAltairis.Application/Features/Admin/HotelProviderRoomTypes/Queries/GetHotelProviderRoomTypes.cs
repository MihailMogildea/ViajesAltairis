using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypes.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypes.Queries;

public record GetHotelProviderRoomTypesQuery : IRequest<IEnumerable<HotelProviderRoomTypeDto>>;

public class GetHotelProviderRoomTypesHandler : IRequestHandler<GetHotelProviderRoomTypesQuery, IEnumerable<HotelProviderRoomTypeDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetHotelProviderRoomTypesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<HotelProviderRoomTypeDto>> Handle(GetHotelProviderRoomTypesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<HotelProviderRoomTypeDto>(
            "SELECT id AS Id, hotel_provider_id AS HotelProviderId, room_type_id AS RoomTypeId, capacity AS Capacity, quantity AS Quantity, price_per_night AS PricePerNight, currency_id AS CurrencyId, exchange_rate_id AS ExchangeRateId, enabled AS Enabled, created_at AS CreatedAt FROM hotel_provider_room_type ORDER BY id");
    }
}
