using System.Net;
using System.Text.Json;

namespace ViajesAltairis.AdminApi.Middleware;

public class RoleAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    // Access levels: "full" = all methods, "read" = GET only, "own" = all methods (data scoping future)
    private static readonly Dictionary<string, Dictionary<string, string>> SectionAccess = new()
    {
        ["system"] = new() { ["admin"] = "full" },
        ["reference"] = new() { ["admin"] = "full", ["manager"] = "read", ["agent"] = "read", ["hotel_staff"] = "read" },
        ["hotels"] = new() { ["admin"] = "full", ["manager"] = "read", ["agent"] = "read", ["hotel_staff"] = "own" },
        ["providers"] = new() { ["admin"] = "full", ["manager"] = "read", ["agent"] = "read", ["hotel_staff"] = "read" },
        ["reservations"] = new() { ["admin"] = "full", ["manager"] = "full", ["agent"] = "own", ["hotel_staff"] = "own" },
        ["users"] = new() { ["admin"] = "full", ["manager"] = "read", ["agent"] = "read", ["hotel_staff"] = "read" },
        ["business-partners"] = new() { ["admin"] = "full", ["manager"] = "read", ["agent"] = "own", ["hotel_staff"] = "own" },
        ["pricing"] = new() { ["admin"] = "full", ["manager"] = "read" },
        ["subscriptions"] = new() { ["admin"] = "full", ["manager"] = "read", ["agent"] = "read", ["hotel_staff"] = "read" },
        ["financial"] = new() { ["admin"] = "full", ["manager"] = "read", ["agent"] = "own", ["hotel_staff"] = "own" },
        ["operations"] = new() { ["admin"] = "full", ["manager"] = "full", ["hotel_staff"] = "own" },
        ["reviews"] = new() { ["admin"] = "full", ["manager"] = "full", ["hotel_staff"] = "own" },
        ["audit"] = new() { ["admin"] = "full" },
        ["statistics"] = new() { ["admin"] = "read", ["manager"] = "read" },
    };

    // Route prefix → section mapping (order matters — first match wins)
    private static readonly (string Prefix, string Section)[] RouteMappings =
    [
        // system (admin-only configuration)
        ("/api/job-schedules", "system"),
        ("/api/administrativedivisiontypes", "system"),
        ("/api/administrativedivisions", "reference"),
        ("/api/webtranslations", "system"),
        ("/api/emailtemplates", "system"),
        ("/api/notificationlogs", "system"),

        // reference (lookup data needed by all roles)
        ("/api/cities", "reference"),
        ("/api/currencies", "reference"),
        ("/api/exchangerates", "reference"),
        ("/api/languages", "reference"),
        ("/api/countries", "reference"),
        ("/api/translations", "reference"),
        ("/api/providertypes", "reference"),

        // hotels
        ("/api/hotels", "hotels"),
        ("/api/roomtypes", "hotels"),
        ("/api/hotelimages", "hotels"),
        ("/api/roomimages", "hotels"),
        ("/api/amenity", "hotels"),          // covers /amenities and /amenitycategories
        ("/api/hotelamenities", "hotels"),
        ("/api/hotelproviderroomtypeamenities", "hotels"),

        // providers (must come after hotelproviderroomtypeamenities)
        ("/api/providers", "providers"),
        ("/api/hotelproviders", "providers"),
        ("/api/hotelproviderroomtype", "providers"),  // covers /hotelproviderroomtypes and /hotelproviderroomtypeboards
        ("/api/boardtypes", "providers"),

        // reservations
        ("/api/reservations", "reservations"),
        ("/api/reservationstatuses", "reservations"),

        // users
        ("/api/users", "users"),
        ("/api/usertypes", "users"),
        ("/api/userhotels", "users"),

        // business-partners
        ("/api/businesspartners", "business-partners"),

        // pricing
        ("/api/seasonalmargins", "pricing"),
        ("/api/taxtypes", "pricing"),
        ("/api/taxes", "pricing"),
        ("/api/promocodes", "pricing"),

        // subscriptions
        ("/api/subscriptiontypes", "subscriptions"),
        ("/api/usersubscriptions", "subscriptions"),

        // financial
        ("/api/invoices", "financial"),
        ("/api/invoicestatuses", "financial"),
        ("/api/paymentmethods", "financial"),
        ("/api/paymenttransactions", "financial"),
        ("/api/paymenttransactionfees", "financial"),

        // operations
        ("/api/hotelblackouts", "operations"),
        ("/api/cancellations", "operations"),
        ("/api/cancellationpolicies", "operations"),

        // reviews
        ("/api/reviews", "reviews"),
        ("/api/reviewresponses", "reviews"),

        // audit
        ("/api/auditlogs", "audit"),

        // statistics
        ("/api/statistics", "statistics"),
    ];

    private static readonly Dictionary<string, string> UserTypeMap = new()
    {
        ["1"] = "admin",
        ["2"] = "manager",
        ["3"] = "agent",
        ["4"] = "hotel_staff",
    };

    public RoleAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";

        // Skip auth endpoints, swagger, metrics, and public web translations (GET only)
        if (path.StartsWith("/api/auth") || path.StartsWith("/swagger") || path == "/metrics"
            || (path.StartsWith("/api/webtranslations") && context.Request.Method == HttpMethods.Get))
        {
            await _next(context);
            return;
        }

        // Require authentication for all other API routes
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Authentication required." }));
            return;
        }

        // Find which section this route belongs to
        var section = FindSection(path);
        if (section is null)
        {
            // Route not mapped — deny by default
            await _next(context);
            return;
        }

        var userTypeId = context.User.FindFirst("user_type_id")?.Value;
        if (userTypeId is null || !UserTypeMap.TryGetValue(userTypeId, out var role))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Access denied." }));
            return;
        }

        // B2B agents (with business_partner_id) are restricted to a subset of sections
        if (role == "agent" && context.User.FindFirst("business_partner_id")?.Value is not null)
        {
            var b2bDenied = new[] { "users", "business-partners", "subscriptions", "financial" };
            if (b2bDenied.Contains(section))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Access denied." }));
                return;
            }
        }

        if (!SectionAccess.TryGetValue(section, out var roleAccess) ||
            !roleAccess.TryGetValue(role, out var accessLevel))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Access denied." }));
            return;
        }

        // "read" access → only GET allowed
        if (accessLevel == "read" && context.Request.Method != HttpMethods.Get)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Read-only access. Modification not allowed." }));
            return;
        }

        // "full" and "own" → allow through (data scoping for "own" is handled by handlers)
        await _next(context);
    }

    private static string? FindSection(string path)
    {
        foreach (var (prefix, section) in RouteMappings)
        {
            if (path.StartsWith(prefix))
                return section;
        }
        return null;
    }
}
