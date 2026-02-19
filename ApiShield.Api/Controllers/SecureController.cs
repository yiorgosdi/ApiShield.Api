using ApiShield.Api.Security.AuthConstants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiShield.Api.Controllers;

[ApiController]
[Route("secure")]
public class SecureController : ControllerBase
{
    [Authorize(AuthenticationSchemes = AuthSchemes.ApiKey)]
    [HttpGet("ping")]
    public IActionResult Ping()
        => Ok(new { message = "pong" });

    [Authorize(AuthenticationSchemes = AuthSchemes.ApiKey, Roles = AuthRoles.Admin)]
    [HttpGet("admin")]
    public IActionResult Admin()
        => Ok(new { message = "admin pong" });
}
