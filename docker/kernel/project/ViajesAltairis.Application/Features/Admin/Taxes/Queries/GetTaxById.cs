using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Taxes.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Taxes.Queries;

public record GetTaxByIdQuery(long Id) : IRequest<TaxDto?>;

public class GetTaxByIdHandler : IRequestHandler<GetTaxByIdQuery, TaxDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetTaxByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<TaxDto?> Handle(GetTaxByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<TaxDto>(
            "SELECT id AS Id, tax_type_id AS TaxTypeId, country_id AS CountryId, administrative_division_id AS AdministrativeDivisionId, city_id AS CityId, rate AS Rate, is_percentage AS IsPercentage, enabled AS Enabled, created_at AS CreatedAt FROM tax WHERE id = @Id",
            new { request.Id });
    }
}
