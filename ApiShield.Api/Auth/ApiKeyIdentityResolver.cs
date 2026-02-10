using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace ApiShield.Api.Auth;

public sealed class ApiKeyIdentityResolver : IApiKeyIdentityResolver
{
    private readonly ApiKeyAuthOptions _options;

    public ApiKeyIdentityResolver(IOptions<ApiKeyAuthOptions> options)
        => _options = options.Value;

    public ClaimsPrincipal CreatePrincipal(string apiKey)
    {
        var entry = _options.Keys.First(k => k.Key == apiKey); // safe: AuthN has already validated exists

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, apiKey),
            new Claim(ClaimTypes.Role, entry.Role)
        };

        var identity = new ClaimsIdentity(claims, authenticationType: "ApiKey");
        return new ClaimsPrincipal(identity);
    }
}
