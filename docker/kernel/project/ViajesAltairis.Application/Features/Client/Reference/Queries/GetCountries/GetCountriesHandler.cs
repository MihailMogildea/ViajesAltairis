using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Reference.Queries.GetCountries;

public class GetCountriesHandler : IRequestHandler<GetCountriesQuery, GetCountriesResponse>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ITranslationService _translationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICacheService _cacheService;

    public GetCountriesHandler(
        IDbConnectionFactory connectionFactory,
        ITranslationService translationService,
        ICurrentUserService currentUserService,
        ICacheService cacheService)
    {
        _connectionFactory = connectionFactory;
        _translationService = translationService;
        _currentUserService = currentUserService;
        _cacheService = cacheService;
    }

    public async Task<GetCountriesResponse> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
    {
        var langId = _currentUserService.LanguageId;
        var cacheKey = $"ref:countries:{langId}";

        var cached = await _cacheService.GetAsync<GetCountriesResponse>(cacheKey, cancellationToken);
        if (cached != null)
            return cached;
        using var connection = _connectionFactory.CreateConnection();

        const string sql = "SELECT id AS Id, iso_code AS Code, name AS Name FROM country WHERE enabled = TRUE ORDER BY name";

        var countries = (await connection.QueryAsync<CountryDto>(sql)).ToList();

        if (langId != 1 && countries.Count > 0)
        {
            var ids = countries.Select(c => c.Id).ToList();
            var names = await _translationService.ResolveAsync("country", ids, langId, "name", cancellationToken);
            foreach (var c in countries)
                if (names.TryGetValue(c.Id, out var n)) c.Name = n;
        }

        var response = new GetCountriesResponse { Countries = countries };
        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromHours(1), cancellationToken);
        return response;
    }
}
