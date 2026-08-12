using Zabil.Api.Models.Enums;

namespace Zabil.Api.Models.Entities;

public class FbMedia
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public FbMediaType MediaType { get; set; }
    public string S3Url { get; set; } = string.Empty;
    public string? ThumbnailS3Url { get; set; }
    public bool IsOriginalMedia { get; set; }
    public int Position { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }

    public FbPost Post { get; set; } = null!;
}
