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

&nbsp;	/secure/ping missing → 401

&nbsp;	/secure/ping not present → 401

&nbsp;	/secure/admin basic → 403

&nbsp;	/secure/admin admin → 200



