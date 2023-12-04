using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Starkov.Application;
using Starkov.Application.Clients;
using Starkov.Application.Interfaces;
using Starkov.Domain.Repositories;
using Starkov.EFCore;
using Starkov.EFCore.Repositories;

namespace Starkov;
public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var provider = ConfigureServices();
            var service = provider.GetRequiredService<IConsoleClient>();

            var context = provider.GetRequiredService<StarkovDbContext>();
            if((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }

            await service.RunAsync(args);

            return 1;
        }
        catch (FileNotFoundException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Файл не найден: " + ex.Message);
            Console.ForegroundColor = ConsoleColor.White;
            return 0;
        }
        catch(Exception ex)
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

        IServiceCollection services = new ServiceCollection();

        services.AddDbContext<StarkovDbContext>(cfg =>
        {
            cfg.UseNpgsql(config.GetConnectionString("DefaultConnection"));
        });
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IJobTitleRepository, JobTitleRepository>();
        services.AddTransient<ImportService>();
        services.AddTransient<IConsoleClient, ConsoleClient>();

        return services.BuildServiceProvider();
    }
}