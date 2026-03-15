using ApiShield.Api.Messaging;
using ApiShield.Api.Security.AuthConstants;
using ApiShield.Core.Usage;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ApiShield.Api.Features.Usage;

public static class UsageEndpoints
{
    public static IEndpointRouteBuilder MapUsageEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/secure/usage")
            .RequireAuthorization(new AuthorizeAttribute 
            { 
                AuthenticationSchemes = AuthSchemes.ApiKey 
            });

        group.MapPost("/increment", Increment);
        group.MapGet("/today", Today);
        group.RequireRateLimiting("usage");

        return app;
    }

    private static string GetKeyId(ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.NameIdentifier)
           ?? throw new InvalidOperationException("Missing NameIdentifier claim for API key.");

    private static async Task<IResult> Increment(
        ClaimsPrincipal user,
        IUsageEventQueue queue,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var apiKey = GetKeyId(user);

        var message = new UsageIncrementRequested(
            Guid.NewGuid(),
            apiKey,
            httpContext.Request.Path,
            DateTime.UtcNow,
            httpContext.TraceIdentifier);

        await queue.EnqueueAsync(message, cancellationToken);

        return Results.Accepted();
    }

    private static async Task<IResult> Today(
        ClaimsPrincipal user,
        IApiKeyUsageService usage,
        CancellationToken ct)
    {
        var keyId = GetKeyId(user);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var usageToday = await usage.GetTodayAsync(keyId, today, ct);
        return Results.Ok(usageToday);
    }
}