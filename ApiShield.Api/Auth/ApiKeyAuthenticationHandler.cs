using ApiShield.Api.Security.AuthConstants;
using ApiShield.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace ApiShield.Api.Auth;

public sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IApiKeyStore _store;
    private readonly IApiKeyIdentityResolver _resolver;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IApiKeyStore store,
        IApiKeyIdentityResolver resolver)
        : base(options, logger, encoder)
    {
        _store = store;
        _resolver = resolver;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // If header missing -> no result (lets framework challenge => 401)
        if (!Request.Headers.TryGetValue(AuthHeaders.ApiKey, out var values))
            return AuthenticateResult.NoResult();

        var apiKey = values.ToString();

        if (string.IsNullOrWhiteSpace(apiKey))
            return AuthenticateResult.NoResult();

        if (apiKey.Length < 16)
            return AuthenticateResult.Fail("API key is too short.");

        var exists = await _store.ExistsAsync(apiKey, Context.RequestAborted);
        if (!exists)
            return AuthenticateResult.Fail("Invalid API key.");

        var principal = _resolver.CreatePrincipal(apiKey);
        var ticket = new AuthenticationTicket(principal, AuthSchemes.ApiKey);

        return AuthenticateResult.Success(ticket);
    }
}
