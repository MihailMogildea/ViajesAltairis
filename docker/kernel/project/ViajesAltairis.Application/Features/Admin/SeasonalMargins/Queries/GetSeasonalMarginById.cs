using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.SeasonalMargins.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.SeasonalMargins.Queries;

public record GetSeasonalMarginByIdQuery(long Id) : IRequest<SeasonalMarginDto?>;

public class GetSeasonalMarginByIdHandler : IRequestHandler<GetSeasonalMarginByIdQuery, SeasonalMarginDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetSeasonalMarginByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<SeasonalMarginDto?> Handle(GetSeasonalMarginByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SeasonalMarginDto>(
            "SELECT id AS Id, administrative_division_id AS AdministrativeDivisionId, start_month_day AS StartMonthDay, end_month_day AS EndMonthDay, margin AS Margin, created_at AS CreatedAt, updated_at AS UpdatedAt FROM seasonal_margin WHERE id = @Id",
            new { request.Id });
    }
}
