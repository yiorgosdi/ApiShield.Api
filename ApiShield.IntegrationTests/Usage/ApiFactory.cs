using ApiShield.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;
using Xunit;

namespace ApiShield.IntegrationTests;

public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _sql =
        new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Your_password123")
            .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            var testSettings = new Dictionary<string, string?>
            {
                ["ApiKeyAuth:Keys:0:Key"] = TestKeys.AdminKey,
                ["ApiKeyAuth:Keys:0:Role"] = "admin",
                ["ApiKeyAuth:Keys:1:Key"] = TestKeys.BasicKey,
                ["ApiKeyAuth:Keys:1:Role"] = "basic"
            };

            config.AddInMemoryCollection(testSettings);
        });

        builder.ConfigureServices(services =>
        {
            // 1. REMOVE the production DbContext
            services.RemoveAll(typeof(DbContextOptions<ApiShieldDbContext>));
            services.RemoveAll(typeof(ApiShieldDbContext));

            // 2. ADD the test DbContext (container SQL Server)
            services.AddDbContext<ApiShieldDbContext>(opt =>
                opt.UseSqlServer(_sql.GetConnectionString()));
        });
    }

    public async Task InitializeAsync()
    {
        await _sql.StartAsync();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApiShieldDbContext>();

        await db.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _sql.DisposeAsync();
    }
}