using System.Net.Http.Headers;

namespace ViajesAltairis.Client.Api.Tests.Helpers;

public static class HttpClientExtensions
{
    public static HttpClient WithClientAuth(this HttpClient client, long userId = 8, string email = "client1@example.com")
    {
        var token = AuthHelper.GenerateClientToken(userId, email);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
