using Microsoft.AspNetCore.Mvc;
using Moq;
using Zabil.Api.Common;
using Zabil.Api.Controllers;
using Zabil.Api.Models.DTOs;
using Zabil.Api.Services.Interfaces;

namespace Zabil.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IGoogleAuthService> _googleAuthService = new();
    private readonly Mock<IJWTService> _jwtService = new();
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _controller = new AuthController(_googleAuthService.Object, _jwtService.Object);
    }

    [Fact]
    public async Task ExchangeAndValidateAsync_ReturnsOkWithToken_WhenExchangeAndJwtSucceed()
    {
        var userInfo = new GoogleUserInfo { Sub = "google-sub-1", Name = "Jane Doe", Email = "jane@example.com" };
        _googleAuthService
            .Setup(s => s.ExchangeAndValidateAsync("valid-code"))
            .ReturnsAsync(Result<GoogleUserInfo>.Ok(userInfo));
        _jwtService
            .Setup(s => s.GenerateToken(userInfo.Sub, userInfo.Name, userInfo.Email))
            .ReturnsAsync(Result<string>.Ok("signed.jwt.token"));

        var result = await _controller.ExchangeAndValidateAsync("valid-code");

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal("signed.jwt.token", okResult.Value);
    }

    [Fact]
    public async Task ExchangeAndValidateAsync_ReturnsBadRequest_WhenGoogleExchangeFails()
    {
        _googleAuthService
            .Setup(s => s.ExchangeAndValidateAsync("bad-code"))
            .ReturnsAsync(Result<GoogleUserInfo>.Fail("Invalid or expired authorization code"));

        var result = await _controller.ExchangeAndValidateAsync("bad-code");

        Assert.IsType<BadRequestObjectResult>(result.Result);
        _jwtService.Verify(
            s => s.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task ExchangeAndValidateAsync_ReturnsBadRequest_WhenJwtGenerationFails()
    {
        var userInfo = new GoogleUserInfo { Sub = "google-sub-1", Name = "Jane Doe", Email = "jane@example.com" };
        _googleAuthService
            .Setup(s => s.ExchangeAndValidateAsync("valid-code"))
            .ReturnsAsync(Result<GoogleUserInfo>.Ok(userInfo));
        _jwtService
            .Setup(s => s.GenerateToken(userInfo.Sub, userInfo.Name, userInfo.Email))
            .ReturnsAsync(Result<string>.Fail("Failed to create user and identity"));

        var result = await _controller.ExchangeAndValidateAsync("valid-code");

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
