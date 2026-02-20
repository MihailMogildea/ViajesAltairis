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
            @"SELECT id AS Id, review_id AS ReviewId, user_id AS UserId, comment AS Comment, created_at AS CreatedAt
              FROM review_response ORDER BY created_at DESC");
    }
}
