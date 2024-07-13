using Candlelight.Backend.Entities.Testing;
using Candlelight.Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Candlelight.Backend.Data;
public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<UserInfo> Users { get; set; }
    public DbSet<TestEntity> Tests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
    }
}