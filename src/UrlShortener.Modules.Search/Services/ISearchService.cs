using UrlShortener.Modules.Search.Models;

namespace UrlShortener.Modules.Search.Services;

public interface ISearchService
{
    Task<IEnumerable<SearchResult>> SearchAsync(string query, CancellationToken cancellationToken = default);
}
