using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Zabil.Tests.Fakes;

// Issues JWTs shaped exactly like JWTService's real output, signed with a key that
// PostsApiFactory overrides the app's config to use — so integration tests exercise
// the real [Authorize(Roles = "Admin")] pipeline rather than bypassing it.
public static class TestJwtFactory
{
    public const string SigningKey = "integration-test-signing-key-that-is-long-enough-1234567890";
    public const string Issuer = "ZabilApi";
    public const string Audience = "ZabilClient";

    public static string CreateToken(string role, Guid? userId = null, string email = "test@example.com")
    {
        var key = Encoding.UTF8.GetBytes(SigningKey);
        var handler = new JwtSecurityTokenHandler();

        var claims = new[]
        {
            new Claim("userId", (userId ?? Guid.NewGuid()).ToString()),
            new Claim("Email", email),
            new Claim("Role", role),
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = Issuer,
            Audience = Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
        };

        return handler.WriteToken(handler.CreateToken(descriptor));
    }
}
