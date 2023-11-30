using Microsoft.EntityFrameworkCore;
using Starkov.Domain;
using Starkov.Domain.Repositories;

namespace Starkov.EFCore.Repositories;
public class DepartmentRepository : IDepartmentRepository
{
    private readonly StarkovDbContextFactory _dbContextFactory;

    public DepartmentRepository(StarkovDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
    public Task<bool> ContainsAsync(string name, string parentName)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.Departments.AnyAsync(x => x.Name == name && x.ParentDepartment.Name == parentName);
    }

    public Task<Department> GetAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Department> UpsertRangeAsync(IEnumerable<Department> department)
    {
        throw new NotImplementedException();
    }

    public async Task<IQueryable<Department>> GetQueryableAsync()
    {
        var context = _dbContextFactory.CreateDbContext();
        return context.Departments;
    }
}
