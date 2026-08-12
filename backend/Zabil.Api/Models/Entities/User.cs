using Zabil.Api.Models.Enums;

namespace Zabil.Api.Models.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? AvatarUrl { get; set; }
    public UserRole Role { get; set; } = UserRole.User;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserIdentity> Identities { get; set; } = new List<UserIdentity>();
    public ICollection<UserPost> Posts { get; set; } = new List<UserPost>();
}
