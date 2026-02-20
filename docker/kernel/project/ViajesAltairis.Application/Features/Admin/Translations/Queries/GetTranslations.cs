using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Translations.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Translations.Queries;

public record GetTranslationsQuery : IRequest<IEnumerable<TranslationDto>>;

public class GetTranslationsHandler : IRequestHandler<GetTranslationsQuery, IEnumerable<TranslationDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetTranslationsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<TranslationDto>> Handle(GetTranslationsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<TranslationDto>(
            "SELECT id AS Id, entity_type AS EntityType, entity_id AS EntityId, field AS Field, language_id AS LanguageId, value AS Value, created_at AS CreatedAt FROM translation ORDER BY entity_type, entity_id, field");
    }
}
