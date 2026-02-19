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
          .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(AuthSchemes.ApiKey, _ => { });

        services.AddAuthorization();

        // Core + stores
       //shmera feyegi services.AddScoped<ApiKeyValidator>();//shmera feygei 
        services.AddSingleton<IApiKeyStore, InMemoryApiKeyStore>();

        // Identity + middleware
        services.AddSingleton<IApiKeyIdentityResolver, ApiKeyIdentityResolver>();
        //shmera feygei  services.AddTransient<ApiKeyAuthMiddleware>();

        return services;
    }
}
