using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.WebTranslations.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.WebTranslations.Queries;

public record GetWebTranslationsQuery : IRequest<IEnumerable<WebTranslationDto>>;

public class GetWebTranslationsHandler : IRequestHandler<GetWebTranslationsQuery, IEnumerable<WebTranslationDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetWebTranslationsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<WebTranslationDto>> Handle(GetWebTranslationsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<WebTranslationDto>(
            "SELECT id AS Id, translation_key AS TranslationKey, language_id AS LanguageId, value AS Value, created_at AS CreatedAt FROM web_translation ORDER BY translation_key");
    }
}
