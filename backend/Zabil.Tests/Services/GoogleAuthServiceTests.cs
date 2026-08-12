using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Zabil.Api.Services.Implementations;
using Zabil.Tests.Fakes;

namespace Zabil.Tests.Services;

public class GoogleAuthServiceTests
{
    private static GoogleAuthService CreateService(HttpResponseMessage response)
    {
        var handler = new FakeHttpMessageHandler(response);
        var client = new HttpClient(handler);

        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(f => f.CreateClient("Google")).Returns(client);

        var configuration = new Mock<IConfiguration>();
        configuration.Setup(c => c["Google:ClientId"]).Returns("test-client-id");
        configuration.Setup(c => c["Google:ClientSecret"]).Returns("test-client-secret");
        configuration.Setup(c => c["Google:RedirectUri"]).Returns("http://localhost:5173/oauth/callback");
        configuration.Setup(c => c["Google:TokenEndpoint"]).Returns("https://oauth2.googleapis.com/token");

        return new GoogleAuthService(
            httpClientFactory.Object,
            configuration.Object,
            NullLogger<GoogleAuthService>.Instance);
    }

    [Fact]
    public async Task ExchangeAndValidateAsync_ReturnsFail_WhenTokenEndpointReturnsNonSuccessStatus()
    {
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("{\"error\":\"invalid_grant\"}"),
        };
        var service = CreateService(response);

        var result = await service.ExchangeAndValidateAsync("expired-code");

        Assert.False(result.Success);
        Assert.Equal("Invalid or expired authorization code", result.Error);
    }

    [Fact]
    public async Task ExchangeAndValidateAsync_ReturnsFail_WhenTokenResponseBodyIsNull()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null", System.Text.Encoding.UTF8, "application/json"),
        };
        var service = CreateService(response);

        var result = await service.ExchangeAndValidateAsync("some-code");

        Assert.False(result.Success);
        Assert.Equal("Could not authenticate user.", result.Error);
    }

    [Fact]
    public async Task ExchangeAndValidateAsync_ReturnsFail_WhenIdTokenIsNotAValidGoogleSignature()
    {
        var body = """
            {
              "access_token": "fake-access-token",
              "id_token": "not-a-real-jwt",
              "expires_in": 3600,
              "token_type": "Bearer"
            }
            """;
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json"),
        };
        var service = CreateService(response);

        var result = await service.ExchangeAndValidateAsync("some-code");

        Assert.False(result.Success);
        Assert.Equal("Invalid ID token", result.Error);
    }
}
