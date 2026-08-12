using Zabil.Api.Common;

namespace Zabil.Api.Services.Interfaces;

public interface IJWTService
{
    public Task<Result<string>> GenerateToken(string id, string name, string email);
}