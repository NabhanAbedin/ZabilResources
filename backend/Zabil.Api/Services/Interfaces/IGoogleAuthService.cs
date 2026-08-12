using Zabil.Api.Common;
using Zabil.Api.Models.DTOs;

namespace Zabil.Api.Services.Interfaces;

public interface IGoogleAuthService
{
    public Task<Result<GoogleUserInfo>> ExchangeAndValidateAsync(string code);
}