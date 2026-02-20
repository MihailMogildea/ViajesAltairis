using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.ReservationStatuses.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.ReservationStatuses.Queries;

public record GetReservationStatusByIdQuery(long Id) : IRequest<ReservationStatusDto?>;

public class GetReservationStatusByIdHandler : IRequestHandler<GetReservationStatusByIdQuery, ReservationStatusDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetReservationStatusByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<ReservationStatusDto?> Handle(GetReservationStatusByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<ReservationStatusDto>(
            "SELECT id AS Id, name AS Name, created_at AS CreatedAt FROM reservation_status WHERE id = @Id",
            new { request.Id });
    }
}
