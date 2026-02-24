using ApiShield.Api.Auth;
using ApiShield.Api.Security.AuthConstants;
using ApiShield.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace ApiShield.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiKeyAuth(this IServiceCollection services, IConfiguration config)
    {
        // Options, reading from configuration (not hardcoded). 
        services.Configure<ApiKeyAuthOptions>(config.GetSection("ApiKeyAuth"));

        // AuthN/AuthZ
        services
          .AddAuthentication(AuthSchemes.ApiKey)
          .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(AuthSchemes.ApiKey, _ => { });

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(AuthSchemes.ApiKey)
                .RequireAuthenticatedUser()
                .Build();
        });
         
        // Core + stores
        services.AddSingleton<IApiKeyStore, InMemoryApiKeyStore>();

        // Identity + middleware
        services.AddSingleton<IApiKeyIdentityResolver, ApiKeyIdentityResolver>();

        return services;
    }
}
