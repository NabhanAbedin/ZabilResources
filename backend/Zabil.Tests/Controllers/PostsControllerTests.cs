using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Zabil.Api.Common;
using Zabil.Api.Controllers;
using Zabil.Api.Models.DTOs;
using Zabil.Api.Services.Interfaces;

namespace Zabil.Tests.Controllers;

public class PostsControllerTests
{
    private readonly Mock<IPostsService> _postsService = new();
    private readonly PostsController _controller;

    public PostsControllerTests()
    {
        _controller = new PostsController(_postsService.Object);
    }

    private void SetUser(Guid? userId)
    {
        var claims = userId is null
            ? Array.Empty<Claim>()
            : [new Claim("userId", userId.Value.ToString())];

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims)),
            },
        };
    }

    [Fact]
    public async Task CreatePostAsync_ReturnsUnauthorized_WhenUserIdClaimIsMissing()
    {
        SetUser(userId: null);

        var result = await _controller.CreatePostAsync(new CreatePostFormDto());

        Assert.IsType<UnauthorizedResult>(result);
        _postsService.Verify(
            s => s.CreatePostAsync(It.IsAny<CreatePostFormDto>(), It.IsAny<Guid>()),
            Times.Never);
    }

    [Fact]
    public async Task CreatePostAsync_ReturnsUnauthorized_WhenUserIdClaimIsNotAGuid()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("userId", "not-a-guid")])),
            },
        };

        var result = await _controller.CreatePostAsync(new CreatePostFormDto());

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task CreatePostAsync_ReturnsCreated_WhenServiceSucceeds()
    {
        var userId = Guid.NewGuid();
        SetUser(userId);
        _postsService
            .Setup(s => s.CreatePostAsync(It.IsAny<CreatePostFormDto>(), userId))
            .ReturnsAsync(Result<bool>.Ok(true));

        var result = await _controller.CreatePostAsync(new CreatePostFormDto());

        Assert.IsType<CreatedResult>(result);
    }

    [Fact]
    public async Task CreatePostAsync_ReturnsBadRequestWithError_WhenServiceFails()
    {
        var userId = Guid.NewGuid();
        SetUser(userId);
        _postsService
            .Setup(s => s.CreatePostAsync(It.IsAny<CreatePostFormDto>(), userId))
            .ReturnsAsync(Result<bool>.Fail("Unsupported media content type: application/pdf"));

        var result = await _controller.CreatePostAsync(new CreatePostFormDto());

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Unsupported media content type: application/pdf", badRequest.Value);
    }

    [Fact]
    public async Task CreatePostAsync_PassesParsedUserIdAndFormDtoToService()
    {
        var userId = Guid.NewGuid();
        SetUser(userId);
        var formDto = new CreatePostFormDto { Title = "Hello", Message = "World" };
        _postsService
            .Setup(s => s.CreatePostAsync(formDto, userId))
            .ReturnsAsync(Result<bool>.Ok(true));

        await _controller.CreatePostAsync(formDto);

        _postsService.Verify(s => s.CreatePostAsync(formDto, userId), Times.Once);
    }
}
