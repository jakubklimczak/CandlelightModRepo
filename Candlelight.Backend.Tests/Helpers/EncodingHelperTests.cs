using Candlelight.Backend.Helpers;

namespace Candlelight.Backend.Tests.Helpers;

[TestFixture]
public class EncodingHelperTests
{
    [Test]
    public void DoesBase64EncodeReturnCorrectlyEncodedString()
    {
        // Arrange
        string plainText = "Hello, World!";
        string expectedEncoded = "SGVsbG8sIFdvcmxkIQ==";

        // Act
        string encoded = EncodingHelper.Base64Encode(plainText);

        // Assert
        Assert.That(encoded, Is.EqualTo(expectedEncoded));
    }

    [Test]
    public void DoesBase64EncodeReturnCorrectlyDecodedString()
    {
        // Arrange
        string encodedText = "SGVsbG8sIFdvcmxkIQ==";
        string expectedPlainText = "Hello, World!";

        // Act
        string decoded = EncodingHelper.Base64Decode(encodedText);

        // Assert
        Assert.That(decoded, Is.EqualTo(expectedPlainText));
    }

    [Test]
    public void DoBase64EncodeAndDecodeReturnOriginalString()
    {
        // Arrange
        string plainText = "This is a test string.";

        // Act
        string encoded = EncodingHelper.Base64Encode(plainText);
        string decoded = EncodingHelper.Base64Decode(encoded);

        // Assert
        Assert.That(decoded, Is.EqualTo(plainText));
    }
}
