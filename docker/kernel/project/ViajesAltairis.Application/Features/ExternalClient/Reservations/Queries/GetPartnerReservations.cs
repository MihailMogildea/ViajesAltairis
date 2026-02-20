using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.ExternalClient.Reservations.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.ExternalClient.Reservations.Queries;

public record GetPartnerReservationsQuery(
    long BusinessPartnerId, int? StatusId,
    int Page = 1, int PageSize = 20) : IRequest<GetPartnerReservationsResponse>;

public record GetPartnerReservationsResponse(List<ReservationSummaryDto> Reservations, int TotalCount, int Page, int PageSize);

public class GetPartnerReservationsHandler : IRequestHandler<GetPartnerReservationsQuery, GetPartnerReservationsResponse>
{
    private readonly IDbConnectionFactory _db;

    public GetPartnerReservationsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<GetPartnerReservationsResponse> Handle(GetPartnerReservationsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();

        var conditions = new List<string> { "u.business_partner_id = @BusinessPartnerId" };
        var parameters = new DynamicParameters();
        parameters.Add("BusinessPartnerId", request.BusinessPartnerId);

        if (request.StatusId.HasValue)
        {
            if (request.StatusId.Value < 1 || request.StatusId.Value > 6)
                throw new ArgumentException("StatusId must be between 1 and 6.");
            conditions.Add("rs.status_id = @StatusId");
            parameters.Add("StatusId", request.StatusId.Value);
        }

        var where = string.Join(" AND ", conditions);
        var offset = (request.Page - 1) * request.PageSize;
        parameters.Add("Offset", offset);
        parameters.Add("PageSize", request.PageSize);

        var countSql = $@"
            SELECT COUNT(*)
            FROM v_reservation_summary rs
            JOIN user u ON u.id = rs.booked_by_user_id
            WHERE {where}";

        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        var sql = $@"
            SELECT rs.reservation_id AS ReservationId, rs.reservation_code AS ReservationCode,
                   rs.status_name AS StatusName,
                   rs.owner_first_name AS OwnerFirstName, rs.owner_last_name AS OwnerLastName,
                   rs.owner_email AS OwnerEmail,
                   rs.total_price AS TotalPrice, rs.currency_code AS CurrencyCode,
                   rs.line_count AS LineCount, rs.created_at AS CreatedAt
            FROM v_reservation_summary rs
            JOIN user u ON u.id = rs.booked_by_user_id
            WHERE {where}
            ORDER BY rs.created_at DESC
            LIMIT @PageSize OFFSET @Offset";

        var reservations = (await connection.QueryAsync<ReservationSummaryDto>(sql, parameters)).ToList();

        return new GetPartnerReservationsResponse(reservations, totalCount, request.Page, request.PageSize);
    }
}
