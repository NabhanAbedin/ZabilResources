using Zabil.Api.Models.Enums;

namespace Zabil.Api.Models.Entities;

public class FbPost
{
    public Guid Id { get; set; }
    public string FbPostId { get; set; } = string.Empty;
    public FbPostType PostType { get; set; }
    public string? Message { get; set; }
    public string PermalinkUrl { get; set; } = string.Empty;
    public bool IsShare { get; set; }
    public string? OriginalAuthorName { get; set; }
    public string? OriginalPermalinkUrl { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime SyncedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public PostCategory Category { get; set; }
    public FbPostStatus Status { get; set; }
    public bool Featured { get; set; }
    public int? DisplayOrder { get; set; }
    public string? AdminTitle { get; set; }

    public ICollection<FbMedia> Media { get; set; } = new List<FbMedia>();
}
