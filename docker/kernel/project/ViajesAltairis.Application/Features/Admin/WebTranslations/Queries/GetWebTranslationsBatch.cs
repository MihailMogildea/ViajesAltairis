using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.WebTranslations.Queries;

public record GetAdminWebTranslationsPublicQuery : IRequest<Dictionary<string, string>>;

public class GetAdminWebTranslationsPublicHandler : IRequestHandler<GetAdminWebTranslationsPublicQuery, Dictionary<string, string>>
{
    private readonly IDbConnectionFactory _db;
    private readonly ICurrentUserService _currentUser;

    public GetAdminWebTranslationsPublicHandler(IDbConnectionFactory db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Dictionary<string, string>> Handle(GetAdminWebTranslationsPublicQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();

        const string sql = """
            SELECT translation_key, value
            FROM web_translation
            WHERE language_id = @LanguageId
              AND translation_key LIKE 'admin.%'
            ORDER BY translation_key
            """;

        var rows = await connection.QueryAsync<(string translation_key, string value)>(
            sql, new { LanguageId = _currentUser.LanguageId });

        return rows.ToDictionary(r => r.translation_key, r => r.value);
    }
}
