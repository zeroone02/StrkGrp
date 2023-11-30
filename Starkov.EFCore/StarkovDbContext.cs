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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Department>(cfg =>
        {
            cfg.HasKey(x => x.Id);
        });
    }
}
