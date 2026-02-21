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
            @"SELECT c.id AS Id, c.reservation_id AS ReservationId, c.cancelled_by_user_id AS CancelledByUserId,
                     u.email AS CancelledByUserEmail, c.reason AS Reason,
                     c.penalty_percentage AS PenaltyPercentage, c.penalty_amount AS PenaltyAmount,
                     c.refund_amount AS RefundAmount, c.currency_id AS CurrencyId, c.created_at AS CreatedAt
              FROM cancellation c
              JOIN user u ON u.id = c.cancelled_by_user_id
              ORDER BY c.created_at DESC");
    }
}
