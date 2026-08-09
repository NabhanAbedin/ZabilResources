using Zabil.Api.Models.Enums;

namespace Zabil.Api.Models.Entities;

public class UserIdentity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public IdentityProvider Provider { get; set; }
    public string ProviderUserId { get; set; } = string.Empty;
    public string? ProviderEmail { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}
