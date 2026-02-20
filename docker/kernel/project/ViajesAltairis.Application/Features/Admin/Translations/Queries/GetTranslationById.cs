using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Translations.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Translations.Queries;

public record GetTranslationByIdQuery(long Id) : IRequest<TranslationDto?>;

public class GetTranslationByIdHandler : IRequestHandler<GetTranslationByIdQuery, TranslationDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetTranslationByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<TranslationDto?> Handle(GetTranslationByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<TranslationDto>(
            "SELECT id AS Id, entity_type AS EntityType, entity_id AS EntityId, field AS Field, language_id AS LanguageId, value AS Value, created_at AS CreatedAt FROM translation WHERE id = @Id",
            new { request.Id });
    }
}
