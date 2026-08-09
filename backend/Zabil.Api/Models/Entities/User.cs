using Zabil.Api.Models.Enums;

namespace Zabil.Api.Models.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? AvatarUrl { get; set; }
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<UserIdentity> Identities { get; set; } = new List<UserIdentity>();
    public ICollection<UserPost> Posts { get; set; } = new List<UserPost>();
}
