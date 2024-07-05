using Candlelight.Backend.Data;
using Candlelight.Server.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Candlelight.Backend.Tests.Healthcheck;

[Ignore("Disabled, use when setting up the database.")]
[TestFixture]
public class DoesDbConnectionWorkTests
{
    private DataContext _context;

    [SetUp]
    public void Setup()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../"))
            .AddUserSecrets("35b07a69-9a80-48e6-a97f-836b0f5f0a7e")
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseNpgsql(connectionString)
            .Options;

        _context = new DataContext(options);
        _context.Database.EnsureCreated();

        SeedTestData();
    }

    private void SeedTestData()
    {
        _context.Users.AddRange(
            new AppUser { Id = Guid.NewGuid(), UserName = "Entity 1", UserEmail="email1@email.com" },
            new AppUser { Id = Guid.NewGuid(), UserName = "Entity 2", UserEmail="email2@email.com" }
        );
        _context.SaveChanges();
    }

    [Test]
    public void CanConnectToDatabase()
    {
        // Act
        var entitiesCount = _context.Users.Count();

        // Assert
        Assert.That(entitiesCount, Is.EqualTo(2));
    }

    [TearDown]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
