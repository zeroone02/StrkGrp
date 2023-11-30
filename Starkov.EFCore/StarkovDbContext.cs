using Microsoft.EntityFrameworkCore;
using Starkov.Domain;

namespace Starkov.EFCore;
public class StarkovDbContext : DbContext
{
    public StarkovDbContext(DbContextOptions<StarkovDbContext> options) 
        : base(options)
    {

    }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<JobTitle> JobTitles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Department>(cfg =>
        {
            cfg.HasKey(x => x.Id);
        });
    }
}
