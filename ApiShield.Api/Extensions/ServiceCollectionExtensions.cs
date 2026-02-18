using ApiShield.Api.Auth;
using ApiShield.Api.Security.AuthConstants;
using ApiShield.Core;
using Microsoft.AspNetCore.Authentication;

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
        .AddScheme<AuthenticationSchemeOptions, PassthroughAuthHandler>(
            AuthSchemes.ApiKey, _ => { });

        services.AddAuthorization();

        // Core + stores
        services.AddScoped<ApiKeyValidator>();
        services.AddSingleton<IApiKeyStore, InMemoryApiKeyStore>();

        // Identity + middleware
        services.AddSingleton<IApiKeyIdentityResolver, ApiKeyIdentityResolver>();
        services.AddTransient<ApiKeyAuthMiddleware>();

        return services;
    }
}
