# ApiShield

> This project is part of an ongoing deep dive into API security, testing methodologies and Azure cloud deployment practices.


[![Build & Deploy](https://github.com/yiorgosdi/ApiShield.Api/actions/workflows/main_apishield-george.yml/badge.svg)](https://github.com/yiorgosdi/ApiShield.Api/actions/workflows/main_apishield-george.yml)

Azure Live - Swagger UI:  
https://apishield-george-hfcsh9hzhjf2c6g6.westeurope-01.azurewebsites.net/swagger

---

Demo API Keys (for demonstration only) - See configuration in appsettings.Development.json.

---

1. What it is

ApiShield is an ASP.NET Core API security lab demonstrating:

- API Key validation via custom authentication scheme
- OIDC / OAuth2 integration (Keycloak)
- Token validation without runtime calls to the Identity Provider
- Clean layering (Domain / Application / Infrastructure)
- Unit & Integration Testing (WebApplicationFactory)
- TDD-oriented development
- Docker-based local orchestration
- Deployed to Azure App Service (Live)

---

2. Endpoints

- `GET /secure/ping`→ requires valid API key  
- `GET /secure/admin`→ requires valid API key + admin role  

---

3. Status Codes (AuthN vs AuthZ)

- 401 Unauthorized → missing/invalid API key  
- 403 Forbidden → valid key, insufficient role  

---

4. How to run locally

```bash
dotnet test
dotnet run --project ApiShield.Api

---

5. Testing Strategy

- Unit tests for validation components
- Integration tests covering full authentication pipeline
- Arrange–Act–Assert pattern
- Focus on realistic security flow scenarios

--- 

6. Architecture Focus

- Authentication vs Authorization separation
- Custom ApiKey scheme via AuthenticationHandler
- Token validation through middleware pipeline
- Production-oriented configuration handling

---

7. Cloud Deployment

- Azure App Service (Linux)
- GitHub Actions CI/CD pipeline
- HTTPS enforced
- Environment-based configuration

---  

8. Roadmap

- Redis-backed caching
- Rate limiting strategies
- Observability (Azure Application Insights)
- Resilience patterns

--- 