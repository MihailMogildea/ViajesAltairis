using Dapper;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.ExternalClientApi.Middleware;

public class BusinessPartnerAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public BusinessPartnerAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip for unauthenticated requests (login endpoint)
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        var partnerIdClaim = context.User.FindFirst("businessPartnerId")?.Value;
        if (partnerIdClaim is null || !long.TryParse(partnerIdClaim, out var partnerId))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { message = "Invalid partner credentials." });
            return;
        }

        // Verify the business partner is still active
        using var scope = context.RequestServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
        using var connection = db.CreateConnection();

        const string sql = "SELECT enabled FROM business_partner WHERE id = @Id";
        var enabled = await connection.QuerySingleOrDefaultAsync<bool?>(sql, new { Id = partnerId });

        if (enabled != true)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { message = "Business partner account is disabled or not found." });
            return;
        }

        await _next(context);
    }
}
