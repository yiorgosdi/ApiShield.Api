using ApiShield.Api.Auth;
using ApiShield.Api.Extensions;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApiKeyAuth(builder.Configuration);

var app = builder.Build();

// Minimal exception-to-HTTP mapping (for 401 instead of 500)
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

app.UseAuthentication();
app.UseMiddleware<ApiKeyAuthMiddleware>(); // sets HttpContext.User for /secure
app.UseAuthorization();

app.MapControllers();
app.Run();

public partial class Program { }