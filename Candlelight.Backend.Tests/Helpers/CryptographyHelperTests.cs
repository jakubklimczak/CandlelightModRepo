using Candlelight.Backend.Entities;
using Candlelight.Backend.Helpers;

namespace Candlelight.Backend.Tests.Helpers;

[TestFixture]
public class CryptographyHelperTests
{

    readonly UserInfo user = new()
    {
        Id = Guid.NewGuid(),
        UserEmail = "email@candlelight.com",
        UserName = "testUser",
        PasswordHash = ""
    };

    [Test]
    public void DoesSamePasswordReturnDifferentHashes()
    {
        // Arrange
        string password = "notasecurepassword";

        // Act
        string hash1 = CryptographyHelper.HashPassword(user, password);
        string hash2 = CryptographyHelper.HashPassword(user, password);

        // Assert
        Assert.That(hash1, Is.Not.EqualTo(hash2)); // Hashes should be different due to different salts
    }

    [Test]
    public void DoesVerifyPasswordReturnTrueForCorrectPassword()
    {
        // Arrange
        string password = "notasecurepassword";
        string hash = CryptographyHelper.HashPassword(user, password);

        // Act
        bool isVerified = CryptographyHelper.VerifyPassword(user, hash, password);

        // Assert
        Assert.That(isVerified, Is.True);
    }

    [Test]
    public void DoesVerifyPasswordReturnFalseForIncorrectPassword()
    {
        // Arrange
        string password = "notasecurepassword";
        string incorrectPassword = "stillnotasecurepassword";
        string hash = CryptographyHelper.HashPassword(user, password);

        // Act
        bool isVerified = CryptographyHelper.VerifyPassword(user, hash, incorrectPassword);

        // Assert
        Assert.That(isVerified, Is.False);
    }
}
