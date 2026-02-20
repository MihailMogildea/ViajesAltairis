using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.UserHotels.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.UserHotels.Queries;

public record GetUserHotelByIdQuery(long Id) : IRequest<UserHotelDto?>;

public class GetUserHotelByIdHandler : IRequestHandler<GetUserHotelByIdQuery, UserHotelDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetUserHotelByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<UserHotelDto?> Handle(GetUserHotelByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<UserHotelDto>(
            "SELECT id AS Id, user_id AS UserId, hotel_id AS HotelId, created_at AS CreatedAt FROM user_hotel WHERE id = @Id",
            new { request.Id });
    }
}
