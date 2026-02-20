namespace ViajesAltairis.Application.Interfaces;

public interface ITranslationService
{
    Task<Dictionary<long, string>> ResolveAsync(
        string entityType, IEnumerable<long> entityIds,
        long languageId, string field = "name", CancellationToken ct = default);
}
