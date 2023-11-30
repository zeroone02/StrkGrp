using Starkov.Domain;
using Starkov.Domain.Repositories;

namespace Starkov.EFCore.Repositories;
public class EmployeeRepository : IEmployeeRepository
{
    public Task<bool> ContainsAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Employee> GetAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Employee> GetAsync(string fullName)
    {
        throw new NotImplementedException();
    }

    public Task<Employee> UpsertRangeAsync(IEnumerable<Employee> department)
    {
        throw new NotImplementedException();
    }
}
