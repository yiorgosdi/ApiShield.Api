using ApiShield.Api.Auth;
using ApiShield.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApiKeyAuth(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.MapGet("/", (HttpContext ctx) =>
{
    // If a client explicitly prefers JSON, keep a tiny JSON response
    var accept = ctx.Request.Headers.Accept.ToString();
    if (accept.Contains("application/json", StringComparison.OrdinalIgnoreCase))
    {
        return Results.Json(new
        {
            name = "ApiShield",
            status = "ok",
            docs = "/swagger",
            github = "https://github.com/yiorgosdi/ApiShield.Api",
            live = "https://apishield-george-hfcsh9hzhjf2c6g6.westeurope-01.azurewebsites.net"
        });
    }

    const string html = """
<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>ApiShield</title>
  <style>
    body{font-family:system-ui,Segoe UI,Roboto,Arial;margin:0;background:#0b0f19;color:#e8eefc}
    .wrap{max-width:980px;margin:0 auto;padding:52px 20px}
    .card{background:#111a2e;border:1px solid #1e2a4a;border-radius:18px;padding:30px}
    h1{margin:0 0 10px;font-size:40px;letter-spacing:.2px}
    p{margin:10px 0 0;line-height:1.6;color:#b8c6e6;font-size:16px}
    .grid{display:grid;grid-template-columns:repeat(auto-fit,minmax(240px,1fr));gap:14px;margin-top:20px}
    a.btn{display:block;text-decoration:none;color:#e8eefc;background:#172446;border:1px solid #26355f;padding:14px 16px;border-radius:12px}
    a.btn:hover{background:#1b2b55}
    code{background:#0b1222;border:1px solid #1e2a4a;padding:2px 7px;border-radius:8px}
    .row{display:flex;flex-wrap:wrap;gap:10px;margin-top:14px}
    .badge{display:inline-block;padding:6px 10px;border-radius:999px;background:#0b1222;border:1px solid #1e2a4a;color:#9fb0d6;font-size:13px}
    .small{margin-top:18px;font-size:14px;color:#9fb0d6}
  </style>
</head>
<body>
  <div class="wrap">
    <div class="card">
      <h1>ApiShield</h1>
      <p>ASP.NET Core security lab deployed to Azure. Demonstrates <b>AuthN/AuthZ</b> (API keys, roles) and a production-style test suite (unit + integration).</p>

      <div class="row">
        <img alt="CI"
        src="https://github.com/yiorgosdi/ApiShield.Api/actions/workflows/ci.yml/badge.svg" />
        <span class="badge">401 = AuthN fail</span>
        <span class="badge">403 = AuthZ fail</span>
        <span class="badge">Azure App Service</span>
      </div>

      <div class="grid">
        <a class="btn" href="/swagger">Open Swagger</a>
        <a class="btn" href="/secure/ping">/secure/ping (needs X-API-Key)</a>
        <a class="btn" href="/secure/admin">/secure/admin (admin only)</a>
        <a class="btn" href="https://github.com/yiorgosdi/ApiShield.Api">GitHub Repository</a>
      </div>

      <p class="small">
        Header: <code>X-API-Key</code>. Demo keys are used for documentation/interview purposes.
      </p>
    </div>
  </div>
</body>
</html>
""";

    return Results.Content(html, "text/html; charset=utf-8");
});

app.UseSwagger();
app.UseSwagger();

app.MapControllers();
app.Run();

public partial class Program { }