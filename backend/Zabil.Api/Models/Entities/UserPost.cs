using Zabil.Api.Models.Enums;

namespace Zabil.Api.Models.Entities;

public class UserPost
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? Title { get; set; }
    public string Message { get; set; } = string.Empty;
    public PostCategory Category { get; set; }
    public UserPostStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public ICollection<UserPostMedia> Media { get; set; } = new List<UserPostMedia>();
}
