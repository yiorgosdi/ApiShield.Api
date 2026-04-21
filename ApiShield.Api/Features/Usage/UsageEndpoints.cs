using ApiShield.Api.Security.AuthConstants;
using ApiShield.Core.Idempotency;
using ApiShield.Core.Usage;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ApiShield.Api.Endpoints;

public static class UsageEndpoints
{
    // UsageEndpoints: usiness flow for usage tracking
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

        //return app;
        return group;
    }

    private static async Task<IResult> Increment(
    ClaimsPrincipal user,
    [FromHeader(Name = "X-Idempotency-Key")] string? idempotencyKey,
    HttpContext httpContext,
    IUsageEventQueue queue,
    IIdempotencyService idempotencyService,
    CancellationToken ct)
    {
        var apiKeyId = GetKeyId(user);

        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            return Results.BadRequest(new
            {
                error = "X-Idempotency-Key header is required."
            });
        }

        idempotencyKey = idempotencyKey.Trim();

        var route = httpContext.Request.Path.ToString();

        var decision = await idempotencyService.TryStartRequestAsync(
            apiKeyId,
            idempotencyKey,
            route,
            ct);

        if (!decision.ShouldEnqueue)
        {
            return Results.Accepted(
                null,
                new IncrementAcceptedResponse(
                    "Duplicate request ignored.",
                    decision.CurrentStatus));
        }

        var message = new UsageIncrementRequested(
            Guid.NewGuid(),
            apiKeyId,
            route,
            DateTime.UtcNow,
            httpContext.TraceIdentifier,
            idempotencyKey);

        await queue.EnqueueAsync(message, ct);

        return Results.Accepted(
            null,
            new IncrementAcceptedResponse(
                "Request accepted for processing.",
                decision.CurrentStatus));
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

    private static string GetKeyId(ClaimsPrincipal user)
    => user.FindFirstValue(ClaimTypes.NameIdentifier)
       ?? throw new InvalidOperationException("Missing NameIdentifier claim for API key.");
}