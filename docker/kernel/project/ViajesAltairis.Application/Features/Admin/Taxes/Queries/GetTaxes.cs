using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Taxes.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Taxes.Queries;

public record GetTaxesQuery : IRequest<IEnumerable<TaxDto>>;

public class GetTaxesHandler : IRequestHandler<GetTaxesQuery, IEnumerable<TaxDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetTaxesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<TaxDto>> Handle(GetTaxesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<TaxDto>(
            "SELECT id AS Id, tax_type_id AS TaxTypeId, country_id AS CountryId, administrative_division_id AS AdministrativeDivisionId, city_id AS CityId, rate AS Rate, is_percentage AS IsPercentage, enabled AS Enabled, created_at AS CreatedAt FROM tax ORDER BY id");
    }
}
