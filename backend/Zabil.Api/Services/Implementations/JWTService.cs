using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Zabil.Api.Common;
using Zabil.Api.Data;
using Zabil.Api.Models.Entities;
using Zabil.Api.Models.Enums;
using Zabil.Api.Services.Interfaces;

namespace Zabil.Api.Services.Implementations;

public class JWTService : IJWTService
{
    private readonly ZabilContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<JWTService> _logger;

    public JWTService(ZabilContext context, IConfiguration configuration, ILogger<JWTService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Result<string>> GenerateToken(string id, string name, string email)
    {
        var identity = await _context.UserIdentities
            .Include(ui => ui.User)
            .FirstOrDefaultAsync(ui => ui.ProviderUserId == id);

        Guid userId;
        UserRole role;

        if (identity == null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = new User
                {
                    Name = name,
                    Email = email
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var userIdentity = new UserIdentity
                {
                    UserId = user.Id,
                    ProviderUserId = id,
                    Provider = IdentityProvider.Google,
                    ProviderEmail = email
                };

                _context.UserIdentities.Add(userIdentity);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                userId = user.Id;
                role = user.Role;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to create user and identity for provider user {ProviderUserId}", id);
                await transaction.RollbackAsync();
                throw;
            }
        }
        else
        {
            userId = identity.User.Id;
            role = identity.User.Role;
        }

        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:key"]);
        var tokenHandler = new JwtSecurityTokenHandler();

        var claims = new[]
        {
            new Claim("userId", userId.ToString()),
            new Claim("Email", email),
            new Claim("Role", role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Result<string>.Ok(tokenHandler.WriteToken(token));
    }
}
