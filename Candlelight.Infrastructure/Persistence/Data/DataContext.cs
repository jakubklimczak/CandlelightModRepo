using Candlelight.Core.Entities.Testing;
using Candlelight.Core.Entities;
using Candlelight.Core.Entities.Steam;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Candlelight.Infrastructure.Persistence.Data;
public class DataContext(DbContextOptions<DataContext> options)
    : IdentityDbContext<
        AppUser,
        IdentityRole<Guid>,
        Guid,
        IdentityUserClaim<Guid>,
        IdentityUserRole<Guid>,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>
    >(options)
{
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<TestEntity> Tests { get; set; }
    public DbSet<Mod> Mods { get; set; }
    public DbSet<ModVersion> ModVersions { get; set; }
    public DbSet<SteamGameDetails> SteamGameDetails { get; set; }
    public DbSet<ModFavourite> ModFavourites { get; set; }
    public DbSet<ModReview> ModReviews { get; set; }
    public DbSet<GameFavourite> GameFavourites { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                        v => v.Kind == DateTimeKind.Local ? v.ToUniversalTime() : v,
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                    ));
                }
            }
        }

        // AspNetCore Identity types
        builder.Entity<IdentityUserLogin<Guid>>()
            .HasKey(l => new { l.LoginProvider, l.ProviderKey });

        builder.Entity<IdentityUserRole<Guid>>()
            .HasKey(r => new { r.UserId, r.RoleId });

        builder.Entity<IdentityUserToken<Guid>>()
            .HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

        builder.Entity<IdentityRole<Guid>>()
            .HasKey(r => new { r.Id, r.Name });

        // AppUser
        builder.Entity<AppUser>()
            .HasIndex(u => u.Id).IsUnique();
        builder.Entity<AppUser>()
            .HasIndex(u => u.Email).IsUnique();
        builder.Entity<AppUser>()
            .HasIndex(u => u.UserName).IsUnique();
        builder.Entity<AppUser>()
            .HasOne(u => u.UserProfile)
            .WithOne(p => p.User)
            .HasForeignKey<UserProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // UserProfile
        builder.Entity<UserProfile>()
            .HasIndex(p => p.Id).IsUnique();
        builder.Entity<UserProfile>()
            .HasIndex(p => p.UserId).IsUnique();

        // TestEntity
        builder.Entity<TestEntity>()
            .HasIndex(t => t.Id).IsUnique();

        // SteamGameDetails
        builder.Entity<SteamGameDetails>()
            .HasIndex(gd => gd.AppId).IsUnique();
        builder.Entity<SteamGameDetails>()
            .OwnsMany(gd => gd.Genres);
        builder.Entity<SteamGameDetails>()
            .OwnsMany(gd => gd.Categories);
        builder.Entity<SteamGameDetails>()
            .OwnsMany(gd => gd.Platforms);
        builder.Entity<SteamGameDetails>()
            .HasIndex(d => d.Id).IsUnique();

        // Game
        builder.Entity<Game>()
            .HasIndex(g => g.Id).IsUnique();
        builder.Entity<Game>()
            .HasOne(g => g.SteamGameDetails)
            .WithOne(d => d.Game)
            .HasForeignKey<SteamGameDetails>(d => d.GameId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Game>()
            .HasMany(g => g.Mods)
            .WithOne(m => m.Game)
            .HasForeignKey(m => m.GameId);

        // Mod
        builder.Entity<Mod>()
            .HasIndex(m => m.Id).IsUnique();
        builder.Entity<Mod>()
            .HasMany(m => m.Versions)
            .WithOne(v => v.Mod)
            .HasForeignKey(v => v.ModId);

        // ModVersion
        builder.Entity<ModVersion>()
            .HasIndex(v => v.Id).IsUnique();

        // ModFavourite
        builder.Entity<ModFavourite>()
            .HasIndex(mf => mf.Id).IsUnique();
        builder.Entity<ModFavourite>()
            .HasIndex(mf => new { mf.ModId, mf.UserId }).IsUnique();

        // ModReview
        builder.Entity<ModReview>()
            .HasIndex(r => r.Id).IsUnique();
        builder.Entity<ModReview>()
            .HasIndex(r => new { r.ModId, r.UserId }).IsUnique();

        // GameFavourite
        builder.Entity<GameFavourite>()
            .HasIndex(gf => gf.Id).IsUnique();
        builder.Entity<GameFavourite>()
            .HasIndex(gf => new { gf.GameId, gf.UserId }).IsUnique();
    }
}