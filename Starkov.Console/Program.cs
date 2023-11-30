using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Starkov.EFCore;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true)
    .Build();

IServiceCollection serviceCollection = new ServiceCollection();
serviceCollection.AddDbContext<StarkovDbContext>();

using(var context = new StarkovDbContextFactory().CreateDbContext(args))
{
    await context.Database.MigrateAsync();
}


