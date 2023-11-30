namespace Starkov.Domain.Repositories;
public interface IJobTitleRepository
{
    Task<JobTitle> GetAsync(int id);
    Task<JobTitle> GetAsync(string name);
    Task InsertRange(IEnumerable<JobTitle> items);
    Task UpdateRange(IEnumerable<JobTitle> items);
    Task<List<JobTitle>> GetListAsync();
    Task<bool> ContainsAsync(string name);
}
