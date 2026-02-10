using ApiShield.Api.Auth;
using ApiShield.Api.Security.AuthConstants;
using ApiShield.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiShield.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiKeyAuth(this IServiceCollection services, IConfiguration config)
    {
        // Options
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
