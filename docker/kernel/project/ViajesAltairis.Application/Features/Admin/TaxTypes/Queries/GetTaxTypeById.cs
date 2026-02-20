using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.TaxTypes.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.TaxTypes.Queries;

public record GetTaxTypeByIdQuery(long Id) : IRequest<TaxTypeDto?>;

public class GetTaxTypeByIdHandler : IRequestHandler<GetTaxTypeByIdQuery, TaxTypeDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetTaxTypeByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<TaxTypeDto?> Handle(GetTaxTypeByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<TaxTypeDto>(
            "SELECT id AS Id, name AS Name, created_at AS CreatedAt FROM tax_type WHERE id = @Id",
            new { request.Id });
    }
}
