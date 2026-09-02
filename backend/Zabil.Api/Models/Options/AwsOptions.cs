namespace Zabil.Api.Models.Options;

public class AwsOptions
{
    public string AccessKeyId { get; set; } = string.Empty;
    public string SecretAccessKey { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string S3RoleArn { get; set; } = string.Empty;
    public string BucketName { get; set; } = "zabil-resources-assets";
}