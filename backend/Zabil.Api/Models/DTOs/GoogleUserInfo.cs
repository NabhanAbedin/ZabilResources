namespace Zabil.Api.Models.DTOs;

public class GoogleUserInfo
{
    public string Sub { get; set; }          // Google's stable user ID — your DB lookup key
    public string Email { get; set; }
    public bool EmailVerified { get; set; }
    public string Name { get; set; }
    public string? PictureUrl { get; set; }
}