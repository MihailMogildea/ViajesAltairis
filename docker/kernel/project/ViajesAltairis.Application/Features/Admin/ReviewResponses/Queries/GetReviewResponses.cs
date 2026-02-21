using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.ReviewResponses.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.ReviewResponses.Queries;

public record GetReviewResponsesQuery : IRequest<IEnumerable<ReviewResponseDto>>;

public class GetReviewResponsesHandler : IRequestHandler<GetReviewResponsesQuery, IEnumerable<ReviewResponseDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetReviewResponsesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<ReviewResponseDto>> Handle(GetReviewResponsesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<ReviewResponseDto>(
            @"SELECT rr.id AS Id, rr.review_id AS ReviewId, rr.user_id AS UserId, u.email AS UserEmail,
                     rr.comment AS Comment, rr.created_at AS CreatedAt
              FROM review_response rr
              JOIN user u ON u.id = rr.user_id
              ORDER BY rr.created_at DESC");
    }
}
