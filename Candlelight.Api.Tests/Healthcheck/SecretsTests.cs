using Microsoft.Extensions.Configuration;

namespace Candlelight.Api.Tests.Healthcheck;

[TestFixture]
public class SecretsTests
{
    [Test]
    public void IsConnectionStringNotNull()
    {
        // Act
        var config = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../"))
            .AddUserSecrets("35b07a69-9a80-48e6-a97f-836b0f5f0a7e")
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");

        // Assert
        Assert.That(connectionString, Is.Not.Null);
    }
}
