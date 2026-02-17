# ApiShield
Live: https://apishield-george-hfcsh9hzhjf2c6g6.westeurope-01.azurewebsites.net/swagger/index.html
Swagger: http://localhost:5084/swagger/index.html 
Sample keys (demo): 
	Admin 1234567890abcdef1234567890abcdef 
	Basic bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb 
	NotPresentKey 1234567890abcdef1234567890abcabc


<img alt="CI" src="https://github.com/yiorgosdi/ApiShield.Api/actions/workflows/ci.yml/badge.svg" />

!\[CI](https://github.com/yiorgosdi/ApiShield.Api/actions/workflows/ci.yml/badge.svg)

Live: https://apishield-george-hfcsh9hzhjf2c6g6.westeurope-01.azurewebsites.net

1. What it is
   ApiShield.TestingLab is a .NET API security lab that demonstrates API-key AuthN, role-based AuthZ, and a layered testing strategy (unit + integration) with clean composition.
2. Endpoints
   GET /secure/ping → requires a valid X-API-Key
   GET /secure/admin → requires valid X-API-Key and admin role
3. Status codes (401 vs 403)
   401 Unauthorized: missing/invalid API key (AuthN fails)
   403 Forbidden: valid key but insufficient role (AuthZ fails)
4. How to run
   dotnet test
   dotnet run --project ApiShield.Api
   Call endpoints with X-API-Key header
5. Test strategy
   Unit tests cover ApiKeyValidator rules (AAA + mocking + Fact/Theory)
   Integration tests verify pipeline outcomes (401/403/200) using WebApplicationFactory
6. What’s next
   Replace in-memory keys with real store
   Upgrade to JWT/Keycloak
   Add policy-based authorization + more observability
7. Test Matrix:

 	/secure/ping missing → 401

 	/secure/ping not present → 401

 	/secure/admin basic → 403

 	/secure/admin admin → 200

