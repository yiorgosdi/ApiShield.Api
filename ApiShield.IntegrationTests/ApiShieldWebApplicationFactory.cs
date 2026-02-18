using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace ApiShield.IntegrationTests;

public sealed class ApiShieldWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            // override config for tests
            var testSettings = new Dictionary<string, string?>
            {
                // ApiKeyAuth:Keys is an array -> use index-based keys
                ["ApiKeyAuth:Keys:0:Key"] = TestKeys.AdminKey,
                ["ApiKeyAuth:Keys:0:Role"] = "admin",
                ["ApiKeyAuth:Keys:1:Key"] = TestKeys.BasicKey,
                ["ApiKeyAuth:Keys:1:Role"] = "basic"
            };

            config.AddInMemoryCollection(testSettings);
        });
    }
}
