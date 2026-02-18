using ApiShield.Api.Auth;
using ApiShield.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using ApiShield.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApiKeyAuth(builder.Configuration);

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

var app = builder.Build();

// Minimal exception-to-HTTP mapping (for 401 instead of 500). 
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var ex = feature?.Error;

        if (ex is ArgumentException or UnauthorizedAccessException)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = ex?.Message
            };

            await context.Response.WriteAsJsonAsync(problem);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync("Internal Server Error");
    });
});

// Serve static files (wwwroot/index.html)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseAuthentication();
app.UseMiddleware<ApiKeyAuthMiddleware>(); // sets HttpContext.User for /secure
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/health", () => Results.Ok("ok"));

app.MapGet("/info", () => Results.Json(new
{
    name = "ApiShield",
    env = app.Environment.EnvironmentName,
    auth = new { header = "X-API-Key", schemes = new[] { "ApiKey" } },
    endpoints = new[] { "/secure/ping", "/secure/admin" },
    docs = "/swagger",
    repo = "https://github.com/yiorgosdi/ApiShield.Api",
    live = "https://apishield-george-hfcsh9hzhjf2c6g6.westeurope-01.azurewebsites.net"
}));

app.MapControllers();
app.Run();

public partial class Program { }