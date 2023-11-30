using Microsoft.EntityFrameworkCore;
using Starkov.Domain;
using Starkov.Domain.Repositories;

namespace Starkov.EFCore.Repositories;
public class EmployeeRepository : IEmployeeRepository
{
    private readonly StarkovDbContext _context;
    public EmployeeRepository(StarkovDbContext context)
    {
        _context = context;
    }
    public Task<bool> ContainsAsync(int id)
    {
        return _context.Employees.AnyAsync(x => x.Id == id);
    }

    public Task<Employee> GetAsync(int id)
    {
        return _context.Employees.FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<Employee> GetAsync(string fullName)
    {
        return _context.Employees.FirstOrDefaultAsync(x => x.FullName == fullName);

    }

    public async Task<IQueryable<Employee>> GetQueryableAsync()
    {
        return _context.Employees
            .Include(x => x.Department)
            .Include(x => x.JobTitle);
    }

    public async Task InsertRangeAsync(IEnumerable<Employee> items)
    {
        await _context.Employees.AddRangeAsync(items);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(IEnumerable<Employee> items)
    {
        _context.Employees.UpdateRange(items.ToList());
        await _context.SaveChangesAsync();
    }
}
