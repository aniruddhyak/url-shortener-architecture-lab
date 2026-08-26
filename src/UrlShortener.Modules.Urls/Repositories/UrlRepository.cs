using Microsoft.EntityFrameworkCore;
using UrlShortener.Modules.Search.Models;
using UrlShortener.Modules.Search.Repositories;
using UrlShortener.Modules.Urls.Models;

namespace UrlShortener.Modules.Urls.Repositories;

public class UrlRepository(DbContext dbContext) : IUrlRepository, ISearchRepository
{
    public Task<UrlEntry?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default) =>
        dbContext.Set<UrlEntry>().SingleOrDefaultAsync(entry => entry.ShortCode == shortCode, cancellationToken);

    public Task<bool> ShortCodeExistsAsync(string shortCode, CancellationToken cancellationToken = default) =>
        dbContext.Set<UrlEntry>().AnyAsync(entry => entry.ShortCode == shortCode, cancellationToken);

    public async Task<UrlEntry> AddAsync(UrlEntry entry, CancellationToken cancellationToken = default)
    {
        dbContext.Set<UrlEntry>().Add(entry);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entry;
    }

    public async Task<IEnumerable<SearchResult>> SearchAsync(string query, CancellationToken cancellationToken = default) =>
        await dbContext.Set<UrlEntry>()
            .Where(entry => EF.Functions.Like(entry.OriginalUrl, $"%{query}%"))
            .OrderByDescending(entry => entry.CreatedAt)
            .Select(entry => new SearchResult(
                entry.Id, entry.OriginalUrl, entry.ShortCode, entry.CreatedAt))
            .ToListAsync(cancellationToken);
}
