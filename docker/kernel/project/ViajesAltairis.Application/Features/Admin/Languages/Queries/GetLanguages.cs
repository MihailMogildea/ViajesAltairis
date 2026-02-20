using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Languages.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Languages.Queries;

public record GetLanguagesQuery : IRequest<IEnumerable<LanguageDto>>;

public class GetLanguagesHandler : IRequestHandler<GetLanguagesQuery, IEnumerable<LanguageDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetLanguagesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<LanguageDto>> Handle(GetLanguagesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<LanguageDto>(
            "SELECT id AS Id, iso_code AS IsoCode, name AS Name, created_at AS CreatedAt FROM language ORDER BY name");
    }
}
