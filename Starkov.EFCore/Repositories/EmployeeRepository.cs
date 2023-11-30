using Starkov.Domain;
using Starkov.Domain.Repositories;

namespace Starkov.EFCore.Repositories;
public class EmployeeRepository : IEmployeeRepository
{
    private readonly StarkovDbContext _context;
    public EmployeeRepository()
    {
        _context = new StarkovDbContextFactory().CreateDbContext();
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
        return _context.Employees;
    }

    public Task InsertRangeAsync(IEnumerable<Employee> items)
    {
        throw new NotImplementedException();
    }

    public Task UpdateRangeAsync(IEnumerable<Employee> items)
    {
        throw new NotImplementedException();
    }
}
