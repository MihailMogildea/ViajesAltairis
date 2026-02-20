using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.TaxTypes.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.TaxTypes.Queries;

public record GetTaxTypesQuery : IRequest<IEnumerable<TaxTypeDto>>;

public class GetTaxTypesHandler : IRequestHandler<GetTaxTypesQuery, IEnumerable<TaxTypeDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetTaxTypesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<TaxTypeDto>> Handle(GetTaxTypesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<TaxTypeDto>(
            "SELECT id AS Id, name AS Name, created_at AS CreatedAt FROM tax_type ORDER BY name");
    }
}
