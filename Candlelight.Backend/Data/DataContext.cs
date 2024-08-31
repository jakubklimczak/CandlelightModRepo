using Candlelight.Backend.Entities.Testing;
using Candlelight.Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Candlelight.Backend.Data;
public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<UserInfo> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<TestEntity> Tests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
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

        modelBuilder.Entity<UserInfo>()
            .HasIndex(u => u.Id)
            .IsUnique();
        modelBuilder.Entity<UserInfo>()
            .HasIndex(u => u.UserEmail)
            .IsUnique();
        modelBuilder.Entity<UserInfo>()
            .HasIndex(u => u.UserName)
            .IsUnique();
        modelBuilder.Entity<TestEntity>()
            .HasIndex(u => u.Id)
            .IsUnique();
        modelBuilder.Entity<UserProfile>()
            .HasIndex(u => u.Id)
            .IsUnique();
        modelBuilder.Entity<UserProfile>()
            .HasIndex(u => u.UserId)
            .IsUnique();
    }
}