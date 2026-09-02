using System.Net;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Zabil.Api.Data;
using Zabil.Api.Models.Entities;
using Zabil.Api.Models.Enums;
using Zabil.Tests.Fakes;

namespace Zabil.Tests.Integration;

public class PostsControllerIntegrationTests : IClassFixture<PostsApiFactory>
{
    private readonly PostsApiFactory _factory;

    public PostsControllerIntegrationTests(PostsApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreatePost_ReturnsUnauthorized_WhenNoTokenProvided()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsync("/api/posts", BuildForm());

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreatePost_ReturnsForbidden_WhenUserIsNotAdmin()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestJwtFactory.CreateToken("User"));

        var response = await client.PostAsync("/api/posts", BuildForm());

        // Proves the RoleClaimType fix: without it, this request would also come back
        // Forbidden, but for the wrong reason (IsInRole("Admin") never matches at all).
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreatePost_ReturnsCreated_WhenAdminSubmitsPostWithNoMedia()
    {
        var userId = Guid.NewGuid();

        // UserPost.UserId has a required FK to Users — PostsService doesn't create the
        // User row itself (that's JWTService's job during login), so it must pre-exist.
        using (var seedScope = _factory.Services.CreateScope())
        {
            var seedContext = seedScope.ServiceProvider.GetRequiredService<ZabilContext>();
            seedContext.Users.Add(new User { Id = userId, Name = "Test Admin", Role = UserRole.Admin });
            await seedContext.SaveChangesAsync();
        }

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestJwtFactory.CreateToken("Admin", userId));

        var response = await client.PostAsync("/api/posts", BuildForm());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ZabilContext>();
        var post = await context.UserPosts.SingleAsync(p => p.UserId == userId);
        Assert.Equal("Integration test post", post.Message);
    }

    private static MultipartFormDataContent BuildForm()
    {
        return new MultipartFormDataContent
        {
            { new StringContent("Test Title"), "Title" },
            { new StringContent("Integration test post"), "Message" },
            { new StringContent("Unclassified"), "Category" },
            { new StringContent("Draft"), "Status" },
        };
    }
}
