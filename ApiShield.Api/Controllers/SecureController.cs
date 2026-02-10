using ApiShield.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiShield.Api.Controllers;

[ApiController]
[Route("secure")]
public class SecureController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping()
        => Ok(new { message = "pong" });

    [Authorize(Roles = "admin")]
    [HttpGet("admin")]
    public IActionResult Admin()
        => Ok(new { message = "admin pong" });
}
