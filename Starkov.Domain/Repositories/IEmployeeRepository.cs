namespace Starkov.Domain.Repositories;
public interface IEmployeeRepository
{
    Task<bool> ContainsAsync(int id);
    Task<Employee> GetAsync(int id);
    Task<Employee> GetAsync(string fullName);
    Task InsertRangeAsync(IEnumerable<Employee> items);
    Task UpdateRangeAsync(IEnumerable<Employee> items);
}
