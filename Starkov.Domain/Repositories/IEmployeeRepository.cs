namespace Starkov.Domain.Repositories;
public interface IEmployeeRepository
{
    Task<bool> ContainsAsync(int id);
    Task<Employee> UpsertRangeAsync(IEnumerable<Employee> department);
    Task<Employee> GetAsync(int id);
}
