using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Candlelight.Infrastructure.Persistence.Data;
public class DesignTimeDataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..");
        var config = new ConfigurationBuilder()
            .AddUserSecrets<UserSecretsAnchor>()
            .SetBasePath(basePath + "/Candlelight.Api/")
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection")
                               ?? config["ConnectionStrings:DefaultConnection"];

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Connection string is missing or invalid! Check UserSecrets for any misconfiguration.");

        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new DataContext(optionsBuilder.Options);
    }
}