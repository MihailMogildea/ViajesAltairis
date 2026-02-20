using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Reviews.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Reviews.Queries;

public record GetReviewsQuery : IRequest<IEnumerable<ReviewDto>>;

public class GetReviewsHandler : IRequestHandler<GetReviewsQuery, IEnumerable<ReviewDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetReviewsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<ReviewDto>> Handle(GetReviewsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<ReviewDto>(
            @"SELECT id AS Id, reservation_id AS ReservationId, user_id AS UserId, hotel_id AS HotelId,
                     rating AS Rating, title AS Title, comment AS Comment, visible AS Visible,
                     created_at AS CreatedAt, updated_at AS UpdatedAt
              FROM review ORDER BY created_at DESC");
    }
}
