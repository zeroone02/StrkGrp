using Microsoft.EntityFrameworkCore;
using Starkov.Domain;
using Starkov.Domain.Repositories;
using System.Xml.Linq;

namespace Starkov.EFCore.Repositories;
public class JobTitleRepository : IJobTitleRepository
{
    private readonly StarkovDbContext _context;
    public JobTitleRepository(StarkovDbContext context)
    {
        _context = context;
    }

    public Task<bool> ContainsAsync(string name)
    {
        return _context.JobTitles.AnyAsync(x => x.Name == name);
    }

    public Task<JobTitle> GetAsync(int id)
    {
        return _context.JobTitles.FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<JobTitle> GetAsync(string name)
    {
        return _context.JobTitles.FirstOrDefaultAsync(x => x.Name == name);
    }

    public Task<List<JobTitle>> GetListAsync()
    {
        return _context.JobTitles.ToListAsync();
    }

    public async Task InsertRange(IEnumerable<JobTitle> items)
    {
        await _context.JobTitles.AddRangeAsync(items);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRange(IEnumerable<JobTitle> items)
    {
        _context.JobTitles.UpdateRange(items);
        await _context.SaveChangesAsync();
    }
}
