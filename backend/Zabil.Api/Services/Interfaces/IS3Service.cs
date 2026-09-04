using Zabil.Api.Common;

namespace Zabil.Api.Services.Interfaces;

public interface IS3Service
{
    Task<string> UploadAsync(string key, Stream content, string contentType);
    Task DeleteAsync(string key);
    Task<string> GetPresignedUrlAsync(string key, TimeSpan expiry);
}