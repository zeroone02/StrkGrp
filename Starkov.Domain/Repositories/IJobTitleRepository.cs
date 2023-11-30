namespace Starkov.Domain.Repositories;
public interface IJobTitleRepository
{
    Task<bool> ContainsAsync(int id);
    Task<JobTitle> UpsertRangeAsync(IEnumerable<JobTitle> department);
    Task<JobTitle> GetAsync(int id);
}
