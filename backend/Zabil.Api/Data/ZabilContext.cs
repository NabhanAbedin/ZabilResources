using Microsoft.EntityFrameworkCore;
using Zabil.Api.Models.Entities;

namespace Zabil.Api.Data;

public class ZabilContext : DbContext
{
    public ZabilContext(DbContextOptions<ZabilContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserIdentity> UserIdentities => Set<UserIdentity>();
    public DbSet<FbPost> FbPosts => Set<FbPost>();
    public DbSet<FbMedia> FbMedia => Set<FbMedia>();
    public DbSet<FbSyncLog> FbSyncLogs => Set<FbSyncLog>();
    public DbSet<UserPost> UserPosts => Set<UserPost>();
    public DbSet<UserPostMedia> UserPostMedia => Set<UserPostMedia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Role).HasConversion<string>();
        });

        modelBuilder.Entity<UserIdentity>(entity =>
        {
            entity.Property(e => e.Provider).HasConversion<string>();

            entity.HasIndex(e => new { e.Provider, e.ProviderUserId }).IsUnique();

            entity.HasOne(e => e.User)
                .WithMany(u => u.Identities)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FbPost>(entity =>
        {
            entity.Property(e => e.PostType).HasConversion<string>();
            entity.Property(e => e.Category).HasConversion<string>();
            entity.Property(e => e.Status).HasConversion<string>();

            entity.HasIndex(e => e.FbPostId).IsUnique();
        });

        modelBuilder.Entity<FbMedia>(entity =>
        {
            entity.Property(e => e.MediaType).HasConversion<string>();

            entity.HasOne(e => e.Post)
                .WithMany(p => p.Media)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FbSyncLog>(entity =>
        {
            entity.Property(e => e.Trigger).HasConversion<string>();
            entity.Property(e => e.Status).HasConversion<string>();

            entity.HasIndex(e => e.FbPostId);
        });

        modelBuilder.Entity<UserPost>(entity =>
        {
            entity.Property(e => e.Category).HasConversion<string>();
            entity.Property(e => e.Status).HasConversion<string>();

            entity.HasOne(e => e.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserPostMedia>(entity =>
        {
            entity.Property(e => e.MediaType).HasConversion<string>();

            entity.HasOne(e => e.Post)
                .WithMany(p => p.Media)
                .HasForeignKey(e => e.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
