using Candlelight.Core.Entities;
using Candlelight.Core.Helpers;

namespace Candlelight.Api.Tests.Helpers;

[TestFixture]
public class CryptographyHelperTests
{

    readonly AppUser _appUser = new()
    {
        Id = Guid.NewGuid(),
        Email = "email@candlelight.com",
        UserName = "testUser",
        PasswordHash = "",
        Created = DateTime.UtcNow,
        LastUpdated = DateTime.UtcNow
    };

    [Test]
    public void DoesSamePasswordReturnDifferentHashes()
    {
        // Arrange
        string password = "notasecurepassword";

        // Act
        string hash1 = CryptographyHelper.HashPassword(_appUser, password);
        string hash2 = CryptographyHelper.HashPassword(_appUser, password);

        // Assert
        Assert.That(hash1, Is.Not.EqualTo(hash2)); // Hashes should be different due to different salts
    }

    [Test]
    public void DoesVerifyPasswordReturnTrueForCorrectPassword()
    {
        // Arrange
        string password = "notasecurepassword";
        string hash = CryptographyHelper.HashPassword(_appUser, password);

        // Act
        bool isVerified = CryptographyHelper.VerifyPassword(_appUser, hash, password);

        // Assert
        Assert.That(isVerified, Is.True);
    }

    [Test]
    public void DoesVerifyPasswordReturnFalseForIncorrectPassword()
    {
        // Arrange
        string password = "notasecurepassword";
        string incorrectPassword = "stillnotasecurepassword";
        string hash = CryptographyHelper.HashPassword(_appUser, password);

        // Act
        bool isVerified = CryptographyHelper.VerifyPassword(_appUser, hash, incorrectPassword);

        // Assert
        Assert.That(isVerified, Is.False);
    }
}
