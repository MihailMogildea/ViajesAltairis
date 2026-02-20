using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.ReservationStatuses.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.ReservationStatuses.Queries;

public record GetReservationStatusesQuery : IRequest<IEnumerable<ReservationStatusDto>>;

public class GetReservationStatusesHandler : IRequestHandler<GetReservationStatusesQuery, IEnumerable<ReservationStatusDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetReservationStatusesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<ReservationStatusDto>> Handle(GetReservationStatusesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<ReservationStatusDto>(
            "SELECT id AS Id, name AS Name, created_at AS CreatedAt FROM reservation_status ORDER BY name");
    }
}
