using Zabil.Api.Common;
using Zabil.Api.Data;
using Zabil.Api.Models.DTOs;
using Zabil.Api.Models.Entities;
using Zabil.Api.Models.Enums;
using Zabil.Api.Services.Interfaces;

namespace Zabil.Api.Services.Implementations;

public class PostsService : IPostsService
{
    private readonly ZabilContext _context;
    private readonly ILogger<PostsService> _logger;
    private readonly IS3Service _s3Service;

    public PostsService(ZabilContext context, ILogger<PostsService> logger, IS3Service s3Service)
    {
        _context = context;
        _logger = logger;
        _s3Service = s3Service;
    }

    public async Task<Result<bool>> CreatePostAsync(CreatePostFormDto formDto, Guid userId)
    {
        var postId = Guid.NewGuid();
        
        var post = new UserPost
        {
            Id = postId,
            UserId = userId,
            Title = formDto.Title,
            Message = formDto.Message,
            Category = formDto.Category,
            Status = formDto.Status
        };
        
        var uploadedKeys = new List<string>();
        
        if (formDto.MediaFiles.Count != 0)
        {
            
            
            try
            {
                foreach (var file in formDto.MediaFiles)
                {
                    var key = $"user-posts/{postId}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    await using var streamContent = file.OpenReadStream();

                    await _s3Service.UploadAsync(key, streamContent, file.ContentType);
                
                    uploadedKeys.Add(key);
                
                    post.Media.Add(new UserPostMedia
                    {
                        S3Url = key,
                        MediaType = GetMediaType(file.ContentType)
                    });
                }
            }
            catch (NotSupportedException e)
            {
                await RollBackUploadsAsync(uploadedKeys);
                return Result<bool>.Fail(e.Message);
            }
            catch (Exception)
            {
                await RollBackUploadsAsync(uploadedKeys);
                throw;
            }
        }

        try
        {
            _context.UserPosts.Add(post);
            await _context.SaveChangesAsync();
            
            return Result<bool>.Ok(true);
        }
        catch (Exception)
        {
            if (uploadedKeys.Count > 0)
            {
                try
                {
                    
                    await RollBackUploadsAsync(uploadedKeys);
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(
                        rollbackEx,
                        "Failed to roll back media after post save failure. Orphaned keys: {Keys}",
                        string.Join(", ", uploadedKeys));
                }
            }

            throw; 
        }
    }

    private async Task RollBackUploadsAsync(List<string> keys)
    {
        foreach (var key in keys)
        {
            try
            {
                await _s3Service.DeleteAsync(key);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to roll back S3 object with key {Key}", key);
            }
        }
    }
    
    private static UserPostMediaType GetMediaType(string contentType)
    {
        if (contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return UserPostMediaType.Image;
        if (contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
            return UserPostMediaType.Video;

        throw new NotSupportedException($"Unsupported media content type: {contentType}");
    }
    
}