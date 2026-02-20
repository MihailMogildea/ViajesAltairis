using System.Net;
using System.Net.Http.Json;
using FluentValidation;
using ViajesAltairis.Reservations.Api.Tests.Fixtures;
using ViajesAltairis.Reservations.Api.Tests.Helpers;

namespace ViajesAltairis.Reservations.Api.Tests;

public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly HttpClient Client;

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Factory.ResetMocks();
        Client = factory.CreateClient();
    }

    /// <summary>
    /// POST with exception-to-status-code mapping.
    /// TestHost propagates handler exceptions — we catch and map them.
    /// </summary>
    protected async Task<HttpResponseMessage> PostAsync<T>(string url, T body)
    {
        try
        {
            return await Client.PostAsJsonAsync(url, body);
        }
        catch (Exception ex)
        {
            return MapExceptionToResponse(ex);
        }
    }

    protected async Task<HttpResponseMessage> GetAsync(string url)
    {
        try
        {
            return await Client.GetAsync(url);
        }
        catch (Exception ex)
        {
            return MapExceptionToResponse(ex);
        }
    }

    protected async Task<HttpResponseMessage> DeleteAsync(string url)
    {
        try
        {
            return await Client.DeleteAsync(url);
        }
        catch (Exception ex)
        {
            return MapExceptionToResponse(ex);
        }
    }

    protected async Task<TResponse?> ReadJsonAsync<TResponse>(HttpResponseMessage response)
    {
        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    protected DapperMockHelper SetupDapper()
    {
        var helper = new DapperMockHelper();
        Factory.SetupDapperConnection(helper);
        return helper;
    }

    private static HttpResponseMessage MapExceptionToResponse(Exception ex)
    {
        var inner = ex;
        while (inner.InnerException != null) inner = inner.InnerException;
        var statusCode = inner switch
        {
            KeyNotFoundException => HttpStatusCode.NotFound,
            ValidationException => HttpStatusCode.BadRequest,
            InvalidOperationException => HttpStatusCode.InternalServerError,
            _ => HttpStatusCode.InternalServerError,
        };
        var response = new HttpResponseMessage(statusCode);
        response.ReasonPhrase = inner.GetType().Name + ": " + inner.Message;
        return response;
    }

    public void Dispose()
    {
        Client.Dispose();
        GC.SuppressFinalize(this);
    }
}
