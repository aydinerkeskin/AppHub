using Microsoft.EntityFrameworkCore;
using AppHub.Domain.Entities;
using ApplicationEntity = AppHub.Domain.Entities.Application;

namespace AppHub.Infrastructure.Data;

public class AppHubDbContext : DbContext
{
    public AppHubDbContext(DbContextOptions<AppHubDbContext> options) : base(options)
    {
    }

    public DbSet<OAuthProvider> OAuthProviders { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserOAuth> UserOAuths { get; set; }
    public DbSet<ApplicationEntity> Applications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // OAuthProvider configuration
        modelBuilder.Entity<OAuthProvider>(entity =>
        {
            entity.ToTable("oauth_providers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
            entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255);
            entity.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(100);
            entity.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(100);
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.IsEmailVerified).HasColumnName("is_email_verified").HasDefaultValue(false);
            entity.Property(e => e.LastLogin).HasColumnName("last_login");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // UserOAuth configuration
        modelBuilder.Entity<UserOAuth>(entity =>
        {
            entity.ToTable("user_oauth");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.ProviderId).HasColumnName("provider_id").IsRequired();
            entity.Property(e => e.ProviderUserId).HasColumnName("provider_user_id").HasMaxLength(255).IsRequired();
            entity.Property(e => e.AccessToken).HasColumnName("access_token");
            entity.Property(e => e.RefreshToken).HasColumnName("refresh_token");
            entity.Property(e => e.TokenExpiresAt).HasColumnName("token_expires_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserOAuths)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Provider)
                .WithMany(p => p.UserOAuths)
                .HasForeignKey(e => e.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => new { e.ProviderId, e.ProviderUserId }).IsUnique();
            entity.HasIndex(e => new { e.UserId, e.ProviderId }).IsUnique();
        });

        // Application configuration
        modelBuilder.Entity<ApplicationEntity>(entity =>
        {
            entity.ToTable("applications");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.AppKey).HasColumnName("app_key").HasMaxLength(100);
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.AppKey).IsUnique();
        });
    }
}

