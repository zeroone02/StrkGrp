namespace Starkov.Domain.Repositories;
public interface IDepartmentRepository
{
    Task InsertRangeAsync(IEnumerable<Department> items);
    Task UpdateRangeAsync(IEnumerable<Department> items);
    Task<Department> GetAsync(string name, string parentName);
    Task<Department> GetAsync(int id);
    Task<IQueryable<Department>> GetQueryableAsync();
}
