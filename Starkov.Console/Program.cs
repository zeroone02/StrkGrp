using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Starkov.Application;
using Starkov.Application.Interfaces;
using Starkov.Domain.Repositories;
using Starkov.EFCore;
using Starkov.EFCore.Repositories;

namespace Starkov;
public class Program
{
    public static async Task<int> Main()
    {
        try
        {
            var provider = ConfigureServices();
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

        IServiceCollection services = new ServiceCollection();

        services.AddDbContext<StarkovDbContext>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IJobTitleRepository, JobTitleRepository>();
        services.AddTransient<ImportService>();
        services.AddTransient<IConsoleClient, IConsoleClient>();

        return services.BuildServiceProvider();
    }
}