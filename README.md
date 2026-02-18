# ApiShield

[![Build & Deploy](https://github.com/yiorgosdi/ApiShield.Api/actions/workflows/main_apishield-george.yml/badge.svg)](https://github.com/yiorgosdi/ApiShield.Api/actions/workflows/main_apishield-george.yml)

Azure Live - Swagger UI:  
https://apishield-george-hfcsh9hzhjf2c6g6.westeurope-01.azurewebsites.net/swagger

---

Demo API Keys (Sample) - These are sample demo keys for educational purposes only.

Admin  
"1234567890abcdef1234567890abcdef"

Basic  
"bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"

---

1. What it is

ApiShield.TestingLab is an ASP.NET Core API security lab demonstrating:

- API-key Authentication (AuthN)
- Role-based Authorization (AuthZ)
- Clean composition root (SOLID)
- Full unit & integration test coverage
- CI/CD deployment to Azure
- Configuration-driven security (no hardcoded credentials)

---

2. Endpoints

- `GET /secure/ping`→ requires valid API key  
- `ApiKeyAuth:Keys`→ requires valid API key + admin role  

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

- Unit Tests: Validate core rules in isolation (ApiKeyValidator) using mocks for IApiKeyStore.
- Integration Tests: Validate the full HTTP security pipeline end-to-end (AuthN/AuthZ) using WebApplicationFactory.
  Integration tests override "ApiKeyAuth:Keys" via in-memory configuration to remain deterministic and environment-independent.

--- 
