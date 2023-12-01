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
            cfg.ToTable("Departments");
            cfg.HasOne(x => x.ParentDepartment)
               .WithMany()
               .HasForeignKey(x => x.ParentDepartmentId);

            cfg.HasOne(x => x.Manager)
               .WithMany();
        });

        modelBuilder.Entity<Employee>(cfg =>
        {
            cfg.ToTable("Employees");
            cfg.HasOne(x => x.JobTitle)
               .WithMany()
               .HasForeignKey(x => x.JobTitleId);
        });

        modelBuilder.Entity<JobTitle>(cfg =>
        {
            cfg.ToTable("JobTitles");
        });
    }
}
