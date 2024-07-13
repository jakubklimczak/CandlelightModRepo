using Candlelight.Backend.Data;
using Candlelight.Backend.Entities.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Candlelight.Backend.Tests.Healthcheck;

[TestFixture]
public class DbConnectionTests
{
    private DataContext _context;
    private readonly Guid id1 = Guid.NewGuid();
    private readonly Guid id2 = Guid.NewGuid();
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
        _context.Tests.AddRange(
            new TestEntity { Id = id1, Name = "Entity 1", Description = "Description 1" },
            new TestEntity { Id = id2, Name = "Entity 2", Description = "Description 2" }
        );
        _context.SaveChanges();
    }

    [Test]
    public void CanDatabaseBeConnectedTo()
    {
        // Act
        var entitiesCount = _context.Tests.Count();

        // Assert
        Assert.That(entitiesCount, Is.EqualTo(2));
    }

    [Test]
    public void AreEntitiesSavedCorrectly()
    {
        // Act
        var entitiesCount = _context.Tests.Count();
        var entity1 = _context.Tests.FirstOrDefault(e => e.Id == id1);
        var entity2 = _context.Tests.FirstOrDefault(e => e.Id == id2);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(entitiesCount, Is.EqualTo(2));
            Assert.That(entity1, Is.Not.Null);
            Assert.That(entity1!.Name, Is.EqualTo("Entity 1"));
            Assert.That(entity1!.Description, Is.EqualTo("Description 1"));
            Assert.That(entity2, Is.Not.Null);
            Assert.That(entity2!.Name, Is.EqualTo("Entity 2"));
            Assert.That(entity2!.Description, Is.EqualTo("Description 2"));
        });
    }

    [TearDown]
    public void Cleanup()
    {
        var sql = @"TRUNCATE TABLE ""Tests""";
        _context.Database.ExecuteSqlRaw(sql);
        _context.Dispose();
    }
}
