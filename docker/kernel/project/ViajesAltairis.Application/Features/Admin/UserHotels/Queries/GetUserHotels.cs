using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.UserHotels.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.UserHotels.Queries;

public record GetUserHotelsQuery(long? UserId) : IRequest<IEnumerable<UserHotelDto>>;

public class GetUserHotelsHandler : IRequestHandler<GetUserHotelsQuery, IEnumerable<UserHotelDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetUserHotelsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<UserHotelDto>> Handle(GetUserHotelsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        if (request.UserId.HasValue)
        {
            return await connection.QueryAsync<UserHotelDto>(
                "SELECT id AS Id, user_id AS UserId, hotel_id AS HotelId, created_at AS CreatedAt FROM user_hotel WHERE user_id = @UserId ORDER BY id",
                new { request.UserId });
        }
        return await connection.QueryAsync<UserHotelDto>(
            "SELECT id AS Id, user_id AS UserId, hotel_id AS HotelId, created_at AS CreatedAt FROM user_hotel ORDER BY id");
    }
}
