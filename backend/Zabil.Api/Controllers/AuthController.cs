using Microsoft.AspNetCore.Mvc;
using Zabil.Api.Services.Interfaces;

namespace Zabil.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IJWTService _jwtService;

    public AuthController(IGoogleAuthService googleAuthService, IJWTService jwtService)
    {
        _googleAuthService = googleAuthService;
        _jwtService = jwtService;
    }

    [HttpPost("google/exchange")]
    public async Task<ActionResult<string>> ExchangeAndValidateAsync([FromQuery] string code)
    {
        var exchangeResult = await _googleAuthService.ExchangeAndValidateAsync(code);
        if (!exchangeResult.Success) return BadRequest("Could not authenticate user");

        var JWTResult = await _jwtService.GenerateToken(exchangeResult.Data.Sub, exchangeResult.Data.Name, exchangeResult.Data.Email);
        if (!JWTResult.Success) return BadRequest("Could not authenticate user");

        return Ok(JWTResult.Data);
    }
}