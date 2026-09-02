using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Microsoft.Extensions.Options;
using Zabil.Api.Common;
using Zabil.Api.Data;
using Zabil.Api.Models.DTOs;
using Zabil.Api.Models.Entities;
using Zabil.Api.Models.Enums;
using Zabil.Api.Models.Options;
using Zabil.Api.Services.Interfaces;

namespace Zabil.Api.Services.Implementations;

public class PostsService : IPostsService
{
    private readonly ZabilContext _context;
    private readonly AwsOptions _options;
    private readonly ILogger<PostsService> _logger;

    public PostsService(ZabilContext context, IOptions<AwsOptions> options, ILogger<PostsService> logger)
    {
        _context = context;
        _options = options.Value;
        _logger = logger;
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
            using var stsClient = new AmazonSecurityTokenServiceClient(
                _options.AccessKeyId,
                _options.SecretAccessKey,
                RegionEndpoint.GetBySystemName(_options.Region));

            var assumeRoleResponse = await stsClient.AssumeRoleAsync(new AssumeRoleRequest
            {
                RoleArn = _options.S3RoleArn,
                RoleSessionName = "railway-backend-user"
            });

            var temp = assumeRoleResponse.Credentials;

            var sessionCredentials = new SessionAWSCredentials(
                temp.AccessKeyId,
                temp.SecretAccessKey,
                temp.SessionToken
            );

            using var s3Client =
                new AmazonS3Client(sessionCredentials, RegionEndpoint.GetBySystemName(_options.Region));
            var transferutility = new TransferUtility(s3Client);
            
            try
            {
                foreach (var file in formDto.MediaFiles)
                {
                    var key = $"user-posts/{postId}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    await using var stream = file.OpenReadStream();

                    await transferutility.UploadAsync(new TransferUtilityUploadRequest
                    {
                        InputStream = stream,
                        BucketName = _options.BucketName,
                        Key = key,
                        ContentType = file.ContentType
                    });
                
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
                await RollBackUploadsAsync(s3Client, uploadedKeys);
                return Result<bool>.Fail(e.Message);
            }
            catch (Exception)
            {
                await RollBackUploadsAsync(s3Client, uploadedKeys);
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
                    using var stsClient = new AmazonSecurityTokenServiceClient(
                        _options.AccessKeyId,
                        _options.SecretAccessKey,
                        RegionEndpoint.GetBySystemName(_options.Region));

                    var assumeRoleResponse = await stsClient.AssumeRoleAsync(new AssumeRoleRequest
                    {
                        RoleArn = _options.S3RoleArn,
                        RoleSessionName = "railway-backend-user"
                    });

                    var temp = assumeRoleResponse.Credentials;
                    var sessionCredentials = new SessionAWSCredentials(
                        temp.AccessKeyId, temp.SecretAccessKey, temp.SessionToken);

                    using var s3Client =
                        new AmazonS3Client(sessionCredentials, RegionEndpoint.GetBySystemName(_options.Region));

                    await RollBackUploadsAsync(s3Client, uploadedKeys);
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

    private async Task RollBackUploadsAsync(AmazonS3Client s3Client, List<string> keys)
    {
        foreach (var key in keys)
        {
            try
            {
                await s3Client.DeleteObjectAsync(_options.BucketName, key);
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