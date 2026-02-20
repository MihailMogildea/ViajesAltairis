using Dapper;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Infrastructure.Translations;

public class TranslationService : ITranslationService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ICacheService _cache;

    public TranslationService(IDbConnectionFactory connectionFactory, ICacheService cache)
    {
        _connectionFactory = connectionFactory;
        _cache = cache;
    }

    public async Task<Dictionary<long, string>> ResolveAsync(
        string entityType, IEnumerable<long> entityIds,
        long languageId, string field = "name", CancellationToken ct = default)
    {
        var ids = entityIds.Distinct().ToList();
        if (ids.Count == 0)
            return new Dictionary<long, string>();

        var cacheKey = $"trans:{entityType}:{field}:{languageId}";
        var cached = await _cache.GetAsync<Dictionary<long, string>>(cacheKey, ct);

        if (cached is not null)
        {
            var result = new Dictionary<long, string>();
            var missing = new List<long>();

            foreach (var id in ids)
            {
                if (cached.TryGetValue(id, out var val))
                    result[id] = val;
                else
                    missing.Add(id);
            }

            if (missing.Count == 0)
                return result;

            var fetched = await QueryTranslationsAsync(entityType, missing, languageId, field);
            foreach (var kv in fetched)
            {
                result[kv.Key] = kv.Value;
                cached[kv.Key] = kv.Value;
            }

            await _cache.SetAsync(cacheKey, cached, TimeSpan.FromHours(1), ct);
            return result;
        }

        var all = await QueryTranslationsAsync(entityType, ids, languageId, field);
        await _cache.SetAsync(cacheKey, all, TimeSpan.FromHours(1), ct);
        return all;
    }

    private async Task<Dictionary<long, string>> QueryTranslationsAsync(
        string entityType, List<long> ids, long languageId, string field)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT entity_id, value
            FROM translation
            WHERE entity_type = @EntityType
              AND field = @Field
              AND language_id = @LanguageId
              AND entity_id IN @Ids
            """;

        var rows = await connection.QueryAsync<(long entity_id, string value)>(
            sql, new { EntityType = entityType, Field = field, LanguageId = languageId, Ids = ids });

        return rows.ToDictionary(r => r.entity_id, r => r.value);
    }
}
