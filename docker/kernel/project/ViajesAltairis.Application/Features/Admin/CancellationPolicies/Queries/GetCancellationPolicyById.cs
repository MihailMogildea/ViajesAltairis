using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.CancellationPolicies.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.CancellationPolicies.Queries;

public record GetCancellationPolicyByIdQuery(long Id) : IRequest<CancellationPolicyDto?>;

public class GetCancellationPolicyByIdHandler : IRequestHandler<GetCancellationPolicyByIdQuery, CancellationPolicyDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetCancellationPolicyByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<CancellationPolicyDto?> Handle(GetCancellationPolicyByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<CancellationPolicyDto>(
            @"SELECT id AS Id, hotel_id AS HotelId, free_cancellation_hours AS FreeCancellationHours,
                     penalty_percentage AS PenaltyPercentage, enabled AS Enabled, created_at AS CreatedAt
              FROM cancellation_policy WHERE id = @Id",
            new { request.Id });
    }
}
