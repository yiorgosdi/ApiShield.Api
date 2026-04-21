using ApiShield.Api.Security.AuthConstants;
using Microsoft.AspNetCore.Authorization;

namespace ApiShield.Api.Endpoints;

public static class SecureEndpoints
{
    // SecureEndpoints: auth/authz verification endpoints
    public static IEndpointRouteBuilder MapSecureEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/secure");

        group.MapGet("/ping", () => Results.Ok(new { message = "pong" }))
            .RequireAuthorization(new AuthorizeAttribute
            {
                AuthenticationSchemes = AuthSchemes.ApiKey
            });

        group.MapGet("/admin", () => Results.Ok(new { message = "admin pong" }))
            .RequireAuthorization(new AuthorizeAttribute
            {
                AuthenticationSchemes = AuthSchemes.ApiKey,
                Policy = AuthPolicies.AdminOnly
            });

        return app;
    }
}