using Fundo.Applications.WebApi.Models;
using Fundo.Applications.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fundo.Applications.WebApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(ITokenService tokenService, IConfiguration configuration) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var users = configuration.GetSection("Users").Get<UserConfig[]>() ?? [];

        var user = users.FirstOrDefault(u =>
            u.Username == request.Username &&
            u.Password == request.Password);

        if (user is null)
            return Unauthorized(new { error = "Invalid credentials." });

        var token = tokenService.GenerateToken(request.Username);
        return Ok(token);
    }
}

internal sealed record UserConfig(string Username, string Password);
