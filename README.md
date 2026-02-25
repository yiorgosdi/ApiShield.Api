ApiShield

A production-oriented ASP.NET Core API security lab focused on authentication, authorization, testing and Azure deployment practices.

🌐 Live Swagger (Azure App Service)
https://apishield-george-hfcsh9hzhjf2c6g6.westeurope-01.azurewebsites.net/swagger

🚀 Purpose

- ApiShield demonstrates how to build and deploy a secure ASP.NET Core API with:
- Custom API Key authentication scheme
- Role-based authorization policies
- OIDC / OAuth2 integration (Keycloak)
- Token validation without runtime calls to Identity Provider
- Clean architecture layering (Domain / Application / Infrastructure)
- Unit & Integration testing
- GitHub Actions CI/CD pipeline
- Azure App Service deployment

# Health check
curl https://<your-url>/health

# Unauthorized (no key)
curl https://<your-url>/secure/ping
# → 401

# Authorized
curl https://<your-url>/secure/ping -H "X-API-Key: <valid-key>"
# → 200

# Forbidden (non-admin key)
curl https://<your-url>/secure/admin -H "X-API-Key: <basic-key>"
# → 403

# Admin access
curl https://<your-url>/secure/admin -H "X-API-Key: <admin-key>"
# → 200

Demo API keys are available in appsettings.Development.json.


🔐 AuthN vs AuthZ Behavior
Scenario	Result
Missing or invalid API key	401 Unauthorized
Valid key, insufficient role	403 Forbidden
Valid key, correct role	200 OK

This clearly separates authentication from authorization in the request pipeline.


🧪 Testing Strategy
- Unit tests for core validation logic
- Integration tests using WebApplicationFactory
- Full authentication pipeline coverage
- Explicit 401 vs 403 verification
- Arrange–Act–Assert structure
- Run locally:
dotnet test
dotnet run --project ApiShield.Api


🏗 Architecture Overview
Client
  ↓
Middleware Pipeline
  ↓
Custom ApiKey AuthenticationHandler
  ↓
ClaimsPrincipal
  ↓
Authorization Policies (Roles)
  ↓
Controller Endpoints

Design focus:
- Clear AuthN / AuthZ separation
- Policy-based role enforcement
- Environment-based configuration
- Production-ready middleware ordering


☁️ Cloud & DevOps
- Azure App Service (Linux)
- GitHub Actions CI/CD
- HTTPS enforced
- OIDC-based deployment authentication
- Environment-specific configuration handling


🔎 Security Notes
- Demo API keys are for testing purposes only.
- No secrets are committed to the repository.
- Production configuration should use secure secret storage (e.g., Azure Key Vault).


🛣 Roadmap
- Redis-backed caching
- Rate limiting middleware
- Structured logging & correlation IDs
- Azure Application Insights integration
- Resilience patterns (retry / idempotency) 
