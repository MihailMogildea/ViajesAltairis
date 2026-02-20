using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Cancellations.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Cancellations.Queries;

public record GetCancellationsQuery : IRequest<IEnumerable<CancellationDto>>;

public class GetCancellationsHandler : IRequestHandler<GetCancellationsQuery, IEnumerable<CancellationDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetCancellationsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<CancellationDto>> Handle(GetCancellationsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<CancellationDto>(
            @"SELECT id AS Id, reservation_id AS ReservationId, cancelled_by_user_id AS CancelledByUserId,
                     reason AS Reason, penalty_percentage AS PenaltyPercentage, penalty_amount AS PenaltyAmount,
                     refund_amount AS RefundAmount, currency_id AS CurrencyId, created_at AS CreatedAt
              FROM cancellation ORDER BY created_at DESC");
    }
}
