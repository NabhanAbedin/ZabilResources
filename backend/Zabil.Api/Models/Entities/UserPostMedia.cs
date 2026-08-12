using Zabil.Api.Models.Enums;

namespace Zabil.Api.Models.Entities;

public class UserPostMedia
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string S3Url { get; set; } = string.Empty;
    public UserPostMediaType MediaType { get; set; }

    public UserPost Post { get; set; } = null!;
}
