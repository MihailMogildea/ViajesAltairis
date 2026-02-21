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
            @"SELECT r.id AS Id, r.reservation_id AS ReservationId, r.user_id AS UserId, u.email AS UserEmail,
                     r.hotel_id AS HotelId, r.rating AS Rating, r.title AS Title, r.comment AS Comment,
                     r.visible AS Visible, r.created_at AS CreatedAt, r.updated_at AS UpdatedAt
              FROM review r
              JOIN user u ON u.id = r.user_id
              ORDER BY r.created_at DESC");
    }
}
