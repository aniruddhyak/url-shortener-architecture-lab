using UrlShortener.Modules.Urls.Models;

namespace UrlShortener.Modules.Urls.Repositories;

public interface IUrlRepository
{
    Task<UrlEntry?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default);
    Task<bool> ShortCodeExistsAsync(string shortCode, CancellationToken cancellationToken = default);
    Task<UrlEntry> AddAsync(UrlEntry entry, CancellationToken cancellationToken = default);
}
