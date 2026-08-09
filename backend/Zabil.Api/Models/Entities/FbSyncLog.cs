using Zabil.Api.Models.Enums;

namespace Zabil.Api.Models.Entities;

public class FbSyncLog
{
    public int Id { get; set; }
    public string? FbPostId { get; set; }
    public SyncTrigger Trigger { get; set; }
    public SyncStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime CompletedAt { get; set; }
}
