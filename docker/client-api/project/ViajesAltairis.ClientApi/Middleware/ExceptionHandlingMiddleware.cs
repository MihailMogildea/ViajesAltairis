using System.Net;
using System.Text.Json;

namespace ViajesAltairis.ClientApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            KeyNotFoundException ex => (HttpStatusCode.NotFound, ex.Message),
            UnauthorizedAccessException ex => (HttpStatusCode.Forbidden, ex.Message),
            InvalidOperationException ex => (HttpStatusCode.BadRequest, ex.Message),
            FluentValidation.ValidationException ex => (HttpStatusCode.BadRequest,
                string.Join("; ", ex.Errors.Select(e => e.ErrorMessage))),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Unhandled exception");
        else
            _logger.LogWarning(exception, "Handled exception: {StatusCode}", statusCode);

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new { error = message };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
