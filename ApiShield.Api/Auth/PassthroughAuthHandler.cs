using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace ApiShield.Api.Auth;

public sealed class PassthroughAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public PassthroughAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // user setup up from ApiKeyAuthMiddleware.
        // No identity = leave it as NoResult ([Authorize] returns 401).
        if (Context.User?.Identity?.IsAuthenticated == true)
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(Context.User, Scheme.Name)));

        return Task.FromResult(AuthenticateResult.NoResult());
    }
}
