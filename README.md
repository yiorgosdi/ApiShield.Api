ApiShield — ASP.NET Core API Security & Architecture Lab

ApiShield is a production-oriented ASP.NET Core security and architecture lab designed to demonstrate how modern backend APIs should be protected, structured, and deployed.
The project focuses on real-world backend concerns including authentication, authorization, usage tracking, asynchronous processing, and secure cloud deployment practices.
Rather than a simple demo API, ApiShield explores the full request pipeline from client request → security middleware → identity resolution → policy enforcement → event-driven backend processing.

LIVE SWAGGER (Azure App Service)
https://apishield-george-hfcsh9hzhjf2c6g6.westeurope-01.azurewebsites.net/swagger

## Key Features

- Custom API Key authentication scheme
- Role-based authorization policies
- Secure request pipeline design
- Usage tracking subsystem with asynchronous processing
- Queue-based event architecture
- Clean architecture layering (Domain / Application / Infrastructure)
- Integration testing of the authentication pipeline
- CI/CD with GitHub Actions
- Azure App Service deployment


PURPOSE 
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


AuthN vs AuthZ BEHAVIOR 
Scenario	Result
Missing or invalid API key	401 Unauthorized
Valid key, insufficient role	403 Forbidden
Valid key, correct role	200 OK

This clearly separates authentication from authorization in the request pipeline.


TESTING STRATEGY 
- Unit tests for core validation logic
- Integration tests using WebApplicationFactory
- Full authentication pipeline coverage
- Explicit 401 vs 403 verification
- Arrange–Act–Assert structure
- Run locally:
dotnet test
dotnet run --project ApiShield.Api


ARCHITECTURE OVERVIEW 
Client > Middleware Pipeline > Custom ApiKey AuthenticationHandler > ClaimsPrincipal > Authorization Policies (Roles) > Controller Endpoints

Design focus:
- Clear AuthN / AuthZ separation
- Policy-based role enforcement
- Environment-based configuration
- Production-ready middleware ordering


CLOUD & DEVOPS 
- Azure App Service (Linux)
- GitHub Actions CI/CD
- HTTPS enforced
- OIDC-based deployment authentication
- Environment-specific configuration handling


SECURITY NOTES
- Demo API keys are for testing purposes only.
- No secrets are committed to the repository.
- Production configuration should use secure secret storage (e.g., Azure Key Vault).


ROADMAP 
- Redis-backed caching
- Advanced rate limiting policies
- Structured logging & correlation IDs
- Azure Application Insights integration
- Resilience patterns (retry / idempotency) 


USAGE TRACKING ENDPOINTS 
ApiShield includes a usage tracking subsystem that demonstrates how authenticated 
API calls can produce usage events that are processed asynchronously.

POST /secure/usage/increment
Registers a usage event for the authenticated API key.
Example:
curl https://<your-url>/secure/usage/increment \
  -H "X-API-Key: <valid-key>" \
  -X POST

Behavior:
- Resolves the authenticated API key identity
- Creates a UsageIncrementRequested message
- Pushes the message to a queue-based processing pipeline
- Returns 202 Accepted

Purpose:
- Prevents blocking API requests
- Enables asynchronous processing
- Protects the API under high traffic

GET /secure/usage/today
Returns usage statistics for the authenticated API key for the current day.
Example:
curl https://<your-url>/secure/usage/today \
  -H "X-API-Key: <valid-key>"

Behavior:
- Resolves the authenticated API key
- Queries the usage service
- Returns usage metrics for the current day

Security characteristics:
- Endpoint requires authentication
- Ensures callers only access their own usage data
- Prevents object-level authorization issues

Usage Processing Architecture
The usage tracking subsystem uses an event-driven pipeline.
Client > Usage Endpoint > UsageEventQueue > Background Worker > Usage Store
This design ensures that the API remains responsive while usage events are processed reliably in the background.
