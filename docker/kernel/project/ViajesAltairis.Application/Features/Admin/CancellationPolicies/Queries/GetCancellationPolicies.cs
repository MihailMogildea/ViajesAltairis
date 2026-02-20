using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.CancellationPolicies.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.CancellationPolicies.Queries;

public record GetCancellationPoliciesQuery : IRequest<IEnumerable<CancellationPolicyDto>>;

public class GetCancellationPoliciesHandler : IRequestHandler<GetCancellationPoliciesQuery, IEnumerable<CancellationPolicyDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetCancellationPoliciesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<CancellationPolicyDto>> Handle(GetCancellationPoliciesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<CancellationPolicyDto>(
            @"SELECT id AS Id, hotel_id AS HotelId, free_cancellation_hours AS FreeCancellationHours,
                     penalty_percentage AS PenaltyPercentage, enabled AS Enabled, created_at AS CreatedAt
              FROM cancellation_policy ORDER BY hotel_id");
    }
}
