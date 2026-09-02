using Zabil.Api.Models.Enums;

namespace Zabil.Api.Models.DTOs;

public class CreatePostFormDto
{
    public string Title { get; set; }
    public string Message { get; set; }
    public PostCategory Category { get; set; }
    public UserPostStatus Status { get; set; }

    public List<IFormFile> MediaFiles { get; set; } = [];
}