namespace Starkov.Domain.Repositories;
public interface IDepartmentRepository
{
    Task<bool> ContainsAsync(string name, string parentName);
    Task<Department> UpsertRangeAsync(IEnumerable<Department> department);
    Task<Department> GetAsync(int id);
    Task<IQueryable<Department>> GetQueryableAsync();
}
