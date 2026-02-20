using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Currencies.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Currencies.Queries;

public record GetCurrencyByIdQuery(long Id) : IRequest<CurrencyDto?>;

public class GetCurrencyByIdHandler : IRequestHandler<GetCurrencyByIdQuery, CurrencyDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetCurrencyByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<CurrencyDto?> Handle(GetCurrencyByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<CurrencyDto>(
            "SELECT id AS Id, iso_code AS IsoCode, name AS Name, symbol AS Symbol, created_at AS CreatedAt FROM currency WHERE id = @Id",
            new { request.Id });
    }
}
