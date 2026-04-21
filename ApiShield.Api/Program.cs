using ApiShield.Api.Endpoints;
using ApiShield.Api.Extensions;
using ApiShield.Api.Middleware;
using ApiShield.Core.Idempotency;
using ApiShield.Core.Usage;
using ApiShield.Infrastructure.BackgroundServices;
using ApiShield.Infrastructure.Persistence;
using ApiShield.Infrastructure.Queue;
using ApiShield.Infrastructure.Usage;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApiKeyAuth(builder.Configuration);// don't forget this one!

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Name = "X-API-Key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Description = "Enter your demo API key (value only)."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                  {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<ApiShieldDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Sql")));

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<IApiKeyUsageService, ApiKeyUsageService>();

builder.Services.AddRateLimiter(o =>
{
    o.AddFixedWindowLimiter("usage", opt =>
    {
        opt.PermitLimit = 60;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });
});

builder.Services.AddDbContext<ApiShieldDbContext>(options =>
{
    options.UseSqlServer("Server=localhost;Database=ApiShield;Trusted_Connection=True;TrustServerCertificate=True;");
    options.EnableSensitiveDataLogging();
    options.LogTo(Console.WriteLine, LogLevel.Information);
});

builder.Services.AddSingleton(
    Channel.CreateUnbounded<UsageIncrementRequested>());

builder.Services.AddSingleton<IUsageEventQueue, InMemoryUsageEventQueue>();

builder.Services.AddScoped<IUsageProcessingService, UsageProcessingService>();

builder.Services.AddHostedService<UsageEventsConsumer>();

builder.Services.AddScoped<IApiRequestLogRepository, ApiRequestLogRepository>();
builder.Services.AddScoped<IIdempotencyService, IdempotencyService>();

var app = builder.Build();

//app.UseExceptionHandler(errorApp =>
//{
//    errorApp.Run(async context =>
//    {
//        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
//        await context.Response.WriteAsync("Internal Server Error");
//    });
//});

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseWhen(
    ctx => !ctx.Request.Path.StartsWithSegments("/swagger"),
    secured =>
    {
        secured.UseAuthentication();
        secured.UseAuthorization();
    });

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/health", () => Results.Ok("ok")).AllowAnonymous();
app.MapGet("/info", () => Results.Json(new { version = "1.0.0", name = "ApiShield" })).AllowAnonymous();

app.MapGet("/health/db", async (ApiShieldDbContext db, CancellationToken ct) =>
{
    var canConnect = await db.Database.CanConnectAsync(ct);
    return Results.Ok(new { canConnect });
});

app.MapControllers();
app.MapSecureEndpoints();
app.MapUsageEndpoints();      

app.Run(); 

public partial class Program { }