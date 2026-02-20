using System.Net;
using ViajesAltairis.Infrastructure.Services;

namespace ViajesAltairis.Scheduled.Api.Tests.Services;

public class ScheduledApiClientTests
{
    private static (ScheduledApiClient client, MockHttpMessageHandler handler) CreateClient(HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var handler = new MockHttpMessageHandler(statusCode);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://scheduled-api:8080") };

        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory.CreateClient("ScheduledApi").Returns(httpClient);

        return (new ScheduledApiClient(httpClientFactory), handler);
    }

    [Fact]
    public async Task TriggerJobAsync_PostsToCorrectUrl()
    {
        var (client, handler) = CreateClient();

        await client.TriggerJobAsync("exchange-rate-sync");

        handler.LastRequest.Should().NotBeNull();
        handler.LastRequest!.Method.Should().Be(HttpMethod.Post);
        handler.LastRequest.RequestUri!.PathAndQuery.Should().Be("/api/jobs/exchange-rate-sync/trigger");
    }

    [Fact]
    public async Task ReloadSchedulesAsync_PostsToCorrectUrl()
    {
        var (client, handler) = CreateClient();

        await client.ReloadSchedulesAsync();

        handler.LastRequest.Should().NotBeNull();
        handler.LastRequest!.Method.Should().Be(HttpMethod.Post);
        handler.LastRequest.RequestUri!.PathAndQuery.Should().Be("/api/jobs/reload");
    }

    [Fact]
    public async Task TriggerJobAsync_NonSuccessStatusCode_ThrowsHttpRequestException()
    {
        var (client, _) = CreateClient(HttpStatusCode.InternalServerError);

        var act = () => client.TriggerJobAsync("exchange-rate-sync");

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task ReloadSchedulesAsync_NonSuccessStatusCode_ThrowsHttpRequestException()
    {
        var (client, _) = CreateClient(HttpStatusCode.InternalServerError);

        var act = () => client.ReloadSchedulesAsync();

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        public HttpRequestMessage? LastRequest { get; private set; }

        public MockHttpMessageHandler(HttpStatusCode statusCode)
        {
            _statusCode = statusCode;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(new HttpResponseMessage(_statusCode));
        }
    }
}
