using System.IdentityModel.Tokens.Jwt;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Zabil.Api.Data;
using Zabil.Api.Models.Entities;
using Zabil.Api.Models.Enums;
using Zabil.Api.Services.Implementations;

namespace Zabil.Tests.Services;

public class JWTServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ZabilContext _context;
    private readonly JWTService _service;

    public JWTServiceTests()
    {
        // The real (Npgsql) provider isn't available in tests, and EF Core's
        // InMemory provider can't run BeginTransactionAsync — SQLite over a
        // held-open connection supports real transactions, so it's the
        // closest fake to how JWTService actually behaves in production.
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ZabilContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new ZabilContext(options);
        _context.Database.EnsureCreated();

        var configuration = new Mock<IConfiguration>();
        configuration.Setup(c => c["Jwt:key"]).Returns("test-signing-key-that-is-long-enough-for-hmacsha256");
        configuration.Setup(c => c["Jwt:Issuer"]).Returns("ZabilApi");
        configuration.Setup(c => c["Jwt:audience"]).Returns("ZabilClient");

        _service = new JWTService(_context, configuration.Object, NullLogger<JWTService>.Instance);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task GenerateToken_CreatesNewUserAndIdentity_WhenProviderUserIdNotFound()
    {
        var result = await _service.GenerateToken("google-sub-123", "Jane Doe", "jane@example.com");

        Assert.True(result.Success);
        Assert.NotNull(result.Data);

        var user = await _context.Users.SingleAsync();
        Assert.Equal("Jane Doe", user.Name);
        Assert.Equal("jane@example.com", user.Email);

        var identity = await _context.UserIdentities.SingleAsync();
        Assert.Equal("google-sub-123", identity.ProviderUserId);
        Assert.Equal(IdentityProvider.Google, identity.Provider);
        Assert.Equal(user.Id, identity.UserId);
    }

    [Fact]
    public async Task GenerateToken_ReusesExistingUser_WhenProviderUserIdAlreadyExists()
    {
        var existingUser = new User { Name = "Existing User", Email = "existing@example.com", Role = UserRole.Admin };
        _context.Users.Add(existingUser);
        _context.UserIdentities.Add(new UserIdentity
        {
            UserId = existingUser.Id,
            ProviderUserId = "google-sub-999",
            Provider = IdentityProvider.Google,
            ProviderEmail = "existing@example.com",
        });
        await _context.SaveChangesAsync();

        var result = await _service.GenerateToken("google-sub-999", "Ignored Name", "ignored@example.com");

        Assert.True(result.Success);
        Assert.Single(_context.Users);

        var claims = ReadClaims(result.Data!);
        Assert.Equal(existingUser.Id.ToString(), claims["userId"]);
        Assert.Equal("Admin", claims["Role"]);
    }

    [Fact]
    public async Task GenerateToken_EmbedsExpectedClaimsIssuerAudienceAndExpiry()
    {
        var beforeCall = DateTime.UtcNow;

        var result = await _service.GenerateToken("google-sub-456", "John Smith", "john@example.com");

        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.Data);

        Assert.Equal("ZabilApi", token.Issuer);
        Assert.Contains("ZabilClient", token.Audiences);
        Assert.Equal("john@example.com", token.Claims.First(c => c.Type == "Email").Value);
        Assert.Equal("User", token.Claims.First(c => c.Type == "Role").Value);
        Assert.InRange(token.ValidTo, beforeCall.AddMinutes(59), beforeCall.AddMinutes(61));
    }

    private static Dictionary<string, string> ReadClaims(string jwt)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(jwt);
        return token.Claims.ToDictionary(c => c.Type, c => c.Value);
    }
}
