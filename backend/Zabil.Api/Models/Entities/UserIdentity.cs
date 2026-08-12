using Zabil.Api.Models.Enums;

namespace Zabil.Api.Models.Entities;

public class UserIdentity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public IdentityProvider Provider { get; set; }
    public string ProviderUserId { get; set; } = string.Empty;
    public string? ProviderEmail { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
