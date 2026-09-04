using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Zabil.Api.Data;
using Zabil.Api.Models.DTOs;
using Zabil.Api.Models.Entities;
using Zabil.Api.Models.Enums;
using Zabil.Api.Services.Implementations;
using Zabil.Api.Services.Interfaces;

namespace Zabil.Tests.Services;

// PostsService now depends on IS3Service instead of constructing AWS clients directly,
// so the upload/rollback logic (previously untestable) can be exercised with a mock.
public class PostsServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ZabilContext _context;
    private readonly Mock<IS3Service> _s3Service = new();
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

        _service = new PostsService(_context, NullLogger<PostsService>.Instance, _s3Service.Object);
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

    private static IFormFile CreateFakeFile(string fileName, string contentType)
    {
        var stream = new MemoryStream([1, 2, 3]);
        var file = new Mock<IFormFile>();
        file.Setup(f => f.FileName).Returns(fileName);
        file.Setup(f => f.ContentType).Returns(contentType);
        file.Setup(f => f.OpenReadStream()).Returns(stream);
        return file.Object;
    }

    private static CreatePostFormDto BuildFormDto(params IFormFile[] mediaFiles) => new()
    {
        Title = "Hello",
        Message = "World",
        Category = PostCategory.ContentFeed,
        Status = UserPostStatus.Draft,
        MediaFiles = [.. mediaFiles],
    };

    [Fact]
    public async Task CreatePostAsync_CreatesPost_WhenNoMediaProvided()
    {
        var user = await SeedUserAsync();

        var result = await _service.CreatePostAsync(BuildFormDto(), user.Id);

        Assert.True(result.Success);

        var post = await _context.UserPosts.SingleAsync();
        Assert.Equal(user.Id, post.UserId);
        Assert.Equal("Hello", post.Title);
        Assert.Equal("World", post.Message);
        Assert.Equal(PostCategory.ContentFeed, post.Category);
        Assert.Equal(UserPostStatus.Draft, post.Status);
        Assert.Empty(post.Media);
        _s3Service.Verify(
            s => s.UploadAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task CreatePostAsync_PropagatesException_WhenSaveChangesFails()
    {
        var user = await SeedUserAsync();

        // Invalidates the connection so SaveChangesAsync fails — regression test for the
        // fix that stopped swallowing infrastructure/DB failures into Result<bool>.Fail.
        await _connection.CloseAsync();

        await Assert.ThrowsAsync<DbUpdateException>(() => _service.CreatePostAsync(BuildFormDto(), user.Id));
    }

    [Fact]
    public async Task CreatePostAsync_UploadsMediaAndCreatesPost_WhenMediaProvided()
    {
        var user = await SeedUserAsync();
        var file = CreateFakeFile("photo.jpg", "image/jpeg");
        _s3Service
            .Setup(s => s.UploadAsync(It.IsAny<string>(), It.IsAny<Stream>(), "image/jpeg"))
            .ReturnsAsync("uploaded");

        var result = await _service.CreatePostAsync(BuildFormDto(file), user.Id);

        Assert.True(result.Success);

        var post = await _context.UserPosts.Include(p => p.Media).SingleAsync();
        var media = Assert.Single(post.Media);
        Assert.Equal(UserPostMediaType.Image, media.MediaType);
        Assert.StartsWith($"user-posts/{post.Id}/", media.S3Url);
        _s3Service.Verify(
            s => s.UploadAsync(It.IsAny<string>(), It.IsAny<Stream>(), "image/jpeg"),
            Times.Once);
    }

    [Fact]
    public async Task CreatePostAsync_RollsBackOnlySuccessfulUploads_WhenUploadFailsPartway()
    {
        var user = await SeedUserAsync();
        var file1 = CreateFakeFile("one.jpg", "image/jpeg");
        var file2 = CreateFakeFile("two.jpg", "image/jpeg");

        _s3Service
            .SetupSequence(s => s.UploadAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync("ok")
            .ThrowsAsync(new InvalidOperationException("S3 network error"));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreatePostAsync(BuildFormDto(file1, file2), user.Id));

        // Only the file that actually finished uploading gets rolled back — the failed
        // one was never added to uploadedKeys, so there's nothing to delete for it.
        _s3Service.Verify(s => s.DeleteAsync(It.IsAny<string>()), Times.Once);
        Assert.Empty(_context.UserPosts);
    }

    [Fact]
    public async Task CreatePostAsync_RollsBackAndReturnsFail_WhenContentTypeIsUnsupported()
    {
        var user = await SeedUserAsync();
        var file = CreateFakeFile("doc.pdf", "application/pdf");
        _s3Service
            .Setup(s => s.UploadAsync(It.IsAny<string>(), It.IsAny<Stream>(), "application/pdf"))
            .ReturnsAsync("ok");

        var result = await _service.CreatePostAsync(BuildFormDto(file), user.Id);

        Assert.False(result.Success);
        Assert.Contains("application/pdf", result.Error);
        _s3Service.Verify(s => s.DeleteAsync(It.IsAny<string>()), Times.Once);
        Assert.Empty(_context.UserPosts);
    }

    [Fact]
    public async Task CreatePostAsync_RollsBackAllUploads_WhenSaveChangesFailsAfterSuccessfulUpload()
    {
        var user = await SeedUserAsync();
        var file = CreateFakeFile("photo.jpg", "image/jpeg");
        _s3Service
            .Setup(s => s.UploadAsync(It.IsAny<string>(), It.IsAny<Stream>(), "image/jpeg"))
            .ReturnsAsync("ok");

        await _connection.CloseAsync();

        await Assert.ThrowsAsync<DbUpdateException>(() => _service.CreatePostAsync(BuildFormDto(file), user.Id));

        _s3Service.Verify(s => s.DeleteAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task CreatePostAsync_PropagatesOriginalException_WhenRollbackAlsoFailsAfterSaveFailure()
    {
        var user = await SeedUserAsync();
        var file = CreateFakeFile("photo.jpg", "image/jpeg");
        _s3Service
            .Setup(s => s.UploadAsync(It.IsAny<string>(), It.IsAny<Stream>(), "image/jpeg"))
            .ReturnsAsync("ok");
        _s3Service
            .Setup(s => s.DeleteAsync(It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("rollback also failed"));

        await _connection.CloseAsync();

        // The original DB failure must win — the rollback failure is caught and logged
        // inside RollBackUploadsAsync's own per-key try/catch, never allowed to mask it.
        await Assert.ThrowsAsync<DbUpdateException>(() => _service.CreatePostAsync(BuildFormDto(file), user.Id));
    }
}
