using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Zabil.Api.Data;
using Zabil.Api.Models.DTOs;
using Zabil.Api.Models.Entities;
using Zabil.Api.Models.Enums;
using Zabil.Api.Models.Options;
using Zabil.Api.Services.Implementations;

namespace Zabil.Tests.Services;

// NOTE: PostsService constructs AmazonSecurityTokenServiceClient/AmazonS3Client directly
// (not injected), so there's no seam to fake AWS here. These tests only cover the
// no-media path; the upload/rollback logic isn't unit-testable without refactoring
// PostsService to accept the S3 client(s) as a dependency.
public class PostsServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ZabilContext _context;
    private readonly PostsService _service;

    public PostsServiceTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ZabilContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new ZabilContext(options);
        _context.Database.EnsureCreated();

        _service = new PostsService(
            _context,
            Options.Create(new AwsOptions()),
            NullLogger<PostsService>.Instance);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    private async Task<User> SeedUserAsync()
    {
        var user = new User { Name = "Test Admin", Email = "admin@example.com", Role = UserRole.Admin };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    [Fact]
    public async Task CreatePostAsync_CreatesPost_WhenNoMediaProvided()
    {
        var user = await SeedUserAsync();
        var formDto = new CreatePostFormDto
        {
            Title = "Hello",
            Message = "World",
            Category = PostCategory.ContentFeed,
            Status = UserPostStatus.Draft,
            MediaFiles = [],
        };

        var result = await _service.CreatePostAsync(formDto, user.Id);

        Assert.True(result.Success);

        var post = await _context.UserPosts.SingleAsync();
        Assert.Equal(user.Id, post.UserId);
        Assert.Equal("Hello", post.Title);
        Assert.Equal("World", post.Message);
        Assert.Equal(PostCategory.ContentFeed, post.Category);
        Assert.Equal(UserPostStatus.Draft, post.Status);
        Assert.Empty(post.Media);
    }

    [Fact]
    public async Task CreatePostAsync_PropagatesException_WhenSaveChangesFails()
    {
        var user = await SeedUserAsync();
        var formDto = new CreatePostFormDto
        {
            Title = "Hello",
            Message = "World",
            Category = PostCategory.ContentFeed,
            Status = UserPostStatus.Draft,
            MediaFiles = [],
        };

        // Invalidates the connection so SaveChangesAsync fails — regression test for the
        // fix that stopped swallowing infrastructure/DB failures into Result<bool>.Fail.
        await _connection.CloseAsync();

        await Assert.ThrowsAsync<DbUpdateException>(() => _service.CreatePostAsync(formDto, user.Id));
    }
}
