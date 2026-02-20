using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.WebTranslations.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.WebTranslations.Queries;

public record GetWebTranslationByIdQuery(long Id) : IRequest<WebTranslationDto?>;

public class GetWebTranslationByIdHandler : IRequestHandler<GetWebTranslationByIdQuery, WebTranslationDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetWebTranslationByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<WebTranslationDto?> Handle(GetWebTranslationByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<WebTranslationDto>(
            "SELECT id AS Id, translation_key AS TranslationKey, language_id AS LanguageId, value AS Value, created_at AS CreatedAt FROM web_translation WHERE id = @Id",
            new { request.Id });
    }
}
