using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Starkov.EFCore;
public class StarkovDbContextFactory : IDesignTimeDbContextFactory<StarkovDbContext>
{
    public StarkovDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<StarkovDbContext>()
            .UseNpgsql(GetConnectionStringFromConfiguration());

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        return new StarkovDbContext(builder.Options);
    }

    private static string GetConnectionStringFromConfiguration()
    {
        return BuildConfiguration()
            .GetConnectionString("DefaultConnection");
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    $"..\\Starkov.Console"
                )
            )
            .AddJsonFile("appsettings.json", optional: false);
       
        return builder.Build();
    }
}
