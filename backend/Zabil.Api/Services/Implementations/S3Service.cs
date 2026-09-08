using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Microsoft.Extensions.Options;
using Zabil.Api.Models.Options;
using Zabil.Api.Services.Interfaces;

namespace Zabil.Api.Services.Implementations;

public class S3Service : IS3Service
{
    private readonly AwsOptions _options;
    private readonly SemaphoreSlim _credentialLock = new(1, 1);

    private SessionAWSCredentials? _cachedCredentials;
    private DateTime _cachedCredentialsExpiry = DateTime.MinValue;

    public S3Service(IOptions<AwsOptions> options)
    {
        _options = options.Value;
    }

    public async Task<string> UploadAsync(string key, Stream content, string contentType)
    {
        using var s3Client = await GetS3ClientAsync();
        var transferutility = new TransferUtility(s3Client);
        
        await transferutility.UploadAsync(new TransferUtilityUploadRequest
        {
            InputStream = content,
            BucketName = _options.BucketName,
            Key = key,
            ContentType = contentType
        });

        return key;
    }

    public async Task DeleteAsync(string key)
    {
        using var s3Client = await GetS3ClientAsync();
        await s3Client.DeleteObjectAsync(_options.BucketName, key);
    }

    public async Task<string> GetPresignedUrlAsync(string key, TimeSpan expiry)
    {
        using var s3Client = await GetS3ClientAsync();

        return s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
        {
            BucketName = _options.BucketName,
            Key = key,
            Expires = DateTime.UtcNow.Add(expiry)
        });
        
    }

    private async Task<AmazonS3Client> GetS3ClientAsync()
    {
        var credentials = await GetAWSCredentialsAsync();
        return new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName(_options.Region));
    }

    private async Task<SessionAWSCredentials> GetAWSCredentialsAsync()
    {
        if (_cachedCredentials != null && DateTime.UtcNow < _cachedCredentialsExpiry.AddMinutes(-5))
        {
            return _cachedCredentials;
        }

        await _credentialLock.WaitAsync();
        try
        {
            if (_cachedCredentials != null && DateTime.UtcNow < _cachedCredentialsExpiry.AddMinutes(-5))
            {
                return _cachedCredentials;
            }
            
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

            _cachedCredentials = new SessionAWSCredentials(
                temp.AccessKeyId,
                temp.SecretAccessKey,
                temp.SessionToken
            );
            _cachedCredentialsExpiry = temp.Expiration ?? DateTime.UtcNow;

            return _cachedCredentials;
        }
        finally
        {
            _credentialLock.Release();
        } 
    }
}