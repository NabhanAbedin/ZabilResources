using Zabil.Api.Common;
using Zabil.Api.Models.DTOs;

namespace Zabil.Api.Services.Interfaces;

public interface IPostsService
{
    public Task<Result<bool>> CreatePostAsync(CreatePostFormDto formDto, Guid userId);
}