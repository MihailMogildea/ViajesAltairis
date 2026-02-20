using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.ReviewResponses.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.ReviewResponses.Queries;

public record GetReviewResponseByIdQuery(long Id) : IRequest<ReviewResponseDto?>;

public class GetReviewResponseByIdHandler : IRequestHandler<GetReviewResponseByIdQuery, ReviewResponseDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetReviewResponseByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<ReviewResponseDto?> Handle(GetReviewResponseByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<ReviewResponseDto>(
            @"SELECT id AS Id, review_id AS ReviewId, user_id AS UserId, comment AS Comment, created_at AS CreatedAt
              FROM review_response WHERE id = @Id",
            new { request.Id });
    }
}
