using Candlelight.Backend.Data;
using Candlelight.Backend.Entities;
using Candlelight.Backend.Entities.Forms;
using Candlelight.Backend.Services;
using Candlelight.Server.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Candlelight.Backend.Tests.Controllers;

[TestFixture]
public class UserRegistrationControllerTests
{
    private DataContext _context;
    private Mock<UserManagementService> _userManagementService;
    private Mock<AuthenticationService> _mockAuthenticationService;
    private UserRegistrationController _controller;

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
        _controller = new UserRegistrationController(_mockAuthenticationService.Object);
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

        UserInfo expectedUserInfo = new()
        {
            Id = Guid.NewGuid(),
            UserName = form.UserName,
            UserEmail = form.UserEmail,
            PasswordHash = "47yxds290"
        };

        _mockAuthenticationService.Setup(x => x.IsRegistrationFormValid(form)).Returns(true);
        _mockAuthenticationService.Setup(x => x.RegisterUser(It.IsAny<RegistrationForm>())).Returns(Task.FromResult(expectedUserInfo));

        // Act
        var result = await _controller.PostAsync(form);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var actualUserInfo = (UserInfo)result;

        Assert.Multiple(() =>
        {
            Assert.That(expectedUserInfo.Id, Is.EqualTo(actualUserInfo.Id));
            Assert.That(expectedUserInfo.UserName, Is.EqualTo(actualUserInfo.UserName));
            Assert.That(expectedUserInfo.UserEmail, Is.EqualTo(actualUserInfo.UserEmail));
        });
    }

    [TearDown]
    public void Teardown()
    {
        _context.Dispose();
    }
}
