using Candlelight.Api.Controllers;
using Candlelight.Infrastructure.Persistence.Data;
using Candlelight.Core.Entities;
using Candlelight.Core.Entities.Forms;
using Candlelight.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Candlelight.Api.Tests.Controllers;

[Ignore("Doesn't work yet")]
[TestFixture]
public class UserAccessControllerTests
{
    private DataContext _context;
    private Mock<UserManagementService> _userManagementService;
    private Mock<AuthenticationService> _mockAuthenticationService;
    private UserAccessController _controller;

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

        _userManagementService = new Mock<UserManagementService>(_context);
        _mockAuthenticationService = new Mock<AuthenticationService>(_userManagementService.Object);
        //_controller = new UserAccessController(_mockAuthenticationService.Object, new UserManagementService(_context));
    }

    [Test]
    public async Task DoesUserRegistrationAddUserWithValidForm()
    {
        // Arrange
        var form = new RegistrationForm
        {
            UserEmail = "email@candlelight.com",
            UserName = "testUser",
            PasswordString = "notasecurepassword",
            ConfirmPasswordString = "notasecurepassword"
        };

        AppUser expectedAppUser = new()
        {
            Id = Guid.NewGuid(),
            UserName = form.UserName,
            Email = form.UserEmail,
            PasswordHash = "47yxds290",
            Created = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        _mockAuthenticationService.Setup(x => AuthenticationService.IsRegistrationFormValid(form)).Returns(true);
        _mockAuthenticationService.Setup(x => x.RegisterUser(It.IsAny<RegistrationForm>())).Returns(Task.FromResult(expectedAppUser));

        // Act
        var result = await _controller.PostAsync(form);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var actualUserInfo = (AppUser)result;

        Assert.Multiple(() =>
        {
            Assert.That(expectedAppUser.Id, Is.EqualTo(actualUserInfo.Id));
            Assert.That(expectedAppUser.UserName, Is.EqualTo(actualUserInfo.UserName));
            Assert.That(expectedAppUser.Email, Is.EqualTo(actualUserInfo.Email));
        });
    }

    [TearDown]
    public void Teardown()
    {
        _context.Dispose();
    }
}
