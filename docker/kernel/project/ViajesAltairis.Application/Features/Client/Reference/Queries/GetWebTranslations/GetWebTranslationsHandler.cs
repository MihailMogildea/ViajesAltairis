using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Reference.Queries.GetWebTranslations;

public class GetWebTranslationsHandler : IRequestHandler<GetWebTranslationsQuery, Dictionary<string, string>>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ICurrentUserService _currentUserService;

    public GetWebTranslationsHandler(IDbConnectionFactory connectionFactory, ICurrentUserService currentUserService)
    {
        _connectionFactory = connectionFactory;
        _currentUserService = currentUserService;
    }

    public async Task<Dictionary<string, string>> Handle(GetWebTranslationsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT translation_key, value
            FROM web_translation
            WHERE language_id = @LanguageId
              AND translation_key LIKE 'client.%'
            ORDER BY translation_key
            """;

        var rows = await connection.QueryAsync<(string translation_key, string value)>(
            sql, new { LanguageId = _currentUserService.LanguageId });

        return rows.ToDictionary(r => r.translation_key, r => r.value);
    }
}
