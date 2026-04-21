using ApiShield.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _sql =
        new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Your_password123") // NOTE: strong passcode, or else does not initiate. 
            .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // 1) delete production DbContext registration
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<ApiShieldDbContext>));

            if (descriptor is not null)
                services.Remove(descriptor);

            // 2) re-write DbContext to indicate to container SQL Server
            services.AddDbContext<ApiShieldDbContext>(opt =>
                opt.UseSqlServer(_sql.GetConnectionString()));

            // 3) (Optional but useful) Override config for demo keys
            // if demo keys remain in appsettings.Development.json,
            // and also in testing. 
        });
    }

    public async Task InitializeAsync()
    {
        await _sql.StartAsync();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApiShieldDbContext>();

        // choice-A: if i have migrations
        await db.Database.MigrateAsync(); 

        // choice-Β: no migrations yet
        // await db.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
        => await _sql.DisposeAsync();
}