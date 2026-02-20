using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Cancellations.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Cancellations.Queries;

public record GetCancellationByIdQuery(long Id) : IRequest<CancellationDto?>;

public class GetCancellationByIdHandler : IRequestHandler<GetCancellationByIdQuery, CancellationDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetCancellationByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<CancellationDto?> Handle(GetCancellationByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<CancellationDto>(
            @"SELECT id AS Id, reservation_id AS ReservationId, cancelled_by_user_id AS CancelledByUserId,
                     reason AS Reason, penalty_percentage AS PenaltyPercentage, penalty_amount AS PenaltyAmount,
                     refund_amount AS RefundAmount, currency_id AS CurrencyId, created_at AS CreatedAt
              FROM cancellation WHERE id = @Id",
            new { request.Id });
    }
}
