using Candlelight.Backend.Data;
using Candlelight.Backend.Entities;
using Candlelight.Backend.Helpers;
using Candlelight.Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Candlelight.Backend.Tests.Services;

[TestFixture]
public class UserManagementServiceTests
{
    private DataContext _context;
    private UserManagementService _userManagementService;
    private readonly UserInfo _userToAdd = new() { Id = Guid.NewGuid(), UserName = "username", UserEmail = "email@email.com", PasswordHash = "unhashedPassword;)" };
    
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
        var result = await _userManagementService.CreateUserAsync(_userToAdd.UserName, _userToAdd.UserEmail, _userToAdd.PasswordHash);
        var user = await _userManagementService.GetUserByNameAsync(_userToAdd.UserName);
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.UserName, Is.EqualTo(_userToAdd.UserName));
            Assert.That(result.UserEmail, Is.EqualTo(_userToAdd.UserEmail));
            Assert.That(CryptographyHelper.VerifyPassword(_userToAdd, result.PasswordHash, _userToAdd.PasswordHash), Is.True);
        });  
        
        Assert.Multiple(() =>
        {
            Assert.That(user, Is.Not.Null);
            Assert.That(user!.UserName, Is.EqualTo(_userToAdd.UserName));
            Assert.That(user!.UserEmail, Is.EqualTo(_userToAdd.UserEmail));
            Assert.That(CryptographyHelper.VerifyPassword(_userToAdd, user.PasswordHash, _userToAdd.PasswordHash), Is.True);
        });
    }

    [TearDown]
    public void Teardown()
    {
        _context.Dispose();
    }
}
