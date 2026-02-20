using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Infrastructure.Services;

public class ScheduledApiClient : IScheduledApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ScheduledApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient() => _httpClientFactory.CreateClient("ScheduledApi");

    public async Task TriggerJobAsync(string jobKey, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var response = await client.PostAsync($"/api/jobs/{jobKey}/trigger", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task ReloadSchedulesAsync(CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var response = await client.PostAsync("/api/jobs/reload", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
