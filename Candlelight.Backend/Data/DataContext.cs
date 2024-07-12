using Candlelight.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace Candlelight.Backend.Data;
public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }
    public DbSet<TestEntity> Tests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>()
            .HasIndex(u => u.Id)
            .IsUnique();
        modelBuilder.Entity<AppUser>()
            .HasIndex(u => u.UserEmail)
            .IsUnique();
        modelBuilder.Entity<AppUser>()
            .HasIndex(u => u.UserName)
            .IsUnique();
        modelBuilder.Entity<TestEntity>()
            .HasIndex(u => u.Id)
            .IsUnique();
    }
}