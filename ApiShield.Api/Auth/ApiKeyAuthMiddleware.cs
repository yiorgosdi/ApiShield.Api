using ApiShield.Api.Security.AuthConstants;
using ApiShield.Core;

namespace ApiShield.Api.Auth;

public sealed class ApiKeyAuthMiddleware : IMiddleware
{
    private readonly ApiKeyValidator _validator;
    private readonly IApiKeyIdentityResolver _resolver;

    public ApiKeyAuthMiddleware(ApiKeyValidator validator, IApiKeyIdentityResolver resolver)
    {
        _validator = validator;
        _resolver = resolver;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // only for /secure endpoints in the lab
        if (!context.Request.Path.StartsWithSegments("/secure"))
        {
            await next(context);
            return;
        }

        var apiKey = context.Request.Headers[AuthHeaders.ApiKey].ToString();

        // AuthN (throws on missing/invalid)
        await _validator.ValidateOrThrowAsync(apiKey, context.RequestAborted);

        // Identity for AuthZ
        context.User = _resolver.CreatePrincipal(apiKey);

        await next(context);
    }
}
