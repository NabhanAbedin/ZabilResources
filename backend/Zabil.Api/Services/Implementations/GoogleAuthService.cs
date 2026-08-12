using Google.Apis.Auth;
using Zabil.Api.Common;
using Zabil.Api.Data;
using Zabil.Api.Models.DTOs;
using Zabil.Api.Services.Interfaces;

namespace Zabil.Api.Services.Implementations;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GoogleAuthService> _logger;

    public GoogleAuthService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<GoogleAuthService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Result<GoogleUserInfo>> ExchangeAndValidateAsync(string code)
    {
        var client = _httpClientFactory.CreateClient("Google");
        var clientId = _configuration["Google:ClientId"];

        var requestBody = new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", clientId },
            { "client_secret", _configuration["Google:ClientSecret"] },
            { "redirect_uri", _configuration["Google:RedirectUri"] },
            { "grant_type", "authorization_code" }
        };

        var response = await client.PostAsync(
            _configuration["Google:TokenEndpoint"],
            new FormUrlEncodedContent(requestBody)
        );

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Google token exchange failed with status {StatusCode}", response.StatusCode);
            return Result<GoogleUserInfo>.Fail("Invalid or expired authorization code");
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>();

        if (tokenResponse == null) return Result<GoogleUserInfo>.Fail("Could not authenticate user.");

        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(
                tokenResponse.IdToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { clientId }
                });
        }
        catch (InvalidJwtException e)
        {
            _logger.LogError(e, "Google ID token validation failed");
            return Result<GoogleUserInfo>.Fail("Invalid ID token");
        }

        return Result<GoogleUserInfo>.Ok(new GoogleUserInfo
        {
            Sub = payload.Subject,
            Email = payload.Email,
            EmailVerified = payload.EmailVerified,
            Name = payload.Name,
            PictureUrl = payload.Picture
        });
    }
}
