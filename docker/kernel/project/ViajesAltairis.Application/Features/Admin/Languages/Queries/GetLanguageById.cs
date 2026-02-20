using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Languages.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Languages.Queries;

public record GetLanguageByIdQuery(long Id) : IRequest<LanguageDto?>;

public class GetLanguageByIdHandler : IRequestHandler<GetLanguageByIdQuery, LanguageDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetLanguageByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<LanguageDto?> Handle(GetLanguageByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<LanguageDto>(
            "SELECT id AS Id, iso_code AS IsoCode, name AS Name, created_at AS CreatedAt FROM language WHERE id = @Id",
            new { request.Id });
    }
}
