using Microsoft.EntityFrameworkCore;
using Starkov.Domain;
using Starkov.Domain.Repositories;

namespace Starkov.EFCore.Repositories;
public class DepartmentRepository : IDepartmentRepository
{
    private StarkovDbContext _context;
    public DepartmentRepository()
    {
        _context = new StarkovDbContextFactory().CreateDbContext();
    }

    public Task<Department> GetAsync(string name, string parentName)
    {
        return _context.Departments.FirstOrDefaultAsync(x => x.Name == name && x.ParentDepartment.Name == parentName);
    }

    public async Task<IQueryable<Department>> GetQueryableAsync()
    {
        return _context.Departments
            .Include(x => x.Manager)
            .Include(x => x.ParentDepartment);
    }

    public Task<Department> GetAsync(int id)
    {
        return _context.Departments.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task InsertRangeAsync(IEnumerable<Department> items)
    {
        await _context.Departments.AddRangeAsync(GetAsCollection(items));
        await _context.SaveChangesAsync();
    }

    public Task UpdateRangeAsync(IEnumerable<Department> items)
    {
        _context.Departments.UpdateRange(GetAsCollection(items));
        return _context.SaveChangesAsync();
    }

    private ICollection<Department> GetAsCollection(IEnumerable<Department> departments) 
    { 
        if(departments is ICollection<Department> collections)
        {
            return collections;
        }

        return departments.ToList();
    }
}
