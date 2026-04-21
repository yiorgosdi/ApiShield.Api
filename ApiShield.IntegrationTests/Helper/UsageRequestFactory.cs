namespace ApiShield.IntegrationTests.Helper;

public static class UsageRequestFactory
{
    public static HttpRequestMessage CreateIncrementRequest(string apiKey, string idempotencyKey)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/secure/usage/increment");
        request.Headers.Add("X-API-Key", apiKey);
        request.Headers.Add("X-Idempotency-Key", idempotencyKey);
        return request;
    }

    public static HttpRequestMessage CreateTodayRequest(string apiKey)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/secure/usage/today");
        request.Headers.Add("X-API-Key", apiKey);
        return request;
    }
}