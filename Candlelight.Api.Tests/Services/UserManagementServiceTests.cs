using Candlelight.Infrastructure.Persistence.Data;
using Candlelight.Core.Entities;
using Candlelight.Core.Helpers;
using Candlelight.Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Candlelight.Api.Tests.Services;

[TestFixture]
public class UserManagementServiceTests
{
    private DataContext _context;
    private UserManagementService _userManagementService;
    private readonly AppUser _appUserToAdd = new() { Id = Guid.NewGuid(), UserName = "username", Email = "email@email.com", PasswordHash = "unhashedPassword;)", Created = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
    
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
        _userManagementService = new UserManagementService(_context);
    }

    [Test]
    public async Task DoesAddingAUserWithProperDataWork()
    {
        // Act
        var result = await _userManagementService.CreateUserAsync(_appUserToAdd.UserName!, _appUserToAdd.Email!, _appUserToAdd.PasswordHash!);
        var user = await _userManagementService.GetUserByNameAsync(_appUserToAdd.UserName!);
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.UserName, Is.EqualTo(_appUserToAdd.UserName));
            Assert.That(result.Email, Is.EqualTo(_appUserToAdd.Email));
            Assert.That(CryptographyHelper.VerifyPassword(_appUserToAdd, result.PasswordHash!, _appUserToAdd.PasswordHash!), Is.True);
        });  
        
        Assert.Multiple(() =>
        {
            Assert.That(user, Is.Not.Null);
            Assert.That(user!.UserName, Is.EqualTo(_appUserToAdd.UserName));
            Assert.That(user!.Email, Is.EqualTo(_appUserToAdd.Email));
            Assert.That(CryptographyHelper.VerifyPassword(_appUserToAdd, user.PasswordHash!, _appUserToAdd.PasswordHash!), Is.True);
        });
    }

    [TearDown]
    public void Teardown()
    {
        _context.Dispose();
    }
}
