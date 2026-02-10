using System.Security.Claims;

namespace ApiShield.Api.Auth;

public interface IApiKeyIdentityResolver { ClaimsPrincipal CreatePrincipal(string apiKey); }
