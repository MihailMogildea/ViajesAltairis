using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.SeasonalMargins.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.SeasonalMargins.Queries;

public record GetSeasonalMarginsQuery : IRequest<IEnumerable<SeasonalMarginDto>>;

public class GetSeasonalMarginsHandler : IRequestHandler<GetSeasonalMarginsQuery, IEnumerable<SeasonalMarginDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetSeasonalMarginsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<SeasonalMarginDto>> Handle(GetSeasonalMarginsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<SeasonalMarginDto>(
            "SELECT id AS Id, administrative_division_id AS AdministrativeDivisionId, start_month_day AS StartMonthDay, end_month_day AS EndMonthDay, margin AS Margin, created_at AS CreatedAt, updated_at AS UpdatedAt FROM seasonal_margin ORDER BY administrative_division_id, start_month_day");
    }
}
