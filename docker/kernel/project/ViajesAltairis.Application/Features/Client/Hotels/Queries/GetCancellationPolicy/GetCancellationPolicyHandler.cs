using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Hotels.Queries.GetCancellationPolicy;

public class GetCancellationPolicyHandler : IRequestHandler<GetCancellationPolicyQuery, GetCancellationPolicyResponse>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetCancellationPolicyHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<GetCancellationPolicyResponse> Handle(GetCancellationPolicyQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT
                free_cancellation_hours AS HoursBeforeCheckIn,
                penalty_percentage AS PenaltyPercentage
            FROM cancellation_policy
            WHERE hotel_id = @HotelId AND enabled = TRUE
            ORDER BY free_cancellation_hours DESC
            """;

        var policies = (await connection.QueryAsync<CancellationPolicyDto>(sql, new { request.HotelId })).ToList();

        return new GetCancellationPolicyResponse { Policies = policies };
    }
}
