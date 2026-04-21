using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace ApiShield.IntegrationTests.Helper;

public static class UsageTestHelper
{
    public static async Task<UsageTodayResponse> GetTodayUsageAsync(HttpClient client, string apiKey)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/secure/usage/today");
        request.Headers.Add("X-API-Key", apiKey);

        var res = await client.SendAsync(request);
        var body = await res.Content.ReadAsStringAsync();

        res.StatusCode.Should().Be(
            HttpStatusCode.OK,
            $"response body was: {body}");

        var payload = JsonSerializer.Deserialize<UsageTodayResponse>(
            body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        payload.Should().NotBeNull();
        return payload!;
    }

    public static async Task<UsageTodayResponse> WaitForUsageCountAsync(
        HttpClient client,
        string apiKey,
        int expectedCount)
    {
        const int maxAttempts = 20;
        var delay = TimeSpan.FromMilliseconds(150);

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            var current = await GetTodayUsageAsync(client, apiKey);

            if (current.Count >= expectedCount)
            {
                return current;
            }

            await Task.Delay(delay);
        }

        var final = await GetTodayUsageAsync(client, apiKey);
        final.Count.Should().BeGreaterThanOrEqualTo(expectedCount,
            "the background usage processor should eventually persist queued increments");

        return final;
    }

    public sealed record UsageTodayResponse(DateOnly UsageDate, int Count);
}