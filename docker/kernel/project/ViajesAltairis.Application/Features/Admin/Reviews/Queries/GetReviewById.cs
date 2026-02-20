using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Reviews.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Reviews.Queries;

public record GetReviewByIdQuery(long Id) : IRequest<ReviewDto?>;

public class GetReviewByIdHandler : IRequestHandler<GetReviewByIdQuery, ReviewDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetReviewByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<ReviewDto?> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<ReviewDto>(
            @"SELECT id AS Id, reservation_id AS ReservationId, user_id AS UserId, hotel_id AS HotelId,
                     rating AS Rating, title AS Title, comment AS Comment, visible AS Visible,
                     created_at AS CreatedAt, updated_at AS UpdatedAt
              FROM review WHERE id = @Id",
            new { request.Id });
    }
}
