using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zabil.Api.Data;
using Zabil.Tests.Fakes;

namespace Zabil.Tests.Integration;

// Boots the real app (real routing, real [Authorize] pipeline, real model binding) via
// WebApplicationFactory, only swapping the DB for SQLite in-memory and overriding JWT
// config so tests don't depend on the gitignored appsettings.json being present.
public class PostsApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("Filename=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:key"] = TestJwtFactory.SigningKey,
                ["Jwt:issuer"] = TestJwtFactory.Issuer,
                ["Jwt:audience"] = TestJwtFactory.Audience,
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ZabilContext>>();
            // EF Core 8+ registers AddDbContext's configuration additively via
            // IDbContextOptionsConfiguration<T> — without removing this too, Program.cs's
            // original UseNpgsql(...) action still gets layered onto the new options.
            services.RemoveAll<IDbContextOptionsConfiguration<ZabilContext>>();

            // Program.cs already registered Npgsql's provider services into this
            // container; UseInternalServiceProvider isolates Sqlite's into their own
            // private container so the two providers don't collide in the same one.
            services.AddDbContext<ZabilContext>(options =>
            {
                options.UseSqlite(_connection);
                options.UseInternalServiceProvider(
                    new ServiceCollection().AddEntityFrameworkSqlite().BuildServiceProvider());
            });

            using var scope = services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ZabilContext>();
            context.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) _connection.Dispose();
    }
}
