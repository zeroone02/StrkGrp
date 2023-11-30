using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Starkov.Application.Interfaces;
using Starkov.EFCore;
namespace Starkov;
public class Program
{
    public static async Task<int> Main()
    {
        try
        {
            var provider = ConfigureServices();
            await MigrateAsync();
            var servie = provider.GetRequiredService<IConsoleClient>();
            await servie.RunAsync();
            return 1;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex);
            Console.ForegroundColor = ConsoleColor.White;
            return 0;
        }
    }

    private static IServiceProvider ConfigureServices()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddDbContext<StarkovDbContext>();

        return serviceCollection.BuildServiceProvider();
    }

    private static async Task MigrateAsync()
    {
        using (var context = new StarkovDbContextFactory().CreateDbContext())
        {
            await context.Database.MigrateAsync();
        }
    }
}