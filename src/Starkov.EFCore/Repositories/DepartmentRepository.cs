using Microsoft.EntityFrameworkCore;
using Starkov.Domain;
using Starkov.Domain.Repositories;

namespace Starkov.EFCore.Repositories;
public class DepartmentRepository : IDepartmentRepository
{
    private StarkovDbContext _context;
    public DepartmentRepository(StarkovDbContext context)
    {
        _context = context;
    }

    public async Task<Department> GetAsync(string name, string parentName)
    {
        var item = await GetAsync(parentName);
        int? id = item?.Id;
        return await _context.Departments
            .Include(x => x.ParentDepartment)
            .Include(x => x.Manager)
            .FirstOrDefaultAsync(x => x.Name == name && x.ParentDepartmentId == id);
    }

    public async Task<IQueryable<Department>> GetQueryableAsync()
    {
        return _context.Departments
            .Include(x => x.Manager)
            .Include(x => x.ParentDepartment);
    }

    public Task<Department> GetAsync(int id)
    {
        return _context.Departments
                .Include(x => x.ParentDepartment)
                .Include(x => x.Manager)
                .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task InsertRangeAsync(IEnumerable<Department> items)
    {
        await _context.Departments.AddRangeAsync(items);
        await _context.SaveChangesAsync();
    }

    public Task UpdateRangeAsync(IEnumerable<Department> items)
    {
        _context.Departments.UpdateRange(items);
        return _context.SaveChangesAsync();
    }

    public Task<Department> GetAsync(string name)
    {
        return _context.Departments
            .Include(x => x.ParentDepartment)
            .Include(x => x.Manager)
            .FirstOrDefaultAsync(x => x.Name == name);
    }
}
