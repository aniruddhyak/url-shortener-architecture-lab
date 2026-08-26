using UrlShortener.Modules.Search.Models;

namespace UrlShortener.Modules.Search.Repositories;

public interface ISearchRepository
{
    Task<IEnumerable<SearchResult>> SearchAsync(
        string query,
        CancellationToken cancellationToken = default);
}
