using UrlShortener.Modules.Urls.Models;

namespace UrlShortener.Modules.Urls.Services;

public interface IUrlService
{
    Task<UrlEntry> CreateAsync(string originalUrl, CancellationToken cancellationToken = default);
    Task<UrlEntry?> GetByShortCodeAsync(string shortCode, CancellationToken cancellationToken = default);
}
