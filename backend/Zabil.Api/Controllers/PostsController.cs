using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zabil.Api.Models.DTOs;
using Zabil.Api.Services.Interfaces;

namespace Zabil.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly IPostsService _postsService;

    public PostsController(IPostsService postsService)
    {
        _postsService = postsService;
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePostAsync([FromForm] CreatePostFormDto formDto)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

        var result = await _postsService.CreatePostAsync(formDto, userId);
        if (!result.Success) return BadRequest(result.Error);

        return Created();
    }
} 