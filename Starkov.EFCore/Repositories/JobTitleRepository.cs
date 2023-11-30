using Starkov.Domain;
using Starkov.Domain.Repositories;

namespace Starkov.EFCore.Repositories;
public class JobTitleRepository : IJobTitleRepository
{
    private readonly StarkovDbContext _context;
    public JobTitleRepository()
    {
        _context = new StarkovDbContextFactory().CreateDbContext();
    }
    public Task<bool> ContainsAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<JobTitle> GetAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<JobTitle> GetAsync(string name)
    {
        throw new NotImplementedException();
    }

    public Task InsertRange(IEnumerable<JobTitle> items)
    {
        throw new NotImplementedException();
    }

    public Task UpdateRange(IEnumerable<JobTitle> items)
    {
        throw new NotImplementedException();
    }
}
