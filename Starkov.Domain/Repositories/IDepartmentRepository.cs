namespace Starkov.Domain.Repositories;
public interface IDepartmentRepository
{
    Task<bool> ContainsAsync(int id);
    Task<Department> UpsertRangeAsync(IEnumerable<Department> department);
    Task<Department> GetAsync(int id);
}
