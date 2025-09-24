using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // Add your authentication methods here
    [HttpPost("login")]
    public async  Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var authResponse = await _authService.LoginAsync(request);
        return Ok(authResponse);
    }
}